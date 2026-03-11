# NPC 生成器与预制体流水线 - 设计文档

## 1. 设计目标

构建一套 NPC 专用编辑器流水线，使用户在导入四向三帧 PNG 后，可以一键完成：
- 切片
- 动画剪辑生成
- Animator Controller 生成
- 预制体生成

并在运行时提供独立于玩家的速度与动画速度调节入口。

## 2. 现状证据与设计依据

### 2.1 玩家侧基线

通过 MCP 审视场景 `Assets/000_Scenes/Primary.unity` 中的玩家对象，确认：
- `PlayerMovement.WalkSpeed = 4`
- `PlayerMovement.RunSpeed = 6`
- `Animator.runtimeAnimatorController = Assets/100_Anim/Player/001 Controller/PlayerMovement Anim Controller.controller`
- `PlayerAnimController.animationSpeedMultiplier = 1`

### 2.2 测试 NPC 基线

通过 MCP 审视 `SCENE/LAYER 1/Props/DialogueTestNPC`，确认现有测试 NPC 仅具备：
- `SpriteRenderer`
- `DynamicSortingOrder`
- `NPCDialogueInteractable`
- `CircleCollider2D`

说明当前项目缺的是“会自动动和播”的 NPC 运行时骨架，而不是单纯的对话组件。

### 2.3 旧编辑器工具的适用性

已审视：
- `ToolAnimationGeneratorTool.cs`
- `SliceAnimControllerTool.cs`
- `ToolAnimationPipeline.cs`

结论：
- 旧工具的切片、AnimationClip、AnimatorController 生成思路可以复用
- 但其命名规则、方向规则、参数设计都偏向工具动画，不适合直接套到 NPC
- NPC 需要新的输入约定、三向参数桥接、Prefab 自动装配逻辑

## 3. 总体方案

## 3.1 编辑器层

新增 `NPCPrefabGeneratorTool` 编辑器窗口，负责：
- 选择输入文件夹
- 扫描动作 PNG
- 按网格切片写入 TextureImporter
- 根据动作和方向生成 AnimationClip
- 生成 NPC 专用 Animator Controller
- 组装 NPC 预制体

## 3.2 运行时层

新增两个运行时脚本：
- `NPCAnimController`
- `NPCMotionController`

职责划分：
- `NPCAnimController` 负责 Animator 参数、方向、Flip 与动画速度
- `NPCMotionController` 负责位移检测、状态切换、MoveSpeed 调试入口与可选自动驱动

## 4. 输入资产设计

### 4.1 文件夹扫描

编辑器工具扫描指定文件夹下的 PNG 文件，并按文件名关键词识别动作：
- `Idle`
- `Run`
- `Death`

识别规则采用“包含关键词即可”，避免素材命名稍有变化就失效。

### 4.2 切片规则

默认切片规则：
- 行数：4
- 列数：3
- 单元格宽高：按整图平均切分
- 默认使用底部中心 Pivot

为适应后续其他 NPC 包，窗口提供以下覆盖项：
- 行数
- 列数
- 行方向顺序
- PPU
- Pivot 模式

## 5. 动画资源设计

### 5.1 动画命名

统一命名：

```text
{Action}_{Direction}_Clip
```

示例：
- `Idle_Down_Clip`
- `Idle_Side_Clip`
- `Idle_Up_Clip`
- `Move_Down_Clip`
- `Move_Side_Clip`
- `Move_Up_Clip`
- `Death_Down_Clip`

说明：
- 输入若为 `Run`，运行时统一映射为 `Move`
- Left / Right 最终都会归并到 `Side`

### 5.2 Left / Right 处理

为避免为 4 向单独建立 4 套 Animator 参数，采用以下桥接：
- 右向素材生成 `Side` clip
- 左向素材不额外建状态
- 运行时记录“是否向左”，由 `SpriteRenderer.flipX` 实现

如果用户未来希望“左右不镜像、而是保留独立立绘”，可在二期扩展为 4 向参数。

## 6. Animator Controller 设计

### 6.1 参数

NPC Controller 使用以下参数：
- `State (int)`：0 Idle，1 Move，2 Death
- `Direction (int)`：0 Down，1 Up，2 Side

### 6.2 状态结构

控制器采用 `Any State -> 目标状态` 的简单结构：
- `Idle_Down_Clip`
- `Idle_Side_Clip`
- `Idle_Up_Clip`
- `Move_Down_Clip`
- `Move_Side_Clip`
- `Move_Up_Clip`
- 可选 `Death_*`

优点：
- 和工具动画控制器的项目习惯保持一致
- 结构简单，适合自动生成
- 运行时只需改 int 参数即可切换

## 7. 预制体设计

### 7.1 预制体默认结构

```text
{NpcName}
├── SpriteRenderer
├── Animator
├── DynamicSortingOrder
├── CircleCollider2D (Trigger)
├── NPCAnimController
└── NPCMotionController
```

### 7.2 默认组件值

参考玩家与现有测试 NPC，默认值设计为：
- Sorting Layer：`Layer 1`
- CircleCollider2D.radius：`0.65`
- CircleCollider2D.offset.y：按 Sprite 高度底部估算
- `DynamicSortingOrder.useSpriteBounds = true`
- `NPCMotionController.moveSpeed = 2.5`
- `NPCAnimController.idleAnimationSpeed = 1.0`
- `NPCAnimController.moveAnimationSpeed = 1.0`

原因：
- 玩家场景速度为 4 / 6，NPC 初始值应更保守
- 用户可以后续在 Inspector 中按角色个性调整

## 8. 运行时脚本设计

### 8.1 NPCAnimController

职责：
- 维护 `State` / `Direction`
- 处理左右翻转
- 分开控制 Idle / Move / Death 的 Animator.speed
- 对外暴露 `SetDirection`、`SetState`、`ApplyMoveVisuals`

### 8.2 NPCMotionController

职责：
- 检测 Rigidbody2D 或 Transform 位移
- 判断当前是否移动
- 计算朝向
- 把结果提交给 `NPCAnimController`
- 提供 Inspector 调试开关与基础预览入口

兼容原则：
- 如果没有外部 AI，NPC 仍可在原地稳定 Idle
- 如果未来有寻路 / 日程系统，只需要调用它的移动接口或直接移动 Transform

## 9. 风险与边界

### 9.1 本次不直接做的内容

本次不包含：
- NPC 日程系统
- NPC 对话数据自动创建
- NPC 商店 / 任务 / 好感度
- 已存在 Prefab 或场景 NPC 的批量迁移

### 9.2 风险控制

本次只创建新工具、新脚本、新资源目录，不主动改已有生产 Prefab 和场景配置。

## 10. 验证方案

按 `sunset-unity-validation-loop` 执行：
1. 确认改动范围
2. 刷新 / 编译
3. 读取控制台
4. 如有必要做最小 EditMode 验证
5. 用 MCP 审查生成结果
6. 输出仍需手动 PlayMode 确认的项

