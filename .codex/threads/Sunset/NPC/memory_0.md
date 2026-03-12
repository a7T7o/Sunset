# NPC 线程记忆

## 2026-03-11

- 用户主线目标：在 Sunset 项目里做一套 NPC 专用生成器，输入四向三帧 PNG 后自动完成切片、动画、控制器、预制体生成，并提供速度与动画速度调试入口。
- 本轮子任务：先学习现有工具动画一键生成、控制器生成、流水线等旧工具，再对齐玩家动画与速度基线，为 NPC 新工具定方案。
- 已完成事项：读取 Sunset 路由与 steering；审视旧编辑器工具；读取玩家 `PlayerAnimController` / `PlayerMovement`；通过 MCP 读取玩家场景实配 `WalkSpeed=4`、`RunSpeed=6`、`Animator.speed=1`；读取 `DialogueTestNPC` 的现有组件结构；创建 `002_After_26.1.21/NPC系统/0_NPC生成器与预制体流水线` 工作区并写入 requirements/design/tasks/memory。
- 关键决策：不直接套用旧工具动画流水线，而是新建 NPC 专用生成器；方向参数继续兼容现有 `Down=0 / Up=1 / Side=2`；四向素材用 `Left/Right -> Side + flipX` 桥接；首版首要支持 `Idle`、`Run(映射为 Move)`、可选 `Death`。
- 涉及文件或路径：`.kiro/specs/002_After_26.1.21/NPC系统/`、`Assets/Editor/ToolAnimationGeneratorTool.cs`、`Assets/Editor/SliceAnimControllerTool.cs`、`Assets/Editor/ToolAnimationPipeline.cs`、`Assets/YYY_Scripts/Anim/Player/PlayerAnimController.cs`、`Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`、`Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`。
- 验证结果：已完成 Git preflight，发现实现前仍在 `main` 且存在多条无关脏改；已切换到 `codex/npc-generator-pipeline` 分支继续。
- 遗留问题 / 下一步：继续实现运行时 NPC 脚本与编辑器窗口，再做 Unity 编译、控制台检查和生成结果审视。
- 当前主线恢复点：从文档阶段进入实现阶段，优先完成运行时基础组件和编辑器生成器骨架。

## 2026-03-11（续）

- 已完成事项：新增 `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`、`Assets/Editor/NPCPrefabGeneratorTool.cs`；完成 NPC 生成器首版落地。
- 关键能力：支持扫描输入文件夹中的 `Idle/Run/Death` PNG；按四向三帧网格切片；将四向桥接为三向 Animator 参数；自动生成 AnimationClip、Animator Controller 和 NPC Prefab；为 MoveSpeed 与动画速度提供 Inspector 调试入口。
- 验证结果：Unity 编译通过；MCP 读取控制台 `Error=0`；当前仅剩 14 条农田系统旧警告，和本轮 NPC 生成器无关。
- 当前阻塞：附件里的 NPC 图片没有直接落在项目可选文件夹里，因此本轮还没法代替用户点选真实 PNG 目录跑一遍完整生成；主线已经恢复到“等待真实素材触发首轮导入验证”。
- 下一步：让用户在 Unity 内打开 `Tools/NPC/NPC预制体生成器`，选择真实 NPC PNG 文件夹后执行一键生成；如果首轮生成结果有方向顺序或动画速度偏差，再微调行方向映射、默认 MoveSpeed、MoveAnimationSpeed。

## 2026-03-11（补记：NPC 内容重新归位到正确分支）
- 先前 NPC 生成器相关文件实际没有进入提交历史，而是停留在未提交现场；后续在仓库清理时被移出工作树，但已被备份保住。
- 本轮已把这些内容从备份恢复到 `codex/npc-generator-pipeline` 对应的 worktree 中。
- 下一步：将当前恢复内容提交并推送，然后释放临时 worktree 占用，避免继续阻塞 NPC 线程切换分支。

## 2026-03-12（治理交叉文档）
- 用户要求把此前围绕“当前在哪个工作区/分支、是否能继续做、memory 应更新到哪里、是否应即时并入 `main`”的讨论整理给另一个 Git/规则线程参考。
- 已在 `D:\迅雷下载\开始\.codex\threads\系统全局\000_git治理交叉内容\NPC.md` 写入完整聊天记录与批注，并确认该文档可按 UTF-8 正常读取。
- 稳定结论：当前 NPC 线程必须继续锚定 `D:\Unity\Unity_learning\Sunset_worktrees\NPC` 与 `codex/npc-generator-pipeline`；memory 跟随 worktree；`main` 采用 checkpoint 合并而非每次小改即时同步。
- 当前主线恢复点不变：治理文档已补齐，接下来仍应回到 NPC 真实素材生成与验证。

## 2026-03-12（新规接管检查）
- 按用户要求执行了“新规接管模式”：已回读全局/项目规则、指定治理文档、NPC 子工作区文档、父工作区 memory 与线程 memory，再做完成态判断。
- 当前实时现场已再次确认：`cwd=D:\Unity\Unity_learning\Sunset_worktrees\NPC`，分支 `codex/npc-generator-pipeline`，HEAD `1f068ed1`。
- 新增真实阻塞证据：`Assets/Sprites/NPC` 已有三张 PNG，但文件名是 `001/002/003`；而现有生成器只按 `Idle / Run / Walk / Move / Death` 关键词识别，所以这批图当前不能直接跑通；同时 Unity MCP 本轮连接失败，无法完成任务 12 的回读验证。
- 当前主线恢复点：先处理真实 PNG 与动作识别不匹配的问题，再做首轮真实生成与验证；在这之前，不能把 NPC 主线写成已完成或可验收。
