# NPC - 开发记忆

## 模块概述

NPC 主工作区用于承接 Sunset 项目中所有 NPC 相关的规划、设计、实现与验收记录。
当前采用“父工作区总览 + 子工作区分阶段推进”的结构，后续 NPC 的规划与开发都以这里为主入口。

## 当前状态

- **完成度**: 15%
- **最后更新**: 2026-03-14
- **状态**: 已建立父子工作区结构，当前进入 `1.0.0初步规划`

## 子工作区总览

| 子工作区 | 说明 | 当前状态 |
|------|------|------|
| `1.0.0初步规划` | NPC 自动移动、停留节奏、气泡表达与轻量社交的第一轮规划 | 规划进行中 |

## 会话记录

### 会话 1 - 2026-03-14

**用户需求**:
> 认为 NPC 还需要子工作区，已将文档移动到 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划`，要求重做主 `memory`，并根据 `npc规划001` 完成任务文档。

**完成任务**:
1. 核查新的父子工作区现场，确认 `NPC` 作为主工作区、`1.0.0初步规划` 作为当前活跃子工作区。
2. 新建父层主 `memory.md`，改用“主工作区总览 + 子工作区列表”的结构承接 NPC 主线。
3. 将子工作区 `memory.md` 重写为真正的“初步规划”记忆，而不再沿用此前误当主 memory 的内容。
4. 基于 `npc规划001.md` 为 `1.0.0初步规划` 新建 `tasks.md`，把 NPC 自动移动 V1 的后续工作拆成明确任务。

**修改文件**:
- `.kiro/specs/NPC/memory.md` - NPC 主工作区记忆
- `.kiro/specs/NPC/1.0.0初步规划/memory.md` - 子工作区记忆重写
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md` - 根据规划文档拆出的任务清单
- `.codex/threads/Sunset/NPC/memory_0.md` - 线程记忆同步

**当前恢复点**:
- NPC 的正确工作区结构已经固定为：主工作区 `NPC`，当前活跃子工作区 `1.0.0初步规划`。
- 后续继续脑暴或进入实现设计时，统一在 `1.0.0初步规划` 下推进。

---

### 会话 2 - 2026-03-15

**用户需求**:
> 没听懂上一轮关于 `git-safe-sync.ps1` 拦截 `main` 的说明，希望直接解释那句话的含义。

**完成任务**:
1. 解释了“白名单同步”是只尝试提交本轮明确指定的文件。
2. 解释了“被脚本拦住”是同步脚本自己的规则阻止在 `main` 上提交，不代表文件内容本身出错。
3. 解释了“无关脏改”是仓库里同时存在其他人或其他线程留下的修改，我没有把它们混进本轮提交，也没有去改动它们。

**当前恢复点**:
- NPC 工作区结构不变。
- 当前只是补充 Git 口径说明，不影响 NPC 规划主线。
---

### 会话 3 - 2026-03-15

**用户需求**:
> 重新确认“从 NPC 分支/独立 worktree 回到 `main`”与“当前无法提交”之间的真实关系，希望结合 NPC 工作区落地情况和当前 Git 现场给出重新梳理后的结论。

**完成任务**:
1. 回读父工作区、子工作区和线程记忆，确认 NPC 当前已落盘内容主要是规划工作区结构、`npc规划001.md`、`tasks.md` 与相关记忆同步。
2. 核对当前仓库现场，确认当前真实分支为 `main`，且工作树包含大量 NPC 外脏改，NPC 自身也处于未正式收口状态。
3. 在父工作区层面明确 Git 规则结论：`main` 作为统一可见现场没有问题，但 NPC 业务工作区推进本身仍属于 task；按当前 `git-safe-sync.ps1` 和 steering 规则，不应在 `main` 上直接提交这类改动。

**关键决策**:
- 后续要区分两件事：`main` 作为查看/联调现场，与 `main` 作为直接 task 提交现场，不是同一件事。
- NPC 这条线当前的阻塞不是“历史上曾被拉回 main”本身，而是“在 `main` 上继续累积了 task 类改动，而且现场还有其他无关 dirty”，导致不能直接安全收口。

**当前恢复点**:
- NPC 仍以 `1.0.0初步规划` 子工作区为当前活跃范围。
- 后续若继续进入实现或继续推进 NPC 工作区正文，需先解决正确的任务分支与工作树收口问题。

---

### 会话 4 - 2026-03-16

**用户需求**:
> 全局解冻后，先不要继续写 NPC 新功能，而是优先收口实体资产 WIP，并要求在 `codex/...` 任务分支上用 task 白名单方式固化应保留的 NPC 资产与相关记忆。

**完成任务**:
1. 在父工作区层面重新核对了当前 NPC 线的真实收口对象，确认本轮主角不是新的运行时代码，而是已经生成出来、但尚未正式固化的实体资产：`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.meta`。
2. 确认 NPC 相关生成结果已经形成一条完整链路：sprite meta 负责切片，`Assets/100_Anim/NPC/` 提供 Idle/Move 动画与控制器，`Assets/222_Prefabs/NPC/` 提供可拖入场景的预制体。
3. 将当前收口动作从 `main` 迁入 `codex/npc-asset-solidify-001`，让后续 task 白名单固化有明确分支承载。

**关键决策**:
- 这次要保留的不是“工具草稿”，而是“已经可被项目直接消费的 NPC 资产成果”。
- 在没有发现明确坏产物的前提下，当前这批 `001~003` 的动画、控制器、预制体和切片 meta 统一按应保留资产处理，不在固化前自行删减。

**当前恢复点**:
- NPC 主工作区当前已从“纯规划”过渡到“首轮实体资产收口”。
- 后续完成这次资产固化后，NPC 才适合继续进入新的行为功能开发。

## 相关文件

| 文件 | 说明 |
|------|------|
| `.kiro/specs/NPC/memory.md` | NPC 主工作区记忆 |
| `.kiro/specs/NPC/1.0.0初步规划/npc规划001.md` | 第一版 NPC 自动移动与环境社交规划 |
| `.kiro/specs/NPC/1.0.0初步规划/tasks.md` | 基于 `npc规划001` 拆出的任务文档 |

### 会话 5 - 2026-03-16

**用户需求**:
> 在 `NPC/1.0.0初步规划` 下开始落地 NPC 自动移动 V1，将随机漫游、短停/长停、长停气泡自言自语和 prefab 直接可用打通。

**完成任务**:
1. 以子工作区 `1.0.0初步规划` 为主战场，回读了 NPC 运行时代码、导航系统与 Unity MCP 现场，确认这一轮 V1 只做单人漫游和自言自语气泡，不变更 `DialogueUI.cs` 和 A 类热文件。
2. 新增 `NPCBubblePresenter` 和 `NPCAutoRoamController` 两个运行时组件，把 `NavGrid2D.TryFindPath(...)`、`NPCMotionController`、`NPCAnimController` 联通起来，让 NPC 具备“活动半径内随机移动 + 短停 / 长停 + 长停气泡”的基础生活化行为。
3. 更新 `NPCPrefabGeneratorTool.cs` 支持新生成 prefab 自动挂载新组件，并通过 Unity MCP 头无地把 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 的现有资产也补加到位，让用户不用先重跑生成器。

**关键决策**:
- V1 气泡层继续采用独立 `NPCBubblePresenter`，不接既有全屏 `DialogueUI`，以免把 NPC 游先聊天误入到大型剧情 UI 链中。
- V1 漫游层直接复用 `NavGrid2D.TryFindPath(...)`，同时通过 `NPCMotionController.SetExternalVelocity(...)` 驱动动画，不新建一套单独的 NPC 移动动画桥接。

**当前恢复点**:
- NPC 父工作区当前已从“首轮实体资产收口”进一步到“自动漫游 V1 代码和 prefab 落地”。
- 下一步仍在 NPC 线上，不换线；直接按白名单收口 Git 并让用户在 Sunset 主项目中手工拖入 NPC prefab 测试。

---

### 会话 6 - 2026-03-16

**用户需求**:
> 继续 NPC 主线：先修当前 bug，再把 `npc规划001` 这轮已实现内容验证清楚，并在能继续的前提下持续推进到需要用户审批为止。

**完成任务**:
1. 在父工作区层面重新核对了当前真实现场，确认这轮用户贴出的 `Animator is not playing an AnimatorController` 不是当前 fresh Console 下的实时阻断；真正挡住 Unity 编译的是 A 类热文件 `GameInputManager.cs` 对 `FarmToolPreview.ClearAllQueuePreviews(bool)` 的新调用与旧签名不一致。
2. 为了不触碰 A 类热文件，转而在非 A 类文件 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 补上兼容重载，成功解除当前项目级红编。
3. 编译恢复后，重新进入 `Primary` 做 Play 验证，确认当前场景内 `001 / 002 / 003` 三个 NPC 已真实进入自动漫游链路，其中至少一名 NPC 已实际进入长停并稳定显示自言自语气泡。
4. 将子工作区 `tasks.md` 更新为当前真实落地状态，明确 NPC 自动漫游、长停气泡、轻量聊天规则和 `NPCRoamProfile` 数据入口都已进入可用状态，当前剩余主要是验收闭环与 Git 收口。
5. 为了让后续白名单提交回到正确语义，当前工作现场从 `codex/farm-1.0.2-correct001` 切到了 `codex/npc-roam-phase2-001`。

**关键决策**:
- 这轮优先解决的是“项目级编译阻断”和“NPC 真实运行态验证”，不是继续盲目扩写新功能。
- `GameObjectSerializer.cs` / `GetPlaybackTime` / `OnAnimatorIK` 这组红字已归类为 Unity MCP 在 Play 中读取 Animator 组件时的工具噪音，不能误判成 NPC 业务代码回归。

**当前恢复点**:
- NPC 父工作区当前已从“自动漫游 V1 代码和 prefab 落地”推进到“编译阻断解除 + `Primary` 场景首轮运行态验证通过”。
- 下一步回到 NPC 主线收口：同步线程记忆后，按 NPC 白名单提交本轮变更，并继续补一组真实场景下的配对聊天正样本或交给用户继续手测。
---

### 会话 7 - 2026-03-17

**用户需求**:
> 在治理裁定下停止借用共享根目录，把 NPC 改到独立救援现场处理，只做最小收口：以 `codex/npc-roam-phase2-001 @ f6b4db2f` 为唯一基线，剔除误带入的 `FarmToolPreview.cs` 并完成固定范围验证。

**完成任务**:
1. 确认新的唯一可写现场为 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue @ codex/npc-roam-phase2-001`，`HEAD=f6b4db2f852910f5249aca4f51639cbddd893c05`。
2. 将 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 按 `8aed637f` 回退，去掉 farm 侧拖带进 NPC 线的改动，保证本轮不再扩大 NPC 改动范围。
3. 回读 `Logs/npc_rescue_compile_wait4.log`，确认编译成功退出且没有新的 NPC 红错，再抽查 `001/002/003` 的 Prefab、Sprite、动画与漫游组件链。

**关键决策**:
- 共享根目录 `D:\Unity\Unity_learning\Sunset` 继续视为 farm 事故现场，NPC 后续写入不得再回到那里执行。
- `FarmToolPreview.cs` 属于 farm 误带入内容，本轮只需要从 NPC 线剔除；`NPCPrefabGeneratorTool.cs` 等 NPC 核心内容继续保留 `f6b4db2f` 版本即可。

**关键文件**:
- `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
- `Assets/222_Prefabs/NPC/001.prefab`
- `Assets/222_Prefabs/NPC/002.prefab`
- `Assets/222_Prefabs/NPC/003.prefab`
- `Logs/npc_rescue_compile_wait4.log`

**当前恢复点**:
- NPC 父工作区当前真实状态是“救援分支最小收口已完成，静态验证通过，等待白名单 Git 固化”。
- 下一步不是继续扩写功能，而是只对白名单文件做提交并确认救援分支重新干净。
---

### 会话 8 - 2026-03-17

**用户需求**:
> NPC 救援通过后，不再继续救火；先处理当前 rescue worktree 里的 4 个无关 TMP 字体 dirty，判断归属并选择后续续航方案。

**完成任务**:
1. 回读 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 的 `git status`，确认当前仅剩 4 个 TMP 字体资源 dirty，路径全部位于 `Assets/TextMesh Pro/Resources/Fonts & Materials/`。
2. 回读 `f7a1c0f5` 的提交边界，确认这 4 个字体资源没有进入当前 NPC 救援提交，说明它们不是 NPC 救援收口的一部分。
3. 结合 rescue 接管前的现场摘要，形成父工作区层面的稳定结论：这 4 个文件与 NPC 业务无关，且属于 rescue 现场既有残留，不宜继续在该现场直接承接新一轮 NPC 开发。
4. 明确推荐方案 A：从 `f7a1c0f562a476febe50084124dbeee382d31ac9` 新起 continuation worktree 和新 `codex/` 分支，把当前 rescue worktree 降级为取证现场。

**关键决策**:
- 当前最优先的不是证明 rescue worktree 还能继续凑合用，而是主动切断无关 dirty 对后续 NPC 提交的污染链。
- continuation 现场应以当前已推送的 `f7a1c0f5` 作为起点，并与 rescue 现场物理隔离。

**关键文件**:
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`

**当前恢复点**:
- NPC 父工作区已完成救援验证与救援提交，当前进入“切换干净 continuation 现场”的治理准备阶段。
- 下一步最小动作是按方案 A 新建 clean worktree，而不是继续在当前 rescue worktree 上承接新开发。
---

### 会话 9 - 2026-03-17

**用户需求**:
> 正式执行方案 A：为 NPC 建立 continuation 可写现场，确认新现场 clean，并将旧 rescue worktree 降级，不在本轮继续开发新功能。

**完成任务**:
1. 成功创建 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue`，分支为 `codex/npc-roam-phase2-002`，创建基线为 `28aef95d16f176ac5538bbd93c73769b2c686a8c`。
2. 在新现场核对 `git status`，确认 continuation worktree 不再带有 rescue 现场中的 4 个 TMP 字体资源 dirty，创建后即为 clean。
3. 在父工作区层面正式确认角色切换：`NPC_roam_phase2_rescue` 保留为阶段性救援收口现场，不再作为 NPC 正常开发现场。
4. 明确新的唯一开发入口：后续 NPC 新开发、验证、提交统一在 `codex/npc-roam-phase2-002` continuation 现场继续。

**关键决策**:
- 现场切换本身就是本轮唯一目标，不顺手扩写任何新的 NPC 功能。
- 继续开发时优先以“clean continuation 现场”作为防混线边界，而不是继续依赖白名单在 rescue worktree 上硬隔离。

**关键文件**:
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`

**当前恢复点**:
- NPC 父工作区的正式开发入口已经切换为新的 continuation worktree。
- 下一步恢复主线时，不再回 rescue 现场承载 NPC 写入。

---

### 会话 10 - 2026-03-17

**用户需求**:
> 发现当前现场又直接落在 `main`，要求我先检查分支/现场是否合规，再继续完成 NPC 主线，并把真正剩余缺口讲清楚。

**完成任务**:
1. 在父工作区层面重新核对规则入口与真实现场，确认这轮仍是 NPC 主线推进，不是治理换线；当前共享根目录开始时处于 `D:\Unity\Unity_learning\Sunset @ main`，`HEAD=64ff9816`，并且已经带着 `Assets/` 级 NPC dirty。
2. 为了恢复 Git 语义，把当前 NPC dirty 从 `main` 纠偏切到 `codex/npc-roam-phase2-003`，同时保留对两份农田 memory dirty 的隔离观察，不让无关改动混入 NPC。
3. 重新编译并回读 Prefab / Scene 静态链：`001/002/003` 的 Sprite、AnimatorController、`NPCBubblePresenter`、`NPCAutoRoamController` 和默认 `NPC_DefaultRoamProfile.asset` 仍完整存在，当前 Console 编译面为 0 error / warning。
4. 在父工作区层面推进第二阶段细化：把长停聊天补成“只对附近无人 / 对方暂不可聊做有限次重试”，并把默认聊天半径调到 `3.8`，以提高真实场景里出现配对聊天正样本的概率，同时保留随机性。
5. 确认新的验证口径：Play 中直接读取 Animator/组件资源会引出 Unity MCP 自身红字噪音，应与 NPC 业务错误分离；安全验证仍以编译、静态链、GameView 手测和 Inspector 调试入口为主。

**关键决策**:
- 当前父工作区的核心纠偏，不是再开一条新的 worktree 救援线，而是把已经落在共享根目录 `main` 上的 NPC dirty 及时迁回正式 `codex/` 任务分支承载。
- 聊天可靠性补丁只对“伙伴暂不可聊”做有限次重试，不对“概率没中”无限重掷，避免把 NPC 社交随机感修成强制触发。

**关键文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
- `Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`
- `.kiro/specs/NPC/1.0.0初步规划/tasks.md`

**当前恢复点**:
- NPC 父工作区当前已恢复到“任务分支语义正确、编译和静态链路可用”的阶段。
- 下一步只剩继续抓一组聊天正样本并完成 NPC 白名单 Git 收口，再交给用户继续主项目手测。
---

### 会话 11 - 2026-03-18

**用户需求**:
> 结合最新手测反馈，先不要继续扩写新功能；重新检查当前 NPC 还有没有遗漏内容，并把“对聊节奏更自然、活动区域规定、路线规划规范化”按代办规则正式登记。

**完成任务**:
1. 在父工作区层面确认当前 NPC 主线仍是 `1.0.0初步规划` 的 V1 收口，当前新增反馈属于“后续体验优化”，不是新的基础功能主线。
2. 明确记录了运行态验收边界：Unity/GameView 若失去前台，Play 中 NPC/玩家可能暂停更新，因此会影响行为节奏类验证，但不影响我继续做静态检查、文档整理和代办收口。
3. 在 `.kiro/specs/000_代办/kiro/NPC/` 下正式建立 NPC 的主代办与子代办文件，把对聊贴近门槛、活动区域场景化、路线规划规范化三项转入后续队列。

**修改文件**:
- `.kiro/specs/000_代办/kiro/NPC/TD_000_NPC.md`
- `.kiro/specs/000_代办/kiro/NPC/TD_1.0.0初步规划.md`
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/memory.md`

**关键结论**:
- 当前父工作区对 NPC 的写实判断已更新为：功能链路基本可用，新的用户反馈主要集中在“自然度”和“场景约束”，应转入代办而非把当前 V1 判回未完成。
- NPC 对聊“是否能发生”这件事已经有用户手测正向观察；后续真正该优化的是触发时机与场景边界。

**当前恢复点**:
- 当前父工作区已完成本轮“新增体验优化点转代办”的收口。
- 下一步继续 NPC 主线时，可以直接从这批 TD 出发判断优先级，而不用再重复回忆这次手测反馈。

### 会话 12 - 2026-03-19

**用户需求**:
> 按 queue-aware 准入规则恢复 NPC 线程进入 continuation branch，但本轮只允许做最小 live 基线复核与边界固化，不直接恢复 Unity 侧开发。

**完成任务**:
1. 在父工作区层面确认本轮仍服务于 NPC 主线，而不是切到治理主线：实际 continuation branch 仍指向 `codex/npc-roam-phase2-003`，本轮只是在 queue-aware 调度框架下合法恢复一次最小 checkpoint。
2. 写实记录 shared root 准入链路：stable launcher 的 `request-branch` 在本轮存在 `-BranchName` 透传异常，因此实际由 canonical `scripts/git-safe-sync.ps1` 完成 `GRANTED + ensure-branch`，shared root 当前被 NPC 合法占用在 `codex/npc-roam-phase2-003 @ 7385d123...`。
3. 在不进入 Unity / MCP / Play 的前提下，完成了 NPC 核心产物的存活复核：运行时代码、默认 roam profile、`001/002/003` 预制体与 prefab generator 均仍在位，说明当前 continuation 基线没有发生业务文件丢失。
4. 父工作区层面额外记录一条治理风险：queue runtime 把 NPC 的 `ticket=1` 标成 `cancelled`，但 occupancy 文档仍保持 `task-active`；该异常目前只作为观察事实保留，后续以治理线程裁定为准，不在 NPC 线私自修。

**修改文件**:
- `.kiro/specs/Codex规则落地/23_前序阶段补漏审计与并发交通调度重建/2026.03.19_queue-aware业务准入_01/memory.md`
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/memory.md`

**关键结论**:
- NPC 父工作区当前的真实阶段是“queue-aware 合法恢复最小 checkpoint”，不是“恢复全面开发”。
- 只要本轮按白名单完成 task sync 并执行 `return-main`，NPC 线程就能把 shared root 交还给下一位线程，而不把未验证的 Unity 写入混进来。

**当前恢复点**:
- 业务基线已复核完，下一步只剩白名单收口与 shared root 归还。

**阻塞更新**:
- 当我按本轮 NPC 白名单执行 task preflight 时，脚本额外发现 shared root 上存在不属于 NPC 的治理 dirty，因而阻断了本轮 sync；当前真实阶段变为“已完成最小 checkpoint，但收口被 shared root 其余 remaining dirty 卡住”。

---

### 会话 16 - 2026-03-21

**用户需求**:
> 在已有放行分支窗口里继续推进 NPC，但这轮只做一个真实的小 checkpoint：把 NPC 气泡的位置、尺寸和灵动感再往正式交付方向推一步，不扩新功能，不进入 Unity / MCP / Play。

**完成任务**:
1. 维持当前 continuation 分支语义不变，继续以 `codex/npc-roam-phase2-003` 承载这轮 NPC 主线收口，而不是回退到旧 continuation / rescue 口径。
2. 把这轮修改范围严格收窄到 `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`，只处理 NPC 气泡表现层，不去触碰 `NPCAutoRoamController`、Profile、Prefab、场景和其他共享热区。
3. 完成了一次真实的表现层 checkpoint：旧 prefab 上的 `NPCBubblePresenter` 通过样式版本升级自动吃到新默认值；气泡高度改为优先依据 `SpriteRenderer.bounds` 抬到头顶上方，再叠加轻微浮动、较舒展的尺寸留白和更柔和的显隐节奏。
4. 用纯 Git 侧 `git diff --check` 做了静态自检，确认本轮 patch 没有明显格式问题；运行态观感验收仍明确留给后续 Unity 主项目手测，而没有把推测写成已验证事实。

**修改文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
- `.kiro/specs/NPC/1.0.0初步规划/memory.md`
- `.kiro/specs/NPC/memory.md`
- `.codex/threads/Sunset/NPC/memory_0.md`

**关键结论**:
- NPC 父工作区当前主线仍是“V1 可用基础上的体验收口”，而不是新增系统开发；这轮新增的是一个更靠近正式交付感的气泡表现 checkpoint。
- 当前最真实的状态是：代码层已补上“别贴脸、稍微更灵动”的默认表现，但是否达到你眼里的最终美观标准，还需要回到 Unity 主项目里继续手测确认。

**当前恢复点**:
- 父工作区现在已经记录了这次 UI 表现层收口；下一步是白名单同步本轮代码与记忆，然后 `return-main`，把 shared root 归还给默认入口模式。

---

### 会话 17 - 2026-03-21

**用户需求**:
> 在当前 `codex/npc-roam-phase2-003` 窗口内继续执行 NPC 气泡位置与碰撞体修正，不扩新功能，只做一次真正可交付前的收口 checkpoint。
**完成任务**:
1. 确认本轮主线仍是 NPC V1 体验收口，而不是切换到新系统开发。
2. 将 `NPCBubblePresenter`、`NPCAutoRoamController`、`NPCPrefabGeneratorTool` 与 `001/002/003.prefab` 一起对齐：气泡按真实底边抬高，NPC 改为下半身实体 `BoxCollider2D + Rigidbody2D`，现有 prefab 与生成器口径统一。
3. 用 `git diff --check` 完成静态自检，本轮仍未把 Unity 观感手测写成已验证事实。
**修改文件**:
- `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- `Assets/Editor/NPCPrefabGeneratorTool.cs`
- `Assets/222_Prefabs/NPC/001.prefab`
- `Assets/222_Prefabs/NPC/002.prefab`
- `Assets/222_Prefabs/NPC/003.prefab`

**关键结论**:
- NPC 这条线当前已从“只修气泡表现”推进到“气泡表现 + 实体碰撞链路 + 新旧 prefab/生成器统一收口”的更完整 checkpoint。
- 当前最小剩余缺口不是代码，而是一次安全的 Unity 手测窗口，用来验证真实 GameView 下的气泡位置和玩家/NPC、NPC/NPC 不穿透效果。

**当前恢复点**:
- 父工作区现在可以把这轮判断写实为：NPC V1 的静态交付面已进一步收口，但 main-ready 仍取决于后续 Unity 手测证据。
- 后续若继续推进，优先级最高的不是加新行为，而是验证这轮碰撞体与气泡在主项目里的真实观感。
