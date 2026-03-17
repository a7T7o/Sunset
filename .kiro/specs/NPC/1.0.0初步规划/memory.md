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
