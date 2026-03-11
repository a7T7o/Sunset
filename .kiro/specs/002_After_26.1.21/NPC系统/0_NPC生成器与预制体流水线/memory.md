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
