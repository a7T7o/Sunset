# NPC 生成器与预制体流水线 - 开发记忆

## 模块概述

本子工作区用于实现一套 NPC 专用流水线，让四向三帧 PNG 可以直接生成：
- Sprite 切片
- AnimationClip
- Animator Controller
- NPC Prefab

并给生成后的 NPC 预留速度与动画播放速度调试入口。

## 当前状态

- **完成度**: 15%
- **最后更新**: 2026-03-11
- **状态**: 进行中

## 会话记录

### 会话 1 - 2026-03-11

**用户需求**:
> 请你学习后写出一个专门用于npc的生成器，我只需要导入npc图片png就行……从动画到动画控制器再到直接生成npc预制体我拖入游戏就能使用的。

**完成任务**:
1. 完成工作区路由，确定本任务需新建 NPC 子工作区推进
2. 读取 `.kiro/steering/rules.md`、`workspace-memory.md`、`scene-modification-rule.md`、`animation.md`、`coding-standards.md`、`git-safety-baseline.md`
3. 审视旧编辑器工具：`ToolAnimationGeneratorTool.cs`、`SliceAnimControllerTool.cs`、`ToolAnimationPipeline.cs`
4. 审视玩家基线脚本：`PlayerAnimController.cs`、`PlayerMovement.cs`
5. 通过 MCP 读取场景玩家实际配置：
   - `WalkSpeed=4`
   - `RunSpeed=6`
   - `Animator.speed=1`
   - `PlayerMovement Anim Controller.controller`
6. 通过 MCP 读取测试 NPC `DialogueTestNPC` 的组件配置，确认当前项目只具备基础对话交互壳
7. 输出本子工作区的需求、设计、任务文档

**修改文件**:
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/requirements.md`
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/design.md`
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/tasks.md`
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/memory.md`

**关键决策**:
- 不直接复用旧工具动画流水线，而是为 NPC 新建专用生成器
- 运行时方向参数继续沿用项目现有 `Down=0 / Up=1 / Side=2`
- 四向素材通过 `Left/Right -> Side + flipX` 桥接到三向参数体系
- 生成器首版最少支持 `Idle`、`Run`，并把 `Run` 统一抽象为 `Move`
- 预制体首版只创建新资源，不主动改现有场景和 Prefab

**涉及的代码文件**:
- `Assets/Editor/ToolAnimationGeneratorTool.cs` - 旧工具动画一键生成器，作为命名与生成思路参考
- `Assets/Editor/SliceAnimControllerTool.cs` - 旧控制器生成器，作为状态机生成参考
- `Assets/Editor/ToolAnimationPipeline.cs` - 旧完整流水线，作为切片与批处理参考
- `Assets/YYY_Scripts/Anim/Player/PlayerAnimController.cs` - 玩家动画基线
- `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs` - 玩家速度基线
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs` - 现有 NPC 对话组件入口

**当前主线**:
- 实现 NPC 生成器与运行时基础组件，让 PNG 导入后能产出可拖拽使用的 NPC Prefab

**本轮子任务 / 阻塞**:
- 子任务：收集玩家速度、现有工具能力、测试 NPC 配置证据
- 服务主线：确定 NPC 工具的默认值、参数桥接和预制体装配结构

**恢复点 / 下一步**:
- 开始实现 `NPCAnimController`、`NPCMotionController` 和 `NPCPrefabGeneratorTool`

---

### 会话 1（续） - 2026-03-11

**完成任务**:
1. 新增 `NPCAnimController.cs`，实现 NPC 的 `State/Direction` 参数写入、左右翻转和分状态动画速度控制
2. 新增 `NPCMotionController.cs`，实现基于 Rigidbody2D / Transform 位移的移动侦测、方向推断和动画驱动
3. 新增 `NPCPrefabGeneratorTool.cs`，实现输入文件夹扫描、四向三帧网格切片、Idle/Move/Death 动画剪辑生成、Animator Controller 生成与 Prefab 自动组装
4. 修正 Unity 6 编辑器 API 兼容问题，去掉 `InternalEditorUtility.sortingLayerNames` 依赖，并消除 `AnimatorController` 歧义引用
5. 使用 Unity MCP 执行脚本刷新与控制台检查

**修改文件**:
- `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs` - 新增 NPC 动画状态与播放速度控制器
- `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` - 新增 NPC 运动侦测与动画桥接器
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 新增 NPC 专用导入、动画、控制器、预制体一键生成窗口
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/tasks.md` - 勾选已完成任务
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/memory.md` - 追加本轮实现记录

**验证结果**:
- Unity 刷新与编译：通过
- 控制台 `Error`：0
- 控制台 `Warning`：仍有 14 条农田放置相关旧警告，与本轮 NPC 生成器无关
- MCP 结果：脚本已可被 Unity 正常编译和加载

**关键决策**:
- 首版运行时只做“可拖入场景即有动画骨架”的 NPC 基础能力，不在本轮强行加入日程、寻路、对话数据自动创建
- 生成器把 `Run/Walk/Move` 文件名统一映射到 `Move` 状态，减少素材命名耦合
- 预制体默认加入 `DynamicSortingOrder + CircleCollider2D + NPCAnimController + NPCMotionController`

**遗留问题**:
- [ ] 尚未使用用户真实四向三帧 PNG 跑一次完整生成
- [ ] 尚未通过 MCP 回读新生成 Prefab 结构（因为当前线程附件图片未落为项目内可选文件夹）
- [ ] 仍需用户在 Unity 内选择实际 NPC PNG 文件夹后执行一次生成，并做 PlayMode 观感确认

**当前主线恢复点**:
- 主线已从“设计并实现生成器”推进到“等待实际 NPC PNG 触发首轮资源产出验证”

## 2026-03-11（恢复补记）
- 本工作区相关文件此前在一次仓库清理中被从工作树移除，但并未彻底丢失。
- 本轮已从备份目录 `D:\Unity\Unity_learning\Sunset_backups\git_dirty_backup_20260311_160659\files` 恢复回项目工作树。
- 已确认以下关键内容重新可见：`Assets/Editor/NPCPrefabGeneratorTool.cs`、`Assets/YYY_Scripts/Anim/NPC/`、`Assets/YYY_Scripts/Controller/NPC/`、本工作区四件套文档。
- 当前恢复点：NPC 生成器相关代码与文档已重新回到项目中，但尚未重新整理到专用分支并提交。

## 2026-03-11（分支归位恢复）
- 先前这批 NPC 生成器相关文件没有真正提交进 `codex/npc-generator-pipeline`，而是停留在本地未提交现场。
- 本轮已从备份目录 `D:\Unity\Unity_learning\Sunset_backups\git_dirty_backup_20260311_160659\files` 中，把本工作区文档、NPC 相关脚本与资源恢复到 `codex/npc-generator-pipeline` 对应 worktree。
- 当前恢复到分支的关键内容包括：`Assets/Editor/NPCPrefabGeneratorTool.cs`、`Assets/YYY_Scripts/Anim/NPC/`、`Assets/YYY_Scripts/Controller/NPC/`、`Assets/YYY_Scripts/Story/Interaction/`、`Assets/Sprites/NPC/` 以及本工作区文档与线程记忆。
- 当前主线恢复点：NPC 生成器内容已经重新回到正确分支，下一步是将恢复内容正式提交并推送，然后释放临时 worktree 占用。

---

### 会话 1（续） - 2026-03-12（治理交叉文档补记）

**用户需求**:
> 请你把我们讨论的聊天记录全部原封不动塞入 `D:\迅雷下载\开始\.codex\threads\系统全局\000_git治理交叉内容\NPC.md`，当然你可以加入批注和理解，这是一个给 git 规则和搭建的线程看的内容，让他知道你的思考和你当前希望的规范和规则以及你希望他对你的友好程度，以及你的建议各种各种，这个文档不限制内容，越详细越好。

**完成任务**:
1. 以当前 NPC worktree 为真实现场重新核对 `cwd`、Git 分支与线程绑定关系，确认仍应以 `D:\Unity\Unity_learning\Sunset_worktrees\NPC` 和 `codex/npc-generator-pipeline` 为准。
2. 新建并写入外部治理交叉文档 `D:\迅雷下载\开始\.codex\threads\系统全局\000_git治理交叉内容\NPC.md`。
3. 在文档中完整收录此前围绕“当前在哪个工作区/分支”“worktree 与根仓库混淆”“是否应该即时更新到 main”“文件夹结构是否合理”等讨论记录。
4. 追加治理侧批注，明确表达对 worktree 识别优先级、memory 跟随 active worktree、`main` 以 checkpoint 合并、以及协作时降低用户焦虑的建议。
5. 用 `cmd /c chcp 65001 & type` 回读文档，确认中文内容按 UTF-8 可正常显示。

**修改文件**:
- `D:\迅雷下载\开始\.codex\threads\系统全局\000_git治理交叉内容\NPC.md` - 写入聊天记录、治理批注与规则建议
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/memory.md` - 追加本轮治理文档记录
- `.kiro/specs/002_After_26.1.21/NPC系统/memory.md` - 追加父工作区摘要
- `.codex/threads/Sunset/NPC/memory_0.md` - 追加线程视角摘要

**解决方案 / 关键结论**:
- 这次文档落盘属于 NPC 主线的治理侧支撑任务，不改变 NPC 生成器实现本身，但沉淀了当前线程后续必须遵循的执行现场判断。
- 当前 NPC 主线的唯一执行现场应继续锁定在 `D:\Unity\Unity_learning\Sunset_worktrees\NPC`，而不是根仓库 `D:\Unity\Unity_learning\Sunset`。
- 当根仓库视角与 worktree 视角冲突时，应先分层说明，再明确当前业务主线跟随 worktree 现场。
- `main` 不应承载每一次试写与试验；更合理的节奏是当前分支内即时推进，在闭环或 checkpoint 节点再进入主干。

**遗留问题**:
- [ ] 这次完成的是治理交叉文档，不是 NPC 真实素材导入验证；主线仍未完成首轮真实 PNG 生成与观感检查。
- [ ] 仍需回到 NPC 业务主线，用真实四向三帧素材跑一轮生成器，验证动画、控制器、Prefab 和默认参数。

**当前主线恢复点**:
- 治理侧说明已补齐，NPC 主线恢复到“在正确的 NPC worktree 里执行首轮真实素材生成与验证”这一步。

---

### 会话 1（续） - 2026-03-12（新规接管检查）

**用户需求**:
> 你现在先不要立即开发或修改内容，先按“新规接管模式”完成启动检查、规则回顾、主线回溯、未完成任务接管，再决定是否执行。

**完成任务**:
1. 按新规回读全局 `AGENTS.md`、当前 worktree `AGENTS.md`、`workspace-memory.md`、`rules.md`，确认当前轮必须先规则接管、再做完成态判断。
2. 回读用户指定的 Sunset 治理文档：`tasks.md`、`Codex统一协议闭环整改总方案_2026-03-12.md`、`Codex真实线程承接修复设计_2026-03-12.md`、`T74_用户真实点击线程闭环验证矩阵_2026-03-12.md`。
3. 回读本子工作区 `requirements.md`、`design.md`、`tasks.md`，以及子/父工作区 memory 与线程 `memory_0.md`，重建 NPC 主线。
4. 只读核对当前真实现场：确认当前线程仍在 `D:\Unity\Unity_learning\Sunset_worktrees\NPC`、分支仍是 `codex/npc-generator-pipeline`、当前 HEAD 为 `1f068ed1`。
5. 只读核对当前资产与产物现场：确认 `Assets/Sprites/NPC` 下存在 `001.png`、`002.png`、`003.png` 三张 PNG，但 `Assets/100_Anim/NPC` 与 `Assets/Prefabs/NPC` 目前没有已生成产物。
6. 只读回读 `NPCPrefabGeneratorTool.cs`，确认当前生成器动作识别仍依赖文件名关键词 `Idle / Run / Walk / Move / Death`。
7. 调用 Unity MCP 读取控制台时收到 `Connection failed: Unknown error`，说明当前轮无法把 Unity MCP 回读结果写成已验证事实。

**修改文件**:
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/memory.md` - 追加新规接管检查结论
- `.kiro/specs/002_After_26.1.21/NPC系统/memory.md` - 追加父工作区摘要
- `.codex/threads/Sunset/NPC/memory_0.md` - 追加线程视角摘要

**解决方案 / 关键结论**:
- 当前 NPC 主线没有换线，仍然是“把四向三帧 PNG 真实跑成动画、控制器、Prefab，并完成首轮验证”。
- 这轮新规接管后，已确认当前完成态不应被写成“基本完成待提交”，而应判定为“核心实现已在，但真实素材生成与回读验证尚未完成”。
- 当前最关键的新阻塞不再是“没有真实 PNG”，而是：
  1. 现有真实 PNG 文件名为 `001/002/003`，与生成器当前仅按动作关键词识别的逻辑不匹配；
  2. 生成输出目录当前为空，说明这批图尚未真实跑出产物；
  3. Unity MCP 当前连接失败，导致任务清单第 12 项“用 MCP 审视生成结果与预制体结构”仍无法完成。

**验证结果**:
- 本地文件系统验证通过：NPC 三个核心脚本仍存在；`Assets/Sprites/NPC` 真实 PNG 存在；生成输出目录为空。
- 本地代码回读验证通过：生成器当前确实依赖动作关键词命名。
- Unity MCP 验证失败：控制台读取返回 `Connection failed: Unknown error`，因此本轮没有新的 Unity 侧实时验证结果。

**遗留问题**:
- [ ] 真实 PNG 语义尚未映射到 `Idle / Move / Death`，当前命名 `001/002/003` 不能直接被生成器识别。
- [ ] 尚未在 Unity 内真实生成动画、控制器、Prefab，因此任务清单第 12 项仍未完成。
- [ ] 尚未得到用户真实点击或 PlayMode 观感验收结果。

**当前主线恢复点**:
- 当前主线恢复到“先解决真实 PNG 与生成器动作识别不匹配，再执行首轮真实生成与验证”这一步。

---

### 会话 1（续） - 2026-03-12（Idle 中帧规范与动作映射修正）

**用户需求**:
> 第 0 行：Down / 第 1 行：Left / 第 2 行：Right / 第 3 行：Up 这个没问题，但是 idle 应该是每一行的第 1 列，也就是第二帧，一共 012 三帧，idle 是只使用这中间的这一帧。

**完成任务**:
1. 复盘当前未对齐点，确认此前实现里最大的两个偏差是：
   - 生成器仍强依赖文件名关键词 `Idle / Run / Walk / Move / Death`
   - Idle 动画仍按整行三帧生成，而不是按用户要求只取中间帧
2. 修改 `NPCPrefabGeneratorTool.cs`，新增动作映射枚举与扫描结果缓存，让生成器支持：
   - 自动识别文件名关键词
   - 对 `001 / 002 / 003` 这类无语义文件名在窗口中手动指定 `Idle / Move / Ignore`
3. 在编辑器窗口新增“动作映射”区域，明确提示：
   - Idle 只使用每一行中间那一帧（第 1 列）
   - Move 使用整行三帧
4. 修改生成流程，`GenerateNpcAssets()` 不再直接依赖文件名命中，而是先读取扫描结果和用户手动指定的动作映射。
5. 修改动画生成逻辑，Idle 状态改为只取每行的中间帧；Move 和 Death（若未来有）仍使用整行帧序列。
6. 尝试用 Unity MCP 执行脚本重编译与控制台读取，但当前返回 `Connection failed: Unknown error`，因此本轮无法把 Unity 编译写成已验证通过。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 新增动作映射入口，支持无语义文件名手动指定 Idle / Move；Idle 动画改为每行只取中间帧
- `.kiro/specs/002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线/memory.md` - 追加本轮实现与验证口径
- `.kiro/specs/002_After_26.1.21/NPC系统/memory.md` - 追加父工作区摘要
- `.codex/threads/Sunset/NPC/memory_0.md` - 追加线程视角摘要

**解决方案 / 关键结论**:
- 方向行序继续保持：
  - 第 0 行：Down
  - 第 1 行：Left
  - 第 2 行：Right
  - 第 3 行：Up
- Idle 新规范已落地为：每个方向只取该行的第 1 列，也就是中间帧。
- Move 规范保持为：每个方向继续使用该行的三帧序列。
- 当前生成器不再把“文件名必须叫 Idle/Move”当硬前置；如果素材是 `001 / 002 / 003` 这种名字，可以先扫描，再在窗口里手动指定动作。

**验证结果**:
- 本地代码回读通过：新枚举 `ActionAssignment`、新窗口区块 `DrawAssignmentSection()`、`BuildGenerationTasks()` 与 `FilterSpritesForState()` 已写入。
- Unity MCP 验证未通过：`recompile_scripts` 与 `get_console_logs` 均返回 `Connection failed: Unknown error`。
- 因此本轮只能确认代码层已落盘，不能确认 Unity 实时编译与生成结果。

**遗留问题**:
- [ ] 仍需在 Unity 内用真实 PNG 跑一轮：先扫描，再指定哪张是 Idle、哪张是 Move，然后真实生成。
- [ ] 仍需确认三张 `001 / 002 / 003` 中具体哪两张是本轮应使用的 Idle / Move 图，剩余那张是忽略还是另有用途。
- [ ] 仍需完成任务 12：用 MCP 审视生成结果与预制体结构。

**当前主线恢复点**:
- 真实素材适配规则已经和用户口径对齐；下一步应直接在 Unity 内对 `001 / 002 / 003` 做动作指定并执行首轮真实生成，而不是再改抽象方案。
