# 农田系统 - cleanroom 记忆

## 模块概览

本卷是针对 farm cleanroom 重建现场重新书写的总记忆，不沿用污染分支或共享根目录中的旧记录文本。它只保留当前可继续接手 farm cleanroom 的最小事实。

## 当前状态

- cleanroom 现场：`D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
- cleanroom 分支：`codex/farm-1.0.2-cleanroom001`
- cleanroom 基线：`b9b6ac4881f4436abbc1f3232f14706ca76bb869`
- 污染现场：`D:\Unity\Unity_learning\Sunset` 上的 `codex/farm-1.0.2-correct001@11e0b7b4c1340e0359a546b038d711b03836dc72`
- 当前主线：`2026.03.16/1.0.2纠正001` 的 farm-only cleanroom 重建
- 当前结论：白名单回放已完成，但 cleanroom 编译未闭环，暂不能接替污染分支继续 farm 后续开发

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
