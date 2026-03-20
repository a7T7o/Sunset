# 1.0.2纠正001 - cleanroom 记忆

## 模块概览

本工作区用于在 cleanroom 中重建 `1.0.2纠正001` 的 farm-only 现场。当前只承接白名单文件回放、记录类文件重写与最小编译验证，不再沿污染分支继续开发。

## 当前状态

- cleanroom 工作目录：`D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- cleanroom 分支：`codex/farm-1.0.2-cleanroom001`
- cleanroom 基线：`4b9ad7ea750ca04d6701aa35f67383044eb3bc9d`
- 回放来源：
  - `07ffe199` 仅保留 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
  - `11e0b7b4` 回放 7 个 farm 代码文件与 4 份正文文档
- 污染现场：`D:\Unity\Unity_learning\Sunset` 上的 `codex/farm-1.0.2-correct001@11e0b7b4c1340e0359a546b038d711b03836dc72`
- 当前阶段：continuation branch 已 merge 最新 `main`，代码与 compile 闭环已复核，当前只剩 live 场景验收与 checkpoint 收尾

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

### 2026-03-18 - continuation branch 归一后复核与收尾准备
**用户目标**：
> 在 `codex/farm-1.0.2-cleanroom001` 上一条龙完成当前 `1.0.2纠正001` 的 continuation 收口：确认 merge `main` 后的真实代码状态、重新验证 compile、核查 MCP 连接，并把现场落盘成可继续接手的 checkpoint。

**完成事项**：
1. 复核 continuation branch 现场：
   - 工作目录 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
   - 分支 `codex/farm-1.0.2-cleanroom001`
   - `HEAD=4b9ad7ea750ca04d6701aa35f67383044eb3bc9d`
2. 回读 merge `main` 后的关键实现，确认本线已包含：
   - `GameInputManager.cs` 的 UI 冻结保护槽位、右键与 `WASD` 统一中断、浇水延迟随机与拒绝反馈同步
   - `FarmToolPreview.cs` 的按层 `PreviewCellKey`、队列/执行预览迁移与清理
   - `PlacementManager.cs` / `PlacementPreview.cs` 的种植排序修正与预览残留重置
3. 使用 `D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Unity.exe` 对 cleanroom 再次执行 batchmode 编译验证，日志输出到 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_continuation_compile.log`。
4. 发现 batchmode 临时改写 4 个 TMP 字体 `.asset`，已在 cleanroom 内恢复，不纳入 farm 白名单。
5. 尝试通过 Unity MCP 读取活动场景与 Console，但当前 transport 返回 `missing-content-type; body:`，本轮未拿到 live Unity 只读结果。

**验证结果**：
- `sunset_farm_continuation_compile.log` 包含：
  - `Exiting batchmode successfully now!`
  - `Exiting without the bug reporter. Application will terminate with return code 0`
- 当前 warning 仅剩两类非 farm 项：
  - `Assets\YYY_Tests\Editor\WorldItemDropSystemTests.cs(272,15)` 的 `groundY` 未使用
  - `Assets\Editor\NPCPrefabGeneratorTool.cs(355,9)` 的 `TextureImporter.spritesheet` obsolete
- `git status --short` 在恢复 batchmode 噪音后重新回到干净状态。
- MCP 本轮不可用，因此“compile 通过”是已证实事实，“Primary 场景 live 行为复验”仍待用户或后续可用 MCP 现场。

**关键结论**：
- continuation branch 在 merge 最新 `main` 后仍保持 farm-only 差异，且 compile 通过。
- `1.0.2纠正001` 当前剩余的不是新的代码恢复阻断，而是 live 场景验收与 branch checkpoint 收尾。

**恢复点 / 下一步**：
- 先按白名单同步本轮 memory 收尾，形成 continuation branch checkpoint。

### 2026-03-20 - batch03 交付面收口续跑与 carrier 清洗
**用户目标**：在 shared root 的 `codex/farm-1.0.2-cleanroom001` 上继续执行 `1.0.2` 交付面收口，不新增业务实现，只把当前 carrier 收束成“仅保留 farm 1.0.2 交付面”的可提交状态。  
**本轮子任务 / 服务主线**：
- 子任务：补记本轮 batch03 收口事实，并对白名单 sync 做最后准备。
- 服务主线：`1.0.2纠正001` 的 continuation branch 收口与可回退 checkpoint 固化。
**已完成事项**：
1. 复核 live 现场位于：
   - 工作目录 `D:\Unity\Unity_learning\Sunset`
   - 分支 `codex/farm-1.0.2-cleanroom001`
   - HEAD `e4ec0d8e44e59cce16c38b91784aa514a3d0e981`
2. 复核 `git diff --name-status main...HEAD`，确认当前 branch 相对 `main` 的主体仍是本子工作区既有交付面：
   - 8 个 farm 代码文件
   - `1.0.2纠正001` 四件正文
   - 三层 / 线程 memory
3. 复核 working tree 里的额外噪音仅为：
   - `AGENTS.md`
   - `scripts/git-safe-sync.ps1`
   - `.kiro/locks/shared-root-branch-occupancy.md`
4. 已将 `AGENTS.md` 与 `scripts/git-safe-sync.ps1` 的工作树内容对齐回 `main`，并确认：
   - `git diff --name-status main -- AGENTS.md scripts/git-safe-sync.ps1` 返回空
   - 说明这两处当前只是在“相对 branch HEAD 的恢复态”，尚未通过新的 sync 落成提交
**关键结论**：
- 本轮不是进入新的热文件或功能实现，而是把 continuation carrier 从“含 branch 噪音的现场”收束为“只保留 1.0.2 交付面”的 checkpoint。
- 当前仍未进入 `GameInputManager.cs` 热文件专项流程；本轮 `hotfile_lock` 口径仍为 `not-needed`。
**恢复点 / 下一步**：
- 继续按白名单提交本轮 memory 与 carrier 清洗结果，然后执行 `return-main` 归还 shared root。
