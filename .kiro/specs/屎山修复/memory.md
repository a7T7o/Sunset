# 屎山修复 - 工作区记忆

## 模块概述

本工作区承接 Sunset 中那些已经不能靠局部补丁长期维持、需要重新定性并系统性修复的主线。  
当前已建立子工作区：

1. `导航检查`
2. `遮挡检查`

## 当前状态

- **完成度**: 5%
- **最后更新**: 2026-03-22
- **状态**: 进行中

## 会话记录

### 会话 1 - 2026-03-22

**用户需求**：
> 这里是你的工作区D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查，你要做的不是写什么分析需求设计任务五件套文档，你只需要一个，详细的阶段设计内容……当然需要遵守kiro工作区的memory文档规范，请你开始吧，文档先行

**完成任务**：
1. 确认父工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复` 当前仅有子目录，没有父层 `memory.md`。
2. 创建父工作区 `memory.md`，作为后续各“屎山修复”子线的总索引。
3. 建立子工作区 `导航检查` 的统一入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
4. 明确父层当前对导航线的定性：
   - 这不是继续普通导航整改
   - 而是进入“共享导航核心重构”的系统级修复

**关键结论**：
- `屎山修复/导航检查` 已正式建立，不再沿用旧 `999_全面重构_26.03.15` 工作区作为唯一执行入口。
- 当前导航问题已经被提升为系统级问题：玩家导航、NPC 漫游、静态路径规划和最终运动语义分裂。
- 后续如果继续推进，应以 `导航检查` 子工作区中的统一主文档为唯一执行基线。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 创建 `屎山修复` 父工作区记忆 | 父层需要对子线做总索引 | 2026-03-22 |
| `导航检查` 不再铺五件套，只维护一个主文档 | 用户明确要求唯一长期执行入口 | 2026-03-22 |
| 导航问题正式升级为系统级重构问题 | 当前已经不适合继续局部补丁推进 | 2026-03-22 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md` | 导航线唯一主文档 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md` | 导航线子工作区记忆 |

### 会话 2 - 2026-03-23

- 子工作区 `导航检查` 本轮进入 `锐评/001.md` 审核。
- 已回读：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\锐评\001.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\steering\code-reaper-review.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
- 父层稳定结论：
  - 本次锐评不走 Path C，走 **Path B**
  - 锐评方向大体正确，但“必须统一改成 `linearVelocity`”属于过强技术预设，不能直接当项目定案
  - 其余三类建议应作为主表的局部修订输入：
    - 为 S3 明确第一版算法边界
    - 为执行顺序补交织迭代口径
    - 为代理契约补 sleeping / moving 状态

### 会话 3 - 2026-03-23

- 子工作区 `导航检查` 本轮已从“锐评审核”推进到“Path B 第一批共享导航核心实现”。
- 父层新增稳定事实：
  - 已落地的共享核心文件：
    - `NavigationAgentSnapshot.cs`
    - `NavigationAvoidanceRules.cs`
    - `NavigationAgentRegistry.cs`
    - `NavigationLocalAvoidanceSolver.cs`
  - 已扩展的统一代理契约：
    - `INavigationUnit.cs`
  - 已接入共享核心的现有执行体：
    - `PlayerAutoNavigator.cs`
    - `NPCAutoRoamController.cs`
- 父层当前判断：
  - `导航检查` 已正式从“架构主表期”进入“共享导航核心第一批实现期”
  - 但这轮还没有完成统一路径执行层，也还没有完成最终运动语义收口，因此当前属于“第一阶段已实装，不是最终闭环”

### 会话 4 - 2026-03-23

- 子工作区 `导航检查` 本轮基于用户的运行态否决反馈，回到了“共享规则重新排查”。
- 父层新增稳定事实：
  - 运行态失败现象“玩家自动导航仍在推着 NPC 走”已经被正式视为 P0 未通过
  - 当前排查确认，至少有一处规则层问题真实存在：自动导航玩家默认没有对 moving NPC 主动让行
- 本轮已补的最小修正：
  - `NavigationAvoidanceRules.cs`
  - `NavigationLocalAvoidanceSolver.cs`
  - `NavigationAvoidanceRulesTests.cs`
- 父层当前判断：
  - 这轮修正属于“共享核心第一阶段的第一轮行为纠偏”
  - 如果复验后仍失败，则下一刀不应继续补规则，而应直接进入共享路径执行层或最终运动语义收口

### 会话 5 - 2026-03-23

- 子工作区 `导航检查` 本轮已针对运行态失败现象完成一轮直接纠偏，而不是继续停在抽象架构层。
- 父层新增稳定事实：
  - 当前已确认的一个直接根因是：自动导航玩家在共享规则中没有对 moving NPC 真正让行
  - 已修正文件：
    - `NavigationAvoidanceRules.cs`
    - `NavigationLocalAvoidanceSolver.cs`
    - `NavigationAvoidanceRulesTests.cs`
- 父层当前判断：
  - 这轮修正属于 P0 行为纠偏，而不是新一轮架构扩写
  - 如果这轮复验后依旧失败，则说明问题主因已越过规则层，进入共享路径执行层或最终运动语义冲突

### 会话 6 - 2026-03-23

- 子工作区 `导航检查` 本轮补做了一个纯止血动作：修复我新加测试引起的 `Tests.Editor` 编译错误。
- 父层稳定结论：
  - 运行时代码的共享导航核心类型仍然存在且可读
  - 真正出错的是测试装配看不到这些类型
  - 现在已经改成反射式测试写法，避免继续在编译期直连主程序集类型

### 会话 7 - 2026-03-23

- 子工作区 `导航检查` 本轮已吸收 NPC 线的正式回执，并继续推进 P0 行为纠偏。
- 父层新增稳定事实：
  - NPC 线当前明确承诺 `Moving / Pause / MovePosition` 与 `001 / 002 / 003` 的刚体/碰撞基线先稳定，不会在联调阶段随意再改
  - 因此当前“玩家仍然推着 NPC 走”的主排查责任继续留在导航线，而不是再把锅推回 NPC 线
- 本轮新增导航侧修正：
  - `PlayerMovement.cs` 改为保留输入强度，不再把减速信息全部归一化吃掉
  - `PlayerAutoNavigator.cs` 增加临时动态绕行点
  - `NavigationLocalAvoidanceSolver.cs` 增加速度缩放与更完整的阻挡信息

### 会话 8 - 2026-03-23

- 子工作区 `导航检查` 本轮先处理中断性卫生问题，而不是继续扩写整改范围。
- 已核实：
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 中导航线新增的 scene 脏改仅为 `PlayerAutoNavigator` 的 6 个新序列化字段。
  - 这些字段值与代码默认值一致，不是必须保留在场景里的长期配置。
  - 同一份 scene diff 还混有非导航内容，因此不适合整文件按导航线 checkpoint。
- 已执行：
  - 从 `Primary.unity` 中移除上述 6 个导航字段，释放导航线对 A 类热场景的字段级占用。
  - 将导航线本轮仍需保留的最小代码变更收口到独立 checkpoint：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- 父层当前判断：
  - `导航检查` 这条线对 `Primary.unity` 的直接占用应视为已解除；
  - 后续这条线如果还需要再次碰场景，必须是带明确验证目的和最小修改面，而不是继续把导航中间态挂在热文件上。

### 会话 9 - 2026-03-23

- 子工作区 `导航检查` 本轮没有继续扩架构，而是围绕用户反复指出的 P0 失败现象回到执行层闭环修复。
- 父层新增稳定结论：
  - 当前“仍然推着走”至少有三个执行层根因同时存在：
    1. 玩家动态 repath / detour 触发当帧没有停旧速度
    2. NPC 执行层此前没有消费共享 solver 的 `SpeedScale`
    3. 玩家/NPC 两侧都缺少近距接触时剥离前冲分量的最后仲裁
  - 因而之前的问题不只是规则层或路径层，而是“共享 solver 输出没有被两端执行层真正闭环”
- 本轮子工作区已落地：
  - `NavigationLocalAvoidanceSolver.cs`
  - `PlayerAutoNavigator.cs`
  - `NPCAutoRoamController.cs`
  - `NavigationAvoidanceRulesTests.cs`
- 父层当前判断：
  - 这一刀比前几轮更接近真正的 P0/P1 根因；
  - 但由于本轮会话没有可用 Unity MCP resources，运行态结论仍需后续真人/Editor 现场终验，不能把“代码已补齐”冒充为“行为已验收通过”。

### 会话 10 - 2026-03-23

- 子工作区 `导航检查` 本轮继续围绕“玩家右键导航遇 NPC 仍像推土机一样顶人”做执行层深挖，而不是再回到 tag / obstacleTags 误区。
- 父层新增稳定事实：
  - NPC prefab 的 `BoxCollider2D` 当前存在显式中心偏移（`m_Offset.y = 0.46`），而导航共享代理此前一直把 NPC 位置当成 `rb.position` 使用，这会直接把 blocker 中心和 avoidance 法线算歪。
  - `check-unity-mcp-baseline.ps1` 已确认 Sunset 当前 `unityMCP@8888` 服务基线是通的；这次读不到 MCP resources 的原因是当前会话资源暴露层异常，不是项目服务没起。
  - 当前真正的新根因是“代理中心点不一致 + 负 clearance 脱离态缺失 + detour 候选未强制净空”三件事的组合，而不是 NPC 有没有 Tag。
- 本轮子工作区已落地：
  - `NPCAutoRoamController.cs`
  - `NavigationLocalAvoidanceSolver.cs`
  - `PlayerAutoNavigator.cs`
  - `NavigationAvoidanceRulesTests.cs`
- 父层当前判断：
  - 这轮修正已经从“规则补丁”深入到接触区几何与执行语义层，比之前更接近真实根因；
  - 但最终是否达标仍取决于下一轮 Editor / Play 运行态终验，且若仍失败，下一优先嫌疑已收敛到玩家 Rigidbody 运行语义，而不是再回头查 NPC Tag。

### 会话 11 - 2026-03-23

- 子工作区 `导航检查` 本轮没有继续直接补代码，而是由治理侧补发了一份高压 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-1.md`
- 父层新增稳定判断：
  - 当前导航线的问题已经不能再被视为“再补一刀规则或参数”的层级，而是“文档承诺、代码落地、Scene 挂载、用户现场体感”四层长期失配。
  - 用户明确要求导航线先正视这种系统级偏移，再继续落地。
- 本轮 prompt 已明确要求导航线：
  - 重读 `统一导航重构阶段设计与执行主表.md`、`005-gemini-1.md`、`003-NPC-2.md`
  - 在主表中新增“当前落地偏移审计”
  - 先通过 `unityMCP` 审计 live Scene / 组件 / 挂载，再决定是否直接推进执行层收口与导航根对象整理
  - 正面审计 `NavGrid2D` 当前挂在 `Systems` 上是否合理，必要时直接调整承载对象
- 父层当前恢复点：
  - `导航检查` 下一步应先完成这次偏移审计与 Scene 层核查，再进入下一轮真正的结构性导航修复，而不是继续沿旧补丁链机械前进。

### 会话 12 - 2026-03-23

- 子工作区 `导航检查` 本轮按 `002-prompt-1.md` 完成了“当前落地偏移审计”，没有继续补局部导航 patch。
- 父层新增稳定事实：
  - unityMCP 已直接证明当前 live Scene 承载是：
    - active scene = `Primary`
    - `Primary/2_World/Systems` 同时挂有 `NavGrid2D + WorldSpawnService + WorldSpawnDebug + GameInputManager`
    - 玩家与 3 个 live NPC 的 `navGrid` 引用都指向同一个 `Systems/NavGrid2D`
  - 当前问题已被正式提升为“系统级失败”，而不是局部参数偏差。
  - 当前真实状态不是“统一导航系统已经成立”，而是：
    - S1-S3 共享基础设施已落
    - S4 共享路径执行层未落
    - 玩家 / NPC 仍在两套私有执行循环里运动
    - Scene 承载仍然混装在 `Systems`
- 父层当前判断：
  - `NavGrid2D` 继续留在 `Systems` 不合理；
  - 下一轮结构性落地应优先考虑：
    1. 建立 `NavigationRoot`
    2. 补齐共享路径执行层
    3. 收口玩家 / NPC 最终运动语义
  - 在这之前，不能再把“共享了一部分 solver”冒充为“已经是同一个导航系统”。

### 会话 13 - 2026-03-23

- 子工作区 `导航检查` 在完成偏移审计后，已由治理侧补发第二轮结构落地执行令：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-2.md`
- 父层新增稳定判断：
  - 当前导航线已不再允许停留在“审计 / 说明 / 继续论证严重性”阶段；
  - 下一阶段的唯一正确方向是：
    1. `NavigationRoot` 承载裁决与必要迁移
    2. S4 共享路径执行层落地
    3. 玩家 / NPC / NPC-NPC 的 live 终验
- 父层当前恢复点：
  - 后续如果导航线仍继续停在分析而没有进入结构落地，就应判定为没有真正执行第二轮 prompt。

### 会话 14 - 2026-03-23

- 子工作区 `导航检查` 本轮已真正执行 `002-prompt-2.md`，不再停在审计层。
- 父层新增稳定事实：
  - `NavigationRoot` 已通过 unityMCP 在 `Primary/2_World` live 建立并承接 `NavGrid2D`
  - `Systems` 已从“导航 + 世界生成 + 世界调试 + 输入”混装节点，收回为 `WorldSpawnService + WorldSpawnDebug + GameInputManager`
  - 玩家与 `001 / 002 / 003` 的 `navGrid` live 引用都已切到 `NavigationRoot/NavGrid2D`
  - S4 共享路径执行层已真实落代码：`NavigationPathExecutor2D.cs` 已新增，玩家 / NPC 都已接入同一套路径执行状态
- 父层新增关键判断：
  - 现在导航线的失败层级已经再次前移：不再是“承载没整理”或“S4 没落”
  - 当前真正的 blocker 是：
    - 玩家 `PlayerAutoNavigator.IsActive = true`
    - NPC `NPCAutoRoamController.IsMoving = true`
    - 路径点数量正常
    - 但 Player / NPC 的 `Rigidbody2D.linearVelocity` 与位置都不推进
  - 因此“玩家像推土机”背后的当前根因，不再优先落在局部规避参数，而是落在运行态位移执行没有真正打出去。
- 本轮 live 终验父层结论：
  - 玩家绕移动 NPC：失败
  - NPC 绕玩家：失败
  - NPC-NPC 会车：失败
  - 三类失败都表现为“setup 成功、路径存在、moving/active 状态成立，但几秒后位置仍停在 setup 起点”
- 父层当前恢复点：
  - 后续导航线不应再回头讨论 `NavigationRoot` 是否需要建、S4 是否需要落、NPC Tag 是否缺失；
  - 下一步应直接围绕“为什么 movement/roam 的 active 状态没有转成真实位移”做运行态排查。

### 会话 15 - 2026-03-24

- 子工作区 `导航检查` 本轮继续执行 `003-prompt-3`，没有再碰结构层。
- 父层新增稳定事实：
  - “位移未推进”这一级 blocker 已不再是当前事实；live 里玩家和 NPC 都已经能真实移动。
  - `NPC Avoids Player` 这轮已通过：
    - `pass=True`
    - `npcReached=True`
  - `PlayerAvoidsMovingNpc` 与 `NpcNpcCrossing` 仍失败，但失败形态已经变成：
    - detour / close-range 生效后会真实位移
    - 然后在 `stuck cancel / short pause / path-end` 收尾阶段提前结束
    - 不再是“完全推土机”或“压根不动”
- 父层新增关键判断：
  1. 当前导航线的新责任点已经前移到“共享执行层的恢复与收尾”，而不是结构层或刚体写入层。
  2. 现阶段最像的两个具体落点是：
    - `PlayerAutoNavigator.CheckAndHandleStuck()` 对动态 detour 的容忍度不够，导致 live 上出现“绕开了，但卡顿 3 次后取消导航”
    - 玩家 / NPC 当前都没有真正消费 `NavigationPathExecutor2D.BuildPathResult.ActualDestination`，导致 live 上出现“path 已结束 / 进入短停，但原目标并未真正到达”的风险
  3. 因而后续导航线不该再回头查 `NavigationRoot`、NPC Tag、是否真的写了 velocity；这些层级已经被本轮 live 排掉了。
- 父层当前恢复点：
  - 下一轮直接让 `导航检查` 进入：
    1. detour 期间的 stuck 免责 / 恢复推进
    2. `ActualDestination` 贯通到玩家 / NPC 收尾判断
    3. 再次 live 终验 `PlayerAvoidsMovingNpc` 与 `NpcNpcCrossing`

### 会话 16 - 2026-03-24

- 子工作区 `导航检查` 本轮已按 `005-prompt-5` 把 live 入口真正收短为单场 `PlayerAvoidsMovingNpc`，不再允许 probe setup 自动串起后两场。
- 父层新增稳定事实：
  - sleeping/stationary blocker 参数已在 `NavigationLocalAvoidanceSolver.cs` 中上调；
  - `Tests.Editor` 本轮为 `119 / 119 Passed`；
  - 单场 live 结果为：`PlayerAvoidsMovingNpc = fail`，但已经变成 `minClearance=0.161 / playerReached=True / npcReached=False`，不再是“推着玩家/NPC硬撞”。
- 父层新增关键判断：
  1. 这轮用户建议的“参数再抬高一点”已经落地并见效，失败形态从接触推挤收缩成“正净空但收尾超时”。
  2. 当前第一责任点已不再优先留在 solver 的 sleeping blocker 裁决，而是更具体地前移到：
     - `NPCAutoRoamController.TryHandleSharedAvoidance()`
     - `shouldAttemptDetour && TryCreateDynamicDetour(...)`
     - 以及其后 `OverrideWaypoint` 清理 / 恢复原目标路径的收尾链
- 父层当前恢复点：
  - 下一轮 `导航检查` 不再回结构层，也不再泛泛调避让参数；
  - 直接锁 NPC detour 恢复链，解释为什么 `001` 在正净空状态下仍会围着旧 detour 区域摆动到超时。

### 会话 17 - 2026-03-24

- 子工作区 `导航检查` 本轮已按 `002-prompt-6` 把 detour 恢复链继续压实到“共享建路层误把 NPC 自己算成终点 blocker”，并完成最小代码修复。
- 父层新增稳定事实：
  - `PlayerAvoidsMovingNpc` fresh 已通过：`pass=True / minClearance=0.145 / playerReached=True / npcReached=True`
  - `NpcAvoidsPlayer` fresh 也通过：`pass=True / minClearance=0.744 / npcReached=True`
  - `NpcNpcCrossing` fresh 仍失败：`pass=False / timeout=6.57 / minClearance=0.553 / npcAReached=False / npcBReached=False`
- 父层新增关键判断：
  1. `导航检查` 当前不再卡在“绕开后原目标被替代”；这条 detour 恢复链已经过线。
  2. 现在导航线新的剩余 blocker 已进一步收缩成“双 NPC 会车中段停滞”，不是玩家推土机，也不是 detour 恢复丢目标。
  3. 因此后续导航线不该回头再调 solver 或回结构层，而应直接查 `NpcNpcCrossing` 的共享执行推进/收尾约束。
- 父层当前恢复点：
  - 下一轮 `导航检查` 直接沿 `NpcNpcCrossing` 这条 fresh blocker 继续；
  - 保持当前事实基线：
    - 玩家绕移动 NPC：通过
    - NPC 绕玩家：通过
    - NPC-NPC 会车：未通过

### 会话 18 - 2026-03-24

- 子工作区 `导航检查` 本轮已把 `002-prompt-6` 真正收口，不再停在“只剩 `NpcNpcCrossing` 未过”的阶段。
- 父层新增稳定事实：
  - `NPCAutoRoamController.TryHandleSharedAvoidance()` 当前决定性修复不是继续抬 solver 参数，而是取消 `!avoidance.HasBlockingAgent` 分支上的即时 detour 清理；
  - `Tests.Editor` 当前 fresh 为 `123 / 123 Passed`；
  - 同轮 unityMCP fresh 三场结果已全部转绿：
    - `PlayerAvoidsMovingNpc = pass=True / minClearance=0.379 / playerReached=True / npcReached=True`
    - `NpcAvoidsPlayer = pass=True / minClearance=0.989 / npcReached=True`
    - `NpcNpcCrossing = pass=True / minClearance=0.632 / npcAReached=True / npcBReached=True`
  - 本轮 live 收尾已退回 Edit Mode，没有把 Play 现场留给别的线程。
- 父层新增关键判断：
  1. 导航线当前已不再存在 prompt 6 口径下的剩余 blocker。
  2. 双 NPC 会车的通过，确认了“detour 不得单帧清理”是当前共享执行层的稳定事实。
  3. 后续若再出现导航回归，排查顺序应从“是否重新引入 detour clear/rebuild 风暴”开始，而不是重新回到 `NavigationRoot` / S4 是否落地。
- 父层当前恢复点：
  - `导航检查` 当前 checkpoint 已完成；
  - 后续导航线如继续推进，应以“三条 live 同轮 fresh 全绿”为新的基线。

### 会话 19 - 2026-03-24

- 子工作区 `导航检查` 本轮没有继续 patch，而是按用户要求对“当前实现方式 vs 实际体感”做了一次可审计拆解。
- 父层新增稳定事实：
  - 当前导航实现本质仍是“静态路径 + 启发式局部避让 + 临时 detour 点”，不是连续 crowd simulation。
  - 玩家 / NPC 当前都用 `ColliderRadius / AvoidanceRadius / InteractionRadius` 这套圆形壳层近似；
    - 因而用户感觉像“NPC 被一个大圆壳包住”与当前实现是吻合的，不是错觉。
  - 当前三条 live fresh 全绿，证明的是受控 probe 场景链路能过，不等价于真实自由玩法手感已经过线。
- 父层新增关键判断：
  1. 当前导航线的争议已经从“功能是否完全坏掉”转成“当前启发式方案是否足够自然”。
  2. 后续如果继续收口用户体感，不能再只拿 probe pass 当最终论据，必须正面验证真实右键玩法体验。

### 会话 20 - 2026-03-24

- 子工作区 `导航检查` 本轮继续停留在“客观审核 / 锐评”模式，没有再补新 patch。
- 父层新增稳定事实：
  - 当前导航已经可以明确归类为：
    - `NavGrid2D` 静态建路
    - `NavigationPathExecutor2D` 统一路径执行 / stuck / override waypoint
    - `NavigationLocalAvoidanceSolver` 启发式局部 steering
    - 玩家 / NPC 各自一层 detour / repath 包装
  - 当前玩家与 NPC 共享了一部分执行底座，但没有真正统一成同一套动态交通语义。
  - 因此“大圆壳感”和“推土机残影”并不是用户误判，而是现实现有结构自然会长出的体感。
- 父层新增关键判断：
  1. 导航线不能判成“从头到尾都走错”；
     - 共享执行层、共享 agent registry、共享 avoidance 入口这些方向本身成立。
  2. 但它的起步抽象层级偏低；
     - 更像“先统一执行，再不断给动态避让补丁”，而不是“先定义统一动态交通规则”。
  3. 这也是为什么它前面会轮番出现：
     - 推着走
     - 不推进
     - 慢蹭
     - detour 清理风暴
     - 恢复原目标失败
  4. 所以当前最准确的结论不是“已经最佳”，而是：
     - 功能口径：已经从系统级失败拉回到受控场景可通过
     - 体验口径：仍不是最优雅、最自然的动态避让框架
- 父层当前恢复点：
  - 后续导航验收必须明确拆成：
    1. 功能通过
    2. 真实玩法手感通过
  - 如果用户要的是“可交付、可稳定复现通过”，当前方案已接近验收；
  - 如果用户要的是“玩家 / NPC 真像同一套活体交通系统”，则仍有架构升级空间。

### 会话 21 - 2026-03-24

- 子工作区 `导航检查` 本轮没有新增 patch，而是按用户要求把 `005-gemini聊天记录-1.md` 整份硬读完并做二次理解。
- 父层新增稳定事实：
  - 这份文件内部其实包含了完整的四段演化：
    1. 对当前实现的技术说明
    2. 对当前架构原罪的猛烈锐评
    3. 用户把目标从“修补能用”升级为“专业级导航底座”
    4. Gemini 给出四层架构白皮书
  - 因而它最重要的价值，不是再次证明“当前像推土机”，而是把用户的真实标准彻底抬高并钉死：
    - 不是能用
    - 是要支撑未来怪物追击、宠物跟随、NPC 生态的专业底座
- 父层新增关键判断：
  1. Gemini 方向上最有价值的部分包括：
     - 宏观寻路与微观避让必须断层
     - 动态活物不能再同时充当静态网格障碍与动态 blocker
     - 当前 detour 机制确实在污染宏观路径层
     - 玩家 / NPC 最终运动语义必须真正统一
  2. 但 Gemini 的白皮书也不能被当成“可直接照抄施工”的圣旨：
     - 它有些表述过于绝对
     - 需要结合 Sunset 现有代码资产做二次收敛
  3. 所以当前导航议题已经从“继续补丁”上升为“要不要正式产出一份项目级导航需求与架构设计稿”。
- 父层当前恢复点：
  - 若继续推进导航，不该再只围绕单点 bug；
  - 应转向“专业导航底座”的需求与架构定义，把：
    1. 现有可继承资产
    2. 必须推翻的旧语义
    3. 功能验收与体验验收
    一次性定清。

### 会话 22 - 2026-03-24

- 子工作区 `导航检查` 本轮已完成从“聊天审视”到“正式设计落盘”的切换。
- 父层新增稳定事实：
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\006-Sunset专业导航系统需求与架构设计.md`
  - 该文档已把导航线主目标正式定义为：
    - 建立 `Sunset` 的专业导航底座
    - 而不再只是修旧启发式系统直到“勉强能用”
  - `统一导航重构阶段设计与执行主表.md` 已同步接入该文档，形成：
    - 主表负责阶段推进
    - `006` 负责长期需求与架构正文
- 父层新增关键判断：
  1. 导航线从这轮开始，已经具备继续做“项目级架构迁移”而不是“局部 bug 累积修复”的文档基础。
  2. 后续任何代码线程如果还绕开 `006` 继续只修 detour / solver 参数，将再次偏离主线目标。
- 父层当前恢复点：
  - 若继续导航线，最合理的下一步不是再发泛化 prompt；
  - 而是直接从 `006` 中拆出：
    1. 资产继承清单
    2. 旧语义废止清单
    3. 第一阶段迁移任务表
