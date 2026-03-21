# 1.0.0初步规划 - 开发记忆

## 模块概述

本子工作区承接 NPC 自动移动与环境社交的第一轮规划。
当前以 `npc规划001.md` 为规划核心，用于明确 NPC 随机移动、短停、长停、气泡表达与轻量聊天的 V1 边界。

## 当前状态

- **完成度**: 25%
- **最后更新**: 2026-03-14
- **状态**: 规划文档已落盘，任务文档已建立，待进入设计细化或实现

## 包含文件

| 文件 | 说明 |
|------|------|
| `npc规划001.md` | NPC 自动移动与环境社交规划正文 |
| `memory.md` | 当前子工作区记忆 |
| `tasks.md` | 根据规划拆出的任务清单 |

## 会话记录

### 会话 1 - 2026-03-14

**用户需求**:
> 先把 NPC 自动移动与聊天基础想法完整写成规划文档，后续继续沿着这个规划脑暴。

**完成任务**:
1. 整理用户提出的基础设想：随机间隔移动、移动后短停、累计短停后长停、长停时自言自语或与附近 NPC 聊天。
2. 结合项目现有的 `NavGrid2D`、`PlayerAutoNavigator`、`INavigationUnit` 与 `DialogueNode.isBubble` 做第一轮可落地化拓展。
3. 完成 `npc规划001.md`，明确了 V1 行为闭环、系统拆分方向、边界和后续脑暴问题。

**修改文件**:
- `.kiro/specs/NPC/1.0.0初步规划/npc规划001.md`

**解决方案**:
- 将 NPC 自动移动问题先定义为“节奏型生活流行为”，而不是单纯随机走路。
- 明确 V1 先做“单 NPC 随机移动 + 短停 / 长停 + 自言自语气泡”，把多人聊天放在第二阶段。

**遗留问题**:
- [ ] 任务拆分尚未落成。
- [ ] 气泡表现层是否可直接复用现有运行时仍待核实。

---

### 会话 2 - 2026-03-14

**用户需求**:
> 已补上子工作区结构，要求重做主 memory，并根据 `npc规划001` 完成任务文档。

**完成任务**:
1. 纠正本文件的定位，使其真正成为 `1.0.0初步规划` 的子工作区记忆。
2. 基于 `npc规划001.md` 新建 `tasks.md`，把 V1 的规划内容拆成后续可执行任务。

**修改文件**:
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md`

**当前恢复点**:
- 当前子工作区已经具备“规划正文 + 记忆 + 任务文档”三件套。
- 下一步可以直接继续补设计细节，或开始进入第一版实现。

---

### 会话 3 - 2026-03-15

**用户需求**:
> 询问上一轮“白名单同步被 `git-safe-sync.ps1` 拦住”的那句话到底是什么意思。

**完成任务**:
1. 用白话解释“白名单同步”“当前在 `main` 被脚本拦住”“无关脏改”三个概念。
2. 明确说明：那次并不是代码有问题，而是安全同步脚本自己的规则不允许直接在 `main` 上提交这类任务。

**当前恢复点**:
- 当前对子工作区的理解没有变化。
- 这次只是补充 Git 规则口径解释，不涉及规划正文或任务清单变更。
---

### 会话 4 - 2026-03-15

**用户需求**:
> 重新回顾自己把开发现场从 NPC 分支/独立 worktree 拉回 `main` 之后，到底是不是这件事导致无法提交；希望结合聊天记录、NPC 工作区现状和当前 Git 现场重新解释清楚。

**完成任务**:
1. 回读当前 NPC 父子工作区记忆与线程记忆，确认这条线最近的实际落地主要是 `.kiro/specs/NPC/` 下的规划、任务和记忆整理，而不是新的 `Assets/` 业务实现。
2. 重新核对当前仓库真实现场：当前分支仍是 `main`，而工作树里同时存在 NPC 工作区新增/迁移内容和大量其他线程、场景、资源改动。
3. 形成新的明确结论：不是“只要被拉回 `main` 就永远不能提交”，而是“回到 `main` 后继续做了会被脚本判为 task 的 NPC 工作区推进，所以按当前正式规则不能直接在 `main` 提交”；同时，现有工作树不干净，也不满足立刻 `ensure-branch` 的条件。

**关键结论**:
- `main` 只是当前可见开发现场，不等于可以直接承载 NPC task 提交。
- `.kiro/specs/NPC/` 属于 NPC 业务工作区推进，不在 `governance` 默认白名单里；按现行脚本，它会被当成 task，而 task 模式在 `main` 上会被直接拦截。
- 如果只是“以前某个 NPC 分支的成果已经合回 `main`，现在在 `main` 看结果”，这本身没问题；真正导致这次无法提交的是“又在 `main` 上继续产生了新的 task 类改动”。
- 即便现在想回到正确做法，也还差一步：当前工作树里有很多和 NPC 无关的脏改，`ensure-branch` 会先要求工作树干净，不能直接从这个现场安全切出任务分支。

**当前恢复点**:
- NPC 子工作区的规划主线没有变化，仍停留在 `1.0.0初步规划`。
- 当前新增的稳定认知是 Git 口径：`main` 可作为可见现场，但 NPC 这类业务工作区推进若要提交，仍应在干净基线下切到 `codex/` 分支收口。
---

### 会话 5 - 2026-03-16

**用户需求**:
> 全局已解冻，要求先不要写新功能，而是优先收口当前 NPC 实体资产 WIP：`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.meta`；先核对现场，再在 `codex/...` 任务分支上用 task 白名单方式固化应保留资产与相关记忆。

**完成任务**:
1. 重新核对当前真实现场，确认本轮开始时位于 `D:\Unity\Unity_learning\Sunset @ main`，`HEAD` 为 `9b9a6bd0dd7c5ee7d18cc82e3ea9da74a146bf9d`，NPC 范围 dirty 主要集中在三张 sprite meta、`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`、`.kiro/specs/NPC/` 与 `.codex/threads/Sunset/NPC/`。
2. 回读生成产物，确认 `Assets/100_Anim/NPC/001~003/` 下的动画剪辑与控制器、`Assets/222_Prefabs/NPC/001~003.prefab`、以及三张 NPC 图的 import meta 都已经形成互相引用的可落地资产，不是一次性临时缓存。
3. 在不触碰 `Primary.unity`、`DialogueUI.cs`、`GameInputManager.cs` 的前提下，切出 `codex/npc-asset-solidify-001` 作为本轮资产固化分支，准备只对白名单中的 NPC 资产与记忆进行收口。

**关键结论**:
- 当前这批 NPC 实体资产里，没有发现明显应直接丢弃的临时产物；`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.meta` 都属于应保留的首轮生成结果。
- 真正的“临时产物/无关产物”在本轮范围外，例如其他线程的 dirty、`Assets/Screenshots/`、字体资源改动等，不会混入 NPC 资产固化。

**当前恢复点**:
- 当前子工作区仍停留在“规划已落盘，开始收口首轮真实生成资产”的阶段。
- 下一步直接对白名单范围执行 task 固化，不在这轮顺手扩展新功能。

### 会话 6 - 2026-03-16

**用户需求**:
> 按照当前 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划` 工作区直接进入 NPC 自动移动 V1 实现，先回顾导航口径，再落地“随机漫游 + 短停 / 长停 + 长停自言自语气泡”，同时让生成器和现有 NPC prefab 都能直接使用。

**完成任务**:
1. 回读当前子工作区 `memory.md`、`npc规划001.md`、`tasks.md`，并回读 `NPCMotionController`、`NPCAnimController`、`NavGrid2D`、`PlayerAutoNavigator` 和 Unity MCP 现场，确认 V1 边界是“单 NPC 随机移动 + 短停 / 长停 + 自言自语气泡”，不接 `DialogueUI.cs`，不做双人聊天。
2. 新增 `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`，使用世界空间 TMP UI 在 NPC 头顶生成独立气泡，提供 `ShowRandomSelfTalk()` / `ShowText()` / `HideBubble()` 入口，并跟随 NPC 的 Sorting Layer 与绘制顺序。
3. 新增 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`，复用 `NavGrid2D.TryFindPath(...)` + `NPCMotionController.SetExternalVelocity(...)`，在活动半径内做随机选点、路径跟随、短停 `0.5~3s`、`3~5` 次短停后长停和长停气泡触发。
4. 更新 `Assets/Editor/NPCPrefabGeneratorTool.cs`，让新生成的 NPC prefab 自动挂载 `NPCBubblePresenter` 和 `NPCAutoRoamController`，不需要手动再补加。
5. 使用 Unity MCP 编译验证，并对 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 做 headless 修改，只追加 `NPCBubblePresenter` / `NPCAutoRoamController`，不动 `NPCDialogueInteractable` 或其他组件。

**修改文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` - 新增 NPC 头顶自言自语气泡表现器
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` - 新增 NPC 随机漫游、停留节奏与长停气泡控制器
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 新增到自动生成的 prefab 组件挂载
- `Assets/222_Prefabs/NPC/001.prefab` - 补加自动漫游 / 气泡组件
- `Assets/222_Prefabs/NPC/002.prefab` - 补加自动漫游 / 气泡组件
- `Assets/222_Prefabs/NPC/003.prefab` - 补加自动漫游 / 气泡组件

**验证结果**:
- 通过 `refresh_unity(compile=request, scope=scripts)` 完成脚本刷新与编译，当前 `read_console(types='["error","warning"]')` 返回 0 条。
- 通过 `manage_prefabs.get_info` 回读确认 `001/002/003.prefab` 根节点都已包含 `NPCBubblePresenter` 和 `NPCAutoRoamController`。
- 本轮未触碰 `Primary.unity`、`DialogueUI.cs`、`GameInputManager.cs` 等 A 类热文件。

**当前恢复点**:
- 当前子工作区已从“纯规划 / 资产收口”推进到“自动漫游 V1 运行时代码已落地并编译通过”。
- 下一步是同步子 memory / 父 memory / 线程记忆后，按白名单形成本轮 Git 收口，再交给用户直接拖入 prefab 手工测试。

---

### 会话 7 - 2026-03-16

**用户需求**:
> 先修复当前现场 bug，再继续推进 NPC 规划剩余实现；优先核查用户贴出的 `Animator is not playing an AnimatorController`，并在能继续的前提下把这轮 NPC 主线往前推到需要用户审批为止。

**完成任务**:
1. 按当前 `NPC/1.0.0初步规划` 工作区重新接管规则、memory、规划文档、导航规则与 Git 现场，确认这轮真实现场一开始位于 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`，随后切到 `codex/npc-roam-phase2-001` 承接 NPC 主线。
2. 通过 Unity MCP 重新编译并读取 fresh Console，确认用户贴出的 `Animator is not playing an AnimatorController` 并不是当前实时阻断；真正挡住编译的是 `GameInputManager.cs` 新调用了 `FarmToolPreview.ClearAllQueuePreviews(bool)`，但 `FarmToolPreview` 还没有兼容重载。
3. 在不触碰 A 类热文件 `GameInputManager.cs` 的前提下，只修改 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`，补上 `ClearAllQueuePreviews(bool clearExecutingPreviews)` 兼容重载，并保留旧的无参入口，先把当前工程重新编译通过。
4. 重新进入 `Primary` Play 验证，确认 `001 / 002 / 003` 三个场景 NPC 都已实际进入随机漫游；其中 `002` 已观察到 `LongPause + 气泡自言自语`，`001 / 003` 已观察到 `Moving` 状态与实时位移。
5. 将 `tasks.md` 更新为当前真实实现状态，明确 V1 自动漫游、长停气泡、轻量环境聊天、Profile 配置入口都已落地，当前剩余主要是验收收口与聊天正样本补强。

**修改文件**:
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` - 补回 `ClearAllQueuePreviews(bool)` 兼容重载，解除当前编译阻断
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md` - 按当前真实实现更新任务完成度与剩余验收项

**验证结果**:
- `refresh_unity(compile=request, mode=if_dirty, scope=scripts)` 后，fresh Console 重新变为 0 条项目编译错误 / warning。
- 进入 `Primary` Play 约 6 秒后，没有复现 `Animator is not playing an AnimatorController`；只出现了与 NPC 无关的 `There are no audio listeners in the scene` 警告。
- 在 Play 中回读 Animator 组件时，Console 出现 `GameObjectSerializer.cs` / `GetPlaybackTime` / `OnAnimatorIK` 一组红字；这已确认为 Unity MCP 读取 Animator 属性时的工具噪音，不是 NPC 业务代码报错。
- Play 运行态回读结果：
  - `001`：`NPCAutoRoamController.DebugState=Moving`，`NPCMotionController.IsMoving=true`
  - `002`：`NPCAutoRoamController.DebugState=LongPause`，`NPCBubblePresenter.IsBubbleVisible=true`
  - `003`：`NPCAutoRoamController.DebugState=Moving`，`NPCMotionController.IsMoving=true`

**当前恢复点**:
- 当前子工作区的真实阶段已经从“V1 代码初次落地”推进到“编译阻断已解除，自动漫游 / 长停气泡已在 `Primary` 场景完成首轮运行态验证”。
- 当前仍未闭环的只剩两类：一是抓到一组真实场景下的 NPC 配对聊天正样本，二是按白名单同步本轮变更并交给用户继续手测。
---

### 会话 8 - 2026-03-17

**用户需求**:
> 按治理裁定，将 NPC 从共享根目录切到独立救援现场，以 `codex/npc-roam-phase2-001 @ f6b4db2f` 作为唯一救援基线，只做最小收口：剔除误带入的 `FarmToolPreview.cs`，完成最小验证后再汇报。

**完成任务**:
1. 在独立救援 worktree `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 接管现场，确认当前分支为 `codex/npc-roam-phase2-001`，当前 `HEAD=f6b4db2f852910f5249aca4f51639cbddd893c05`。
2. 将 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 从 `8aed637f` 恢复，剔除误带入 NPC 线的 farm 侧改动，不再扩写新的 NPC 功能。
3. 回读 `Logs/npc_rescue_compile_wait4.log`，确认 batchmode 编译成功退出，未发现新的 `error CS`、`Animator is not playing an AnimatorController`、`Missing Sprite` 或 `Missing Script`。
4. 静态抽查 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab`，确认三者的 `m_Sprite`、`Animator.m_Controller` 都非空，并继续保留 `NPCAnimController`、`NPCMotionController`、`NPCBubblePresenter`、`NPCAutoRoamController` 组件链。
5. 抽查 `Assets/100_Anim/NPC/001~003` 下的 `Idle_Down` / `Move_Down` clip 与 controller，确认 Sprite 曲线引用不为空，controller 仍保留 `State` / `Direction` 参数和 `Idle_*` / `Move_*` 状态。

**修改文件**:
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` - 按 `8aed637f` 回退，剔除 farm 误带入改动
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 追加本轮救援收口记录
- `.kiro/specs/NPC/memory.md` - 同步父工作区收口摘要
- `.codex/threads/Sunset/NPC/memory_0.md` - 同步线程视角下的救援结论

**验证结果**:
- `Logs/npc_rescue_compile_wait4.log` 中出现 `Exiting batchmode successfully now!` 与 `return code 0`，当前救援 worktree 可编译。
- 非阻断日志仅包含 Licensing token、Cinemachine editor 提示、`MCP-FOR-UNITY: No process found listening on port 8080` 和 `Curl error 42`，不是 NPC 红错。
- `001/002/003` 的 Prefab / Sprite / 动画 / 漫游组件链均完整，`idle/move` 与 Sprite 引用无空值。

**当前恢复点**:
- 当前子工作区主线已经从“功能推进”切换到“救援基线最小收口”，后续 NPC 写入只允许发生在独立救援 worktree。
- 下一步只剩对白名单文件做 Git 收口，并确认 `codex/npc-roam-phase2-001` 回到干净可继续开发状态。
---

### 会话 9 - 2026-03-17

**用户需求**:
> NPC 救援已通过治理核验，本轮不再继续救火，只读核查当前 rescue worktree 里剩余的 4 个 TMP 字体资源 dirty，判断它们是否属于 NPC、是否早于本轮救援已存在，并给出最安全的后续续航方案。

**完成任务**:
1. 在 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 重新回读 `git status --short`，确认剩余 dirty 仅有 4 个 TMP 字体资源：`Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`、`DialogueChinese Pixel SDF.asset`、`DialogueChinese SDF.asset`、`DialogueChinese V2 SDF.asset`。
2. 回读 `git show --stat --name-only f7a1c0f5`，确认当前 NPC 救援提交只包含 `FarmToolPreview.cs` 与三层 memory，没有把上述 4 个字体资源卷入本轮提交。
3. 结合本轮接管前的 rescue 现场摘要与当前提交边界，形成稳定判断：这 4 个字体 dirty 与 NPC 业务无关，且在本轮最小救援收口前就已经作为现场残留存在，不属于当前 NPC 提交结果。
4. 给出续航建议时明确推荐方案 A：基于 `f7a1c0f562a476febe50084124dbeee382d31ac9` 另起一个全新的 NPC continuation 可写现场，让当前 rescue worktree 退回只读取证/收口现场。

**修改文件**:
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 追加 rescue worktree 无关 dirty 核查结论
- `.kiro/specs/NPC/memory.md` - 同步父工作区层面的续航决策
- `.codex/threads/Sunset/NPC/memory_0.md` - 同步线程视角下的现场切换建议

**关键决策**:
- 当前最安全的 NPC 续航方式是“换新 continuation 现场”，而不是继续在已带无关 dirty 的 rescue worktree 上开发。
- 当前 rescue worktree 的角色从“可继续写入的救援现场”降级为“取证 / 收口现场”，后续仅保留用于回读和对照。

**当前恢复点**:
- NPC 子工作区当前已经结束救援收口，进入“切换到全新 continuation 现场前的治理清场”阶段。
- 下一步不是做新功能，而是按方案 A 建立一个 `git status` clean 的新 NPC worktree，再继续主线开发。
---

### 会话 10 - 2026-03-17

**用户需求**:
> 按治理执行令，把 NPC 从 rescue 现场切换到正式 continuation 现场：在 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue` 上创建 `codex/npc-roam-phase2-002`，起点固定为 `28aef95d16f176ac5538bbd93c73769b2c686a8c`，本轮只完成现场切换，不扩写新功能。

**完成任务**:
1. 从 `28aef95d16f176ac5538bbd93c73769b2c686a8c` 成功创建新 worktree：`D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue`，并新建分支 `codex/npc-roam-phase2-002`。
2. 在新现场回读 `git rev-parse --show-toplevel`、`git branch --show-current`、`git rev-parse HEAD` 与 `git status --short`，确认新 continuation 现场起点正确，且创建完成后工作树为 clean。
3. 记录治理口径：旧的 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 已完成阶段性救援职责，后续降级为只读取证 / 收口现场，不再承接新的 NPC 正常开发。
4. 明确后续主线恢复点：NPC 的新功能开发、验证与提交只允许在 `codex/npc-roam-phase2-002` continuation 现场继续。

**修改文件**:
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 追加 continuation 现场切换完成记录
- `.kiro/specs/NPC/memory.md` - 同步父工作区层面的现场切换结论
- `.codex/threads/Sunset/NPC/memory_0.md` - 同步线程视角下的新正式开发入口

**关键决策**:
- continuation 现场以 `28aef95d` 为创建基线，但后续 NPC 正常开发入口从本轮开始统一切换到 `codex/npc-roam-phase2-002`。
- rescue worktree 不删除，但角色已从“可写救援现场”降级为“只读取证 / 收口现场”。

**当前恢复点**:
- NPC 子工作区已经完成从救援现场到正式 continuation 现场的切换，且新现场创建完成后是 clean。
- 下一步恢复 NPC 主线时，只在 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue @ codex/npc-roam-phase2-002` 上继续。

---

### 会话 11 - 2026-03-17

**用户需求**:
> 发现当前现场又直接落在 `main`，要求我先按规范检查分支与现场是否合规，再继续完成 NPC 主线，不要忽略 Git 和工作区口径。

**完成任务**:
1. 按 Sunset 规则重新回读 `AGENTS.md`、steering、NPC 父/子 memory 与 `tasks.md`，确认当前主线仍是 `NPC/1.0.0初步规划`，而不是切线到治理或其他系统。
2. 核对真实 Git 现场：共享根目录此轮开始时位于 `D:\Unity\Unity_learning\Sunset @ main`，`HEAD=64ff9816`，且 `Assets/` 级 NPC 改动已经直接挂在 `main` 上；为纠偏承载关系，已把当前 dirty 现场切到新分支 `codex/npc-roam-phase2-003` 继续。
3. 识别并隔离当前无关 dirty：`.codex/threads/农田交互修复V2/memory_0.md` 与 `.kiro/specs/农田系统/memory.md` 不属于 NPC，本轮只观察、不卷入 NPC 白名单。
4. 重新编译并回读当前静态链路：Console 为 `0` 条 error / warning；`001/002/003` 三个 NPC 的场景实例与 Prefab 都确认保留 Sprite、AnimatorController、`NPCBubblePresenter`、`NPCAutoRoamController` 与默认 `NPC_DefaultRoamProfile.asset`。
5. 补强聊天可靠性而不破坏随机感：在 `NPCAutoRoamController` 中把长停聊天改成“只对附近无人 / 伙伴暂不可聊做最多 3 次轻量重试”，不会对“概率没中”反复重掷；同时把默认 `ambientChatRadius` 从 `3.4` 微调到 `3.8`。
6. 做了一轮安全 Play 验证并清掉临时截图：在不读取运行态 Animator/组件资源的前提下，Play Console 只剩非 NPC 的 `There are no audio listeners in the scene`；同时确认 MCP 直接读 Play 中 Animator/组件资源会产生工具噪音，不能再把这类红字当成 NPC 业务回归。

**修改文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` - 长停聊天重试改为有限次、只对“伙伴缺席/临时拒绝”重试
- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs` - 默认聊天半径改为 `3.8`
- `Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset` - 同步默认聊天半径到 `3.8`
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md` - 写实补充聊天可靠性补丁与 MCP 运行态验证边界
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 记录本轮分支纠偏、补丁与验证边界

**关键结论**:
- 这轮真正需要修的不是“NPC 业务逻辑全面失效”，而是“当前 NPC dirty 不该继续挂在 `main` 上”；这一点已通过切到 `codex/npc-roam-phase2-003` 纠正。
- 当前 NPC 运行时代码的真实剩余缺口，已经从“基础漫游 / 动画报错”收窄为两件事：抓到一组真实聊天正样本，以及继续做不穿墙 / 不越界的人工视觉验收。
- `read_mcp_resource(mcpforunity://scene/gameobject/*/components)` 在 Play 中会触发 Unity MCP 自身的 Animator 序列化噪音；后续聊天抓样优先走 GameView 手测或 Inspector 调试入口，不再用它判断 NPC 业务是否报错。

**当前恢复点**:
- 当前子工作区已从“主线被 main 承载关系打歪”恢复到“业务改动挂在合规任务分支 `codex/npc-roam-phase2-003`，编译与静态链路通过”。
- 下一步回到主线收口：继续做一轮不依赖运行态组件读回的聊天正样本验证，然后同步白名单 Git 提交供用户继续测试。
---

### 会话 12 - 2026-03-18

**用户需求**:
> 询问 Unity 失焦时是否会影响 MCP 与调试；补充了最新手测反馈：NPC 互相对话已经基本可见，但“刚靠近就立刻面对面聊一句”略显牵强，同时提出活动区域与路线规划应作为后续代办，并要求按代办规则落盘。

**完成任务**:
1. 重新按当前现场核对 NPC 主线状态，确认当前分支仍为 `codex/npc-roam-phase2-003 @ 7bc94fc8`，NPC 业务代码不需要本轮继续扩写，当前重点是把新的体验优化点按代办规则接住。
2. 明确了本轮验证口径：Unity/GameView 若失去前台焦点，Play 中角色可能暂停或不再继续更新，因此会影响节奏观察类手测；这不妨碍我做静态检查与文档整理，但会影响运行中行为真相的获取。
3. 在正式 TD 区新建 `000_代办/kiro/NPC/`，并新增 `TD_000_NPC.md` 与 `TD_1.0.0初步规划.md`，把“贴近约 2 秒后再触发对聊”“活动区域场景化规划”“路线规划更规范”三项登记为后续代办。

**修改文件**:
- `.kiro/specs/000_代办/kiro/NPC/TD_000_NPC.md` - 新建 NPC 主代办，汇总当前后续优化项
- `.kiro/specs/000_代办/kiro/NPC/TD_1.0.0初步规划.md` - 新建当前子工作区代办，记录对聊节奏/活动区域/路线规划
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 追加本轮代办收口与验证口径

**关键结论**:
- 当前 NPC V1 的真实状态是“对聊已可发生，但自然度仍可继续优化”，不是“对聊功能还没通”。
- “贴近后需要一段酝酿时间再开聊”已经成为明确的后续优化方向，但由于当前场景未设计完成，本轮只登记为代办，不直接实现。
- Unity 失焦会影响 Play 中行为观察，因此后续凡是需要看 NPC 节奏、对聊时机、漫游动态的验收，都应尽量在 Unity/GameView 保持前台时完成。

**当前恢复点**:
- 当前子工作区的代码主线没有新增阻断 bug；这轮新增的是正式代办承接。
- 下一步若继续推进主线，应先复核还有没有未收口的非代办缺口，再决定是否进入下一轮实现或验收。

---

### 会话 13 - 2026-03-19

**用户需求**:
> 按 `queue-aware业务准入_01` 分发 prompt 领取 NPC 专属准入卡，在 shared root 只读 preflight 后进入 continuation branch，完成本轮最小 checkpoint，并回写回执单。

**完成任务**:
1. 按 Sunset 启动闸门要求先在 shared root 做 live preflight，复核当前现场为 `D:\Unity\Unity_learning\Sunset @ main @ 9b5279a58128e902770df92f706c6796378de7fc`，当时 `git status --short --branch` 为 clean，`shared-root-branch-occupancy.md` 处于 neutral。
2. 读取本轮 NPC prompt 与回执模板后，先尝试 stable launcher 走 `request-branch`，两次都报 `request-branch 必须提供 -BranchName。`；随即改用 canonical `scripts/git-safe-sync.ps1`，成功获得 `GRANTED` 并通过 `ensure-branch` 进入 `codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`。
3. 严格遵守 prompt 边界，只做最小 live 基线复核：未进入 Unity / MCP / Play Mode，未触碰 `Primary.unity`、`GameInputManager.cs`；已确认 `NPCAutoRoamController.cs`、`NPCBubblePresenter.cs`、`NPC_DefaultRoamProfile.asset`、`001~003.prefab` 与 `NPCPrefabGeneratorTool.cs` 仍在位。
4. 写实记录 queue-aware runtime 异常：`active/shared-root-queue.lock.json` 中 NPC 的 `ticket=1` 被 reconciler 标成 `cancelled`，但 occupancy 仍登记为 `task-active / owner_thread=NPC`；本轮只记录该事实，不擅自修租约状态。

**修改文件**:
- `.kiro/specs/Codex规则落地/23_前序阶段补漏审计与并发交通调度重建/2026.03.19_queue-aware业务准入_01/memory.md` - 记录本轮准入、launcher 异常与 queue runtime 写实结论
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 追加本轮 queue-aware 最小 checkpoint 摘要

**关键结论**:
- NPC 当前 continuation carrier 仍以 `codex/npc-roam-phase2-003` 为准，本轮没有回退到旧 continuation 或 rescue 语义。
- 这轮的真实目标不是继续开发新 NPC 功能，而是把“shared root 准入 -> continuation branch 最小基线复核 -> 白名单收口 -> 归还 shared root”这条治理链走完整。

**当前恢复点**:
- 本轮最小 checkpoint 已完成基线复核，下一步只剩显式白名单 task sync、补回执卡并执行 `return-main`。

**阻塞更新**:
- 本轮 task sync preflight 已实际执行，但被 shared root 上新增的无关治理 dirty 阻断；当前不能把 `return-main` 写成已完成，需等待这些非 NPC remaining dirty 先清尾或隔离。

---

### 会话 16 - 2026-03-21

**用户需求**:
> 在已获放行的 NPC 分支窗口内继续做一个真实 checkpoint，把 NPC 头顶气泡从“贴脸、尺寸和观感一般”往更美观、更合理、更灵动的方向收口，但这轮不要扩成新功能开发，也不要进入 Unity / MCP / Play。

**完成任务**:
1. 继续以 `codex/npc-roam-phase2-003` 作为当前 NPC continuation 分支，锁定本轮只改 `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`，不碰场景、Prefab、Profile、动画、生成器和热文件。
2. 将 `NPCBubblePresenter` 的样式版本提升到 `CurrentStyleVersion = 3`，让已有 NPC prefab 上的旧默认值也能自动吃到这次新的气泡布局与动效预设，而不是只有新挂载组件才生效。
3. 新增“基于 `SpriteRenderer.bounds` 自动抬高气泡”的头顶偏移计算：气泡现在会优先贴着角色头顶往上浮，不再只依赖固定 `bubbleLocalOffset`，从而减少“气泡糊在脸上”的情况。
4. 同步微调了气泡的 padding、最大宽度、尾巴尺寸、阴影、显隐时长和轻微浮动效果，让默认观感更轻一些、更像头顶提示，而不是硬贴在角色脸前的一块牌子。
5. 用 `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 做了纯 Git 静态自检，确认这轮单文件 patch 没有格式类问题；本轮仍未进入 Unity / MCP / Play，运行态验收留到用户继续主项目手测阶段。

**修改文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`

**关键结论**:
- 这轮主线仍然是 NPC V1 收口；本轮子任务是“气泡位置与表现优化 checkpoint”，它服务于此前用户已经确认可用、但仍需更美观的 NPC 漫游/对聊表现层。
- 这次不是在补新系统，而是在现有 `NPCBubblePresenter` 上做一次更接近正式交付感的样式修整，所以不需要重新动 `NPCAutoRoamController` 或 `NPCRoamProfile`。
- 本轮验证边界依旧保持写实：目前只有代码级静态回读和 Git 自检通过；真正的“看起来是不是更顺眼、是否还会贴脸”仍需要回到 Unity 主项目里做手测。

**当前恢复点**:
- 子工作区当前新增了一次真实的 UI 表现层 checkpoint，下一步是按白名单同步 `NPCBubblePresenter.cs` 与三层 memory，然后立刻 `return-main`。
