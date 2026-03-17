# NPC 线程记忆

## 线程概述

- **用户主线**: 围绕 Sunset 的 NPC 系统持续推进，从工具、动画、运行时，到后续的自动移动与生活化行为。
- **当前现场**: `D:\Unity\Unity_learning\Sunset @ main`
- **线程说明**: 当前线程用于承接 NPC 相关的长期开发与规划记录。

## 当前状态

- **完成度**: 35%
- **最后更新**: 2026-03-13
- **状态**: 已进入 NPC 自动移动与社交行为规划阶段

## 会话记录

### 会话 1 - 2026-03-13

**用户需求**:
> 为 NPC 规划自动移动系统，要求先把“随机间隔移动、短停、长停、气泡自言自语/聊天”等基础设想完整写进一个新的 `npc规划001.md`，再做第一轮拓展，作为后续脑洞风暴基线。

**完成任务**:
1. 读取 Sunset 当前规则、导航工作区记忆与导航规范，确认本轮规划应挂接到“导航系统重构”工作区。
2. 回读现有导航与对话基础，确认项目内已有 `NavGrid2D`、`PlayerAutoNavigator`、`INavigationUnit` 以及 `DialogueNode.isBubble` 等可复用能力。
3. 新建 `npc规划001.md`，先完整记录用户提出的基础设想，再补出第一轮可开发化拓展与 V1 边界。

**修改文件**:
- `.kiro/specs/001_BeFore_26.1.21/导航系统重构/npc规划001.md` - NPC 自动移动与环境社交的第一版规划文档

**解决方案**:
- 不新建空壳 workspace，而是将本轮规划挂在现有“导航系统重构”工作区中，确保后续讨论与实现都能直接接到当前导航基建。
- 文档内将需求拆成“用户原始设想”“真实需求理解”“现成可复用基础”“V1 行为闭环”“系统拆分方向”和“后续脑暴问题”六个层级，方便后续继续深入。

**遗留问题**:
- [ ] 还未开始自动移动脚本实现，当前仅完成规划落盘。
- [ ] 现有 `isBubble` 字段已确认存在，但头顶气泡运行时表现层是否可直接复用仍待后续核实。

### 会话 2 - 2026-03-14

**用户需求**:
> 你的文档放错地方了，不是导航系统，改到 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC`，全部移动过来。

**完成任务**:
1. 确认 `.kiro/specs/NPC/` 是空白但可用的正确 NPC 工作区。
2. 在 `.kiro/specs/NPC/` 下新建 `memory.md` 与 `npc规划001.md`，把 NPC 自动移动规划正文迁入正确位置。
3. 将导航工作区中的旧 `npc规划001.md` 改为迁移说明，避免后续继续在错误位置维护。
4. 追加导航工作区记忆，明确这次是工作区归属纠正，不再把 NPC 规划挂靠在导航系统下。

**修改文件**:
- `.kiro/specs/NPC/memory.md`
- `.kiro/specs/NPC/npc规划001.md`
- `.kiro/specs/001_BeFore_26.1.21/导航系统重构/npc规划001.md`
- `.kiro/specs/001_BeFore_26.1.21/导航系统重构/memory.md`

**恢复点 / 下一步**:
- 后续 NPC 脑洞风暴与实现规划统一以 `.kiro/specs/NPC/` 为主。

### 会话 3 - 2026-03-14

**用户需求**:
> 认为还需要子工作区，已将内容移动到 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划`，要求重做主 memory，并根据 `npc规划001` 完成任务文档。

**完成任务**:
1. 核查 NPC 现场结构，确认当前应采用“主工作区 `NPC` + 子工作区 `1.0.0初步规划`”的父子模式。
2. 新建父层 `.kiro/specs/NPC/memory.md`，改为真正的主工作区总览记忆。
3. 重写子层 `.kiro/specs/NPC/1.0.0初步规划/memory.md`，纠正其原先误写成主 memory 的问题。
4. 基于 `.kiro/specs/NPC/1.0.0初步规划/npc规划001.md` 新建 `tasks.md`，完成第一版任务拆解。

**修改文件**:
- `.kiro/specs/NPC/memory.md`
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md`

**恢复点 / 下一步**:
- 当前 NPC 工作区已经具备标准父子结构与任务文档，后续可继续补设计或直接进入第一阶段实现设计。

### 会话 4 - 2026-03-15

**用户需求**:
> 没听懂“白名单同步被脚本拦住”的说明，要求解释清楚。

**完成任务**:
1. 解释了“白名单同步”指只尝试提交指定文件。
2. 解释了 `git-safe-sync.ps1` 拦截 `main` 是脚本规则问题，不是 NPC 文件内容报错。
3. 解释了“无关脏改”是现场其他改动，我没有把它们混入 NPC 这轮提交。

**恢复点 / 下一步**:
- 当前只是规则说明补充，不涉及 NPC 代码或文档结构变更。
### 会话 5 - 2026-03-15

**用户需求**:
> 怀疑自己把开发现场从 NPC 分支/独立 worktree 拉回 `main`，是不是这件事导致了无法提交；要求结合聊天记录、当前 NPC 线程落地和 Git 现场重新回应。

**完成任务**:
1. 回读 NPC 父子工作区记忆与线程记忆，重新串起这条线最近真实落地的内容：当前新增主要是 `.kiro/specs/NPC/` 下的规划、任务和记忆，不是新的运行时代码实现。
2. 重新核对当前仓库现场：当前真实分支为 `main`，工作树包含大量与 NPC 无关的脏改，同时 NPC 工作区自身也还未完成正式 Git 收口。
3. 形成新的稳定结论并向用户澄清：问题不在于“回到 `main`”这件事本身，而在于“回到 `main` 后继续做了会被脚本视为 task 的 NPC 业务工作区推进”；此外，当前工作树不干净，连安全切回 `codex/` 任务分支也会被 `ensure-branch` 拦住。

**关键结论**:
- `main` 可以是统一可见测试现场，但不自动等于“允许直接提交 task 改动”。
- `.kiro/specs/NPC/` 这类 NPC 业务工作区内容不属于 `governance` 默认白名单；按正式脚本，它仍算 task。
- 真实阻塞是“两层叠加”：当前人在 `main`，且现场还有大量无关 dirty；不是单独某一次“被拉回 main”就决定一切。

**恢复点 / 下一步**:
- NPC 主线仍停在 `1.0.0初步规划`，工作区结构已稳定。
- 后续若要继续安全推进并可提交，必须先按真实 Git 规则处理分支与 dirty 收口，再进入下一步实现或文档推进。

### 会话 6 - 2026-03-16

**用户需求**:
> 将当前 `NPC` 线程的冻结快照正式写入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\`，文件名按线程名落盘，写完只要一句话回复。

**完成任务**:
1. 只读核对了冻结汇总目录的现有命名方式，确认采用“线程名.md”。
2. 将上一轮已经整理好的 NPC 线程 12 段冻结快照，正式写入：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\NPC.md`
3. 按项目规则同步更新了冻结汇总子工作区 `memory.md`、父治理工作区 `memory.md` 与当前 NPC 线程记忆。

**关键结论**:
- `NPC` 线程的冻结现场现在已经从聊天回复转成正式治理文档，可直接供全局排期、防冲突和恢复开发时引用。
- 本轮是文档落盘，不是恢复开发；冻结状态不变。

**恢复点 / 下一步**:
- 继续等待 A 类共享热文件 owner / 物理锁裁决。
- 在统一裁决前，NPC 线程保持冻结，不自行恢复开发。

### 会话 7 - 2026-03-16

**用户需求**:
> 全局解冻后，先不要继续写 NPC 新功能，而是优先收口当前实体资产 WIP：`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.meta`；要求先核对现场，再在 `codex/...` 分支上用 task 白名单方式把应保留资产与相关记忆固化。

**完成任务**:
1. 重新核对当前真实工作目录 / 分支 / HEAD / dirty 范围，确认本轮开始时位于 `D:\Unity\Unity_learning\Sunset @ main`，`HEAD=9b9a6bd0dd7c5ee7d18cc82e3ea9da74a146bf9d`，NPC 范围 dirty 已经收窄为 sprite meta、动画目录、预制体目录和 NPC 记忆目录。
2. 回读了 `Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/` 与 `Assets/Sprites/NPC/*.meta` 的代表文件，确认它们不是临时缓存，而是互相引用的首轮真实生成成果。
3. 为了让本轮 task 固化有独立承载，在不触碰 A 类热文件的前提下，从当前 `main` 的 `HEAD` 切出了 `codex/npc-asset-solidify-001` 分支，后续只对白名单中的 NPC 资产与记忆做提交。

**关键结论**:
- 需要保留的 NPC 资产包括：
  - `Assets/Sprites/NPC/001.png.meta`
  - `Assets/Sprites/NPC/002.png.meta`
  - `Assets/Sprites/NPC/003.png.meta`
  - `Assets/100_Anim/NPC/`
  - `Assets/222_Prefabs/NPC/`
  - `.kiro/specs/NPC/`
  - `.codex/threads/Sunset/NPC/`
- 本轮没有在上述范围内发现必须立即丢弃的临时产物。

**恢复点 / 下一步**:
- 当前已进入 `codex/npc-asset-solidify-001`，下一步直接按 task 白名单固化这批 NPC 资产与相关记忆。

### 会话 8 - 2026-03-16

**用户需求**:
> 在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划` 工作区下直接开始 NPC 自动移动 V1 开发，先按 Sunset 规则接管上下文和导航基线，再实现自动漫游、长停气泡和 prefab 直接可用链路。

**完成任务**:
1. 按当前工作区读完了 Steering / workspace memory / NPC 记忆 / `npc规划001.md` / `tasks.md` / `NPCMotionController.cs` / `NPCAnimController.cs` / `NavGrid2D.cs` / `PlayerAutoNavigator.cs` / `NPCPrefabGeneratorTool.cs`，并核实 Unity MCP 已可用，当前真实现场为 `D:\Unity\Unity_learning\Sunset @ codex/npc-asset-solidify-001`，HEAD=`88055425`。
2. 新增 `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`，使用世界空间 TMP UI 做独立 NPC 头顶气泡，能随机抽自言文本并在长停时显示。
3. 新增 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`，将 `NavGrid2D.TryFindPath(...)`、`NPCMotionController.SetExternalVelocity(...)` 和 `NPCBubblePresenter` 组合起来，实现活动半径内随机选点、短停 `0.5~3s`、`3~5` 次短停后长停、长停自言气泡的 V1 闭环。
4. 更新 `Assets/Editor/NPCPrefabGeneratorTool.cs`，让新生成的 NPC prefab 自动挂载 `NPCBubblePresenter` 和 `NPCAutoRoamController`。
5. 通过 Unity MCP 编译和头无 prefab 修改，将 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 补加到新组件，让用户在不重跑生成器的前提下也能直接拖入场景测试。

**修改文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- `Assets/Editor/NPCPrefabGeneratorTool.cs`
- `Assets/222_Prefabs/NPC/001.prefab`
- `Assets/222_Prefabs/NPC/002.prefab`
- `Assets/222_Prefabs/NPC/003.prefab`
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/memory.md`
- `.codex/threads/Sunset/NPC/memory_0.md`

**验证结果**:
- Unity MCP `refresh_unity(compile=request, mode=if_dirty, scope=scripts)` 后，`read_console(types='["error","warning"]')` 返回 0 条，当前没有新的编译错误或 warning。
- `manage_prefabs.get_info` 回读确认 `001/002/003.prefab` 根节点组件列表已包含 `NPCBubblePresenter` 和 `NPCAutoRoamController`。
- 本轮未触碰 `Primary.unity`、`DialogueUI.cs`、`GameInputManager.cs` 等 A 类共享热文件。

**关键决策**:
- 当前线程主线仍然是 NPC，本轮子任务是“V1 自动漫游和气泡链路落地”，它服务于“让 NPC 具备基础生活化行为”而非换线到其他系统。
- V1 气泡独立于 `DialogueUI`，以免把全屏剧情 UI 拉进 NPC 游先自动行为主线；所以当前只做独立 `NPCBubblePresenter`。

**恢复点 / 下一步**:
- 当前 `codex/npc-asset-solidify-001` 已从资产收口推进到 NPC 自动漫游 V1 真实实现。
- 下一步不再重做规划，而是按白名单同步当前变更，再向用户提供手工功能测试入口。

### 会话 9 - 2026-03-16

**用户需求**:
> 先看当前 Console 报错，再修掉现场 bug，并继续把 NPC 规划里已经能落地的内容推进下去，直到需要用户审核或批准为止。

**完成任务**:
1. 按 Sunset 当前规则重新读取了 NPC 工作区、父/子 memory、`npc规划001.md`、`tasks.md`、导航规则与 Git 现场，确认真实工作目录仍是 `D:\Unity\Unity_learning\Sunset`，当前轮进入时实际分支已漂到 `codex/farm-1.0.2-correct001`。
2. 通过 Unity MCP 做 fresh Console 核查，确认用户贴出的 `Animator is not playing an AnimatorController` 不是当前实时阻断；真正挡住编译的是 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 对 `FarmToolPreview.ClearAllQueuePreviews(bool)` 的新调用，而 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 尚未提供对应重载。
3. 在不触碰 A 类热文件 `GameInputManager.cs` 的前提下，只修改 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`，补上兼容重载并保留旧入口，先把整个项目重新编译通过。
4. 编译恢复后重新进入 `Primary` Play 验证：未复现 `Animator is not playing an AnimatorController`；`001 / 002 / 003` 都已真实进入自动漫游，其中 `002` 已观察到长停与自言自语气泡，`001 / 003` 已观察到移动状态。
5. 将本轮承载分支切回 NPC 语义正确的 `codex/npc-roam-phase2-001`，并更新 `NPC/1.0.0初步规划/tasks.md`，把自动漫游、长停气泡、轻量聊天、Profile 配置入口的完成状态写实落盘。

**修改文件**:
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md`
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/memory.md`
- `.codex/threads/Sunset/NPC/memory_0.md`

**验证结果**:
- `refresh_unity(compile=request, mode=if_dirty, scope=scripts)` 后，fresh Console 恢复为 0 条项目编译错误 / warning。
- `Primary` Play 约 6 秒后，Console 仅出现与 NPC 无关的 `There are no audio listeners in the scene` 警告。
- 在 Play 中回读 Animator 组件时，Unity MCP 自己会抛出 `GameObjectSerializer.cs` / `GetPlaybackTime` / `OnAnimatorIK` 一组工具噪音；已确认不是 NPC 项目错误。

**关键结论**:
- 当前 NPC 这条线的真实状态已经从“代码初次落地”推进到“编译阻断解除 + 首轮运行态验证完成”。
- 当前剩余的主线缺口不再是基础漫游或动画报错，而是两件更小的收口项：真实场景下抓到一组 NPC 配对聊天正样本，以及按白名单提交本轮变更供用户继续测试。
---

### 会话 10 - 2026-03-17

**用户需求**:
> 按治理正式执行令，把 NPC 迁入独立救援现场，以 `codex/npc-roam-phase2-001 @ f6b4db2f` 为唯一基线，只做 `FarmToolPreview.cs` 剔除和最小验证，不再在共享根目录写 NPC。

**完成任务**:
1. 核实独立救援现场为 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`，当前分支 `codex/npc-roam-phase2-001`，当前 `HEAD=f6b4db2f852910f5249aca4f51639cbddd893c05`。
2. 将 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 回退到 `8aed637f` 版本，剔除 farm 误带入的兼容重载改动，本轮不再新增任何 NPC 功能。
3. 回读 `Logs/npc_rescue_compile_wait4.log` 确认 batchmode 编译成功，未见新的 NPC 红错，也未再见 `Animator is not playing an AnimatorController`、`Missing Sprite`、`Missing Script`。
4. 抽查 `001/002/003` 的 Prefab / Sprite / 动画 / controller / 漫游组件链，确认 Sprite 与 Animator Controller 引用非空，`NPCAnimController`、`NPCMotionController`、`NPCBubblePresenter`、`NPCAutoRoamController` 仍完整保留。

**关键决策**:
- 当前线程主线仍然是 NPC，本轮子任务收窄为“救援分支最小收口”，服务于恢复干净可继续开发的基线，而不是继续扩写功能。
- `D:\Unity\Unity_learning\Sunset` 继续视为 farm 事故现场；NPC 这条线今后的可写现场只认独立救援 worktree。

**关键文件**:
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Logs\npc_rescue_compile_wait4.log`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\222_Prefabs\NPC\001.prefab`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\222_Prefabs\NPC\002.prefab`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\222_Prefabs\NPC\003.prefab`

**当前恢复点**:
- 线程视角下的真实状态已经切换为“救援分支完成最小收口并通过固定范围验证”。
- 下一步只剩在救援 worktree 内对白名单文件做 Git 收口并确认分支干净，不再回共享根目录承载 NPC 写入。
---

### 会话 11 - 2026-03-17

**用户需求**:
> 救援提交完成后，不再继续做 NPC 救火；先只读核查 rescue worktree 中剩余的 4 个 TMP 字体 dirty，并给出后续续航方案 A/B 的明确建议。

**完成任务**:
1. 重新核对 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue @ codex/npc-roam-phase2-001` 的 `git status`，确认唯一剩余 dirty 是 4 个 TMP 字体资源文件。
2. 回读 `git show --stat --name-only f7a1c0f5`，确认当前 NPC 救援提交未包含这 4 个文件，它们没有进入已推送的 NPC 结果。
3. 结合 rescue 接管前已记录的现场摘要，得出线程级稳定判断：这 4 个 TMP dirty 与 NPC 业务无关，并且早于本轮最小救援收口已存在。
4. 在线程层面明确推荐方案 A：基于 `f7a1c0f562a476febe50084124dbeee382d31ac9` 另起一个新的 NPC continuation 可写现场，让当前 rescue worktree 降级为取证/收口现场，避免未来 NPC 提交再次混线。

**关键决策**:
- 当前主线仍然是 NPC，但本轮子任务已经切换为“救援后续航治理”，服务于恢复一个长期可写、`git status` clean 的 NPC 开发现场。
- 方案 B 只有在能证明这 4 个文件有稳定 owner、稳定隔离策略、且不会再次被 Unity 自动改脏时才成立；当前证据不足，因此不推荐。

**关键文件**:
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\TextMesh Pro\Resources\Fonts & Materials\DialogueChinese BitmapSong SDF.asset`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\TextMesh Pro\Resources\Fonts & Materials\DialogueChinese Pixel SDF.asset`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\TextMesh Pro\Resources\Fonts & Materials\DialogueChinese SDF.asset`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue\Assets\TextMesh Pro\Resources\Fonts & Materials\DialogueChinese V2 SDF.asset`

**当前恢复点**:
- 当前线程的真实状态是“NPC 救援已完成，rescue worktree 只剩无关 TMP dirty 待隔离”。
- 下一步最小动作是按方案 A 新建 clean continuation 现场，再恢复 NPC 主线开发。
