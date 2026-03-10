# 农田系统 - 开发记忆

> 本卷起始于 memory_0.md 之后。历史详细记录请查阅 memory_0.md。

## 模块概述

农田系统，包括：
- 耕地状态管理（锄地、浇水）
- 作物生长周期
- 水分系统
- 季节适应性
- **种子种植**（SeedData 专属，与树苗分离）

## 当前状态

- **完成度**: 65%
- **最后更新**: 2026-02-25
- **状态**: 🚧 进行中
- **当前焦点**: 10.1.5补丁005（种子并入放置系统）+ 10.1.4补丁004（三层预览架构）

---

## 🔴 重要架构决策 🔴

### 1. 种子归属农田系统（2025-12-30）
种子（SeedData）由农田系统管理，与树苗（SaplingData）分离。

### 2. 彻底解耦架构（2026-01-27）
FarmingManagerNew 已废弃，FarmTileManager/CropController/CropManager 各自自治。

### 3. 联邦制预览架构（2026-02-06）
FarmToolPreview 独立于 PlacementPreview，共享 PlacementGridCalculator。

### 4. 种子应并入放置系统（2026-02-25，待执行）
种子走 FIFO 队列是架构错位，放置系统是正确归宿（树苗是成功案例）。

---

## 关键文件索引

### 核心脚本
| 文件 | 说明 |
|------|------|
| `FarmTileManager.cs` | 耕地状态管理，自治时间事件 |
| `FarmlandBorderManager.cs` | 边界视觉管理，预览接口 |
| `FarmToolPreview.cs` | 农田工具预览组件 |
| `CropController.cs` | 作物控制器，自治时间事件 |
| `CropManager.cs` | 作物工厂 |
| `GameInputManager.cs` | 锄地/浇水/种植/收获调用入口 |
| `PlacementGridCalculator.cs` | 坐标对齐（共享） |

---

## 子工作区索引

| 编号 | 名称 | 状态 | 说明 |
|------|------|------|------|
| 0~7 | 早期工作区 | ✅ 完成 | 详见 memory_0.md |
| 8 | 修复 | 🚧 进行中 | P0 bug 修复 |
| 9.0.1 | 重生之我在种田 | ✅ 完成 | 进度整理 |
| 9.0.2 | 放置农田 | 🚧 进行中 | 预览系统设计 |
| 9.0.5 | 智能交互bug修复 | ✅ 代码完成 | 锁定机制+5状态机，待验收 |
| 10.0.1 | 农作物设计与完善 | ✅ 代码完成 | 种子袋+作物7样式+CropController重构，待验收 |
| 10.0.2.1 | 纠正（废弃CropManager） | ✅ 完成 | 全面对齐树木Prefab驱动模式 |
| 10.1.0 | 全面持久化前夕 | 🚧 进行中 | 全面审查报告完成 |
| 10.1.1补丁001 | 交互漏洞修补 | ✅ 完成 | 二次修复完成，待二次验收 |
| 10.1.1补丁002 | 交互体验优化 | 🚧 进行中 | design V2 + tasks V2 完成，待审核 |
| 10.1.1补丁003 | 预览系统改造 | ✅ 代码完成 | shader覆盖层+双Tilemap分离+队列预览，待验收 |
| 10.1.4补丁004 | 三层预览架构重构 | 🚧 进行中 | 005锐评系列完成，种子剥离方向确定 |
| 10.1.5补丁005 | 种子并入放置系统 | ✅ 代码完成 | 全部代码任务完成，待手动验收 |

---

## 上一卷最后记录回顾

memory_0.md 最后记录（会话2续53，2026-02-25）：
- 10.1.5补丁005 完成锐评001审核 + design.md + tasks.md 生成
- 种子并入放置系统方案确定，15个子任务待执行

---

## 会话记录

（新卷起始，后续会话从此处追加）

### 元讨论 - 2026-02-25（Memory 分卷制改造方案讨论与实施）

**用户需求**：主 memory 膨胀（3267行）导致上下文压力，讨论并实施分卷制改造方案。

**完成任务**：
1. ✅ 确定分卷参数：主 memory 200行/卷，子 memory 300行/卷
2. ✅ 确定命名方案：活跃卷始终 memory.md，归档为 memory_0/1/2...
3. ✅ memory-update-check.kiro.hook 更新为 v9.0（分卷检查 + 全对话记录 + 主memory摘要3-5行限制）
4. ✅ rules.md "工作区记忆"部分重写（融入 workspace-memory 关键规则：分卷制、更新规则、禁令）
5. ✅ 000-context-recovery.md 加入分卷感知规则
6. ✅ 农田系统主 memory 归档（3267行 → memory_0.md，新建精简 memory.md）

**未完成**：
- ❌ 10.1.4补丁004 子 memory 归档（1774行，待归档）
- ❌ 其他超限子 memory 检查

**关键决策**：workspace-memory.md 降级为参考文档，关键规则融入 rules.md 常驻；所有对话都必须记录到 memory。

**修改文件**：memory-update-check.kiro.hook、rules.md、000-context-recovery.md、memory_0.md（归档）、memory.md（重建）

### 元讨论续1 - 2026-02-25（10.1.4补丁004 子 memory 归档）

**完成任务**：10.1.4补丁004 子 memory 归档（1774行 → memory_0.md + 新建82行精简 memory.md）。

**发现**：其他超限子 memory（10.1.3补丁003: 920行、10.1.0全面持久化前夕: 763行、9.0.5智能交互bug修复: 555行、10.0.1农作物设计与完善: 507行、10.1.2补丁002: 447行），用户未明确要求处理。

**修改文件**：10.1.4补丁004/memory_0.md（归档）、10.1.4补丁004/memory.md（重建）

### 会话2续55 - 2026-02-25（锐评002审核 + 两件套完备性检查）

**完成**：锐评002事实核查（layerIndex断层⚠️部分正确但高估）、10.1.4遗留分析（不影响005）、两件套完备性检查通过并补充 layerIndex 获取方式。

**修改文件**：design.md、tasks.md（补充 layerIndex 获取明确化）

### 会话3续1~续2 - 2026-02-25（补丁005一条龙执行完成）

**完成**：补丁005全部代码任务完成。任务A修复房子不标红（FarmToolPreview.UpdateHoePreview条件修正）；任务B种子从FIFO剥离并接入放置系统（SeedData标记isPlaceable、PlacementValidator/Manager/Preview种子分支、GameInputManager+FarmToolPreview FIFO清理）。编译通过（0错误2警告，均为已有）。

**修改文件**：SeedData.cs、PlacementManager.cs、PlacementValidator.cs、PlacementPreview.cs、FarmToolPreview.cs、GameInputManager.cs

**状态**：✅ 代码完成，待手动验收（验收指南已创建）

### 会话3续8 - 2026-02-28（补丁005验收反馈：种子预览+耕地预览方向纠正）

**内容**：用户批评续4~续7方向错误。种子预览应保留方框（和箱子一致），hideCellVisuals 需回滚；耕地不可交互未耕地应显示红色1+8 tile预览（shader红色叠加）而非只有红方框。方案已确认待执行。
**待定**：障碍物检测标签白名单（稻草人/洒水器豁免）、树苗不能种在耕地上。

### 会话4续1~续2 - 2026-02-28（补丁005验收修复执行完成）

**完成**：PlacementPreview.cs hideCellVisuals 回滚彻底完成（UpdateGridCellPositions 残留清理）；FarmToolPreview.cs UpdateHoePreview else 分支拆分为"未耕地不可交互→红色1+8预览+shader红色叠加"和"已耕地不可交互→红方框"。编译通过。
**修改文件**：PlacementPreview.cs、FarmToolPreview.cs


### 会话4续3 - 2026-03-03（10.1.6补丁006：耕地判定bug全面审核）

**用户需求**：耕地判定严重bug，树苗附近显示绿色可创建但应该红色。测试案例：树苗(-21.98, 9.97, 0)，在(-22, 10, 0)等位置错误显示绿色。要求审核报告不直接改代码。
**完成**：创建审核001.md，发现根本原因：HasTreeAtPositionStatic只检查格子索引相同而非1.5×1.5距离，提出AABB正方形距离检测修复方案。
**修改文件**：10.1.6补丁006/审核001.md


## 2026-03-03：10.2.0改进001 - V 键统一放置模式方案分析
创建工作区，完成方案分析报告。推荐新增 PlacementModeManager 管理统一放置模式状态。待确认 7 个关键问题（架构设计、功能细节、UI 显示、工具切换）。
文件：审核001.md


## 2026-03-03：10.1.6补丁006 - 锐评审视通过，文档补充完成
审核三个锐评版本（V1/V2/V3），验证核心观点全部正确。创建锐评审视报告，补充10.1.6审核文档和10.2.0方案文档。
文件：001锐评审视报告.md、000审核001.md、000方案001.md


## 2026-03-03：10.1.6补丁006 - 双线并行重构代码完成
战线一：PlacementValidator AABB距离检测修复。战线二：V键状态机隔离（GameInputManager + FarmToolPreview）。
文件：PlacementValidator.cs、GameInputManager.cs、FarmToolPreview.cs


## 2026-03-06：10.2.0改进001 - V 键统一授权语义纠正与收尾修复
修正此前把 `V` 误收缩成“只控制农田工具模式”的错误理解，正式恢复为“统一约束农田工具链 + placeable 放置链”的总开关；已将纠正后的需求和设计记录到 `10.2.0改进001/纠正001.md`。代码侧完成 `GameInputManager.cs` 与 `HotbarSelectionService.cs` 的最小兼容修复：当前手持 placeable 时按 `V` 可立即进入/退出有效放置态，ESC/右键不再误伤 placeable 状态机；Unity 重新编译通过（0 error，2 个既有 warning）。
文件：10.2.0改进001/纠正001.md、Assets/YYY_Scripts/Controller/Input/GameInputManager.cs、Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs


## 2026-03-10：10.2.1补丁001 - 农田交互修复V2 审查建档完成
当前主线切入 `.kiro/specs/农田系统/2026.03.01/10.2.1补丁001/`，本轮未改代码，先完成审查五件套建档：`requirements.md`、`analysis.md`、`design.md`、`tasks.md`、`memory.md`。已核实五个主问题都成立：放置导航与成功判定脱钩、放置链漏检作物占位、`V` 模式下 `Hoe` 仍混入清作物/挖树苗语义、第四点必须采用“1+8 + 0.75/1/1.5”口径、树苗成长未并入作物占位。同步记录三个补充风险：`PlacementValidator.IsOnFarmland()` 仍为 `TODO + false`、`FarmTileManager.GetCurrentLayerIndex()` 恒返回 `0`、`PlacementManager` 导航中改点位会丢失 seed/sapling 专用验证。当前状态：文档待用户审核，审核通过后再进入实现。
文件：10.2.1补丁001/requirements.md、10.2.1补丁001/analysis.md、10.2.1补丁001/design.md、10.2.1补丁001/tasks.md、10.2.1补丁001/memory.md


## 2026-03-10：10.2.1补丁001 - 问题四口径统一结论补充
补充澄清第四点的核心不是“选 3x3 还是选 1.5”这种单值争论，而是统一同一交互足迹在三层表达中的口径：逻辑层是中心 1 格，视觉层是 1+8 邻域结构，物理层是 `1.5 x 1.5` 检测盒与 `0.75` half extent 的同义描述。后续实现应围绕“统一交互足迹”推进，而不是把九个格子错误地理解成等权硬占位。


## 2026-03-10：10.2.1补丁001 - 问题四最终确认补充
用户进一步确认后，正式把第四点实现基线固定为“基于放置系统格心坐标的 `1.5 x 1.5` 方框 footprint”。已核实主路径坐标基本统一：`GameInputManager` 用 `PlacementGridCalculator.GetCellCenter()` 对齐鼠标坐标，再转 `WorldToCell`，`FarmToolPreview` 再取回格心并交给 `PlacementValidator.HasFarmingObstacle()`。同时记录一个残留一致性问题：`FarmToolPreview.GetCellCenterWorld()` 的 fallback 仍用“先 `+0.5` 再交给 `PlacementGridCalculator`”的写法，表达口径不够干净，后续实现应顺手收掉。


## 2026-03-10：10.2.1补丁001 - 用户审核通过后整包实现完成
用户确认审查文档与问题四口径纠正后，主线从“待审核”切换为“直接完成 10.2.1 整包实现”。本轮落地完成：`PlacementManager` 新增统一验证入口并把导航改点位/导航到达都收束到同源重验；`FarmTileManager` / `PlacementValidator` / `FarmToolPreview` 完成作物占位与 `1.5 x 1.5` footprint 统一；`GameInputManager` / `PlayerToolHitEmitter` / `TreeController` 收口施工模式下 `Hoe/WateringCan` 的隔离边界，并把树成长阻挡并入农作物占位。同步更新 `10.2.1补丁001/tasks.md`，勾选实现任务并补充手动回归清单。验证方面：`mcp-unity` 当前连接失败，无法直接重编译；已通过 `Editor.log` 抓到并修复本轮唯一新增编译错误（`FarmTileManager.cs` 的 `Random` 歧义），仍需用户在编辑器内补跑一次最终编译确认。
文件：Assets/YYY_Scripts/Farm/FarmTileData.cs、Assets/YYY_Scripts/Farm/FarmTileManager.cs、Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs、Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs、Assets/YYY_Scripts/Farm/FarmToolPreview.cs、Assets/YYY_Scripts/Service/Placement/PlacementManager.cs、Assets/YYY_Scripts/Controller/Input/GameInputManager.cs、Assets/YYY_Scripts/Combat/PlayerToolHitEmitter.cs、Assets/YYY_Scripts/Controller/TreeController.cs、10.2.1补丁001/tasks.md


## 2026-03-10：10.2.1补丁001 - 独立编译闭环完成，等待编辑器内手动回归
继续沿 `10.2.1补丁001` 主线收尾。由于 `mcp-unity` 仍不可用，本轮改走 Unity 自带 Roslyn 编译链：直接复用 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp` 与 `Assembly-CSharp-Editor.rsp`，使用 `Unity 6000.0.62f1` 自带 `dotnet.exe + csc.dll` 做独立编译验证。结果：运行时程序集 `Assembly-CSharp` 独立编译 `0 error`，只剩 1 个既有 obsolete warning；编辑器程序集 `Assembly-CSharp-Editor` 独立编译 `0 error`。同时纠正 `10.2.1补丁001/tasks.md` 中“手动回归清单”被误勾选的问题，明确当前真实状态是“整包实现完成、源码独立编译通过、待 Unity 编辑器内逐项回归”。主线恢复点更新为：等待用户按清单验证箱子、树苗、种子、`Hoe/WateringCan` 的现场行为，如有现象再继续验收修补。
文件：10.2.1补丁001/tasks.md、10.2.1补丁001/memory.md、Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp、Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp-Editor.rsp


## 2026-03-10：10.2.2补丁002 - 重新审视 placeable/种子/树苗边界并完成文档建档
用户基于现场截图和实际需求，明确纠正 `10.2.1` 的理解偏差：普通 placeable、`SeedData`、`SaplingData` 不能再混用同一套耕地规则。本轮新建 `.kiro/specs/农田系统/2026.03.01/10.2.2补丁002/`，重新读取 `10.1.5补丁005`、`10.2.0改进001`、`10.2.1补丁001` 相关文档和 `PlacementManager / PlacementPreview / PlacementValidator / FarmToolPreview / SeedData / SaplingData / PlaceableItemData` 代码后，正式落盘五件套。新的稳定结论是：普通 placeable 的“禁压耕地”必须做成逐格红判定；种子继续走 `PlacementPreview`，但验证语义收敛到“播种”；树苗继续保留树苗专用验证链；`FarmToolPreview` 的 `1.5 x 1.5 footprint` 不向普通 placeable 泛化。当前状态：10.2.2 文档待用户审核，暂不进入代码实现。
文件：10.2.2补丁002/requirements.md、10.2.2补丁002/analysis.md、10.2.2补丁002/design.md、10.2.2补丁002/tasks.md、10.2.2补丁002/memory.md


## 2026-03-10：10.2.2补丁002 - 进入实现前先补 Git 可回退基线
用户在准备进入 `10.2.2补丁002` 真实实现前，要求先全面确认 `Sunset` 仓库是否已经具备足够完善的 git 工作流，确保龙虾后续操作全部可以退回。本轮审视确认两项核心基础已具备：仓库是正常 git 仓库，且 Unity 已开启 `Force Text + Visible Meta Files`；但同时也识别出四项关键缺口：当前仍在 `main` 且超前 `origin/main` 4 个本地提交、工作树不干净、`.claude/worktrees/agent-a2df3da0` gitlink 内存在本地设置改动并持续污染根状态、仓库缺失 `.gitattributes` 且 `core.autocrlf=true` 已产生行尾噪音。因此当前结论不是“不能回滚”，而是“有基础但还不够工程化”，不建议直接在现状上进入 `10.2.2` 代码实现。下一步应先补 git 安全基线：独立任务分支、处理 dirty 状态 / worktree 噪音、补 `.gitattributes`、固定 preflight + checkpoint 流程，再进入补丁实现。
文件：10.2.2补丁002/memory.md、ProjectSettings/EditorSettings.asset、ProjectSettings/VersionControlSettings.asset、.gitignore


## 2026-03-10：10.2.2补丁002 - Git 基线任务转交全局规则线程
用户决定不在当前业务线程里处理 git 工作流治理，而是转交给项目全局规则对话完成。本轮已收束好要转告的核心内容：当前 `10.2.2` 业务文档已就位，但实现前必须先补 `Sunset` 仓库级可回退基线，包括独立任务分支策略、dirty 状态治理、`.claude/worktrees/agent-a2df3da0` gitlink 噪音处理、`.gitattributes` 增补、以及 preflight / checkpoint / rollback 固化流程。当前农田主线恢复点更新为：等待全局规则线程完成 git 安全基线，然后再回到 `10.2.2` 实现阶段。
文件：10.2.2补丁002/memory.md、农田系统/memory.md


## 2026-03-11：10.2.2补丁002 - Git 基线已显著补齐，但实现仍需等待 dirty 状态拆分
治理线程已正式落地仓库级 Git 基线：新增 `.kiro/steering/git-safety-baseline.md`、`.gitattributes`、Git preflight Hook，重写安全版 `git-quick-commit.kiro.hook`，并把 `.claude/worktrees/agent-a2df3da0` 与 `.claude/settings.local.json` 从根仓库跟踪中移除。旧结论里的 `main ahead 4` 已经过期，当前 `main` 与 `origin/main` 同步；但这并不等于农田 `10.2.2` 已可直接开工，因为仓库仍 dirty，且 dirty 范围横跨治理线、about 文档、农田线和 `Assets/` 现场改动。当前农田主线新的恢复点是：继续暂停 `10.2.2` 实现，先等待 dirty 状态拆分清楚，再建立 `codex/farm-10.2.2-patch002` 任务分支并做一次正式 preflight。
文件：.kiro/steering/git-safety-baseline.md、.gitattributes、.gitignore、.kiro/hooks/git-preflight.kiro.hook、.kiro/hooks/git-quick-commit.kiro.hook、10.2.2补丁002/memory.md