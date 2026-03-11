# NPC 生成器与预制体流水线 - 需求文档

## 1. 背景

Sunset 当前已有玩家动画系统、工具动画流水线、对话测试 NPC 和若干编辑器工具，但缺少一套面向 NPC 的专用导入链路。用户希望只导入 NPC 的 PNG 素材图，即可自动完成：
- 切片
- 动画剪辑生成
- Animator Controller 生成
- 可直接拖入场景使用的 NPC 预制体生成

本次目标面向用户已明确说明的 NPC 模板：
- NPC 素材为 **四向三帧**
- 与玩家现有三向动画结构不同
- 需要兼顾 NPC 移动速度与动画播放速度的适配
- 必须提供后续调试和配置入口

## 2. 输入需求

### 2.1 输入形式

生成器必须支持以下输入方式：
- 选择一个 NPC 素材文件夹
- 文件夹内包含若干 PNG 动作图
- 至少支持 `Idle`、`Run`
- 可选支持 `Death`

### 2.2 PNG 模板约定

默认以四向三帧模板处理单张 PNG：
- 每张 PNG 为网格图
- 默认 `4 行 × 3 列`
- 每行表示一个方向
- 每列表示该方向的帧序列

### 2.3 可配置输入参数

生成器必须暴露以下输入配置：
- 每方向帧数，默认 `3`
- 方向数，默认 `4`
- 行方向顺序预设，默认 `Down / Left / Right / Up`
- Pixels Per Unit，默认 `16`
- 是否自动应用底部 Pivot
- 是否启用网格切片

## 3. 输出需求

### 3.1 动画资源

生成器必须自动生成以下资源：
- 切片后的 Sprite
- NPC 动画剪辑 `.anim`
- NPC Animator Controller `.controller`

### 3.2 预制体资源

生成器必须自动生成 NPC 预制体 `.prefab`，且拖入场景后满足：
- 有可见 Sprite
- 有 Animator
- 有排序组件
- 有基础碰撞交互范围
- 有用于运动与动画联动的运行时脚本

### 3.3 目录结构

生成结果必须按 NPC 名称归档，避免资源混乱。默认目录结构：

```text
Assets/100_Anim/NPC/{NpcName}/Clips/
Assets/100_Anim/NPC/{NpcName}/Controller/
Assets/Prefabs/NPC/{NpcName}.prefab
```

## 4. 动画需求

### 4.1 动画状态

首版必须支持：
- `Idle`
- `Move`
- `Death`（可选，若素材存在）

### 4.2 方向映射

为了兼容项目现有 Animator 方向规则，生成器需要把四向素材适配为运行时三向参数体系：
- Down → `Direction=0`
- Up → `Direction=1`
- Left / Right → `Direction=2`（Side）

运行时通过 `SpriteRenderer.flipX` 处理 Left / Right 的视觉差异。

### 4.3 缺失动作兼容

若未提供 `Death`，生成器仍应成功生成最小可用 NPC。

## 5. 运行时需求

### 5.1 基础运行时组件

生成的 NPC 预制体必须包含：
- `SpriteRenderer`
- `Animator`
- `DynamicSortingOrder`
- `CircleCollider2D`（默认 Trigger）
- NPC 动画驱动脚本
- NPC 运动/调试脚本

### 5.2 速度适配

运行时必须提供可调入口，用于分别控制：
- NPC 移动速度
- Idle 动画速度
- Move 动画速度
- 是否根据移动状态自动切换动画

### 5.3 与玩家基线适配

生成器默认值需要参考项目当前玩家实配：
- 玩家 WalkSpeed：`4`
- 玩家 RunSpeed：`6`
- 玩家 Animator.speed：`1`

NPC 默认值应偏保守，避免刚生成就显得过快。

## 6. 调试入口需求

### 6.1 Inspector 调试入口

生成的 NPC 组件必须在 Inspector 中可直接调整：
- 默认朝向
- Move 阈值
- MoveSpeed
- IdleAnimationSpeed
- MoveAnimationSpeed
- 是否启用自动速度同步
- 是否打印调试日志

### 6.2 编辑器入口

需要提供统一的 EditorWindow 入口，用户可重复使用，不依赖一次性脚本。

## 7. 验证需求

本次实现完成后，至少需要完成：
- 脚本编译通过
- 控制台 Error 为 0 或明确说明非本任务遗留错误
- 生成工具可创建一套 NPC 资产
- 生成的预制体结构符合设计
- 给出仍需用户手动 PlayMode 验证的清单

