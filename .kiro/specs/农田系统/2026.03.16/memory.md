# 农田系统 2026.03.16 - cleanroom 记忆

## 模块概览

本层用于承接 2026-03-16 之后的农田交互修复工作。当前唯一活跃子工作区为 `1.0.2纠正001` 的 cleanroom 重建现场，不再沿污染分支继续开发。

## 当前状态

- cleanroom 工作目录：`D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- cleanroom 分支：`codex/farm-1.0.2-cleanroom001`
- cleanroom 基线：`b9b6ac4881f4436abbc1f3232f14706ca76bb869`
- 污染现场：`D:\Unity\Unity_learning\Sunset` 上的 `codex/farm-1.0.2-correct001@11e0b7b4c1340e0359a546b038d711b03836dc72`
- 当前阶段：farm-only 回放已完成，cleanroom 记忆已重写，最小编译验证失败，尚未达到可接替后续 farm 开发状态

## 会话记录

### 2026-03-17 - 1.0.2 cleanroom 重建与最小验证

**用户目标**：
> 按治理正式执行令停止沿污染分支开发，在独立 cleanroom 中以 `b9b6ac48` 为起点，严格白名单回放 farm-only 文件，重写记录类文件，并给出 cleanroom 是否已经可以接替污染分支继续开发的事实结论。

**完成事项**：
1. 确认 cleanroom 现场为 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`、分支 `codex/farm-1.0.2-cleanroom001`、HEAD `b9b6ac48...`。
2. 按用户指定白名单完成 farm-only 回放：
   - 从 `07ffe199` 仅保留 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
   - 从 `11e0b7b4` 回放 7 个 farm 代码文件与 4 份正文文档
3. 核对 `git status --short`，当前 tracked 改动仍只包含 12 个回放文件，没有混入 NPC、治理归档或额外业务文件。
4. 按 cleanroom 新现场重写当前子工作区、父工作区、农田系统总记忆与线程记忆，不机械复制污染现场旧记忆。
5. 用 Unity 6000.0.62f1 的 batchmode 对 cleanroom 做最小编译验证，日志落在 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`。

**明确排除**：
- `18f3a9e1` 的全部内容
- `07ffe199` 中除 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 外的全部 NPC 资产、脚本、文档与 NPC 线程记忆
- 所有 `NPC` 路径
- 治理归档 / 项目总览类文件
- 污染现场旧 `memory` 文本的直接照搬

**验证结果**：
- 回放后 `git status --short` 仍只包含 12 个白名单文件：
  - 8 个 farm 代码文件
  - 4 份 `1.0.2纠正001` 正文文档
- cleanroom 未复用共享根目录的 Unity live / MCP 会话；本轮验证以 batchmode 编译日志为准。
- batchmode 编译失败，主要阻断集中在：
  - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 的 `PreviewCellKey` 改造未闭合，文件内部仍大量以 `Vector3Int` 直接访问 `queuePreviewPositions / tillQueueTileGroups / executingTileGroups / executingWaterPositions`
  - `Assets/YYY_Scripts\Farm\FarmToolPreview.cs` 的 `Random` 调用在当前 `using` 组合下触发 `UnityEngine.Random` / `System.Random` 二义性
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 仍按双参数签名调用 `PromoteToExecutingPreview` / `RemoveExecutingPreview`
- 额外看到 1 条非 farm 代码 warning：`Assets/YYY_Tests/Editor/WorldItemDropSystemTests.cs(272,15)` 的未使用变量 `groundY`
- 当前结论：cleanroom 回放边界正确，但指定回放集在 `b9b6ac48` 上尚未形成可编译闭环

**恢复点 / 下一步**：
- 后续必须先在 cleanroom 内确认“最小补齐集”：
  - 要么继续补齐真正缺失的 farm-only 代码回放
  - 要么在已回放的 8 个 farm 代码文件内做最小兼容修复
- 在 compile 通过前，不执行 checkpoint、提交或同步动作
- 回放得到的 `tasks.md` 仅保留源现场正文；cleanroom 的实时状态以本 `memory.md` 与上层 cleanroom 记忆为准

## 涉及文件

- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Service\Placement\PlacementNavigator.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Service\Placement\PlacementPreview.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\requirements.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\analysis.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\design.md`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\tasks.md`
- `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`

### 2026-03-17 - cleanroom 第二轮接口闭环已通过
**用户目标**：
> 在 cleanroom 现场继续完成第二轮接口闭环，只闭合 `FarmToolPreview.cs / GameInputManager.cs` 的接口关系并重新验证 compile，不回污染分支，不扩围到更多文件。

**完成事项**：
1. 复核 `GameInputManager.cs` 当前调用已是 `11e0b7b4` 口径，本轮无需再改它。
2. 仅在 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 内补齐 `PreviewCellKey` 追踪、按层收集占位集合以及执行态提升/移除接口。
3. 重新执行 cleanroom batchmode compile，验证 `FarmToolPreview.cs` 与 `GameInputManager.cs` 的接口闭环。

**已证实事实**：
- `FarmToolPreview.cs` 现已对齐到 `11e0b7b4` 版接口口径。
- batchmode 编译已通过，日志位于 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`。
- 当前仅剩 `Assets\Editor\NPCPrefabGeneratorTool.cs(355,9)` 的 obsolete warning，不属于 farm cleanroom。

**当前结论**：
- `2026.03.16/1.0.2纠正001` cleanroom 已从“compile 未闭环”推进到“代码与编译可接替污染分支”状态。
- 当前主线已从“确认最小补齐集”切换为“做 cleanroom 收尾并形成 checkpoint”。
