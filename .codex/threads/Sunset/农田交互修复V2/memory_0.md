# 农田交互修复V2 - cleanroom 线程记忆

## 线程概览

本线程当前不再沿 `D:\Unity\Unity_learning\Sunset` 上的污染分支继续 farm 开发，而是转入 cleanroom `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 重建 `1.0.2纠正001` 的 farm-only 现场。

## 当前主线

- 主线目标：把 `1.0.2纠正001` 从污染分支拆出为可独立继续开发的 continuation branch，并在对齐最新 `main` 后继续实现与验收
- 当前 continuation branch：`codex/farm-1.0.2-cleanroom001`
- 污染分支：`codex/farm-1.0.2-correct001`
- 当前阶段：continuation branch 已归一到最新 `main` 并重新验证 compile，当前只剩 live 场景验收与 checkpoint 收尾

## 会话记录

### 2026-03-17 - cleanroom 正式执行

**用户目标**：
> 停止沿污染分支开发，严格按治理裁定在 cleanroom 里只回放 farm-only 文件，重写记录类文件，并确认 cleanroom 是否已经可以接替污染分支继续 farm 后续开发。

**已完成事项**：
1. 确认 cleanroom 工作目录、分支、HEAD。
2. 按指定白名单回放 8 个 farm 代码文件与 4 份正文文档。
3. 明确排除 `18f3a9e1` 全部内容、`07ffe199` 中除 `FarmToolPreview.cs` 外的全部 NPC 内容，以及旧 memory 直接照搬。
4. 用 cleanroom 新现场重写工作区记忆与线程记忆。
5. 运行 Unity batchmode 最小编译验证，日志为 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`。

**关键结论**：
- cleanroom 回放边界是干净的，当前 tracked 改动只有指定的 farm-only 文件。
- cleanroom 当时还不能接替污染分支继续开发，因为 compile 未通过。
- 阻断集中在：
  - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 的 `PreviewCellKey` / `Vector3Int` 混用与 `Random` 二义性
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 对 `PromoteToExecutingPreview` / `RemoveExecutingPreview` 的旧签名调用

**恢复点 / 下一步**：
- 先在 cleanroom 内确认最小补齐集并消除 compile 阻断
- compile 通过前，不执行 checkpoint、提交或同步

### 2026-03-17 - cleanroom 第二轮接口闭环收口

**本轮主线目标**：
> 继续在 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 中完成 `1.0.2纠正001` 的第二轮接口闭环，让 cleanroom 真正具备接替污染分支的编译基础。

**本轮子任务**：
- 只闭合 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 与 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 的接口关系。

**已完成事项**：
1. 复核当前 cleanroom 位于 `codex/farm-1.0.2-cleanroom001@b9b6ac4881f4436abbc1f3232f14706ca76bb869`。
2. 仅修改 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`，补齐 `PreviewCellKey`、按层占位集合、执行态提升/移除接口与 `UnityEngine.Random.Range` 口径。
3. 重新执行 batchmode compile，确认不再出现 farm `error CS` 或 `Scripts have compiler errors.`。

**验证结果**：
- 编译日志 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log` 显示 return code 0。
- 当前仅剩 NPC editor 工具的 obsolete warning，不属于 farm cleanroom 阻断。

**关键结论**：
- cleanroom 已从“白名单回放但 compile 未闭环”推进到“代码与编译已闭环，可接替污染分支继续 farm 后续开发”。
- 当前真实恢复点已从“补齐接口”切换为“cleanroom Git 白名单收尾与 checkpoint”。

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `C:\Users\aTo\AppData\Local\Temp\sunset_farm_cleanroom_compile.log`

**下一步主线动作**：
- 清理 batchmode 噪音并执行 cleanroom 白名单同步，形成可回退 checkpoint。

### 2026-03-18 - continuation branch 基线归一并恢复 `1.0.2纠正001` 主线

**用户目标**：
> 不再沿历史杂乱线程继续开发，而是把有效 farm 内容收拢后，在唯一 continuation branch 上继续 `1.0.2纠正001`；要求规范先行，但文档只为约束开发和记录进程服务。

**本轮子任务 / 阻塞**：
- 先确认共享根目录不适合作为 farm 可写现场，再把 `codex/farm-1.0.2-cleanroom001` 对齐到最新 `main`，消除旧基线继续开发的风险。

**已完成事项**：
1. 复核共享根目录 `D:\Unity\Unity_learning\Sunset` 当前被 NPC 分支占用，位于 `codex/npc-roam-phase2-003@7bc94fc830d9fcb739abd40feff3a911b9fb01a0`，且存在与 farm 无关的 dirty，不适合直接切回 farm 开发。
2. 复核 continuation branch `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 当前与 `main` 的关系为“落后 10 个提交、领先 1 个提交”，说明必须先归一基线。
3. 在 continuation worktree 中执行 `git merge main`，确认唯一冲突集中在本线程记忆文件，而不是 farm 业务代码。
4. 明确本线程记忆应保留 cleanroom 自身的连续性，不把旧主线程的大段历史机械并入 continuation branch。

**关键结论**：
- 当前 farm 后续开发的唯一正确 carrier 仍是 `codex/farm-1.0.2-cleanroom001`，但它必须先站到最新 `main` 上，后续 `1.0.2纠正001` 的代码行为才有意义。
- 共享根目录现在只适合作为治理 / 只读核查入口，不适合作为当前 farm 代码实现现场。
- continuation branch 的真实来源层级已经收敛为：`main` 承载已并回的 `1.0.0 / 1.0.1`，`codex/farm-1.0.2-cleanroom001` 承载新增的 `1.0.2纠正001`。

**恢复点 / 下一步**：
- 先完成 `merge main` 收尾，确认 continuation branch 已基于最新 `main`
- 然后回读 `1.0.2纠正001` 文档与关键代码，逐条完成尚未关闭的验收项

### 2026-03-18 - continuation branch 复核、编译与 MCP 状态确认
**用户目标**：
> 在 continuation branch 上一条龙完成当前可做的收尾：确认 merge `main` 后的真实农田实现状态，重新验证 compile，核查 MCP 是否可连接，并把当前现场沉淀成后续可直接接手的线程记忆。

**本轮子任务 / 服务对象**：
- 子任务：对 `1.0.2纠正001` 做 continuation 现场复核与验证收尾。
- 服务主线：确保后续 farm 开发继续沿 `codex/farm-1.0.2-cleanroom001` 推进，而不是再回到污染分支或过期快照。

**已完成事项**：
1. 复核 continuation branch 当前位于：
   - 工作目录 `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
   - 分支 `codex/farm-1.0.2-cleanroom001`
   - `HEAD=4b9ad7ea750ca04d6701aa35f67383044eb3bc9d`
2. 回读 merge `main` 后的关键代码，确认当前分支确实承载了：
   - UI 打开时的活跃手持槽位保护
   - 右键 / `WASD` 对农田自动链的一致收尾
   - 浇水样式“入队后、移出原格再刷新”
   - 种植后图层排序修正与预览残留清理
3. 使用 `D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Unity.exe` 对 cleanroom 重新执行 batchmode compile，日志为 `C:\Users\aTo\AppData\Local\Temp\sunset_farm_continuation_compile.log`。
4. 恢复了本次 batchmode 触发的 4 个 TMP 字体 `.asset` 脏改，避免把无关资源噪音混入 farm 现场。
5. 尝试通过 Unity MCP 读取活动场景与 Console，但当前 transport 报错 `missing-content-type; body:`，因此本轮没有 live Unity 读数。

**验证结果**：
- batchmode compile 通过，日志中包含：
  - `Exiting batchmode successfully now!`
  - `Exiting without the bug reporter. Application will terminate with return code 0`
- 当前 warning 仅剩两类非 farm 项：
  - `Assets\YYY_Tests\Editor\WorldItemDropSystemTests.cs(272,15)` `groundY` 未使用
  - `Assets\Editor\NPCPrefabGeneratorTool.cs(355,9)` `TextureImporter.spritesheet` obsolete
- `git status --short` 已回到干净状态。

**关键结论**：
- 当前 continuation branch 已经是 merge 最新 `main` 后仍可编译、可继续接手 `1.0.2纠正001` 的唯一正确现场。
- 当前真正剩余的是用户在 Unity 场景里的 live 验收，不是新的代码恢复缺口。

**恢复点 / 下一步**：
- 先执行本轮 memory 的白名单 Git 收尾，形成 continuation branch checkpoint。

### 2026-03-20 - batch03 交付面收口续跑
**用户目标**：在 NPC 线程退场后继续执行农田 `1.0.2` 的真实业务批次收口，让本线程把 shared root 上的 continuation branch 收束到可直接验收、可直接回退的 checkpoint。  
**当前主线目标**：
- 主线仍是 `1.0.2纠正001` 的 continuation branch 收口，不是新的 farm 功能开发。
**本轮子任务 / 服务对象**：
- 子任务：在已成功 `request-branch -> ensure-branch` 进入 `codex/farm-1.0.2-cleanroom001` 后，完成 memory 回写、carrier 噪音清洗提交与 `return-main`。
- 服务主线：把本线程当前持有的 shared root 分支占用一次性收口，避免继续占槽。
**已完成事项**：
1. 复核 live 现场：
   - 工作目录 `D:\Unity\Unity_learning\Sunset`
   - 分支 `codex/farm-1.0.2-cleanroom001`
   - HEAD `e4ec0d8e44e59cce16c38b91784aa514a3d0e981`
2. 复核 `git diff --name-status main...HEAD`，确认 branch 相对 `main` 的核心差异仍是农田 `1.0.2` 既有交付面，而不是新的混线内容。
3. 复核当前 working tree 的 branch 噪音只剩：
   - `AGENTS.md`
   - `scripts/git-safe-sync.ps1`
   - `.kiro/locks/shared-root-branch-occupancy.md`
4. 已把 `AGENTS.md`、`scripts/git-safe-sync.ps1` 的内容恢复到 `main` 版本；它们当前仍显示为 `M`，原因是 branch HEAD 里带有旧版本，尚需一次新的 sync 把“恢复态”正式提交。
**关键决策**：
- 本轮不重新回到 smoke / hotfile 专项，不做新的业务实现。
- 本轮优先走“真实 carrier 清洗 / 真实 checkpoint”路径，先把非热文件噪音从 continuation branch 清出。
- 当前尚未触碰 `GameInputManager.cs` 热文件锁；本轮 hotfile 口径维持 `not-needed`。
**恢复点 / 下一步**：
- 继续按白名单提交四层 memory 与 `AGENTS.md` / `scripts/git-safe-sync.ps1` 的恢复改动；若 sync 成功，则立刻 `return-main` 并结束本轮占槽。

### 2026-03-21 - batch04 热文件真实 checkpoint 续跑
**用户目标**：在 `ticket=15` 的租约已经发给本线程后，继续执行 `农田交互修复V2_1.0.2热文件真实checkpoint与main-ready核验.md`，并产出一个真正的热文件 checkpoint。  
**当前主线目标**：
- 主线已从 batch03 的交付面收口切换到 batch04 的热文件真实 checkpoint 与 `main-ready` 核验。
**本轮子任务 / 服务对象**：
- 子任务：优先收口 `GameInputManager <-> HotbarSelectionService` 入口链，让 Toolbar/直接 `SelectIndex()` 不再绕开农田拒绝与收尾语义。
- 服务主线：把 `1.0.2` 推进到“已产生新的热文件可提交增量”的状态。
**已完成事项**：
1. live preflight 现场：
   - 工作目录 `D:\Unity\Unity_learning\Sunset`
   - 分支 `main`
   - HEAD `defe85857a9e4c09a2963eea364106efefefc35e`
   - occupancy 显示 `lease_state = branch-granted`，租约归属 `农田交互修复V2 -> codex/farm-1.0.2-cleanroom001`
2. `request-branch` 返回 `ALREADY_GRANTED`，`ensure-branch` 成功进入 `codex/farm-1.0.2-cleanroom001`。
3. `Check-Lock` 显示 `GameInputManager.cs = unlocked`，随后 `Acquire-Lock` 成功。
4. 已修改：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
5. 本轮真实 checkpoint 内容：
   - 新增 `GameInputManager.TryPrepareHotbarSelectionChange(int requestedIndex)`
   - `HotbarSelectionService.SelectIndex()` 统一先走该钩子
   - `GameInputManager` 自己的正常热栏切换路径改为复用服务层统一入口
6. `git diff --check` 通过，当前未发现内容级格式错误。
**关键决策**：
- 本轮不再重复证明旧 branch 已有什么，而是让“热栏切换的统一入口权威”真正落到代码里。
- 当前热文件锁已持有；在 sync 成功前不允许自行 release / return-main。
**恢复点 / 下一步**：
- 继续完成白名单 sync；若成功，则释放 `GameInputManager.cs` 锁并 `return-main`，随后给出最小回执中的 `carrier_ready / main_ready`。

## 2026-03-21：recovery-control-01 当前业务 checkpoint
本轮用户通过 `恢复开发总控_01` 正式放行当前农田线程继续开发，要求在 `codex/farm-1.0.2-cleanroom001` 上推进一个真实业务 checkpoint，而不是只做 carrier 说明。按放行 prompt 已完成 `request-branch -> ensure-branch`，并确认允许域仍限定在 `GameInputManager / Farm / Placement / UI/Inventory / HotbarSelectionService` 及 `2026.03.16` 相关文档。回读当前分支实现后，识别出一个不需要 Unity/MCP、但确实属于 `1.0.2` 主线的剩余缺口：虽然当前 branch 已经挡住交换落点、整理与热栏切换，但普通拖拽起手仍可能绕过“受保护手持槽位不可交换”的规则。因此本轮把最小真实 checkpoint 收敛为：在 `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs` 的 `OnBeginDrag(...)` 中补上受保护手持槽位拒绝，让背包第一行 / Toolbar 对应槽位在“当前正在使用中”时，连普通拖拽起手也会被既有拒绝反馈拦下。这个 checkpoint 服务的仍是用户明确提出的主线语义：“UI 打开时只允许背包更新，但不会因为 UI 交换而改变正在执行、正在播放或正在使用的手持内容。” 当前恢复点：继续完成本轮白名单 `sync`，然后 `return-main`，再根据 live diff 回答 `carrier_ready / main_ready`。
