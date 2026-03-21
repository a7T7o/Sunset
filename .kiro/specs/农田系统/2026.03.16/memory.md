# 农田系统 2026.03.16 - 开发记忆

## 模块概述

本工作区承接 2026-03-16 起的新一轮农田交互优化，按两条子线推进：
- `1.0.0图层与浇水修正`：作物创建后的图层/排序修正，浇水样式随机时机修正
- `1.0.1自动农具进阶中断`：自动农具队列的拒绝切换、拒绝拖拽、失败反馈与中断口径收束

## 当前状态

- **完成度**: 0%
- **最后更新**: 2026-03-16
- **状态**: 进行中
- **当前焦点**: 先完成 1.0.0，再收口 1.0.1

## 会话记录

### 会话 2026-03-16（新主线建档与实现前接管）

**用户需求**:
> 农作物创建时图层顺序不对；浇水应在队列创建后且鼠标移出当前农田框时才更新样式；自动农具队列中打开背包拖走工具、Toolbar/滚轮/快捷键切换都应被拒绝并给失败反馈；这次拆成 1.0.0 和 1.0.1 两个子工作区推进。

**完成任务**:
1. 创建 `2026.03.16` 父工作区和两个子工作区目录。
2. 明确主线拆分：`1.0.0` 负责图层与浇水时机，`1.0.1` 负责自动农具进阶中断。
3. 在动代码前重新核对 `PlacementManager`、`FarmToolPreview`、`CropController`、`GameInputManager`、`ToolbarSlotUI`、`InventoryInteractionManager`、`InventorySlotInteraction`、`PlayerInteraction`。

**关键决策**:
- 自动拒绝切换/拖拽逻辑只针对“自动农具队列进行中”，不能误伤手动长按连续动作。
- 作物排序以玩家现有正确遮挡关系为基准，优先复用项目已有 `DynamicSortingOrder`。
- 浇水随机样式以“成功入队后，移出本次格子时才刷新下一次样式”为唯一口径。

**涉及文件**:
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\DynamicSortingOrder.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`

**验证结果**:
- 已完成代码与规则接管。
- 尚未开始本轮代码修改与 Unity 验证。

**下一步**:
- 先完成 `1.0.0图层与浇水修正` 的实现与验证，再进入 `1.0.1自动农具进阶中断`。

## 2026-03-16：1.0.0 / 1.0.1 代码已落地，当前停在同步回 main 前的收尾阶段
本轮已在 `D:\Unity\Unity_learning\Sunset` 仓库内完成 `1.0.0图层与浇水修正` 与 `1.0.1自动农具进阶中断` 的代码实现，实际工作分支为 `codex/farm-1.0.0-1.0.1`，当前 `HEAD` 为 `9b9a6bd0dd7c5ee7d18cc82e3ea9da74a146bf9d`。代码层面已经落地的内容包括：`PlacementManager.cs` 的作物图层/排序修正、`FarmToolPreview.cs` 与 `GameInputManager.cs` 的浇水随机样式时机修正、`GameInputManager.cs` / `ToolbarSlotUI.cs` / `InventorySlotUI.cs` / `InventorySlotInteraction.cs` 的自动农具队列拒绝切换与拒绝拖拽反馈。验证层面已完成 Roslyn 编译通过与 Unity MCP/Console 只读核查，当前 Console 未见新的 farm 相关红编译，但用户真实 PlayMode 手动验收尚未完成。由此更新父工作区现场：当前不是“尚未开始实现”，而是“实现已完成，停在 tasks/memory 补写、白名单提交、释放锁并同步回 `main` 前的收尾阶段”；同时仓库仍有大量无关 dirty / untracked，后续 Git 收尾必须严格白名单，不能无边界提交。

## 2026-03-16：白名单同步前的最终验证已完成
本轮继续沿 `2026.03.16` 父工作区推进收尾，先通过 Unity MCP 复核当前活动场景仍为 `Assets/000_Scenes/Primary.unity`，再触发 Unity 刷新/编译并读取 Console。当前 Console 仅见共享 Editor warning：`Assets/Editor/NPCPrefabGeneratorTool.cs(355,9)` 的 `TextureImporter.spritesheet` obsolete，未出现新的 farm 相关 error / warning。随后使用 `scripts/git-safe-sync.ps1 -Action preflight -Mode task` 对 `1.0.0/1.0.1` 相关代码、文档、父/线程记忆做白名单预检，结果允许继续同步，且无关 dirty 仍被正确隔离在白名单之外。由此更新当前恢复点：`2026.03.16` 工作区的实现、验证、tasks/memory 补写都已完成，下一步仅剩创建 checkpoint、释放 `GameInputManager.cs` 锁并把结果同步回 `main`。

## 2026-03-16：2026.03.16 农田补丁已同步回 main
本轮已完成 `2026.03.16` 工作区的 Git 收尾：先在 `codex/farm-1.0.0-1.0.1` 上通过白名单同步生成 checkpoint `7aadbde7`，随后将 `main` 快进到该提交并推送远端。同步完成后，又显式释放了 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 的 A 类锁，确保验收现场不再带锁。由此父工作区状态正式切换为“实现、验证、Git 同步均已完成，当前等待用户在 `main` 做真实场景验收”，不再是“待回 main”的中间态。

## 2026-03-21：main-only 重新接管后，1.0.2 也正式收拢到 2026.03.16 体系
用户在 `main-only` 新规则下重新唤醒农田线程，要求不再把 `1.0.2` 继续理解为 cleanroom/continuation 分支上的历史材料，而是直接在 `D:\Unity\Unity_learning\Sunset @ main` 上彻查并完成全部交互升级。本轮只读审计后确认：当前 `main@8ac0fb5d0db0714f9879ed12885aefc056a03624` 的 working tree 已经承接了大部分 `1.0.2纠正001` 的有效代码 dirty，范围包括 `GameInputManager.cs`、`FarmToolPreview.cs`、`PlacementManager.cs`、`PlacementNavigator.cs`、`PlacementPreview.cs`、`InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`InventorySlotUI.cs`、`ToolbarSlotUI.cs`、`HotbarSelectionService.cs`、`CropController.cs`。同时重新验证：`Assembly-CSharp.rsp` 运行时代码独立编译通过（`0 error / 0 warning`）；`Assembly-CSharp-Editor.rsp` 当前仍被共享 NPC Editor 文件 `Assets/Editor/NPCPrefabGeneratorTool.cs(789,43)` 阻断；MCP 读场景/读 Console 当前返回 HTML 网关页，不可作为 Unity live 验收依据。基于这组 live 事实，本轮在 `main` 新补回了 `2026.03.16/1.0.2纠正001` 的 `requirements.md / analysis.md / design.md / tasks.md / memory.md`，并改写为当前 `main-only` 现场口径，而不是机械复制旧 cleanroom 文稿。当前父工作区恢复点更新为：`2026.03.16` 体系下的三条子线 `1.0.0 / 1.0.1 / 1.0.2` 现在都已在 `main` 语义内合拢；下一步只剩按白名单做本线 Git 收口，然后交给用户在 `main` 做真实 Unity 场景验收。
