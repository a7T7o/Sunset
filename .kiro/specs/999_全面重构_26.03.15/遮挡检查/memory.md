# 遮挡检查 - 工作区记忆

## 模块概述

本工作区用于对 Sunset 当前遮挡系统做只读审计与证据化核实，围绕 `OcclusionManager`、`OcclusionTransparency` 与树木状态、导航刷新、命中判定、Prefab/场景配置之间的真实联动建立基线。

## 当前状态

- 完成度：25%
- 最后更新：2026-03-15
- 状态：初步核实完成，尚未进入整改设计

## 会话记录

### 会话 1 - 2026-03-15

**用户需求**：
> `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\遮挡检查，这里是你的线程工作区，D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\遮挡检查，这里是你的sunset工作区，请你显彻底阅读线程文件夹下的所有内容然后进行核实，核实当然是理解所有内容后去进行阅读与当前系统的所有相关的代码和sunset相关工作区的所有文档，我要的是你最准确的理解然后生成一份从你的角度你的思考的分析文件在D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\遮挡检查\1.0.0初步检查，并且要记得按照规则创建并更新memory，请你开始`

**当前主线目标**：
- 对 Sunset 遮挡系统做一次从线程旧稿到当前代码、Prefab、场景、测试、Unity 现场的彻底核实，形成后续整改前可继续接手的现状基线。

**本轮子任务 / 阻塞**：
- 用户指定的子工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\遮挡检查` 当前并不存在，父工作区只有总 `memory.md`，没有现成遮挡检查文档，需要先补建工作区记忆再收敛结论。

**完成任务**：
1. 读取线程目录既有文档：
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/memory.md`
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/遮挡系统代码索引与调用链.md`
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/遮挡-导航-命中一致性与性能风险报告.md`
2. 读取路由与规则文档：
   - `.kiro/steering/README.md`
   - `.kiro/steering/rules.md`
   - `.kiro/steering/workspace-memory.md`
   - `.kiro/steering/documentation.md`
   - `.kiro/steering/scene-modification-rule.md`
   - `.kiro/steering/systems.md`
   - `.kiro/steering/trees.md`
   - `.kiro/steering/layers.md`
   - `.codex/threads/线程分支对照表.md`
3. 读取历史遮挡资料与交接文档：
   - `.kiro/specs/云朵遮挡系统/memory.md`
   - `.kiro/specs/云朵遮挡系统/old/requirements.md`
   - `.kiro/specs/云朵遮挡系统/old/design.md`
   - `.kiro/specs/云朵遮挡系统/old/tasks.md`
   - `Docx/分类/遮挡与导航/000_遮挡与导航系统完整文档.md`
   - `Docx/分类/遮挡与导航/遮挡透明系统v2.0-最终总结.md`
   - `Docx/分类/遮挡与导航/遮挡透明系统-问题解决记录.md`
   - `Docx/分类/树木/树木成长状态与遮挡透明联动实现方案.md`
   - `Docx/分类/树木/树林隐蔽与导航完善总结.md`
   - `Docx/分类/交接文档/树林遮挡核心系统优化交接文档.md`
4. 核实当前运行时代码、Editor 脚本与测试：
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
   - `Assets/YYY_Scripts/Controller/TreeController.cs`
   - `Assets/YYY_Scripts/Combat/PlayerToolHitEmitter.cs`
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementPreview.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
   - `Assets/YYY_Scripts/Combat/ResourceNodeRegistry.cs`
   - `Assets/YYY_Scripts/Interfaces/IResourceNode.cs`
   - `Assets/YYY_Scripts/Controller/StoneController.cs`
   - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - `Assets/Scripts/Utils/ConvexHullCalculator.cs`
   - `Assets/YYY_Scripts/Service/DynamicSortingOrder.cs`
   - `Assets/Editor/BatchAddOcclusionComponents.cs`
   - `Assets/Editor/OcclusionManagerEditor.cs`
   - `Assets/Editor/OcclusionTransparencyEditor.cs`
   - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
5. 核实场景、Prefab、meta 与纹理可读配置：
   - `Assets/000_Scenes/Primary.unity`
   - `Assets/222_Prefabs/Tree/M1.prefab`
   - `Assets/222_Prefabs/Tree/M2.prefab`
   - `Assets/222_Prefabs/Tree/M3.prefab`
   - `Assets/222_Prefabs/Rock/C1.prefab`
   - `Assets/222_Prefabs/Rock/C2.prefab`
   - `Assets/222_Prefabs/Rock/C3.prefab`
   - `Assets/222_Prefabs/House/House 1.prefab`
   - `Assets/222_Prefabs/Box/Box_1.prefab`
   - `Assets/222_Prefabs/Box/Box_2.prefab`
   - `Assets/222_Prefabs/Box/Box_3.prefab`
   - `Assets/222_Prefabs/Box/Box_4.prefab`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs.meta`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs.meta`
   - `Assets/YYY_Scripts/Service/DynamicSortingOrder.cs.meta`
   - `Assets/ZZZ_999_Package/Pixel Crawler/Environment/Props/Static/Trees/Model_01/Size_02.png.meta`
6. 通过 Unity MCP 复核编辑器现场：
   - 活跃场景为 `Primary`，路径 `Assets/000_Scenes/Primary.unity`
   - 当前 `Primary` 已加载且 `isDirty = false`
   - Console 当前有 1 条与 `Assets/Editor/NPCPrefabGeneratorTool.cs` 相关的过时 API warning
   - 场景内组件计数：`OcclusionManager = 1`、`OcclusionTransparency = 44`、`TreeController = 20`
   - `OcclusionSystemTests` EditMode 测试 11/11 通过
7. 在线程目录新增核实分析文件：
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/03_遮挡现状核实与差异分析（Codex视角）.md`

**关键结论**：
1. 旧线程报告里脚本 GUID 映射写反了，当前正确映射为：
   - `OcclusionManager.cs.meta` -> `4a3f1c967ec52c946a301ffbacd49b6c`
   - `OcclusionTransparency.cs.meta` -> `9b41652a450cc9447abb94ac5ce72c1a`
   - `DynamicSortingOrder.cs.meta` -> `45a3d1d4bd5a8744aa9ed982679f875a`
2. 旧资料与当前实现已经明显漂移；诸如 `Bounds.Contains` 主判定、Y 预过滤、`affectChildren`、`occlusionTags`、`upwardOffset`、若干清理/批处理工具等内容，不能再直接当作真实现状。
3. `Primary.unity` 中 `OcclusionManager` 当前真实参数不是旧文档常写的那套：`rootConnectionDistance = 1.5`、`useOcclusionRatioFilter = 1`、`minOcclusionRatio = 0.4`、`enableSmartEdgeOcclusion = 1`，`occludableTags` 实际包含 `Interactable`、`Building`、`Tree`、`Rock`、`Buildings`、`Placed`。
4. `Tree/M1.prefab`、`M2.prefab`、`M3.prefab` 每棵树都挂了两份 `OcclusionTransparency`，分别在子物体 `Tree` 与父物体 `M1/M2/M3` 上；场景内 `TreeController = 20` 但 `OcclusionTransparency = 44`，和“20 棵树双组件 + 其他遮挡物”高度吻合。
5. `TreeController` 只在本节点执行 `GetComponent<OcclusionTransparency>()`，阶段切换时调用的是当前节点这一份 `SetCanBeOccluded()`；父节点那份组件不会同步关闭，因此“树苗/树桩不应遮挡”的规则存在被父组件绕开的真实风险。
6. `OcclusionManager` 的 `registeredOccluders`、`currentForest` 等集合都按 `OcclusionTransparency` 组件计数，不按“物理树”计数；配合 `FindConnectedForest()` 与 `HandleForestOcclusion()` 中 `overlappingTreeCount >= 2` 的保底逻辑，单棵物理树就可能因为父/子双组件被误判成“多棵树重叠”。
7. 像素采样不是纸面开关：树 Prefab 上 `usePixelSampling = 1`，树纹理 `Size_02.png.meta` 为 `isReadable: 1`，运行时会真实进入 `GetPixel` 路径。
8. 命中判定存在更广泛的双标准问题：`PlayerToolHitEmitter` 使用 `GetColliderBounds()`，而 `GameInputManager` 的右键资源节点分支使用 `GetBounds().Contains(world)`；树和箱子都实现了 `GetBounds/GetColliderBounds`，因此点击与工具命中的体感边界无法保证一致。
9. `Assets/Editor/BatchAddOcclusionComponents.cs` 仍在写不存在的序列化字段 `affectChildren` 与 `occlusionTags`，说明批处理工具已经落后于当前组件定义。
10. `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs` 虽然 11/11 通过，但测试仍偏向旧思路里的 `Bounds.Contains` 和局部数学片段，不能代表当前 Prefab 结构、树林联动和双组件现状是可靠的。

**验证结果**：
- 线程类型与现场：`遮挡检查` 按对照表属于 A 类审计线程，当前工作目录 `D:\Unity\Unity_learning\Sunset`、当前分支 `main` 正确。
- 场景证据：`Primary.unity` 当前确有 `OcclusionManager`，参数块与旧稿存在差异；Unity 场景现场显示 `Primary` 已加载且未 dirty。
- Prefab 证据：三棵树 Prefab 都命中两次 `guid: 9b41652a450cc9447abb94ac5ce72c1a`；根节点 `m_Children` 顺序是 `Tree` 在前、`Shadow` 在后，因此父节点 `Awake()` 选到的第一个 `SpriteRenderer` 仍是 `Tree` 而不是 `Shadow`。
- 代码证据：
  - `OcclusionTransparency.Awake()` 使用 `GetComponentsInChildren<SpriteRenderer>()` 并拿 `childRenderers[0]`
  - `TreeController` 用 `GetComponent<OcclusionTransparency>()`
  - `OcclusionManager` 使用 `HashSet<OcclusionTransparency>`
  - `PlayerToolHitEmitter` 用 `GetColliderBounds()`
  - `GameInputManager` 用 `GetBounds().Contains(world)`
- Unity 现场：
  - Console 有 1 条 NPC 编辑器工具 warning，但当前没有遮挡系统现成报错
  - `OcclusionSystemTests` EditMode 11/11 通过

**遗留问题 / 下一步**：
- [ ] 如果进入整改设计，第一优先级应先厘清“树要不要保留父/子双遮挡组件”，否则阶段联动和树林判定的其他修复都可能建立在错误对象粒度上。
- [ ] 为“单棵树双组件是否触发树林保底透明”建立最小复现实验，避免只凭代码阅读推断。
- [ ] 统一资源节点的点击判定与工具命中判定基准，至少先确定项目最终要以 Sprite 边界还是 Collider 边界为准。
- [ ] 将 `OcclusionSystemTests` 扩到当前真实主链，至少覆盖双组件树、Prefab 配置、`SetCanBeOccluded()` 联动与树林保底逻辑。
- [ ] 清理或重写 `BatchAddOcclusionComponents.cs` 等过时工具，并在文档层纠正 GUID 与现状参数漂移。

### 会话 2 - 2026-03-16

**用户需求**：
- 当前先处理“审计成果的固化与阶段口径澄清”，先核对工作目录 / 分支 / `HEAD` / dirty，说明手头持有的文档重组 / 删除 / 未跟踪内容，先固化本轮遮挡审计成果，再判断是否进入代码整改阶段并给出最小目标。

**当前主线目标**：
- 把 `遮挡检查` 从“线程审计已经做完”推进到“子工作区有稳定可承接的审计基线和阶段口径”，而不是默认直接进入代码整改。

**本轮子任务 / 阻塞**：
- 当前并非继续实现，而是在清理阶段边界。当前最大的阻塞不是业务理解，而是 Git 现场已经漂移到 `codex/npc-main-recover-001`，并混入大量与遮挡线程无关的重组 / 删除 / 未跟踪内容。

**完成任务**：
1. 复核当前现场：
   - 工作目录：`D:\Unity\Unity_learning\Sunset`
   - 分支：`codex/npc-main-recover-001`
   - `HEAD`：`b9b6ac48`
2. 复核当前 `遮挡检查` 相关文档状态：
   - 旧根目录文档被 Git 标记为删除
   - `1.0.0初步检查` 子目录中的新文档当前仍是未跟踪
   - 子工作区当前只有 `memory.md`，缺少一份明确的阶段口径文档
3. 新增：
   - `.kiro/specs/999_全面重构_26.03.15/遮挡检查/审计成果固化与阶段口径.md`
4. 明确当前阶段口径：
   - 审计成果已形成
   - 还未正式进入代码整改执行
   - 后续如果进入整改，最小目标应先锁定“树 Prefab 双 `OcclusionTransparency` 粒度问题”

**关键结论**：
1. 当前最大的现实风险不是“遮挡问题不知道怎么修”，而是“当前 Git 现场不是遮挡单线现场，不能默认直接开修”。
2. `遮挡检查` 现阶段应该先以工作区正文文档承接审计成果，而不是继续把关键判断只放在线程文档里。
3. 后续若进入整改阶段，最小目标应先围绕树双组件粒度做验证与设计，而不是直接大范围改 `OcclusionManager` / `GameInputManager`。

**验证结果**：
- 当前分支已不再是 2026-03-15 审计时的 `main`，而是 `codex/npc-main-recover-001`。
- 当前工作树存在大量与 `遮挡检查` 无关的治理重组与 NPC 恢复内容。
- `审计成果固化与阶段口径.md` 已落入子工作区，工作区侧现已具备稳定承接入口。

**遗留问题 / 下一步**：
- [ ] 需要先决定是否在当前分支继续做遮挡整改，还是回到更干净的遮挡专用现场后再开工。
- [ ] 如果进入整改设计，先做“单棵双组件树是否触发树林保底逻辑”的最小复现实验设计。

### 会话 3 - 2026-03-17

**用户需求**：
- 只做只读核查，并回答三个问题：
  1. 当前线程后续是否需要独立 worktree
  2. 如果进入实际整改，是否接受“共享根目录 + 分支”模式
  3. 当前线程后续最合理的 branch-only 开工入口是什么

**当前主线目标**：
- 对 `遮挡检查` 后续整改的 Git / worktree 进入方式做一次最小而明确的路线确认，但不进入实现。

**本轮子任务 / 阻塞**：
- 这是主线的治理路由澄清，不是代码整改；本轮只核对现行规则、线程映射与当前现场。

**完成任务**：
1. 只读复核当前现行入口与线程映射：
   - `.codex/threads/线程分支对照表.md`
   - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset当前唯一状态说明_2026-03-16.md`
   - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/Sunset Git系统现行规则与场景示例_2026-03-16.md`
   - `.kiro/steering/git-safety-baseline.md`
2. 复核当前线程与子工作区 memory，确认此前“当前现场已漂移到 `codex/npc-main-recover-001`”只是现场事实，不代表 `遮挡检查` 的默认进入口径已经被改写。

**关键结论**：
1. `遮挡检查` 当前**不需要**独立 worktree 才能继续后续重构；现行规则明确它属于 A 类审计线程，默认入口仍是 `D:\Unity\Unity_learning\Sunset @ main`。
2. 如果后续进入实际整改，我接受默认走“共享根目录 + 分支”模式；只有遇到共享热文件高冲突、高风险隔离或特殊实验时，才升级为 worktree 例外。
3. 当前线程最合理的 branch-only 开工入口是：
   - 先回到 `D:\Unity\Unity_learning\Sunset @ main` 做只读现场核对 / preflight
   - 若确认要进入整改实现，再切到新的 `codex/...` 任务分支
4. 推荐分支名基线：
   - `codex/occlusion-remediation-001`
   - 若要更强调来源，可用 `codex/occlusion-audit-remediation-001`

**验证结果**：
- 线程映射表已明确写明：`遮挡检查` 默认入口工作目录是 `D:\Unity\Unity_learning\Sunset`，默认入口分支是 `main`，默认只读；若转整改再切 `codex/...`，且 `worktree` 口径为“不默认使用”。
- `Sunset当前唯一状态说明_2026-03-16.md` 已明确：默认开发现场是共享根目录，`worktree` 只用于高风险隔离、故障修复、特殊实验。
- `Sunset Git系统现行规则与场景示例_2026-03-16.md` 已明确：真实任务改动默认是“先从主项目目录读取现场，再切 `codex/...` 分支实现”。

**遗留问题 / 下一步**：
- [ ] 如果正式开整改，先回到 `main` 做一次新的 `preflight`，确认当前 dirty 和热文件占用，再决定是否创建 `codex/occlusion-remediation-001`。
