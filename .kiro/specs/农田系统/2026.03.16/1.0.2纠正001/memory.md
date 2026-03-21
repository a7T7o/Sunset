# 1.0.2纠正001 - 开发记忆

## 模块概述

本子工作区承接用户在 `1.0.0` / `1.0.1` 验收后追加的那一组深层交互纠偏，核心关注：

- 活跃手持槽位保护
- UI 打开时的冻结与恢复
- `WASD` / 右键导航 / 高频点击的统一中断语义
- 农田预览残留与执行预览回收
- 种植后幽灵透明作物的显示链防守

## 当前状态

- **完成度**: 85%
- **最后更新**: 2026-03-21
- **状态**: 代码已在 `main` working tree，待白名单收口与用户场景验收
- **当前焦点**: 把 `1.0.2` 正文与记忆补回 `main`，再执行本线 Git 收口

## 会话记录

### 2026-03-21：main-only 现场重接管并把 1.0.2 正文补回 main

**用户需求**:
> 当前已经切到 `main-only`，要求重新拾起此前所有关于农田交互升级的讨论与实现，把 `1.0.2` 的真实落地情况彻底对齐到 `main`，并开始守底线的 main-only 自治。

**当前主线目标**:
- 在 `D:\Unity\Unity_learning\Sunset @ main` 上收拢 `1.0.2纠正001` 的代码、文档与记忆，让用户后续直接在 `main` 做 Unity 场景验收。

**本轮子任务 / 阻塞**:
- 子任务是把当前 farm dirty 与历史 cleanroom 口径重新对齐，并把 `1.0.2` 正文和记忆正式补进 `main`。
- 共享阻断有两项，但都不属于 farm runtime 专属缺口：
  - `Assembly-CSharp-Editor` 仍卡在 `Assets/Editor/NPCPrefabGeneratorTool.cs -> NPCAutoRoamController`
  - MCP 当前返回 HTML 网关页，不能拿来做 Unity live 场景验收

**已完成事项**:
1. 重新核对 live 现场为 `D:\Unity\Unity_learning\Sunset @ main @ 8ac0fb5d0db0714f9879ed12885aefc056a03624`。
2. 重新核对本轮 farm dirty 代码范围，共 11 个脚本，均属于 `1.0.2` 的农田交互升级白名单。
3. 重新对照 `codex/farm-1.0.2-cleanroom001` 的 `requirements.md / analysis.md / design.md / tasks.md`，确认当前 `main` working tree 已承接大部分有效实现，且还有若干额外补位：
   - `CropController.cs`
   - `FarmToolPreview.cs`
   - `InventoryInteractionManager.cs`
   - `ToolbarSlotUI.cs`
4. 运行 runtime 独立编译，确认 `Assembly-CSharp.rsp = 0 error / 0 warning`。
5. 运行 Editor 独立编译，确认当前红编译来自共享 NPC Editor 文件，而不是 farm 代码。
6. 重新尝试 MCP 读场景 / Console，确认当前失败是网关 HTML，不是 Unity 项目报错。
7. 正式在 `main` 新建并补齐：
   - `requirements.md`
   - `analysis.md`
   - `design.md`
   - `tasks.md`
   - 本 `memory.md`

**关键决策**:
- 不再把 cleanroom 文档机械搬回 `main`，所有正文都改写为当前 `main` live 现场口径。
- 当前 `1.0.2` 的正确落点已经从“分支 continuation”切换为“`main` working tree + 白名单提交”。
- 当前不把共享 NPC Editor 红编译和 MCP 网关异常伪装成 farm 自身未闭环。

**涉及文件或路径**:
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\HotbarSelectionService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementNavigator.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\analysis.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.2纠正001\memory.md`

**验证结果**:
- Runtime 编译：通过，`0 error / 0 warning`
- Editor 编译：失败，但失败点为共享 NPC Editor 缺口
- MCP：失败，原因为网关 HTML 返回，不可作为 Unity live 验收依据

**恢复点 / 下一步**:
- 当前已经回到主线的“做本线白名单 Git 收口，然后交给用户在 `main` 做真实场景验收”这一步。
