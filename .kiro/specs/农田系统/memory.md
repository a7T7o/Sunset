# 农田系统 - cleanroom 记忆

## 模块概览

本卷是针对 farm cleanroom 重建现场重新书写的总记忆，不沿用污染分支或共享根目录中的旧记录文本。它只保留当前可继续接手 farm cleanroom 的最小事实。

## 当前状态

- cleanroom 现场：`D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- cleanroom 分支：`codex/farm-1.0.2-cleanroom001`
- cleanroom 基线：`4b9ad7ea750ca04d6701aa35f67383044eb3bc9d`
- 污染现场：`D:\Unity\Unity_learning\Sunset` 上的 `codex/farm-1.0.2-correct001@11e0b7b4c1340e0359a546b038d711b03836dc72`
- 当前主线：`2026.03.16/1.0.2纠正001` 的 farm-only cleanroom 重建
- 当前结论：continuation branch 已 merge 最新 `main` 并重新验证 compile，通过后已具备继续承接 farm 开发的代码基础，当前只剩 live 场景验收与 checkpoint 收尾

## 会话记录

### 2026-03-17 - farm cleanroom 正式接管

**用户目标**：
> 停止沿污染分支开发，在独立 cleanroom 中按指定提交与白名单文件重建 farm-only 现场，并确认 cleanroom 是否已经达到可接替状态。

**完成事项**：
1. 确认 cleanroom 推荐起点 `b9b6ac48` 已建立为独立 worktree / 分支现场。
2. 只回放以下 farm-only 文件：
   - `07ffe199`：`Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
   - `11e0b7b4`：7 个 farm 代码文件 + `1.0.2纠正001` 的 4 份正文文档
3. 重写以下记录类文件为 cleanroom 版本：
   - `2026.03.16/1.0.2纠正001/memory.md`
   - `2026.03.16/memory.md`
   - `农田系统/memory.md`
   - `.codex/threads/Sunset/农田交互修复V2/memory_0.md`
4. 用 batchmode 独立验证 cleanroom，而不是借用共享根目录的 Unity live 会话。

**已证实事实**：
- `git status --short` 仅包含 12 个回放文件，没有混入 NPC 或治理文件。
- cleanroom 的 compile 阻断集中在 `FarmToolPreview.cs` 与 `GameInputManager.cs` 的签名/键类型不一致：
  - `PreviewCellKey` 与 `Vector3Int` 混用
  - `PromoteToExecutingPreview` / `RemoveExecutingPreview` 调用签名不匹配
  - `Random` 二义性
- 因 compile 失败，当前不满足 `git-safe-sync.ps1` 的安全提交前提。

**明确排除**：
- `18f3a9e1` 的全部内容
- `07ffe199` 中除 `FarmToolPreview.cs` 外的全部 NPC 资产、脚本、NPC 文档与 NPC 线程记忆
- 所有 `NPC` 路径
- 治理归档 / 项目总览类文件
- 污染现场旧 `memory` 的直接复制

**恢复点 / 下一步**：
- 先在 cleanroom 内补齐 compile 闭环，再决定是否进入白名单提交与后续 farm 开发
- 在 compile 通过前，污染分支继续只保留取证身份，不恢复 farm 写入

### 2026-03-17 - farm cleanroom 已完成第二轮编译闭环
**用户目标**：
> 在 farm cleanroom 中完成 `1.0.2纠正001` 的第二轮接口闭环，确认是否已经可以接替污染分支继续承担后续 farm 开发。

**完成事项**：
1. 维持 cleanroom 基线 `b9b6ac48` 不变，不回到 `codex/farm-1.0.2-correct001`。
2. 只修改 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`，补齐与 `GameInputManager.cs@11e0b7b4` 的接口闭环。
3. 重新通过 batchmode 验证 cleanroom compile。

**当前主线状态**：
- farm-only cleanroom 已具备可继续开发的代码与编译基础。
- 剩余工作不再是业务恢复，而是 cleanroom 现场的白名单收尾与 checkpoint 固化。

**当前阻塞/边界**：
- 已无 farm 专属 compile 阻断。
- 仍需在 Git 收尾前排除 batchmode 触发的 TMP 资产噪音，避免污染 cleanroom 提交边界。

### 2026-03-18 - continuation branch merge main 后的总线复核
**用户目标**：
> 不再从杂乱旧线程回看结论，而是直接以 continuation branch 的最新现场为准，确认当前 farm 还剩什么、哪些已经就位，并把本轮进展沉淀成可继续接手的真实记忆。

**完成事项**：
1. 复核 cleanroom continuation branch 当前位于 `4b9ad7ea750ca04d6701aa35f67383044eb3bc9d`，已 merge 最新 `main`。
2. 确认 `main..HEAD` 的差异仍是 farm 专属白名单内容，不含 NPC 业务文件。
3. 重新回读 `GameInputManager.cs`、`FarmToolPreview.cs`、`PlacementManager.cs`、`PlacementPreview.cs`、背包链相关脚本，确认 `1.0.2纠正001` 的主要实现口径仍然在位。
4. 使用 Unity 6000.0.62f1 对 cleanroom 再次执行 batchmode compile，并恢复编译产生的无关 TMP 字体资产噪音。
5. 只读尝试 Unity MCP，确认当前共享 Unity 实例存在 transport 异常，无法作为本轮 live 结论来源。

**验证结果**：
- batchmode compile 通过；非 farm warning 仍只剩测试/Editor 工具两类旧项。
- cleanroom worktree 已恢复到干净状态。
- MCP live 读取失败，错误为 `missing-content-type; body:`。

**当前结论**：
- farm continuation branch 现在已经是“代码闭环 + 编译通过 + 可继续承接”的现场。
- 当前未完成的是用户在真实 `Primary` 场景里的交互验收，而不是新的恢复工程。

**恢复点 / 下一步**：
- 先执行白名单 Git 收尾，形成 continuation branch checkpoint，再等待用户场景验收。

### 2026-03-20 - shared root branch-only 收口续跑
**用户目标**：在 `main + branch-only` 现行模型下，把 farm 的唯一 continuation branch 进一步收束为干净的 `1.0.2` 交付 carrier，并在完成后及时归还 shared root。  
**本轮子任务 / 服务主线**：
- 子任务：只做 branch 收口，不做新的农田功能开发，不进入 Unity/MCP，不新增热文件改动。
- 服务主线：让农田系统当前唯一 continuation branch 具备稳定可接手、可回退、可继续分发的 checkpoint 形态。
**已完成事项**：
1. 复核当前 live 现场已不再使用历史 cleanroom worktree，而是在 shared root `D:\Unity\Unity_learning\Sunset` 的任务分支模式下续跑。
2. 复核当前 active carrier 仍是：
   - 分支 `codex/farm-1.0.2-cleanroom001`
   - HEAD `e4ec0d8e44e59cce16c38b91784aa514a3d0e981`
3. 确认本轮实际要收口的是两层内容：
   - branch 中本就存在的 `1.0.2` farm 交付面
   - 当前 working tree 中相对 branch HEAD 的 carrier 清洗恢复态（`AGENTS.md`、`scripts/git-safe-sync.ps1`）
**关键结论**：
- farm 当前唯一正确 continuation 口径没有变化，仍是 `codex/farm-1.0.2-cleanroom001`。
- 本轮主线不是“证明 farm 是否还在”，而是把这个 continuation carrier 真正整理成只保留农田交付面的分支现场。
**恢复点 / 下一步**：
- 继续完成本轮白名单 sync 与 `return-main`；完成后 shared root 才算被正式归还。
