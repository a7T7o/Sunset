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
用户确认审查文档与问题四口径纠正后，主线从“待审核”切换为“直接完成 10.2.1 整包实现”。本轮落地完成：`PlacementManager` 新增统一验证入口并把导航改点位/导航到达都收束到同源重验；`FarmTileManager` / `PlacementValidator` / `FarmToolPreview` 完成作物占位与 `1.5 x 1.5` footprint 统一；`GameInputManager` / `PlayerToolHitEmitter` / `TreeController` 收口施工模式下 `Hoe/WateringCan` 的隔离边界，并把树成长阻挡并入农作物占位。同步更新 `10.2.1补丁001/tasks.md`，勾选实现任务并补充手动回归清单。验证方面：`旧 MCP 桥口径（已失效）` 当前连接失败，无法直接重编译；已通过 `Editor.log` 抓到并修复本轮唯一新增编译错误（`FarmTileManager.cs` 的 `Random` 歧义），仍需用户在编辑器内补跑一次最终编译确认。
文件：Assets/YYY_Scripts/Farm/FarmTileData.cs、Assets/YYY_Scripts/Farm/FarmTileManager.cs、Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs、Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs、Assets/YYY_Scripts/Farm/FarmToolPreview.cs、Assets/YYY_Scripts/Service/Placement/PlacementManager.cs、Assets/YYY_Scripts/Controller/Input/GameInputManager.cs、Assets/YYY_Scripts/Combat/PlayerToolHitEmitter.cs、Assets/YYY_Scripts/Controller/TreeController.cs、10.2.1补丁001/tasks.md


## 2026-03-10：10.2.1补丁001 - 独立编译闭环完成，等待编辑器内手动回归
继续沿 `10.2.1补丁001` 主线收尾。由于 `旧 MCP 桥口径（已失效）` 仍不可用，本轮改走 Unity 自带 Roslyn 编译链：直接复用 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp` 与 `Assembly-CSharp-Editor.rsp`，使用 `Unity 6000.0.62f1` 自带 `dotnet.exe + csc.dll` 做独立编译验证。结果：运行时程序集 `Assembly-CSharp` 独立编译 `0 error`，只剩 1 个既有 obsolete warning；编辑器程序集 `Assembly-CSharp-Editor` 独立编译 `0 error`。同时纠正 `10.2.1补丁001/tasks.md` 中“手动回归清单”被误勾选的问题，明确当前真实状态是“整包实现完成、源码独立编译通过、待 Unity 编辑器内逐项回归”。主线恢复点更新为：等待用户按清单验证箱子、树苗、种子、`Hoe/WateringCan` 的现场行为，如有现象再继续验收修补。
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
### 会话 2026-03-11（农田父工作区同步跨工作区说明入口）
**完成任务**:
- 已同步当前仓库 Git 自动同步与治理现状说明文档的存在与用途，供后续农田线接手时查阅。

### 会话 2026-03-11（农田父工作区同步补完后的说明口径）
**完成任务**:
- 已同步跨工作区说明文档已补完的事实，后续农田线接手时应优先通过该文档理解 Git 现场，而不是继续沿用旧的 `ahead 4` 口径。
- 已再次固定父工作区恢复点：农田线当前仍等待仓库 dirty 拆分与独立任务分支建立，不进入 `10.2.2补丁002` 实现。

## 2026-03-13：farm 主线切换到 main 控制台 warning 清理
当前 `farm` 的统一基线已切换为 `Sunset/main` 持续开发，不再默认使用独立 worktree，也不再把主线描述为“恢复回 main”。本轮只读核查确认：当前真正属于 farm 的控制台 warning 有两类。第一类是 7 个种子资产（`Seed_1000~1006`：大蒜、生菜、花椰菜、卷心菜、西兰花、甜菜、胡萝卜）反复报“已启用放置但未设置放置类型/预制体”，源头在 `ItemData.OnValidate()` 的通用 placeable 校验，而这些种子真实走的是 `SeedData.cropPrefab + ValidateSeedPlacement(...)` 专用播种链，所以这是代码逻辑缺口导致的假阳性，不是资源真实漏配。第二类是 `GameInputManager._hasPendingFarmInput` obsolete warning，已确认只剩 `GameInputManager` 内部遗留缓存输入链自引用，无外部调用，属于低优先级技术债。由此更新父工作区口径：farm 当前真实开发起点是“在 main 上清理放置链 warning 与控制台噪音，然后继续 10.2.2 回归验收”；`NPCPrefabGeneratorTool.cs` 的 `TextureImporter.spritesheet` obsolete 属于共享 Editor warning，不计入 farm 主线阻断。涉及文件：`Assets/YYY_Scripts/Data/Items/ItemData.cs`、`Assets/YYY_Scripts/Data/Items/SeedData.cs`、`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`、`Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`、`Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`、`Assets/111_Data/Items/Seeds/Seed_1000_大蒜.asset` 至 `Seed_1006_胡萝卜.asset`、`C:/Users/aTo/AppData/Local/Unity/Editor/Editor.log`。下一最小动作：先修 `SeedData` 被通用 placeable 校验误伤的 warning，再重新编译确认放置链控制台收敛。

## 2026-03-14：完成种子 placement 假阳性 warning 最小修复
本轮已执行上一条记录中的最小动作：在 `Assets/YYY_Scripts/Data/Items/ItemData.cs` 中，将通用放置校验从所有 `isPlaceable` 物品收窄为“非 `SeedData` 的 isPlaceable 物品”。这次修改的目标非常明确：保留普通 placeable / 树苗的 `placementType / placementPrefab` 数据校验，同时停止对种子报“已启用放置但未设置放置类型/预制体”的假阳性 warning。验证方面，`旧 MCP 桥口径（已失效）` 仍为 transport closed，无法读取 Unity live console；因此本轮用 Unity Roslyn 直接编译 `Assembly-CSharp.rsp`，结果运行时程序集编译通过，仅剩 1 条既有 warning：`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 中 `_hasPendingFarmInput` obsolete。由此更新父工作区状态：farm 放置链的首要控制台噪音已完成代码级收口，当前与农田主线直接相关的剩余 warning 只剩旧缓存农田输入链技术债；共享 Editor warning 仍不计入 farm 主线阻断。下一最小动作：清理 `GameInputManager` 中 `_hasPendingFarmInput` 及其废弃配套链。

## 2026-03-15：farm 主线 warning 全部收口并完成 Unity 现场闭环
本轮继续沿 `Sunset/main` 推进农田主线，在 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 中把已被 FIFO 队列彻底替代的旧“农田输入缓存链”整体退出编译路径，并顺手清掉 `CancelFarmingNavigation` 中对 `_hasPendingFarmInput` 的残余赋值。验证分两层完成：首先使用 Unity 6000.0.62f1 自带 Roslyn 重新编译 `Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp`，结果 `0 error / 0 warning`；随后通过 Unity MCP 清空 Console、触发编译并回读日志，当前控制台只剩 `Assets/Editor/NPCPrefabGeneratorTool.cs(355,9)` 的共享 Editor warning，已不再存在 farm 相关 warning。由此更新父工作区结论：农田主线当前已完成“种子 placement 假阳性 + 旧缓存输入链 obsolete”两类 warning 的全部收口，主线不再被控制台噪音阻断，下一步可直接进入 `10.2.2` 的现场交互回归验收。

## 2026-03-16：农田主线新增 1.0.0 / 1.0.1 实现分支，当前未同步回 main
在 `2026.03.16` 父工作区下已继续推进两条新的农田实现子线：`1.0.0图层与浇水修正` 与 `1.0.1自动农具进阶中断`。当前真实代码现场不在 `main`，而在分支 `codex/farm-1.0.0-1.0.1`，`HEAD` 为 `9b9a6bd0dd7c5ee7d18cc82e3ea9da74a146bf9d`。已落地实现包括：作物创建后的图层与排序修正、浇水随机样式延后到“入队后且鼠标移出当前农田格”再刷新、自动农具队列进行中的 Toolbar/背包切换与拖拽拒绝反馈。当前已完成 Roslyn 编译与 Unity MCP/Console 只读复核，但尚未完成该轮 tasks/memory 全量补写、白名单提交、锁释放和同步回 `main`，因此此刻不能对外宣称“main 已可验收这两项新补丁”。本主线的恢复点已更新为：先完成 `2026.03.16` 工作区的收尾同步，再回到 `main` 让用户做真实场景验收。

## 2026-03-16：农田 1.0.0 / 1.0.1 已达到可提交前状态
`2026.03.16` 农田子线在本轮完成了最后一轮提交前复核：Unity MCP 可正常连接，活动场景为 `Primary`，Console 仅剩共享 Editor warning，未见新的 farm 专属报错；`1.0.0` 与 `1.0.1` 的 tasks 已全部勾完，子/父工作区与线程记忆也已补齐到“待同步回 main”的现场。随后对白名单路径执行 `git-safe-sync.ps1` 预检，结果允许在当前 `codex/farm-1.0.0-1.0.1` 分支继续收口。由此农田主线当前唯一剩余动作已收敛为：创建本轮 checkpoint，释放 `GameInputManager.cs` 锁，并将这批实现同步到 `main` 供用户验收。

## 2026-03-16：农田 1.0.0 / 1.0.1 已进入 main 验收阶段
农田 `2026.03.16` 子线已完成 Git 收口与回主线同步：实现 checkpoint 为 `7aadbde7`，已从 `codex/farm-1.0.0-1.0.1` 推送，并同步到 `Sunset/main`。本轮同时释放了 `GameInputManager.cs` 的 A 类锁，避免验收期继续占用热点文件。由此农田主线当前真实阶段已更新为“代码在 main、等待用户做 Unity 手动验收”，而不是“仍停留在任务分支收尾中”。

## 2026-03-21：农田主线在 main-only 口径下完成 1.0.2 文档与 live 现场重合拢
本轮用户明确要求“不要再被 cleanroom / continuation 口径拖住，直接在 `D:\Unity\Unity_learning\Sunset @ main` 上把很早之前讨论过的那批农田交互问题全部重新拾起并彻底落地”。基于 live Git 复核，当前 `main@8ac0fb5d0db0714f9879ed12885aefc056a03624` 的 working tree 中，已经存在 11 个属于 `1.0.2纠正001` 的 farm 代码 dirty：`GameInputManager.cs`、`CropController.cs`、`FarmToolPreview.cs`、`HotbarSelectionService.cs`、`PlacementManager.cs`、`PlacementNavigator.cs`、`PlacementPreview.cs`、`InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`InventorySlotUI.cs`、`ToolbarSlotUI.cs`。这批改动已经覆盖：活跃手持槽位保护、UI 冻结与导航恢复、右键与 `WASD` 中断统一、预览跨层 key 与 safe clear、以及幽灵透明作物显示链防守。验证方面，`Assembly-CSharp.rsp` 独立编译通过（`0 error / 0 warning`）；`Assembly-CSharp-Editor.rsp` 仍红，但失败点是共享 NPC Editor 文件 `Assets/Editor/NPCPrefabGeneratorTool.cs(789,43)` 对 `NPCAutoRoamController` 的缺失引用，不属于 farm 专属缺口；MCP 当前读场景/读 Console 会返回 HTML 网关页，因此此刻不能伪称“Unity live 验收已完成”。本轮同时将 `2026.03.16/1.0.2纠正001` 的 `requirements.md / analysis.md / design.md / tasks.md / memory.md` 正式补回 `main`，并改写为当前 `main-only` live 现场口径。由此农田主线最新恢复点收敛为：当前 `1.0.0 / 1.0.1 / 1.0.2` 三条线都已在 `main` 语义内合拢，下一步只剩对白名单路径做 Git 收口，然后交给用户在 `main` 做真实场景验收。
## 2026-03-22：背包 reject shake 收尾修复完成，下一阶段 UI/交互入口被重新校准
用户这轮没有要求新建工作区，而是要求先处理一个收尾细节，再对下一阶段全面交互改进做纯只读盘点。当前 live 现场仍为 `D:\Unity\Unity_learning\Sunset @ main @ c6af26574234329e3525acbdfd5b645a3f5b278a`，仓库里存在大量 unrelated dirty，因此这轮只认领农田自己的白名单路径。已完成的代码修复只有一个：在 `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs` 的 `Awake()` 中补齐 `toggle.targetGraphic = null;` 与 `toggle.transition = Selectable.Transition.None;`，让背包槽位和 `ToolbarSlotUI` 一样关闭 Toggle 默认视觉过渡，解决 reject shake 观感“发黏/卡顿”的真实根因；Roslyn 最小编译验证结果为 `0 error / 0 warning`。同轮只读核查还确认了下一阶段 UI/交互改进的真实断点：`ItemTooltip.cs` 目前只显示 `itemData.description`，没有使用 `ItemData.GetTooltipText()`；并且 Tooltip 只拿到 `ItemStack`/`ItemData`，拿不到 `InventoryItem` 实例态动态属性，这意味着种子袋保质期、开袋状态、剩余种子数之类内容不能靠“改成 GetTooltipText()”一步解决，后续必须同时补“统一静态 tooltip 出口”和“实例态 tooltip 数据入口”。旧口径中“精力系统不存在/待实现”也已被本轮正式判定过时：`EnergySystem`、`ToolData.energyCost`、`FoodData.energyRestore`、`PotionData.energyRestore`、`PlayerInteraction`、`TreeController`、`StoneController` 都已经在 runtime 中工作。当前农田主线恢复点已更新为：这轮收尾修复可以进入白名单 Git 收口；之后若进入“全面交互改进阶段”，第一优先级应围绕 Tooltip 出口统一与实例态展示链路展开，而不是重复造底层逻辑。

## 2026-03-22：1.0.3 基础 UI 与交互统一改进正式建档
用户随后明确批准把这一轮的高质量分析正式沉淀为新阶段工作区，并建议以 `2026.03.16` 下的 `1.0.3` 作为承接入口。本轮已确认这个建议成立：当前方向和 `1.0.2纠正001` 已明显不同，不再主要解决农田交互正确性，而是系统性处理 Tooltip 接线、实例态信息出口、掉落链实例态保持、食物/药水真实生效以及精力术语统一。已在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.3基础UI与交互统一改进\` 下新建 `requirements.md / analysis.md / design.md / tasks.md / memory.md` 五件套，并把这轮关键事实正式写实：背包 shake 的真实根因与最小修复已确认；`ItemTooltip` 现在不是“内容少”，而是“无真实入口且未消费 `GetTooltipText()`”；`InventoryItem` 实例态、种子袋保质期链、工具耐久链和 `EnergySystem` runtime 能力已经存在，但 UI 出口和使用入口没有闭合。当前根工作区恢复点更新为：农田主线在 `main` 上的收尾不再只有 `1.0.2` 验收，而是已经明确分出 `1.0.3` 作为下一阶段基础 UI / 交互统一改进的正式入口，后续进入实现时应以该工作区文档为准推进。

## 2026-03-23：1.0.3 第一轮实现已落地，当前停在共享编译阻断前
- 已完成代码落地：
  - `ItemTooltip.cs` 不再直接显示 `itemData.description`，开始统一走 `GetTooltipText()` 的静态出口，并叠加实例态信息。
  - 新增 `ItemTooltipTextBuilder.cs`，集中拼装静态文本、工具当前耐久、种子袋开袋状态、剩余种子数与剩余保质期。
  - `InventorySlotInteraction.cs` 与 `ToolbarSlotUI.cs` 已接入 Tooltip 悬浮显示/隐藏入口；装备槽通过 `InventorySlotInteraction` 复用该入口。
  - `EquipmentSlotUI.cs` 暴露当前装备槽的 `ItemStack / InventoryItem / Database` 读取口径，供 Tooltip 与后续实例态链使用。
  - `InventoryItem.cs` 新增动态属性可见性口径，`PlayerInventoryData.cs` 与 `ChestInventoryV2.cs` 改为把所有实例态属性都视为不可堆叠数据，避免种子袋/耐久物品被错判成普通静态物品。
  - `InventoryInteractionManager.cs`、`InventorySlotInteraction.cs`、`SlotDragContext.cs` 已补齐主要拖拽/归位/丢弃链上的实例态随行，避免工具耐久、种子袋状态在换格或丢到地上时直接丢失。
  - `ItemDropHelper.cs` 与 `WorldItemPickup.cs` 已支持 `InventoryItem` 实例态掉落与拾取回背包。
  - `BoxPanelUI.cs` 的 SlotDragContext 丢弃入口也已跟上实例态掉落分流。
  - `ItemUseConfirmDialog.cs` 已把食物/药水的精力恢复接到 `EnergySystem`；生命恢复因当前项目缺少明确的玩家生命系统，仍写实保留 warning，不伪称已完成。
- 已完成验证：
  - 中途 Roslyn 运行时代码独立编译曾通过。
  - 当前再次跑全量 `Assembly-CSharp.rsp` 时，新的阻断来自共享现场：`Assets/YYY_Scripts/Story/Managers/StoryManager.cs(127,13): SpringDay1Director` 未定义，不属于农田这批脚本。
  - `git diff --check` 针对本轮农田白名单脚本已无空白格式错误，只剩 CRLF/LF 提示。
- 当前未完成 / 残留：
  - `1.0.3` 的任务文档 checkbox 还未回填到实现后状态。
  - 父工作区、根工作区、线程记忆与 skill 审计还未同步这轮真实实现结果。
  - 生命恢复仍未真正落地，因为项目内暂未发现可接的玩家生命系统。
  - 当前箱子主 UI 仍走 `ChestInventory` 旧链，因此“箱子内实例态完整保真”不应冒充已彻底解决。
- 当前恢复点：
  - 农田 `1.0.3` 已从“纯设计”推进到“Tooltip + 实例态 + 掉落 + 食物药水精力恢复”的第一轮实现，下一步先同步文档/记忆，再按白名单提交 checkpoint，然后交给用户按清单验收。

## 2026-03-23：修复背包“全部选中”回归
- 根因：`InventorySlotUI.cs` 在此前为了修复 reject shake 手感，把 `Toggle` 的默认视觉过渡关闭后，背包槽位自己的 `selectedOverlay` 仍未像 `ToolbarSlotUI.cs` 那样被显式同步管理；结果原本隐性的选中覆盖层状态被直接暴露出来，表现为“背包内容全部选中”。
- 修复：
  - 在 `InventorySlotUI.cs` 中补齐与 `ToolbarSlotUI.cs` 同口径的选中视觉管理。
  - `Awake()` 中显式将 `Toggle` 初始化为未选中。
  - 监听 `toggle.onValueChanged`，统一驱动 `selectedOverlay.enabled`。
  - `Bind()` / `BindContainer()` 以及 `Select()` / `Deselect()` 都改为同步更新选中覆盖层。
- 验证：`Assembly-CSharp.rsp` Roslyn 运行时代码独立编译通过，`git diff --check` 针对本次最小修复通过。
- 当前恢复点：已把背包“全选”回归单独收敛成一个最小修复点，下一步可直接由用户在背包界面手动确认选中视觉是否恢复正常。

## 2026-03-23：背包第一行与 Toolbar 热槽同步修复 + 食物药水生命恢复接线
- 已补完的代码闭环：
  - `InventorySlotUI.cs` 不再强行覆盖 prefab 上的 `Toggle` 过渡配置，恢复尊重原始 `Target / Selected / ToggleGroup` 关系。
  - `InventorySlotUI.cs` 已直接订阅 `HotbarSelectionService.OnSelectedChanged`，使背包第一行与 Toolbar 在同一热槽索引上实时同步，而不是等到第二次打开后才对齐。
  - `InventoryPanelUI.cs` 与 `BoxPanelUI.cs` 刷新背包区域时已同步调用 `RefreshSelection()`，确保第一次打开面板就能看到正确热槽。
  - `ToolbarSlotUI.cs` 已撤销脚本侧对 `Toggle.transition=None` 的强制覆盖，恢复 prefab 原配置口径。
  - `ItemUseConfirmDialog.cs` 已在现有 `EnergySystem` 基础上进一步接入 `HealthSystem`，使食物 / 药水的生命恢复与精力恢复都走真实状态系统，不再只是日志占位。
- 本轮根因修正：
  - 背包“全部红框常亮”以及“第一次打开背包第一行不跟 Toolbar 同步”的核心问题，不在业务逻辑，而在于此前脚本错误地覆盖了 prefab 原有 `Toggle` 配置关系，同时背包第一行又没有像 Toolbar 一样直接订阅热槽选择服务。
- 验证结果：
  - `Assembly-CSharp.rsp` Roslyn 运行时代码独立编译通过。
  - `git diff --check` 针对本轮相关脚本通过，仅剩 CRLF/LF 换行警告。
- 当前恢复点：
  - `1.0.3` 的食物/药水真实状态接线现在已从“只精力恢复”推进到“精力 + 生命恢复”；背包第一行与 Toolbar 的热槽同步也已修正，下一步继续回到剩余未闭环项（尤其是箱内实例态保真与用户现场验收）。

## 2026-03-23：恢复 prefab 原始 Toggle 配置口径，并补完背包第一行与 Toolbar 的实时同源同步
- 用户在现场指出一个关键事实：`InventorySlotUI` / `ToolbarSlotUI` 原本的 prefab 配置是 `Transition = Color Tint`、`Target Graphic = Target`、`Graphic = Selected`，而不是脚本硬改成 `transition=None / targetGraphic=null`。这说明此前为了修 shake 手感而在脚本中强制覆盖 Toggle 配置，本质上是偏离了项目既有配置口径。
- 本轮已执行的修正：
  - 撤销脚本侧对 `InventorySlotUI.cs`、`ToolbarSlotUI.cs` 的强制 Toggle 配置覆盖，重新尊重 prefab 中的 `Target / Selected / ToggleGroup` 关系。
  - `InventorySlotUI.cs` 现已直接订阅 `HotbarSelectionService.OnSelectedChanged`，并新增 `RefreshSelection()`，使背包第一行与 Toolbar 在同一热槽索引上实时同步，不再出现“Toolbar 切换后第一次打开背包未选中、第二次才正确”的延迟现象。
  - `InventoryPanelUI.cs` 与 `BoxPanelUI.cs` 在刷新背包区域时，现会同步调用 `InventorySlotUI.RefreshSelection()`，确保第一次打开面板就拿到正确热槽状态。
- 同轮还继续推进了未完成项：`ItemUseConfirmDialog.cs` 已识别到项目内现有 `HealthSystem.cs`，因此食物 / 药水效果已从“仅精力恢复”推进到“精力 + 生命恢复”都走真实状态系统。
- 当前运行时代码编译状态：`Assembly-CSharp.rsp` Roslyn 独立编译通过。
- 当前恢复点：
  - 背包第一行 / Toolbar 同步问题已按“恢复原配置 + 补实时同步”方向修正；
  - `1.0.3` 剩余未闭合的最大实质缺口已收敛为“箱子主 UI 仍主要走 `ChestInventory` 旧链，箱内实例态保真还未彻底统一到 V2”。

## 2026-03-23 MCP 桥口径纠偏
- 本文件中若出现“旧 MCP 桥口径（已失效）”，均表示历史阶段使用过的旧桥结论，不再代表当前 live 入口。
- 当前唯一有效 live 基线以 D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md 为准：unityMCP + http://127.0.0.1:8888/mcp。
## 2026-03-23：1.0.3 第二轮收口已把箱子实例态与农田遮挡联动并入主线
用户这轮没有换题，而是明确告诉本线程：“你剩下的任务不止是箱子，遮挡检查线程那边已经确认农田预览遮挡联动应该由 farm 线程做，做完箱子后继续把遮挡也一起做掉。” 基于这个新增要求，当前农田主线没有变化，仍是 `2026.03.16/1.0.3基础UI与交互统一改进`，但其真实交付面已扩到两类收口：箱子实例态保真，以及农田 hover 预览遮挡。代码层已经完成：`ChestInventory.cs`、`ChestInventoryV2.cs`、`ChestController.cs`、`BoxPanelUI.cs`、`InventorySlotInteraction.cs`、`InventoryInteractionManager.cs`、`SlotDragContext.cs` 补齐运行时容器与 runtime item 保真；`FarmToolPreview.cs` 接入 `OcclusionManager.SetPreviewBounds(Bounds?)`，只同步当前 hover 预览，不把 queue/executing 并入遮挡。验证层已经完成：白名单 `git diff --check` 通过，`Assembly-CSharp.rsp` 运行时代码独立编译通过。当前根工作区恢复点更新为：农田 `1.0.3` 代码层已经达到可交用户现场验收的阶段，剩余不再是代码闸门，而是 Unity 场景中的手动行为验收与本轮白名单 checkpoint 收口。

## 2026-03-23：回读旧截图后，确认农田 `1.0.3` 代码面已覆盖旧剩余项
用户本轮贴出的截图是农田 `1.0.3` 更早一版进度摘要，其中“下一刀最该继续做箱子实例态”已经被后续 main 上的正式 checkpoint 覆盖。live 现场核查结果：当前工作目录仍是 `D:\Unity\Unity_learning\Sunset`，分支 `main`，`HEAD=4f76b1b87efb455dc0cc370988ca8b69afc601a3`；farm 相关白名单路径无未提交 dirty；近期 farm 提交链里，`0e87c430` 已补完背包第一行 / Toolbar 同步与食物药水真实状态接线，`2218b47d` 已补完箱子实例态保真与农田 hover 预览遮挡。本轮再次独立编译 `Assembly-CSharp.rsp` 也通过，说明当前 `main` 上这批代码并未退回半成品。真正剩下的阻塞已经不是“还有哪些农田代码没写”，而是 Unity live 验收入口没有回正：本会话 MCP resources / templates 为空，`C:\Users\aTo\.codex\config.toml` 仍残留旧的 `8080` 口径，而 docs 规定的 `8888 + unityMCP` 基线实际在线。当前根工作区恢复点因此更新为：农田 `1.0.3` 现在应把重点放在恢复 Unity live 验收入口并完成场景回归，而不是重复开发旧截图中的箱子/遮挡项。

## 2026-03-23：shell 版 unityMCP 验收恢复后，农田 `1.0.3` 只剩手动交互验收
用户随后确认当前 live 端口已回到 `8888`，并要求我先验收，再继续未完成内容。本轮已通过 shell 版 `unityMCP@8888` 成功完成 live 基线核查：读取到唯一实例 `Sunset@21935cd3ad733705`、活动场景 `Primary`、项目信息与自定义工具清单；清空 Console 后执行 `Play -> 等待 -> read_console -> Stop`，当前窗口内为 `0 error / 0 warning`，且已确保 Editor 回到 Edit Mode。只读场景对象也已确认 `Primary/1_Managers/OcclusionManager` 存在且激活，因此农田 hover 遮挡当前不再被“场景里缺 Manager”这个假前提阻断。与此同时，尝试运行 `OcclusionSystemTests` 时 Unity 插件会话在等待 `command_result` 期间断开，随后 `refresh_unity` 可以恢复连接；这条现象被归类为 MCP/plugin 级不稳定，不写成项目失败。基于这轮 live 验收恢复，本轮继续补完 `1.0.3` 剩余的代码项“术语与文案统一”：`EquipmentData` 将 `Vitality` 的用户显示统一为“精力”，`FoodData` / `PotionData` 的 Tooltip 将 `HP` 统一为“生命”，`ItemUseConfirmDialog` 的结果日志也同步对齐。运行时代码再次独立编译通过，说明农田 `1.0.3` 当前已没有继续扩写的新代码缺口。当前根工作区恢复点更新为：农田主线现在真正剩下的只有手动交互验收，也就是箱内实例态拖拽/交换/装备回滚和农田 hover 遮挡的真实操作确认。

## 2026-03-23：Toolbar 输入规则纠偏已同步到代码与 live 规则
用户随后明确纠正：Toolbar 的正确设计不是“只有前五格”，而是“数字键 `1~5` 直选前五格，滚轮仍允许在 12 个快捷栏槽位间循环”。本轮已据此完成最小收口：代码侧在 `InventoryService.cs` 中显式新增 `HotbarDirectSelectCount = 5`，并让 `GameInputManager.HandleHotbarSelection()` 把数字键选择边界与滚轮循环边界彻底分开；规则侧同步修正 `.kiro/steering/ui.md`、`.kiro/steering/items.md`、`.kiro/steering/maintenance-guidelines.md` 以及农田 `1.0.2纠正001` 的 `requirements.md / tasks.md` 和 `最终交互矩阵.md`，去掉“1-8/1-9 数字键”“快捷键泛化”等旧表述。验证方面，`Assembly-CSharp.rsp` 独立编译通过，白名单 `git diff --check` 通过，仅剩 CRLF/LF 提示。当前根工作区恢复点更新为：Toolbar 输入边界已经重新对齐 live 设计，后续主线继续回到剩余手动验收与下一阶段基础 UI/交互改进，不再需要回头重判 `HotbarWidth = 12` 本身。

## 2026-03-23：农田主线正式升级为 `全局交互V3`，`1.0.4` docs-first 已落盘
用户随后通过 `1.0.4交互全面检查/001最后通牒.md` 明确要求：这条线从本轮起不再以“农田交互修复V2”的局部修补语义继续推进，而是升级为 `全局交互V3（原：农田交互修复V2）`，先收尾 `1.0.3`，再 docs-first 进入 `1.0.4`，并且这轮禁止先改业务代码。业务线程已按要求完成只读接管和正式落盘：在当前 live 现场 `D:\Unity\Unity_learning\Sunset @ main @ 2a304c6f80199f0e34c65ac9ce71a8dd61015bcb` 下，系统回读 `1.0.2`、`1.0.3`、`最终交互矩阵`、线程记忆、父/根记忆、以及放置 / 箱子 / 背包 / Toolbar / 工具参数链等核心代码后，先把 `1.0.3` 的最终边界明确为“基础 UI 与交互统一改进的第一轮代码闭环”，再把未闭合的五大系统性问题统一升级到 `1.0.4`：放置与幽灵占位、箱子放置导航 retry、背包/Toolbar 双选中与错误锁定、工具耐久/精力/Tooltip/SO 参数链、箱子双库存同步递归。随后，`1.0.4` 两份核心文档已正式完成：`全面理解需求与分析.md` 负责需求入口、用户原文完整摘抄、历史回顾和五大主题问题分析；`全面设计与详细任务执行清单.md` 负责设计原则、统一交互边界、子系统拆分、根因假设、结构改造、日志测试、验收和实施顺序。当前根工作区恢复点更新为：农田这条长期主线已经正式从“局部农田交互修补”升级为“全局交互处理”，后续任何实现都必须以 `1.0.4` 这两份主文档为执行总入口。

## 2026-03-23：`全局交互V3` 第一刀已完成箱子 authoritative source 收口
在 `1.0.4` 从文档阶段切入实现阶段后，线程没有平均推进五个主题，而是按 `002-从文档转入实现与阻塞清除.md` 的硬顺序，先处理最阻断验收的箱子爆栈链。当前 live 现场仍为 `D:\Unity\Unity_learning\Sunset @ main @ 03c2c530b0e05a3757c6772d70f23993d5ea26ea`，且 shared root 同时存在他线对 `Primary.unity`、字体材质、`SpringDay1` 脚本与 `TagManager.asset` 的 unrelated dirty；本轮 farm 只认领箱子链和新增编辑器测试，不触碰这些高危脏区。本刀已经把箱子运行时口径定为：`ChestInventoryV2` 是 authoritative runtime/save source，`ChestInventory` 仅作为 legacy mirror 存在。代码层补齐了 silent bridge API、一次性事件补发机制和 controller 级 bridge guard，明确切断“事件互相反写”造成的 `StackOverflowException` 根因；同时去掉了保存前对 V2 的无条件旧库存覆盖，避免 authoritative runtime 数据被 legacy mirror 反向写坏。验证方面，运行时代码和编辑器测试程序集均独立编译通过，新加的 `Assets/Editor/ChestInventoryBridgeTests.cs` 在 Unity EditMode 下 `3 passed / 0 failed`，清 Console 后未出现新的项目级红错。当前根工作区恢复点更新为：`全局交互V3` 已不再停留在 docs-first，而是成功落下第一刀可验证实现；下一刀按既定顺序进入放置系统的“幽灵占位”和箱子放置导航 retry 统一治理。

## 2026-03-24：`全局交互V3` 第二刀已推进到可 checkpoint 的第一轮代码闭环
本轮继续沿 `1.0.4` 的第二刀推进，没有重新扩题到 Tooltip / Toolbar / 其他系统。当前根工作区已确认的新增事实有三类。第一类是箱子链补强：`Assets/Editor/ChestInventoryBridgeTests.cs` 已新增 `SaveLoad_RestoresAuthoritativeInventoryAndLegacyMirrorWithoutReintroducingBridgeLoop()`，因此“bridge 运行时最小稳定”现在补成了“bridge + Save/Load 往返最小稳定”。第二类是放置 reach envelope 与事务层：`PlacementPreview.cs` 已明确区分交互 envelope 与视觉 envelope；`PlacementManager.cs` 新增 `ResumePreviewAfterSuccessfulPlacement()` 和树苗 post-spawn confirm；`PlacementExecutionTransaction.cs` 则把一次放置的提交/回滚边界显式化，使树苗/箱子类放置不再只是裸串行步骤。第三类是验证口径：当前白名单 `git diff --check` 通过，运行时与编辑器 Roslyn 编译都通过，Unity 清 Console 后再次触发编译返回 `0 error / 0 warning`；但 MCP `run_tests` 仍只返回 `total=0`，因此这轮不能把测试 runner 的输出冒充成可信通过结论。当前根工作区恢复点更新为：`全局交互V3` 的第一刀已经完成，第二刀也已完成第一轮代码闭环；这条线现在真正缺的不是再补设计，而是用户在 Unity 现场对“箱子走到边上即可稳定放置、树苗连续快速点击不再幽灵占位”做 live 手动复测。

## 2026-03-24：second-blade 的 4 条 live 场景已全部通过
本轮继续沿 `全局交互V3 / 1.0.4` 推进，没有另开平行子线。当前根工作区新增的关键事实是：第二刀已经不再停在“代码闭环待手测”，而是拿到了真实的 live 终验结果。箱子存读失败的最后根因已经被锁定并修复到 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\ChestInventoryV2.cs`：`ToSaveData()` 现在会把 `InventoryItem` 的动态属性一并写出，所以 `customName / seedRemaining` 这类 runtime property 不再在 `Save() / Load()` 中丢失。随后在 `Primary` 单实例里，通过 second-blade live runner 做了完整重跑，最终四条场景全部通过：箱子放置 reach envelope 通过、放置成功后的 preview 即时重刷通过、树苗连续快速点击幽灵占位防护通过、箱子 `Save() / Load()` 回归通过。与此同时，本轮新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\第二刀验收报告_2026-03-24.md`，把第二刀的交付面和后续移交边界固化下来。当前根工作区恢复点更新为：`全局交互V3` 的第二刀已经完成并拿到 live 证据；后续如果继续推进，应转入更高层的 Toolbar / 背包 / 锁定态等后续交互问题，而不是再回头补 second-blade。

## 2026-03-24：`全局交互V3` 已完成第二刀后的阶段总结与第三刀前移交
本轮用户没有要求继续实现，而是明确要求在第二刀完成后先做一次阶段总结与移交收束。根工作区层面当前新增的稳定事实是：`1.0.4` 已经不只是“第二刀完成”，还已经补齐了正式的阶段移交文件 `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\阶段总结与后续刀次移交报告_2026-03-24.md`。这份报告已经把当前已完成边界、成果总表、真实剩余问题池、后续刀次建议排序与待用户裁决项集中固化。当前根层建议也已经明确：第三刀最该先打 `Toolbar / 背包 / 锁定态输入边界统一`，因为它直接覆盖用户最新补充的 `Toolbar 左键后 A/D 独立框选`，并且属于核心输入正确性问题；工具耐久 / 精力 / Tooltip / SO 参数统一口径可后移处理。同时，当前项目级结论不是“立刻继续开第三刀”，而是“先等用户确认第二刀体感，再决定是否放行第三刀”。当前根工作区恢复点更新为：`全局交互V3` 现在已经完成“第二刀闭环 + 阶段总结与移交”；下一步的分叉点已经从实现问题，转成用户裁决问题。

## 2026-03-24：`全局交互V3` 已直接补齐高层交互实现，当前唯一剩余阻塞是 shared root 的他线红编译
用户随后明确推翻“继续一刀一刀聊”的推进方式，要求直接回到 `1.0.4/001最后通牒.md` 的整包诉求，把当前还能一次性落地的全局交互修正直接做完。根工作区层面的新增事实是：这条长期主线现在不只是完成了 second-blade 和阶段总结，还已经把第三刀 / 第四刀里最核心的一整串实现提前落地到 `main` 工作现场，覆盖三条链。第一条是“受保护槽位与焦点边界”链：`PlacementManager.HasProtectedHeldSession` 现在只保护真实 `Locked / Navigating / Executing` 状态，`GameInputManager` 不再因为纯 preview 误锁树苗/种子/placeable；`InventorySlotUI`、`ToolbarSlotUI`、`InventorySlotInteraction` 则统一关掉 `Toggle.navigation` 并在鼠标点击/按下时清理 `EventSystem` 选中态，用来修复 Toolbar 左键后 `A/D` 跑出独立框选的问题。第二条是“工具 runtime 参数链”链：新增 `ToolRuntimeUtility.cs`，并把 `PlayerInteraction`、`GameInputManager`、`TreeController`、`StoneController`、`PlayerInventoryData`、`ChestInventoryV2`、`EquipmentService`、`InventorySortService`、`WorldItemPickup`、`SaveDataDTOs`、`ChestController` 统一收进 tool runtime 创建、耐久初始化、精力/耐久提交、掉落/整理/装备/存档保真的同源口径里。第三条是“Tooltip 与 hover”链：新增 `EnergyBarTooltipWatcher.cs`，`ItemTooltip.cs` 改成真实延迟显示协程，并额外用代码硬下限把 `0.6s` 固定下来，避免 `ItemTooltip.prefab` 里旧的 `showDelay: 0.3` 继续污染 live 行为。验证方面，这批白名单脚本已经逐个通过 `validate_script`，`git diff --check` 也通过；当前唯一没法闭上的不是脚本质量，而是 shared root 上有他线删除 `SpringDay1WorkbenchCraftingOverlay.cs` 后留下的项目级红编译，Console 仍报在 `CraftingStationInteractable.cs` 上。当前根工作区恢复点更新为：`全局交互V3` 这批高层交互实现已实装完成；真正只剩的最后一步是等 shared root 清掉该他线红编译，然后立即回到 Unity live 做最终手动验收和白名单提交。

## 2026-03-24：shared root 脏改清扫复核后，当前农田线尾账已收敛为单个 live 文档 `006`
用户随后把当前任务切到 shared root 清扫，不再让农田线继续扩 `1.0.4`。根工作区层面的最新结论是：在当前 live Git 现场 `D:\Unity\Unity_learning\Sunset @ main @ 1744c09b182c1aea61d0c06d6a491987d9cb8c69` 下，农田线当前真正还留在 shared root 里的 owned tracked dirty 只剩一个：`D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\006-续工裁决入口与用户补充区.md`。它承接的是用户最新补充，因此应视为 live 工作区文档，不应静默删除。其余 dirty 均不属于本轮农田清扫范围，包括导航检查、`屎山修复`、`Primary.unity`、字体材质、NPC / 导航脚本、`DialogueUI.cs`、`TagManager.asset` 等。当前根工作区恢复点更新为：农田线这轮 shared root 尾账已经确认只剩 `006`，后续只需把它与必要记忆做最小白名单收口，不再需要补报其他农田 dirty。

## 2026-03-24：农田线对最新 5 个 live 回归点已补完脚本闭环，当前真实阻断转为 shared root 级 compile blocker
用户在随后一轮又直接指出 5 个 live 回归：锄地无限精力、农田 hover 预览导致箱子始终透明、箱子 `Sort` 不堆叠、隔天树成长卡顿、放置成功后角色继续多走。根层当前新增的稳定事实是：这些问题这次不是停在分析，而是已经分别落到了农田主线自己的脚本改动里，而且修改面被控制在 6 个文件内。`FarmToolPreview.cs` 把农田预览遮挡 bounds 从“整张 tilemap 包围盒”收回到“当前 hover 预览格子 + cursorRenderer”的真实范围；`ChestInventoryV2.cs` 的排序现在会先合并普通堆叠物，再保留实例态物品的独立性；`TreeController.cs` 用 shared debounce 收掉多树同时成长时重复触发导航网格重建的尖峰；`PlacementNavigator.cs` 统一了到达停下口径，命中可放置距离后会立即取消 auto navigator，并把目标从中心点回算到角色 pivot；`PlayerInteraction.cs` 与 `GameInputManager.cs` 则补齐了成功动作的工具消耗提交回执，让锄地 / 浇水这类动作不会再出现“动作成功但精力/耐久没真正提交”的漏口。验证方面，当前 live 现场已前进到 `D:\Unity\Unity_learning\Sunset @ main @ b40e4cf150bcca3bc3d7a0a7af90c05223c31976`；这 6 个脚本逐个 `validate_script` 均无新增 error，`git diff --check -- <6 files>` 通过，活动场景仍是 `Primary` 且 `isDirty=false`。根层同时需要写实一条新变化：此前 memory 里记录的 shared root compile blocker 已经过时，当前最新项目级红编译不是 `CraftingStationInteractable.cs`，而是 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 第 274/283 行 `CardColor` 未定义；这条 compile blocker 仍不属于农田线白名单。当前根工作区恢复点更新为：农田线现在已经把用户最新这 5 个 live 回归点收到了脚本级闭环，后续真正需要的是 shared root 先清掉他线 compile blocker，再把这 6 个脚本连同必要记忆做白名单收口。

随后本轮已进一步完成白名单收口：`git-safe-sync` 成功把 6 个农田脚本和四层记忆提交到 `main`，提交号为 `124caccc`，标题 `2026.03.24_农田交互修复V2_05`。根层最新结论因此变为：这 5 个 live 回归点已经不再只是“本地修好待收口”，而是正式进入 `main` 基线；shared root 里仍存在的 `SpringDay1WorkbenchCraftingOverlay.cs` compile blocker 属于他线剩余问题，但没有被混入本次农田 checkpoint。

## 2026-03-24：`1.0.4` 新增工具运行时 / 玩家反馈要求已进入阶段日志与脚本级闭环
用户随后继续把一批新的农田 live 问题并入当前主线，而不是另起任务：锄地要同时扣精力和耐久、工具耐久归零要直接损坏消失且有反馈、水壶要改成水量口径、低等级斧头砍高等级树不能扣精力且要有玩家版头顶气泡、农田 hover 遮挡范围要以中心格为准。当前根层新增的稳定事实有两类。第一类是文档层：`D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\当前续工计划与日志.md` 已正式建立并补成 live 事实入口，里面不再停留在“计划稿”，而是明确记录了这批新增要求、当前任务列表、已落地代码面、编译状态与剩余待办。第二类是代码层：`InventoryItem.cs`、`ToolRuntimeUtility.cs`、`ToolData.cs`、`PlayerInteraction.cs`、`GameInputManager.cs`、`TreeController.cs`、`FarmToolPreview.cs`、`ItemTooltipTextBuilder.cs`、`InventorySlotUI.cs`、`ToolbarSlotUI.cs` 已推进到新口径；同时新增了 `PlayerThoughtBubblePresenter.cs` 与 `PlayerToolFeedbackService.cs` 两个玩家反馈脚本，用于承接工具损坏、空水壶、斧头等级不足和恢复可砍的玩家气泡/音效/槽位 shake / burst 特效。验证方面，`Assembly-CSharp.rsp` 与 `Assembly-CSharp-Editor.rsp` 的 Roslyn 编译已再次通过，`unityMCP@8888` 基线也已回正，活动场景确认是 `Primary`，当前 Console 只看到 2 条 “There are no audio listeners in the scene” warning，没有新的 farm 红错。当前根工作区恢复点更新为：这批新增工具运行时与反馈链已经回到“可继续验收”的脚本级闭环，下一步不是再写计划，而是直接做 Unity / MCP live 行为验收，并根据现场结果决定是否补冷却前置拦截与失败回滚。

## 2026-03-25：农田新增运行时 / 玩家反馈链已升级到 `008` live 验收入口
根层当前新增的稳定事实是：这条线不能继续停在“当前续工计划与日志已回正、脚本编译已绿、MCP 基线已对”的阶段，因为用户自己还没有做现场验收，而这些问题本身又偏逻辑和运行态。基于最新子工作区回执与当前续工日志，本轮已新增 [008-新增工具运行时与玩家反馈链进入live验收与补口.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/农田系统/2026.03.16/1.0.4交互全面检查/008-新增工具运行时与玩家反馈链进入live验收与补口.md)。`008` 已把下一轮唯一主刀固定为：线程自己先跑 4 组 live 验收，即锄头耐久链、水壶水量链、高等级树与玩家气泡链、hover 遮挡链；若不过，则继续同轮补口，不得再把第一轮真实逻辑验收甩给用户。当前根工作区恢复点更新为：农田这轮后续默认以 `008` 为执行入口，当前真正该审的不是脚本是否解释得通，而是这 4 组现场行为到底有没有过。

## 2026-03-25：`008` live 当前稳定收敛为“3 组通过 + hover 遮挡唯一剩余点”
根层这轮新增的稳定事实是：`008` 不再停在“应该去做 live”，而是已经真的把 live 跑出来了，而且结果不是模糊的“差不多”。本轮在 `Primary` 中实际建立了最小 runner 与菜单入口，反复进入 Play 做 4 组 Unity / MCP live。运行过程中先后清掉了 `PlayerToolFeedbackService` 的两个真实运行时错误：burst 粒子创建即播放导致的 assert、以及非法 `velocityOverLifetime` 配置导致的粒子错误；同时把“反馈音效是否触发”的 live 判定收敛到 `PlayerToolFeedbackService.FeedbackSoundDispatchCount`，避免被临时 `AudioSource` 数量误导。当前最新稳定结果已经非常清楚：锄头 / 普通工具运行时链通过；水壶运行时链通过；高级树与玩家气泡链通过；hover 遮挡链仍未通过。hover 的失败也已经不再是“看起来不对”，而是被 live 证据压成了一个单点：`OcclusionManager.previewBounds` 在这条链上仍为 `null`，于是 `centerTrackedByManager=False`、`centerBecameTransparent=False`。这意味着当前农田新增运行时 / 玩家反馈这条主线已经不是“整体未完成”，而是“4 组里只有 hover 遮挡链还剩一个明确的 `previewBounds` 非空化问题”。当前根工作区恢复点更新为：后续若继续推进这条线，应只围绕 `FarmToolPreview -> OcclusionManager` 的 hover 遮挡联动补最后一刀，不必再回头重验锄头、水壶和高级树三组逻辑。

## 2026-03-25：hover 遮挡链也已通过，农田新增运行时 / 玩家反馈这批 live 全绿
根层当前新增的稳定事实是：`008` 收敛出的最后单点已经被 `009` 真正闭上，农田这批新增运行时 / 玩家反馈 live 不再存在“4 组里还差 hover 一处”的悬空状态。本轮没有继续扩题，只围绕 `hover-occlusion-chain` 做精确闭环：回读 `FarmToolPreview`、`OcclusionManager` 与 live runner 后，已确认此前 `previewBounds=null` 的来源不是业务 hover 链仍不会提交 bounds，而是 live runner 在 menu 触发的 editor 焦点场景下，被 `GameInputManager.UpdatePreviews()` 的鼠标预览循环抢写掉了手动注入的 preview。当前修正也只落在 live 验证层：`FarmRuntimeLiveValidationRunner.cs` 新增 `All / HoverOnly` 范围控制，并在 hover 取样窗口临时停掉 `GameInputManager` 的每帧预览覆盖；`FarmRuntimeLiveValidationMenu.cs` 新增 hover-only 菜单，保证这轮只重跑 hover，不再泛跑整轮。最新 Unity / MCP live 结果已经明确：`sideStayedOpaque=True`、`centerBecameTransparent=True`、`centerRecovered=True`、`centerBoundsIntersected=True`、`centerTrackedByManager=True`、`previewBounds` 不再为 `null`。当前根工作区恢复点更新为：农田 `1.0.4` 下这批新增工具运行时 / 玩家反馈 4 组 live 现已全部闭环，后续应转入用户整包验收或新的明确范围，而不是继续在 hover 单点上打转。

## 2026-03-25：根层已纠正口径，当前不是“农田线整体已绿”，而是“放置链发生事故回退”
根层当前新增的稳定事实是：用户已经明确否定把 `009` 外推成整条农田线已绿，因此根层口径必须更新。当前真正的高优先级结论不是“4 组 live 都绿了”，而是“placeable / 放置交互链整体发生了事故回退，必须先恢复到不比旧基线更差”。只读回看 farm 提交链与当前 dirty 后，根层现阶段更优先认定：最后一个至少可工作的放置基线是 `124caccc`，而当前更像是 `124caccc` 之后未提交的 `GameInputManager.cs / FarmToolPreview.cs` 改动把 placeable 主链拖进了事故态；`PlacementManager.cs / PlacementNavigator.cs` 当前并没有新的 dirty，因此不应先把锅泛化到整个 placement 子系统。根层由此把后续恢复策略钉为：优先走 `selective restore`，以 `124caccc` 为 placeable 恢复锚点，只回退 `GameInputManager / FarmToolPreview` 中直接导致放置链事故的部分，同时保住已证明确有价值的工具 runtime、玩家反馈、箱子链与 Tooltip 改动。当前根工作区恢复点更新为：农田这条线现在真正服务的主线不是 hover 收口，而是 `010` 定义下的“放置链事故回退与自治恢复”。

## 2026-03-25：农田主线再纠偏，当前首先不是“整包验收”，而是“放置链回归事故先止血”

根层当前新增的稳定事实是：农田这条线不能继续只看新增工具运行时 / 玩家反馈那 4 组 live，因为用户最新现场已经明确指出 placeable / 放置交互整体变得更差，且这组更差并不是 hover 单点，而是放置链本体：玩家很远就停、代表性 placeable 放不成功、而且到处都是幽灵。也就是说，`009` 的 hover-only 通过只代表局部链路已跑通，不代表农田整条交互主线现在适合直接转入整包验收。基于这一治理纠偏，本轮已新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\010-放置链事故回退与全局自治重建.md`，并把业务线程下一轮唯一主刀改成：先找最后一个至少可工作的放置基线，再按线程自决的 `selective rollback / selective restore / forward fix` 路线，把当前比旧基线更差的事故态拉回去。当前根工作区恢复点更新为：农田线后续第一优先级已从“继续报 4 组 live 全绿”改成“先恢复放置链可用性”，在这件事完成之前，不再把当前状态外推成可交最终验收。

## 2026-03-25：总闸审查补充，`010` 现阶段还不是恢复完成，而是未过线的恢复前 checkpoint

根层本轮补充记录一条治理硬结论：农田线程对 `1.0.4 / 010` 的最新回执不能算完成，只能算“读回现场并选定恢复路径”的 checkpoint。治理侧已经把 `010` 文档中的唯一主刀、禁止项、完成定义和 live 覆盖要求，与线程本轮回执逐项核对；结论是线程目前还停在恢复前阶段。它虽然已经把最后可工作基线优先认定为 `124caccc`，也给出了优先走 `selective restore` 的方向，但回执同时明确承认：本轮没有实际执行业务代码 restore / rollback，没有跑任何恢复向代表性 live，远停 / 无法放置 / 全场幽灵 3 个坏现象也仍都在事故集合中，当前状态仍未恢复到“不比旧基线更差”。所以根层必须明确写死：`010` 当前未命中恢复完成定义，农田主线仍停在“放置链事故回退待实施”这一步，而不是“已经恢复，只剩小尾巴”。后续如果继续农田线，线程必须先真正把放置链拉回到至少不比 `124caccc` 更差，再拿恢复向 live 回来交账。

## 2026-03-25：根层补记，农田已新增 `011`，且 placeable hierarchy 现已升级为本轮硬验收项

根层本轮又新增一条稳定事实：农田治理这条线已经不再只停在“`010` 未过线”的裁定上，而是继续补出了下一轮真正可执行的文件 `011-从路径checkpoint转入124caccc定点恢复与placeable-live复验.md`。`011` 已把农田当前唯一主刀固定成：先以 `124caccc` 为锚点，对 `GameInputManager.cs / FarmToolPreview.cs` 做真实 `selective restore`，再用代表性 live 把 placeable 主链拉回到至少不比旧基线更差。与此同时，用户又补了一条不属于“小美观尾巴”、而属于 placeable 正确性本体的要求：放置出来的对象不应该继续挂在场景根目录，必须落到当前层级 / 当前图层对应的正确 parent / container 下。根层已经据此把 hierarchy 归属也升级成了本轮硬验收项：后续农田若声称恢复通过，除了不能再有远停 / 无法放置 / 全场幽灵，还必须证明代表性 placeable 不再默认刷在根目录，而是回到正确层级容器中。

## 2026-03-25：`011` 当前已完成 parent/container 补口与 live 触发取证，但 placeable 恢复仍被 shared root 外部红编译卡住

根层当前新增的稳定事实是：农田这轮已经不再只是“准备做 `011`”，而是实际把 `124caccc` 对照、placeable parent 解析和 second-blade live 入口都做到了当前现场里。当前真实 owned 施工面集中在 `PlacementManager.cs`、`PlacementSecondBladeLiveValidationRunner.cs`、`PlacementSecondBladeLiveValidationMenu.cs`：`PlacementManager.cs` 已新增 `EnsureValidatorInitialized()`，修掉 live 首次命中 `validator == null` 的空引用；同时新增 `ResolvePlacementParent()`，普通 placeable 放下时会优先解析 `SCENE/LAYER */Props`，找不到时才回退到 `FarmTileManager` 当前层的 `propsContainer`，从代码层堵住“实例刷在场景根目录”的错误 parent 语义。配套地，second-blade runner 现在把 `ChestReachEnvelope` 的 parent sanity 纳入通过条件，菜单也已支持从 Edit Mode 自动切到 Play 并启动 runner。与此同时，线程再次对照 `124caccc` 复核 placeable 第一刀切口：`FarmToolPreview.cs` 当前 placeable / preview 口径已不再偏离 `124caccc`，而 `GameInputManager.cs` 当前剩余 diff 主要落在工具运行时提交链和 live 调试辅助入口，并未在本轮继续做机械回退。验证层方面，`PlacementManager / runner / menu` 的脚本级检查与 `git diff --check` 都通过，MCP 基线也已再次确认是 `8888 + unityMCP`，活动场景 `Primary`、`Edit Mode`、`isDirty=false`。但 placeable 恢复向 live 仍未真正跑起来，因为一触发 `Tools/Sunset/Placement/Run Second Blade Live Validation`，Console 就先被 shared root 的 external compile blocker 截断：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs(1012,122): error CS0103: The name '_panelVelocity' does not exist in the current context`。所以根层当前恢复点必须写实为：`011` 已经进入真实 restore / 补口阶段，但还没有拿到足够证明“当前 placeable 至少不比 `124caccc` 更差”的 live 证据；下一步只能在这条外部红编译清掉后，立刻重跑 second-blade live，把 placeable 主链和 hierarchy sanity 真正跑到结果。

## 2026-03-25：根层回正，`012` 已完成 placeable 恢复向 live 取证，当前剩余点从主链事故收敛到 runner 取样稳定性

根层当前新增的稳定事实是：农田 placeable 这条线已经不再停在“被外部 blocker 截断”的状态。线程本轮已在 `main` 语义下继续完成 hygiene 报实与 second-blade live，至少有一轮 fresh 结果完整打出 `ChestReachEnvelope / PreviewRefreshAfterPlacement / SaplingGhostOccupancy / ChestSaveLoadRegression` 全部通过，且 hierarchy 证据明确显示代表性放置物当前进入 `SCENE/LAYER 1/Props/Farm`，不再落在场景根目录。与此同时，`GameInputManager.cs` 当前 diff 也已被再次钉清：它在这轮里不是 placeable 主链事故的继续扩面，而是给导航 live runner 保留的最小兼容入口。后续线程又做了 validation-runner 自己的 hygiene 降噪补口，并重跑了若干轮 second-blade；这些附加复跑中，箱子放置、preview 原格重刷和 parent/container 仍稳定通过，但树苗场景出现了候选点采样波动导致的 `SaplingGhostOccupancy` timeout。根层因此更新总口径为：当前 placeable 主链已经至少恢复到“不比 `124caccc` 更差”的工作基线，代表性 placeable / hierarchy 也已经回正；新的单一剩余点只剩 second-blade runner 在树苗场景下的取样稳定性，而不再是用户最痛的“远停 / 根本放不下 / 全场幽灵”主链事故态。

## 2026-03-26：根层补记，`013` 当前已完成 sapling-only 入口稳定性修复，剩余阻断转为 shared root 并发 live 干扰下的 fresh 树苗验证

根层当前新增的稳定事实是：农田 `013` 这轮已经不再停在“sapling-only 菜单请求发出了，但进 Play 后不自动启动”的入口层问题。线程本轮对 second-blade runner/menu 做了两处最小稳定性补口：菜单层引入 `SessionState` 持久化待执行 scope，并通过 `[InitializeOnLoad]` 确保 domain reload 后仍能恢复 `playModeStateChanged` 回调；runner 层则在 sapling 场景里新增 primed preview 稳定性检查，并优先通过反射触发 `LockPreviewPosition()`，避免自动化 live 被 UI 指针门或 preview 瞬态抖动吞掉。最新 `Editor.log` 已经真实出现 `runner_started scene=Primary scope=SaplingOnly`，说明 sapling-only 入口 bug 已被压掉。与此同时，根层也需要继续把 hygiene 口径写实：`FarmRuntimeLiveValidationRunner.cs / FarmRuntimeLiveValidationMenu.cs` 当前是保留的有效 live 资产，而不是应删除的残留；`GameInputManager.cs` 仍按 mixed-owner hot file 处理，并未在本轮被当成农田独占文件硬回退。当前最新 fresh 结果仍未转绿，而是停在 `all_completed=false failed_scenario=SaplingGhostOccupancy scenario_count=1`。更关键的是，同一时间窗里 `NavValidation] all_completed=...` 日志持续与农田 sapling-only 结果交织，说明当前 shared root 的 Unity live 存在并发导航线验证干扰。根层据此更新总口径为：placeable 主链仍保持 `012` 已成立的恢复基线，不应被重新打回事故态；`013` 当前真正剩余的不是业务主链故障，而是 shared root 并发 live 干扰下的 fresh `SaplingGhostOccupancy` 仍未通过。后续若继续，只应在无并发 live 噪音的窗口里重跑一次 sapling-only，不再把范围放大。

## 2026-03-26：根层补记，`1.0.4` 需求总入口已追加新的 6 条现场诉求

根层当前新增的稳定事实是：农田 `1.0.4` 的需求面又被用户明确扩充了一轮，而且这次不是“开新工作区”，而是要求直接把新增诉求写回当前需求总入口，作为后续统一参照。当前已正式落盘到 `全面理解需求与分析.md` 的新增 6 条，覆盖了三类新的高优先级边界：一是箱子 / placeable 主链的新现场要求，包括“右键箱子必须真的走到可开启距离”“打开箱子不应退出放置模式”“parent/container 不能继续按临时 Farm 容器语义乱放”“幽灵占位仍严重存在”；二是工具与玩家反馈链的新要求，包括“工具本次挥砍后耐久归零时应在当次动画结束后立刻终止动作并触发气泡 / 音效 / 特效”“精力耗尽也应有玩家反馈设计”；三是树生命周期的新问题，即第 2 阶段树被砍倒后会出现看得见碰不到的幽灵僵尸 sprite。根层因此需要更新总口径：后续农田实现与验收，不能再只围绕旧的 5 大主题或 placeable runner 稳定性本身，而必须同时把这 6 条新增现场诉求纳入同级需求边界。

## 2026-03-26：根层补记，农田线当前已具备无失真代际交接条件

根层当前新增的稳定事实是：农田这条线已经通过了“进入下一代线程交接前状态确认”。当前根层接受的交接口径已经明确为三层：第一层是业务基线层，`012` 已经把 placeable 主链恢复到至少不比 `124caccc` 更差；第二层是验证尾差层，`013` 当前剩下的是 sapling runner 稳定性，不再等同于业务主链仍在事故态；第三层是需求扩展层，新增 6 条现场诉求已经并入 `1.0.4` 需求总入口。基于这三层口径，线程已在自己的 `.codex` 线程目录下生成 `V2交接文档`，并将未来继任线程名固定为 `农田交互修复V3`。根层恢复点更新为：农田后续已经可以由 `V3` 继续承接，不需要再由 `V2` 留在现场重复解释当前到底算不算已恢复。

## 2026-03-26：根层补记，农田 `V3` 已进入 shared-root cleanup 收口阶段，当前明确不吞并 mixed / hot-file

根层当前新增的稳定事实是：农田这条线在 `V3` 接手后，不只是继续围绕 `013` 判断 sapling runner 尾差，同时也已经显式进入一轮“只做 own dirty / untracked 清扫和白名单收口”的 shared-root cleanup 阶段。根层现在已能清楚写出农田白名单：允许继续纳入农田 own 的，是 `PlacementSecondBladeLiveValidationMenu.cs / PlacementManager.cs / PlacementSecondBladeLiveValidationRunner.cs`、`FarmRuntimeLiveValidationRunner.cs` 及其 Editor 菜单/元文件、农田工作区 `.kiro/specs/农田系统/` 下当前记忆与 `008~013`、`2026-03-26-*`、`当前续工计划与日志.md` 等尾账，以及 `农田交互修复V2` 的线程记忆 / `V2交接文档` 和 `农田交互修复V3/memory_0.md`。与之配套，根层本轮又把两个最容易被误吞的路径重新钉死：`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 继续按 mixed hot-file 只报实、不认领；`ProjectSettings/TagManager.asset` 当前 owner 仍未明确，也只报异常、不让农田线擅自吞并。为了让这轮 cleanup 真能收口，线程只做了一刀 warning-cleanup，而不是业务续工：把 `PlacementManager.cs` 中 3 个仅靠 Inspector 注入的序列化字段补了显式 `= null;` 初值，从而消掉 `git-safe-sync` preflight 会卡住白名单同步的 3 条 CS0649 warning。根层恢复点因此更新为：农田当前 own dirty / untracked 与 foreign dirty 已经重新分离清楚，mixed / hot-file 也已明确不碰；后续只应继续做农田白名单 sync 到 `main`，不应再借 cleanup 顺手扩回 placeable / runner 业务推进。

## 2026-03-26：根层补记，农田 `V3` cleanup 已把 own 白名单同步回 `main`，shared root 剩余脏改不再属于农田线

根层当前新增的最终稳定事实是：农田 `V3` 这轮 shared-root cleanup 已完成真正的白名单提交，而不是停在 preflight。线程已在 `main` 语义下完成农田 own 白名单 sync，业务/资源主提交为 `f5a2bf5078be32f37f99f9599b47a99492fd7ec3`，提交与推送均成功。sync 后对农田白名单重新执行 `git status --short --untracked-files=all -- <农田白名单>` 已为空，因此根层现在可以把结论写死：农田 own dirty / untracked 已从 shared root 清空，剩余未 clean 仅是 foreign / mixed dirty，包括 `GameInputManager.cs`、`TagManager.asset`、`Primary.unity`、`StaticObjectOrderAutoCalibrator.cs`、治理线文档以及 `tmp/pdfs/resume_check/*` 等，不再属于这轮农田认领面。根层恢复点据此更新为：农田 `V3` 当前已完成 cleanup 收口，后续如果再推进农田线，应重新回到明确的新业务委托，而不是继续在 shared-root 清扫语义下扩写业务。

## 2026-03-26：根层补记，农田 `V3` 恢复开工轮次当前应判定为 blocker，不是新的已完成功能包

根层当前新增的稳定事实是：农田 `V3` 在完成 shared-root cleanup 后，确实已重新尝试回到业务续工，但这轮不是再去重跑 `013` 或 placeable 主链，而是严格按 `恢复开工委托-05` 瞄准“工具运行时资源链 -> 玩家反馈 -> 树木 / hover 遮挡口径”这条 non-hot vertical slice。根层现在需要把最关键的真相写实：当前主分支上这条 slice 虽已有部分实现，但还不能被包装成“现在只差用户复测”的已完成功能包。原因不是编译红错，而是业务事务边界仍卡在 hot-file：`GameInputManager.cs` 的 `ExecuteTillSoil(...)` 与 `ExecuteWaterTile(...)` 现行顺序仍停在“世界结果先落地、工具提交后补账”，这直接阻断了“工具运行时资源链”和“空壶不应浇水成功”两个完成定义。与此同时，树木侧当前只能确认 `TreeController.cs` 已实现不足等级不扣精力、30 秒冷却计时和正向切换气泡入口，但还没有证明输入层已经彻底挡住 30 秒内再次挥砍高树动作；hover 侧则已经定位到 `FarmToolPreview.cs` 当前仍把整组 `currentPreviewPositions` 的联合 bounds 上报给 `OcclusionManager`，并未收紧到中心格 / 主格优先。由于委托-05 明确要求“一旦确认必须重开 `GameInputManager.cs` 才能成立，就立刻停在 blocker”，线程本轮没有继续改任何业务代码，而是把结论沉淀成详细汇报文件、工作区记忆与线程记忆。根层恢复点因此更新为：农田 `V3` 当前并未在这轮新交出一个可 claim 通过的 vertical slice，而是已明确定位出 hot-file blocker；后续只有两条合法前进路径，要么用户授权重开 `GameInputManager.cs`，要么把范围重新切窄成不依赖 hot-file 的单点。
