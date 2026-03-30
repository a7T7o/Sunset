# 2026-03-30｜shared runtime 残面定责

## 一句话结论

`OcclusionManager.cs` 可以按小范围 shared runtime 尾差处理。  
`TreeController.cs` 不能。

更准确地说：

1. `OcclusionManager.cs` = `农田 preview 遮挡尾差`
2. `TreeController.cs` = `农田/砍树表现包`，不是轻尾账

## 文件 1：`Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`

### 这轮钉死的事实

当前 diff 只有 4 行量级，核心改动是：

- 预览遮挡从 `GetBounds()` 改为 `GetColliderBounds()`

这条改动与以下已有记忆完全对得上：

1. `.codex/threads/Sunset/农田交互修复V3/memory_0.md`
   - 明确写过 `preview 遮挡改按 GetColliderBounds()`
2. `.kiro/specs/农田系统/2026.03.16/1.0.4交互全面检查/0.0.1交互大清盘/memory.md`
   - 明确写过 `preview 遮挡检测改为使用 occluder 的 GetColliderBounds()`

### 裁定

1. 这不是新的 shared-root 事故。
2. 这就是农田 preview 遮挡那条线的残面。
3. 它可以继续按 `农田交互` 方向归口，不需要单独重开别的业务线程来认领。

## 文件 2：`Assets/YYY_Scripts/Controller/TreeController.cs`

### 这轮钉死的事实

当前 diff 不是“几行 shared runtime 小尾差”，而是：

- `1055` 行级别 diff
- `633 insertions / 426 deletions`

实际主题已经明显超出“小修”范围，新增内容包括：

1. `isChopDownCommitted`
2. `isFallAnimationInProgress`
3. 倒下过程中的二次命中屏蔽
4. side fall / up fall 的新动画曲线与压扁、回弹、淡出节奏
5. 砍倒事务锁和动画完成后解锁逻辑

这些内容和以下既有材料对得上：

1. `.kiro/specs/农田系统/2026.03.16/1.0.4交互全面检查/memory.md`
   - 明确记录过这轮补了 `A2` 倒下动画表现层
2. `.kiro/specs/农田系统/2026.03.16/1.0.4交互全面检查/0.0.1交互大清盘/2026-03-27-交互大清盘_根因分析与全局总账.md`
   - 明确分析过“树倒下动画期间继续挥砍会让树重新站起来”的问题

### 裁定

1. `TreeController.cs` 当前不能再被叫做“小范围 shared runtime 残面”。
2. 它本质上是：
   - 一整包农田/砍树表现与事务锁逻辑
   - 只是恰好落在 shared runtime 根上
3. 治理线程不应把它顺手吞成“轻尾账”。

## 当前正确分法

1. `OcclusionManager.cs`
   - 继续按农田 preview 遮挡尾差看待
2. `TreeController.cs`
   - 单独按农田/砍树表现包处理
   - 不再混进“shared runtime 小尾账”叙事

## 这轮故意不做什么

1. 不直接提交这两个 runtime 文件。
2. 不把 `TreeController.cs` 包装成已经完成的小修。
3. 不让无关业务线顺手吞 shared runtime 根。

## 下一步唯一正确切法

如果后续要继续收这些 runtime 残面，应分两刀：

1. 一刀处理 `OcclusionManager.cs`
   - 作为农田 preview 遮挡尾差
2. 一刀单独处理 `TreeController.cs`
   - 作为农田/砍树表现包
   - 必须有单 owner、单验证口径、单 checkpoint
