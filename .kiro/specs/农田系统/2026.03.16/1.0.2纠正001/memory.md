# 1.0.2纠正001 - cleanroom 记忆

## 模块概览

本工作区用于在 cleanroom 中重建 `1.0.2纠正001` 的 farm-only 现场。当前只承接白名单文件回放、记录类文件重写与最小编译验证，不再沿污染分支继续开发。

## 当前状态

- cleanroom 工作目录：`D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- cleanroom 分支：`codex/farm-1.0.2-cleanroom001`
- cleanroom 基线：`b9b6ac4881f4436abbc1f3232f14706ca76bb869`
- 回放来源：
  - `07ffe199` 仅保留 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
  - `11e0b7b4` 回放 7 个 farm 代码文件与 4 份正文文档
- 污染现场：`D:\Unity\Unity_learning\Sunset` 上的 `codex/farm-1.0.2-correct001@11e0b7b4c1340e0359a546b038d711b03836dc72`
- 当前阶段：回放已完成，cleanroom 记忆已建立，compile 未闭环

## 会话记录

### 2026-03-17 - cleanroom 建立、回放与最小验证

**用户目标**：
> 从 `b9b6ac48` 起一条 cleanroom farm 分支，严格只回放指定的 farm-only 文件和 `1.0.2纠正001` 正文文档，禁止带入 NPC / mixed 污染内容，并确认 cleanroom 是否已经可以接替污染分支继续开发。

**完成事项**：
1. 确认 cleanroom 现场：
   - 工作目录 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
   - 分支 `codex/farm-1.0.2-cleanroom001`
   - HEAD `b9b6ac4881f4436abbc1f3232f14706ca76bb869`
2. 严格按白名单回放 12 个文件：
   - 代码：
     - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
     - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
     - `Assets/YYY_Scripts/Service/Placement/PlacementNavigator.cs`
     - `Assets/YYY_Scripts/Service/Placement/PlacementPreview.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
   - 正文文档：
     - `.kiro/specs/农田系统/2026.03.16/1.0.2纠正001/requirements.md`
     - `.kiro/specs/农田系统/2026.03.16/1.0.2纠正001/analysis.md`
     - `.kiro/specs/农田系统/2026.03.16/1.0.2纠正001/design.md`
     - `.kiro/specs/农田系统/2026.03.16/1.0.2纠正001/tasks.md`
3. 核对 `git status --short`，当前 tracked 变更仍只落在上述 12 个回放文件。
4. 用 cleanroom 新现场重写四份记录类文件，不直接照搬污染现场旧 memory。
5. 通过 Unity 6000.0.62f1 batchmode 执行最小编译验证，日志输出到 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`。

**明确排除**：
- `18f3a9e1` 的全部内容
- `07ffe199` 中除 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 外的全部 NPC 内容
- 所有 `NPC` 路径
- 治理归档 / 项目总览类文件
- 污染现场旧 `memory` 的直接复制

**验证结果**：
- `git status --short` 在编译前后都只包含 12 个指定回放文件，没有新增 tracked 污染。
- cleanroom 未接入独立 Unity MCP，本轮不使用共享根目录的 live Console 作为 cleanroom 结论依据。
- batchmode 编译失败，阻断为项目级 compile 错误，不是 MCP 或命令传输问题：
  - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
    - `PreviewCellKey` 与 `Vector3Int` 混用，导致多处 `Contains` / `TryGetValue` / `Add` / `Remove` 调用签名不匹配
    - `Random` 同时命中 `UnityEngine.Random` 与 `System.Random`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - `PromoteToExecutingPreview` / `RemoveExecutingPreview` 仍按双参数调用
- 额外有 1 条非 farm 代码 warning：`Assets/YYY_Tests/Editor/WorldItemDropSystemTests.cs(272,15)` 的未使用变量 `groundY`

**关键结论**：
- 当前 cleanroom 的回放边界是正确的，但指定回放集尚未构成可编译闭环。
- 因 compile 未通过，当前不能确认 cleanroom 已可接替污染分支继续 farm 后续开发。
- replay 得到的 `tasks.md` 是源现场正文，不代表 cleanroom 的实时验证状态；实时状态以本 `memory.md` 与上层 cleanroom 记忆为准。

**恢复点 / 下一步**：
- 先确认 cleanroom 的最小补齐集，消除 `FarmToolPreview.cs` / `GameInputManager.cs` 的 compile 阻断
- compile 通过前，不执行 checkpoint、提交或同步

### 2026-03-17 - 第二轮接口闭环修正完成
**用户目标**：
> 按治理修正版执行令，继续留在 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 的 `codex/farm-1.0.2-cleanroom001` 上，只围绕 `FarmToolPreview.cs / GameInputManager.cs` 完成第二轮接口闭环，不回污染分支、不扩围到更多文件，并再次执行 batchmode 编译验证。

**完成事项**：
1. 回读 `GameInputManager.cs@11e0b7b4` 的目标调用口径，确认 compile 阻断来自 `FarmToolPreview.cs` 仍停留在旧版接口。
2. 仅修改 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`，将其对齐到 `11e0b7b4` 口径：
   - 引入 `PreviewCellKey(layerIndex, cellPos)` 作为队列/执行态追踪主键。
   - 新增 `CollectOccupiedPreviewPositionsForLayer(layerIndex)`。
   - 新增 `PromoteToExecutingPreview(int layerIndex, Vector3Int cellPos)`。
   - 新增 `RemoveExecutingPreview(int layerIndex, Vector3Int cellPos)`。
   - 新增 `RemoveQueuePreview(int layerIndex, Vector3Int cellPos)`。
   - 将 `Random.Range(...)` 明确改为 `UnityEngine.Random.Range(...)`，消除 `System.Random` 二义性。
3. 保持 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 不再追加改动，只让现有 `11e0b7b4` 口径调用与 `FarmToolPreview.cs` 闭环。
4. 使用 Unity `6000.0.62f1` 重新执行 batchmode 编译验证，日志更新于 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`。

**验证结果**：
- 编译日志未再出现 `error CS`。
- 编译日志未再出现 `Scripts have compiler errors.`。
- 编译日志包含：
  - `Exiting batchmode successfully now!`
  - `Exiting without the bug reporter. Application will terminate with return code 0`
- 当前只看到 1 类非 farm warning：
  - `Assets\Editor\NPCPrefabGeneratorTool.cs(355,9): warning CS0618: TextureImporter.spritesheet is obsolete`

**关键结论**：
- `FarmToolPreview.cs` 已对齐到 `11e0b7b4` 的接口口径，不再停留在 `07ffe199` 版。
- 当前 cleanroom 的 compile 闭环已经打通，剩余 warning 属于 NPC editor 工具，不属于 farm cleanroom 阻断。
- 从代码与编译角度看，cleanroom 已达到可接替污染分支继续 farm 后续开发的状态；后续只剩 cleanroom 记忆与 Git 收尾。

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`

**恢复点 / 下一步**：
- 清理 batchmode 触发的非白名单 asset 噪音，避免混入 checkpoint。
- 按 cleanroom 现状追加更新三层 memory 与线程记忆。
- 若白名单 preflight 通过，则执行 `git-safe-sync.ps1` 做 cleanroom checkpoint。
