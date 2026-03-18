# 农田交互修复V2 - cleanroom 线程记忆

## 线程概览

本线程当前不再沿 `D:\Unity\Unity_learning\Sunset` 上的污染分支继续 farm 开发，而是转入 cleanroom `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001` 重建 `1.0.2纠正001` 的 farm-only 现场。

## 当前主线

- 主线目标：把 `1.0.2纠正001` 从污染分支拆出为可独立继续开发的 continuation branch，并在对齐最新 `main` 后继续实现与验收
- 当前 continuation branch：`codex/farm-1.0.2-cleanroom001`
- 污染分支：`codex/farm-1.0.2-correct001`
- 当前阶段：已完成 cleanroom 回放与 compile 闭环，正在把 continuation branch 归一到最新 `main` 基线，再继续 `1.0.2纠正001`

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
