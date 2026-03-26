# 屎山修复 - 工作区记忆

## 模块概述

本工作区承接 Sunset 中那些已经不能靠局部补丁长期维持、需要重新定性并系统性修复的主线。  
当前已建立子工作区：

1. `导航检查`
2. `遮挡检查`
3. `导航V2`

## 当前状态

- **完成度**: 5%
- **最后更新**: 2026-03-26
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

### 会话 23 - 2026-03-24

- 子工作区 `导航检查` 本轮已完成对 `005-gemini锐评-2.md` 的正式审核。
- 父层新增稳定事实：
  - 本轮路径判断为 `Path B`，不是 `Path A` 全面放行，也不是 `Path C` 生成打回型审视报告。
  - 该锐评的主干方向成立，但“工业级白皮书已成”“全面放行”这类表述过满，不能直接当最终结论。
  - 该锐评补出的 3 个隐患已被吸收到 `006-Sunset专业导航系统需求与架构设计.md`：
    - 状态平滑 / 刹停恢复过渡
    - shape-aware 方案的性能边界
    - 交通记忆 / 状态惯性
- 父层新增关键判断：
  1. `006` 现在比上一版更完整，已不只是方向正确，还补上了几个后续最容易埋雷的工程口子。
  2. 但导航线仍不能被外推成“设计已最终完成，无需继续细化”。
- 父层当前恢复点：
  - 后续导航设计评审继续维持：
    - 严肃对待外部锐评
    - 但不接受过满吹捧
    - 有效补洞直接写回正式正文

### 会话 24 - 2026-03-24

- 子工作区 `导航检查` 本轮已从“正式设计正文已落盘”继续推进到“后续开发路线图已落盘”。
- 父层新增稳定事实：
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\007-Sunset专业导航底座后续开发路线图.md`
  - 当前导航线已经形成三层分工：
    - `006` 负责长期需求与架构正文
    - `007` 负责后续开发方向、阶段路线与验收路线
    - 主表负责当前阶段推进与执行勾选
- 父层新增关键判断：
  1. 导航线下一步不再是“继续锐评”或“继续调参数”；
  2. 唯一正确入口已收紧为：
     - 先打 `S0 基线冻结与施工蓝图`
     - 再进入 `S1-S8` 的底座迁移
- 父层当前恢复点：
  - 后续若继续导航线，应直接按 `007` 的 9 阶段路线推进，而不是重新回到旧 patch 叙事。

### 会话 25 - 2026-03-24

- 子工作区 `导航检查` 本轮已进一步从“路线图已落盘”推进到“连续闭环执行授权已落盘”。
- 父层新增稳定事实：
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-7.md`
  - 当前对子工作区的治理口径已切为：
    - 不再继续分阶段碎刀下令
    - 直接授权它把 `S0-S6` 当成一个连续闭环工程推进
- 父层新增关键判断：
  1. 用户当前更担心的是“多轮碎 prompt 导致偏移”，不是“线程自由度不够”；
  2. 因此本轮最合理的治理动作不是继续拆更细，而是：
     - 给足 `S0-S6` 的连续推进空间
     - 同时用固定护栏与固定回执格式防漂移
- 父层当前恢复点：
  - 后续审导航线程时，应直接看它是否真正把 `S0-S6` 往闭环推进，而不是再按旧细刀 checkpoint 审它。

### 会话 26 - 2026-03-24

- 子工作区 `导航检查` 本轮已新增一份不是发给导航线程、而是发给审稿者自己的高压验收 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\008-给Codex与Gemini的导航验收审稿prompt.md`
- 父层新增稳定事实：
  - 这份 `008` 已把后续审稿口径固定为：
    - 对照 `006 / 007 / 002-prompt-7 / 主表 / memory`
    - 强制拆解 `S0-S6`
    - 强制区分功能线与体验线
    - 强制检查旧 patch 是否仍在托底
- 父层当前恢复点：
  - 后续如果继续导航验收，应优先使用 `008` 审回执，而不是再临场即兴判断。

### 会话 27 - 2026-03-24

- 子工作区 `导航检查` 本轮已正式按 `008` 口径做了一次真正的热区审稿，不再接受导航线程“自述回执”替代代码事实。
- 父层新增稳定事实：
  - 当前方向没有漂回纯旧 patch，因为共享代理、共享裁决、共享路径资产、统一运动命令都已真实进入代码。
  - 但当前交付物仍只是“带新骨架的过渡系统”，不是 `006/007` 承诺的 `S0-S6` 第一版闭环。
  - `NavigationTrafficArbiter.cs` 与 `NavigationMotionCommand.cs` 当前仍是未跟踪文件；`Primary.unity`、`TagManager.asset` 与多份导航热区代码仍处于 dirty 状态，不能当已收口基线。
  - `PlayerAutoNavigator` 与 `NPCAutoRoamController` 仍各自保留完整私有导航主循环，旧私有闭环没有真正下线。
  - `NavigationLiveValidationRunner` 仍主要提供功能线 probe 证据，不足以证明真实体验线通过。
- 父层当前判断：
  1. `S0` 只算文档层完成。
  2. `S1/S2/S4` 只能判部分完成。
  3. `S3/S5/S6` 不能判通过。
  4. 用户当前“没有验收到任何内容、看起来像没改”的反馈，和当前代码现实是一致的，不应被历史 probe 绿灯掩盖。
- 父层当前恢复点：
  - 后续若继续导航验收，必须沿这条新事实基线继续：
    - 不再把 probe 绿灯当底座闭环
    - 不再把接口统一当语义统一
    - 先盯旧私有导航闭环是否真实下线，再谈 `S7/S8`

### 会话 28 - 2026-03-24

- 子工作区 `导航检查` 本轮已基于硬审核结论补发新的彻底复工指令：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-8.md`
- 父层新增稳定事实：
  - 当前对子工作区的口径已进一步收紧为：
    - 不再接受“方向没错 / 有新骨架 / probe 过线”作为缓冲
    - 必须按“`S0-S6` 仍未闭环、体验线未交卷”的事实继续施工
  - `002-prompt-8` 已明确要求导航线程把 4 件事真正做出来：
    1. 旧私有导航闭环下线
    2. 交通裁决真正前置
    3. 统一运动执行语义真正成立
    4. 功能线之外的体验线证据补齐
- 父层当前恢复点：
  - 后续若继续导航线，默认不再用“继续优化”口径；
  - 直接沿 `002-prompt-8` 的未交卷复工口径推进，并继续把 `S7/S8` 后延。

### 会话 29 - 2026-03-24

- 子工作区 `导航检查` 本轮已补发：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-9.md`
- 父层新增稳定事实：
  - 上一轮导航线程并非完全中断，而是把外部 compile blocker 当成了合法停车位；
  - 这轮 prompt 已明确纠偏：
    - `SpringDay1WorkbenchCraftingOverlay.cs` 只允许作为 `external_blocker_note`
    - 不再允许作为导航主线的收口理由
  - 当前对子工作区的下一步要求已进一步收紧成：
    - 继续导航结构施工
    - 至少交出一个真实“退壳 checkpoint”
    - 用代码责任迁移，而不是验证恢复叙事，来证明主线继续推进
- 父层当前恢复点：
  - 后续若继续导航线，优先看它是否真的让某一组私有导航责任从控制器迁到底座共享层；
  - 若再次停在 compile blocker，应视为未按 prompt 执行。

### 会话 29 - 2026-03-24

- 子工作区 `导航检查` 本轮已按 `002-prompt-8` 真正落了一次施工刀，不再只停留在解释与审稿：
  - 修改了 `NavigationTrafficArbiter.cs`
  - 修改了 `NPCMotionController.cs`
  - 修改了 `NavigationAvoidanceRulesTests.cs`
- 父层新增稳定事实：
  1. 当前玩家面对 sleeping / inactive blocker 的永久 `Wait`，已经被收口到一个明确机制：
     - `Wait` 锁态在正净空下不再允许无条件续杯；
     - 当前已允许释放到 `SidePass / Recover` 路径。
  2. `S5` 本轮也有实质推进：
     - NPC 最终运动执行开始优先使用 `rb.linearVelocity`
     - 不再默认落在 `MovePosition(...)` 这一套独立语义里。
  3. 这轮新增的两条编辑器测试已经写入导航测试文件，但 fresh 运行态验证尚未拿到，因为 shared root 当前被他线文件编译红错阻断。
- 父层当前判断：
  - 这轮不是闭环完成，但属于 `002-prompt-8` 口径下的有效 checkpoint；
  - 当前唯一外部 blocker 是：
    - `Assets\\YYY_Scripts\\Story\\UI\\SpringDay1WorkbenchCraftingOverlay.cs`
    - 它阻断了导航线本轮 fresh compile / live 终验。
- 父层当前恢复点：
  - 下一轮若继续导航施工，应从“Wait 锁态已松、NPC 速度语义已收一步”继续；
  - 不要回头再从 tag / NavGrid 参数 / NPC preserve 半刀重新猜。

### 会话 30 - 2026-03-24

- 子工作区 `导航检查` 本轮先处理了本线程自引入的编译阻塞：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`
- 父层新增稳定事实：
  1. `NavigationAvoidanceRulesTests.cs` 中对 `NPCMotionController` 的直接强类型引用已被移除，测试重新回到反射式挂载与调用。
  2. Unity MCP 静态校验显示：
     - `NavigationAvoidanceRulesTests.cs`：`0 error`
     - `NPCMotionController.cs`：`0 error`
     - `NavigationTrafficArbiter.cs`：`0 error`
  3. fresh compile 里已不再出现该测试文件的 `CS0246`；shared root 当前剩余显式 blocker 仍是：
     - `Assets\\YYY_Scripts\\Story\\UI\\SpringDay1WorkbenchCraftingOverlay.cs`
- 父层当前判断：
  - 这轮对子工作区来说是“先清自己引入的红错，再回到主线”的必要 checkpoint；
  - 由于外部 UI 文件仍处于语法红态，导航线本轮仍不能伪装成 fresh compile / live 已恢复。
- 父层当前恢复点：
  - 下一轮继续导航线时，不需要再处理这条测试 `CS0246`；
  - 直接从“我的测试 blocker 已清，外部 compile blocker 仍在”继续推进 `S0-S6`。

### 会话 31 - 2026-03-24

- 子工作区 `导航检查` 本轮没有再把外部 compile blocker 当停车位，而是继续做了一个真实结构 checkpoint：
  - 将 `BuildPath / RebuildPath / ActualDestination / 路径后处理` 这组责任继续迁向共享 `NavigationPathExecutor2D`
- 父层新增稳定事实：
  1. `NavigationPathExecutor2D` 现在开始真实持有“路径实际终点”的执行状态：
     - 新增 `HasDestination`
     - 新增 `TryRefreshPath(...)`
     - 新增 `GetResolvedDestination(...)`
  2. `PlayerAutoNavigator` 已少养一层私货：
     - 删除私有 `resolvedPathDestination / hasResolvedPathDestination`
     - 删除私有 `SmoothPath()` / `CleanupPathBehindPlayer()`
     - `BuildPath()` 改为共享刷新入口
  3. `NPCAutoRoamController` 已少养一层私货：
     - `DebugMoveTo()` / `TryBeginMove()` / `TryRebuildPath()` 都改接共享刷新入口
     - 不再各自手写 `TryBuildPath -> ActualDestination` 回填链
  4. 测试也已同步到这条新责任链：
     - `NavigationAvoidanceRulesTests` 现在直接校验 `TryRefreshPath(...)` 后 `HasDestination/Destination`
- 父层当前判断：
  - 这轮属于 `002-prompt-9` 要求的真实“退壳 checkpoint”；
  - 但它只解决了路径请求责任簇，`stuck/repath`、`detour lifecycle`、`先裁决后求解` 仍未完成。
- 父层当前恢复点：
  - 下一轮继续导航线时，应优先再拆一组：
    - `stuck/repath`
    - 或 `detour create/clear/recover`
  - external blocker 只保留备注，不再作为导航主线停工叙事。

### 会话 32 - 2026-03-24

- 子工作区 `导航检查` 本轮已基于最新回执补发：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-10.md`
- 父层新增稳定事实：
  - 最新回执已被正式承认为一个真实退壳 checkpoint：
    - `BuildPath / RebuildPath / ActualDestination / 路径后处理`
    - 已开始由共享 `NavigationPathExecutor2D` 接手
  - 但这不再是下一轮的主刀；当前对子工作区的新要求已经收紧成：
    - 继续拆 `stuck / repath / 恢复入口`
    - 继续验证共享执行层是不是开始成为这组语义的真 owner
  - 新 prompt 已明确禁止三种偏差：
    1. 停在轻责任簇上重复盘旋
    2. 再拿 external blocker 当停车位
    3. 只做 wrapper，不做 owner 迁移
- 父层当前恢复点：
  - 后续若继续导航线，默认直接沿 `002-prompt-10` 推进；
  - 审稿重点继续从“是否真的退掉更重私有闭环”出发，而不再回到轻责任簇讲故事。

### 会话 33 - 2026-03-24

- 子工作区 `导航检查` 本轮已按 `002-prompt-10` 继续施工，没有再停在“路径请求责任簇”上：
  - 这次主刀落在共享 `stuck / repath / 恢复入口`
- 父层新增稳定事实：
  1. `NavigationPathExecutor2D` 现在开始真实持有 stuck/repath 的 owner 状态：
     - `LastRepathTime`
     - `LastRecoveryTime`
     - `LastRecoveryDistance`
     - `LastRecoverySucceeded`
  2. `NavigationPathExecutor2D` 新增共享入口 `TryHandleStuckRecovery(...)`，并在内部统一处理：
     - stuck 判定
     - repath cooldown
     - 共享刷新路径
     - 恢复结果写回
  3. 玩家 / NPC 控制器已不再直接调用 `EvaluateStuck(...)`：
     - 玩家 `CheckAndHandleStuck()` 改吃共享 `StuckRecoveryResult`
     - NPC `CheckAndHandleStuck()` 改吃共享 `StuckRecoveryResult`
  4. 新增两条测试已锁住这次 owner 迁移：
     - 共享恢复成功时写回 owner 状态
     - 共享 repath cooldown 生效时阻断恢复
- 父层当前判断：
  - 这轮属于第二个真实退壳 checkpoint，不是 wrapper 换皮；
  - 但 `arrival/cancel/path-end`、`detour lifecycle`、`先裁决后求解` 仍未完成。
- 父层当前恢复点：
  - 下一轮继续导航线时，优先接着拆：
    - `arrival / cancel / path-end`
    - 或 `detour create / clear / recover`
  - 不需要再回头质疑“stuck/repath 是否还在各自控制器私判”。

### 会话 34 - 2026-03-24

- 子工作区 `导航检查` 本轮已基于最新回执补发：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-11.md`
- 父层新增稳定事实：
  1. 最新回执已被正式承认为第二个真实退壳 checkpoint：
     - `stuck / repath / 恢复入口`
     - 已开始由共享 `NavigationPathExecutor2D` 接手
  2. 但这不再允许线程继续在两个候选下一步之间摇摆；
     - 当前对子工作区的新要求已经收紧成：
       - 继续拆 `detour create / clear / recover`
       - 继续验证共享执行层是不是开始成为 detour lifecycle 的真 owner
  3. 新 prompt 已明确禁止三种偏差：
     - 再在 `arrival / cancel / path-end` 和 `detour lifecycle` 之间摇摆
     - 再拿 external blocker 当停车位
     - 只做 wrapper，不做 detour lifecycle owner 迁移
- 父层当前恢复点：
  - 后续若继续导航线，默认直接沿 `002-prompt-11` 推进；
  - 审稿重点继续从“detour 生命周期是否真实退壳”出发，而不再回到 stuck/repath 本身。

### 会话 35 - 2026-03-25

- 子工作区 `导航检查` 本轮已把 `002-prompt-11` 的唯一主刀落成代码：
  - `PlayerAutoNavigator / NPCAutoRoamController` 的 `detour create / clear / recover`
  - 已开始迁成共享 `NavigationPathExecutor2D` owner
- 父层新增稳定事实：
  1. 这轮可以接受为第三个真实退壳 checkpoint，不再只是 wrapper：
     - 共享执行层现在持有 detour candidate 解析、clear 时间戳与恢复入口
     - 玩家 / NPC 控制器已少掉私写 candidate loop 与直接 override waypoint 操作
  2. fresh compile 已回正，当前不再是 external compile blocker 叙事；
     - targeted detour owner 测试两条都 pass
  3. 但单场 `PlayerAvoidsMovingNpc` fresh live 仍失败：
     - `pass=False / timeout=6.50 / minClearance=0.526 / playerReached=False / npcReached=False`
     - 当前第一责任点已收缩到 shared detour clear/recover 触发仍过密，导致 live 中反复 `clear -> recover -> rebuild`
- 父层当前恢复点：
  - 下一轮继续导航线时，不必再回结构层，也不必再等 compile/live；
  - 直接锁共享 detour clear hysteresis / cooldown / owner 释放条件，先把 `PlayerAvoidsMovingNpc` 从 timeout 拉出来。

### 会话 36 - 2026-03-25

- 子工作区 `导航检查` 本轮已把共享 detour clear/recover 的节制条件真正落地：
  - `NavigationPathExecutor2D.TryClearDetourAndRecover()` 新增 clear hysteresis + recovery cooldown
  - 玩家 / NPC traffic clear 分支都改为把节制阈值交给共享 owner
- 父层新增稳定事实：
  1. 这轮不再只是“失败形态收窄”，而是单场 `PlayerAvoidsMovingNpc` fresh live 已过线：
     - `pass=True / minClearance=0.385 / playerReached=True / npcReached=True / timeout=3.13`
  2. live 证据说明 clear/recover 风暴已被压住：
     - 前段多次 `detour_clear_hysteresis` skip
     - 中段仅出现一次成功 `Recovered=True`
     - 随后玩家完成 `SidePass / Yield / Proceed` 到达
  3. 当前 external blocker note 变更为：
     - `Assets\Editor\Story\SpringDay1BedSceneBinder.cs` 的 `CS0104 Debug` 冲突
     - 但这次 targeted tests 与单场 live 已继续跑通，没有再成为导航停车位
- 父层当前恢复点：
  - detour clear/recover 过密这刀已过线；
  - 后续导航治理若继续推进，应回到剩余 old fallback / private loop 的退壳，而不是重打本轮已过线责任点。

### 会话 37 - 2026-03-25

- 子工作区 `导航检查` 本轮未再扩散责任簇，也没有新增代码改动；
  - 只补了 `002-prompt-13` 要求的三场同轮 fresh live 验证。
- 父层新增稳定事实：
  1. 当前 detour clear / recover 节制已不只是“单场 `PlayerAvoidsMovingNpc` 过线”；
     - 三条核心 probe 同轮 fresh 全绿：
       - `PlayerAvoidsMovingNpc pass=True`
       - `NpcAvoidsPlayer pass=True`
       - `NpcNpcCrossing pass=True`
  2. 当前这版节制条件没有把另外两条 probe 打回归；
  3. 本轮中途出现过“脏 Play 会话 / runner 被外部退出打断”的无效现场，但线程最终重新取到了真正 fresh 的三场结果，没有把无效证据混进最终结论。
- 父层当前恢复点：
  - 当前 detour 节制簇 checkpoint 已从“单场过线”升级为“三场同轮 fresh 无回归”；
  - 后续若继续导航治理，应在保住这条基线的前提下，再回到剩余 old fallback / private loop 的退壳。

### 会话 38 - 2026-03-25

- 子工作区 `导航检查` 本轮没有新增代码或 live 施工；
  - 只按 `002-prompt-13` 固定口径完成收束回执补记。
- 父层新增稳定事实：
  1. 当前 detour clear / recover 节制簇仍维持三场同轮 fresh 全绿结论，无新增回归信号；
  2. 本轮只是复核并确认：
     - `PlayerAvoidsMovingNpc`
     - `NpcAvoidsPlayer`
     - `NpcNpcCrossing`
     三条结果已足够作为当前 checkpoint 基线。
- 父层当前恢复点：
  - 后续如继续导航治理，应等待用户明确下一责任簇；
  - 当前不再重跑已完成的 detour 节制簇。

### 会话 39 - 2026-03-25

- 子工作区 `导航检查` 本轮新增的是一次“代码与设计对账复盘”，不是继续施工。
- 父层新增稳定事实：
  1. `006/007` 作为目标架构方向仍成立，不应整份推翻；
  2. 当前真正的问题不是“用户把线程压去错误设计”，而是线程自己把过渡层 owner 迁移过早包装成架构闭环；
  3. 当前仍未完成的硬缺口继续固定为：
     - `先裁决、后求解` 未真正落地
     - `NavigationLocalAvoidanceSolver` 仍是启发式壳层系统
     - 玩家 / NPC 旧私有导航主循环未真正退干净
     - 统一运动学只收了命令边界，未完全收口语义
- 父层当前恢复点：
  - 后续若继续导航治理，应以这次复盘作为新的诚实基线；
  - 不再允许“骨架前进 = 已经交卷”的口径回潮。

### 会话 40 - 2026-03-25

- 子工作区 `导航检查` 本轮新增的是一次“代码、设计、验收口径”的二次审视，不是继续写新结构。
- 父层新增稳定事实：
  1. 导航线当前最关键的失败已进一步收窄为：
     - 不是目标设计全错；
     - 而是 `S2 / S5 / S6` 仍未在代码与真实入口体验上真正闭环。
  2. 当前必须继续坚持的诚实口径：
     - 共享 owner / probe baseline 可以记成绩；
     - 但不能再把它们包装成“专业导航底座已完成”。
  3. 当前还新增一条流程风险：
     - `导航检查/memory.md` 记载了“`002-prompt-14.md` 已落盘”，但磁盘现场无该文件；
     - 后续治理不能再让 memory 先于真实文件闭环。
- 父层当前恢复点：
  - 若继续导航治理，应把“真实入口体验闭环”继续压在结构 checkpoint 之前；
  - 不再允许以缺失的 prompt 文件或 memory 自述代替真实现场。

### 会话 41 - 2026-03-25

- 子工作区 `导航检查` 本轮没有继续顺着旧口径写 `002-prompt-14` 的幽灵结论，而是基于最新用户手测重判主线。
- 父层新增稳定事实：
  1. 当前导航已不只是“结构成立、体验未闭环”，而是进入了真实点击体验回归事故态：
     - 用户现场明确反馈“保护罩 / 推挤 / 被围抽搐”，并判定当前体感比旧基线更差；
  2. 子工作区已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-15.md`
     作为当前唯一有效续工文件；
  3. 子工作区也已把一条治理脱节写实钉死：
     - `memory.md` 曾写“`002-prompt-14.md` 已落盘”，但磁盘现场无该文件；
     - 这轮起缺失的 `002-prompt-14` 不再作为有效依据继续外推；
  4. 当前 live `git status` 里 `NavigationTrafficArbiter.cs / NavigationMotionCommand.cs` 仍为 untracked，说明导航骨架本身是否真正进基线仍未收干净。
- 父层关键裁定：
  1. 后续导航治理的优先级已变成：
     - 先恢复真实点击入口下至少不比旧基线更差的体验；
     - 再谈 `S2/S3/S5/S6` 的长期闭环；
  2. 线程被允许在导航线内自主判断：
     - `selective rollback`
     - `selective restore`
     - `forward fix`
     但不允许继续把当前更差的体感留在现场。
- 父层当前恢复点：
  - 若继续导航治理，下一刀必须先把“最坏回归”压掉，而不是继续把结构成绩外推成体验进展；
  - 导航这条线现在的硬门槛已经从“有没有更多共享 owner”改成“真实点击是否至少恢复到可接受基线”。

### 会话 42 - 2026-03-25

- 子工作区 `导航检查` 本轮已按 `002-prompt-15` 从“结构主线续工”切成“回归事故处理”。
- 父层新增稳定事实：
  1. 导航线程这轮明确选择了 `selective restore`：
     - 把 runtime 从未提交的 `TrafficArbiter + MotionCommand` 新链撤回到旧的 solver 直出链；
     - 同时正式撤回了：
       - `Assets/YYY_Scripts/Service/Navigation/NavigationTrafficArbiter.cs`
       - `Assets/YYY_Scripts/Service/Navigation/NavigationMotionCommand.cs`
  2. 最坏体验回归已经被压缩：
     - 真实点击入口 `RealInputPlayerAvoidsMovingNpc` 重新过线；
     - `pushDisplacement=0.000 / minClearance=0.388 / playerReached=True / npcReached=True`
     - 说明“玩家推着 NPC 走”这条最痛现场已经不再成立。
  3. 另外两条 NPC 场景没有过线，但失败形态继续收窄：
     - `NpcAvoidsPlayer pass=False / minClearance=0.582 / npcReached=False`
     - `NpcNpcCrossing pass=False / minClearance=0.514 / npcAReached=False / npcBReached=False`
     - 当前主形态已不再是 `保护罩 / 推挤`，而是 NPC 自身提前停摆、未完成到达。
  4. 子工作区新的第一责任点已被钉到：
     - `NPCAutoRoamController.TickMoving / CheckAndHandleStuck / TryRebuildPath`
     - `NPCMotionController` 的到达/停止语义
- 父层关键裁定：
  - 这轮可以接受为“最坏回归已压掉，但导航仍未真正交卷”；
  - 后续若继续导航治理，不准再回去争论 `TrafficArbiter` 架构，也不准再把问题描述成“还是推土机”；
  - 应直接围绕 NPC 提前停摆这条更窄责任点推进。
- 父层当前恢复点：
  - 导航线当前真实状态是：
    - 玩家真实点击入口已回到至少不比旧基线更差；
    - NPC 运行终点语义仍未恢复；
  - 下一刀应继续留在导航工作区，锁 NPC runtime 到达簇，不再重开大架构话题。

### 会话 43 - 2026-03-25

- 子工作区 `导航检查` 本轮把 `002-prompt-15` 的事故处置又往前收了一步，但没有改主线。
- 父层新增稳定事实：
  1. 上一轮白名单 sync 里那组 “`NavigationPathExecutor2D` 缺少 `TryRefreshPath / GetResolvedDestination`” 并不是导航 runtime 又退坏了，而是 `CodexCodeGuard` 的编译视角和 working tree 视角不一致：
     - 漏白名单的 dirty `.cs` 文件会按 `HEAD` 编译；
     - 导航线程这轮已确认需要把 `NavGrid2D.cs` 和 `NavigationPathExecutor2D.cs` 一起纳入 owned 白名单，才能正确评估当前导航热区。
  2. 子工作区已把最后一个 owned blocker 清成 editor-only 调试字段：
     - `NPCAutoRoamController.drawDebugPath` 现已收进 `#if UNITY_EDITOR`
     - 完整 owned 白名单下 `CodexCodeGuard` 已通过。
  3. 子工作区又补了一轮最小 real-input 复验：
     - `RealInputPlayerAvoidsMovingNpc`
     - `pass=True / minClearance=0.383 / pushDisplacement=0.000 / timeout=5.43`
     - 说明“玩家推着 NPC 走”这条最坏回归在当前源码截面上仍已压掉。
  4. 导航线程把“最后一个至少不更差基线”进一步钉实为：
     - `4613255c`
     - 代表旧 solver 直出链而非 `TrafficArbiter / MotionCommand` runtime 接线状态。
- 父层关键裁定：
  - 当前导航线可以接受为“真实点击入口最坏回归已压掉，并且当前 owned 代码已能自洽编译收口”；
  - 但父层对导航的未完成判断不变：
    - `NpcAvoidsPlayer / NpcNpcCrossing` 的 NPC 提前停摆仍是下一刀主责任点。
- 父层当前恢复点：
  - 若导航继续施工，下一刀必须继续留在 NPC 到达/停止语义；
  - 不需要再回头证明“是不是还在推着走”，也不需要再扩成新的结构迁移叙事。

### 会话 44 - 2026-03-25

- 子工作区 `导航检查` 这轮新增的不是顺着旧收口继续走，而是一次明确的“用户现场打回后重判”。
- 父层新增稳定事实：
  1. 用户最新复测已经推翻上一轮“最坏回归已压掉”的对外口径；
  2. 当前导航线能保留的上一轮成果，只剩：
     - runtime 已回到 `4613255c` 语义的旧 solver 直出链；
     - `TrafficArbiter / MotionCommand` 已撤出运行时；
  3. 子工作区已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-16.md`
  4. `002-prompt-16` 已把下一刀从“NPC 提前停摆簇”重新拉回用户当前仍能肉眼看见的 real-input 坏体验：
     - `保护罩`
     - `很远就停`
     - `被围抽搐`
  5. 父层当前不再接受：
     - 用旧轮结果顶 fresh 复跑
     - 用 runner 数值单独封印用户体感
- 父层当前恢复点：
  - 导航这条线现在的诚实状态是：
    - 基线已退回旧 solver 直出链；
    - 但 real-input 体验仍被用户判定为失败。
  - 后续导航治理应先压掉用户当前仍在骂的可见坏体验，再谈更窄的责任簇。

### 会话 45 - 2026-03-25

- 子工作区 `导航检查` 这轮没有回漂大架构，而是继续只打 `002-prompt-16` 指定的 runtime 热区。
- 父层新增稳定事实：
  1. 导航线程继续收缩了当前最像“保护罩”的运行时来源：
     - `NavigationAvoidanceRules.GetInteractionRadius(...)` 已从 avoidance-radius 叠壳，改成 collider-first + 小壳层 cap；
     - `NavigationLocalAvoidanceSolver` 里 sleeping/stationary blocker 与 moving yield 的 clearance / slowdown / repath 阈值被继续收紧。
  2. 本轮没有拿旧 live 顶账，反而先把 fresh live 触发链重新核了一遍；
     - 当前阻塞不是“导航线程又不肯跑”，而是 shared root 现有外部 compile blocker 让 fresh Play 窗口不可用。
  3. 当前直接挡住 same-round fresh live 的外部错误是：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs` 的 `PageRefs` 缺失
     - 刷新时还出现过 `Assets/YYY_Tests/Editor/NPCToolchainRegularizationTests.cs` 的外部缺类型错误
  4. 导航线程源码里新增的 `NavigationLiveValidationMenu` 排队起跑逻辑目前还没被 Unity 编译采纳；
     - 从现场回读看，菜单仍命中旧逻辑，只会提示“请先进入 Play Mode”
- 父层关键裁定：
  - 这轮可以接受为“继续推进了热区修复，但 fresh live 证据仍未补齐”；
  - 当前不能再把状态说成“只剩 NPC 提前停摆”，因为 `002-prompt-16` 这轮要求的 real-input fresh 结果还没拿到。
- 父层当前恢复点：
  - 若导航继续施工，下一步不是回漂架构，而是：
    1. 在 shared root 拿到可进 Play 的编译窗口
    2. 立即补齐 `002-prompt-16` 规定的 5 组 fresh live
    3. 再判断“保护罩 / 很远就停 / 被围抽搐”是否真正被压掉

### 会话 46 - 2026-03-26

- 子工作区 `导航检查` 本轮没有再去泛调 solver，而是按用户要求做了一次“执行层握手链路开颅审计”。
- 父层新增稳定事实：
  1. `NavigationLocalAvoidanceSolver` 当前已经能产出 `ShouldRepath + SuggestedDetourDirection`，但玩家/NPC controller 并没有稳定把它保活成 detour owner；
  2. 玩家侧当前最致命的吞意图点是：
     - `HandleSharedDynamicBlocker()` 在 `!ShouldRepath`、cooldown、detour 创建失败这三处都会直接掉回普通执行或 `BuildPath()`；
     - solver 某一帧失去 blocker 时，还会经 `ClearOverrideWaypointIfChanged()` 把刚有机会落地的 detour 清掉；
  3. NPC 侧当前最致命的吞意图点是：
     - `TryHandleSharedAvoidance()` 在真正创建 detour 前就 `StopForSharedAvoidance()`；
     - detour 失败后立即 `TryRebuildPath()`，没有共享 owner 保活窗口；
  4. `NavigationPathExecutor2D.TryClearDetourAndRecover()` 虽然存在，但当前 runtime controller 没有调用点，因此 detour clear/recover API 还没成为现行运行闭环。
- 父层当前恢复点：
  - 导航线下一刀不应再漂回“调 solver 参数”；
  - 应直接留在 `PlayerAutoNavigator / NPCAutoRoamController / NavigationPathExecutor2D` 的 detour owner 接管与保活链。

### 会话 47 - 2026-03-26

- 子工作区 `导航检查` 这轮没有继续施工，而是按最新委托做了“能否进入下一代交接”的状态确认。
- 父层新增稳定事实：
  1. 当前导航线主叙事已经稳定收敛到：
     - `ShouldRepath` 已出现
     - detour owner 没有稳定接管执行层
  2. 当前 single first blocker 已足够单一，不再是多路径怀疑并行；
  3. 当前 own / non-own / hot-file 边界已足够清楚，可以无失真交给下一代线程；
  4. 子工作区已正式生成：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\V2交接文档\`
     下的 7 份 `导航检查V2` 重型交接文件。
- 父层当前恢复点：
  - 导航线当前可以从“本代收束”切换到“下一代接手”；
  - 后续若继续导航，不应再要求当前线程回去补新的参数实验，而应让 `导航检查V2` 直接从执行层 detour owner 闭环继续。

### 会话 48 - 2026-03-26

- 子工作区 `导航检查` 本轮没有继续推进导航业务，只补报了“本轮真实遵守/执行的最新文档入口与落点”。
- 父层新增稳定事实：
  1. 当前实际被执行的最新输入源已明确包括：
     - 全局 `AGENTS.md`
     - Sunset 项目 `AGENTS.md`
     - 最新交接前状态确认委托
     - 最新有效续工入口 `002-prompt-17.md`
     - 当前线程/工作区/父工作区记忆
     - 当前统一交接写作 prompt
  2. 当前实际输出落点继续固定为：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\V2交接文档\`
- 父层当前恢复点：
  - 后续若再问导航交接依据，可直接沿上述规则/委托/记忆链回溯，不需要再重新口述来源。

### 会话 49 - 2026-03-26

- 子工作区 `导航检查` 本轮没有继续导航实现，而是对 `005-genimi锐评-4.0.md` 走了正式审核路线。
- 父层新增稳定事实：
  1. 该锐评不是“完全错误”，但其若干绝对化处方与 `006/007` 上位设计直接冲突；
  2. 子工作区已正式生成：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-genimi锐评-4.0审视报告.md`
  3. 子工作区本轮还主动纠正了一处自身旧口径：
     - `TryClearDetourAndRecover()` 当前并非完全未接入 runtime，而是已在 release 分支部分接入；
     - 当前真正问题应收束为“detour owner 仍未稳定统治执行层”。
- 父层当前恢复点：
  - 后续若继续导航，不应把 Gemini 4.0 锐评当成直接施工蓝图；
  - 应以 `006/007 + 审视报告 + V2 交接包` 作为后续判断依据。

### 会话 48 - 2026-03-26

- 子工作区 `导航检查` 这轮没有继续业务推进，而是补做了一次“最新遵守与执行文档路径”的自我审视。
- 父层新增稳定事实：
  1. 当前导航线实际遵守的最新入口已经明确分成两层：
     - 规则/委托/记忆入口
     - `导航检查V2` 的 7 份正式交接正文
  2. 这意味着下一代线程接手时，不只知道代码热区，还知道自己应该以哪些文档为现行依据。
- 父层当前恢复点：
  - 导航线当前已完成“可交接”与“交接依据路径报实”两步；
  - 后续若继续，应由 `导航检查V2` 以这些入口为准继续。

### 会话 48 - 2026-03-26

- 子工作区 `导航检查`（V2 首轮）这轮没有回漂 solver / Scene，而是只在 `PlayerAutoNavigator / NPCAutoRoamController / NavigationPathExecutor2D` 上补了 detour owner 最小保活闭环。
- 父层新增稳定事实：
  1. direct fallback detour 现在也会写 `LastDetourCreateTime / LastDetourOwnerId / LastDetourPoint`，玩家侧 no-blocker release 与 NPC 侧 release 都已接到 `TryClearDetourAndRecover(..., rebuildPath:false)`；
  2. 玩家/NPC 的旧 stuck/rebuild 在 detour 创建后 `0.35s` 保护窗内不再抢跑；
  3. fresh live `RealInputPlayerAvoidsMovingNpc` 当前结果已到 `pass=True / pushDisplacement=0.000 / playerReached=True / npcReached=True`，且 Unity 已退回 `Edit Mode`。
- 父层当前恢复点：
  - 导航线当前单一第一进展已从“owner 被瞬时吞掉”前推到“owner 保活最小闭环已接上并拿到首个有效窗口”；
  - 后续若继续，只应在同一 detour owner 闭环上补更直接的命中证据或扩第二组 fresh live，不要回漂旧 solver 泛调或 `Primary.unity`。

### 会话 50 - 2026-03-26

- 子工作区 `导航检查` 本轮继续执行 `005-genimi锐评-4.0` 审核收口，但不改业务代码，只做并行复核与审视报告补记。
- 父层新增稳定事实：
  1. 已完成“主审 + v1 并行审查”双视角交叉核对，结论一致维持 `Path C`；
  2. 子工作区已更新：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-genimi锐评-4.0审视报告.md`
     并新增“与 v1 并行审查一致性回执”段落；
  3. 当前更稳妥口径仍是：
     - 吸收锐评的问题意识
     - 不直接执行其绝对化重构处方
     - 继续按 `006/007 + V2 交接包` 分阶段推进。
- 父层当前恢复点：
  - 后续导航审查或续工，应以已更新审视报告作为“是否采纳锐评方案”的前置裁定文档；
  - 在用户明确拍板前，不把 `005-genimi锐评-4.0.md` 直接当实现指令。

### 会话 51 - 2026-03-26

- 子工作区 `导航检查` 本轮按 `2026-03-26-导航检查V2共享根大扫除与白名单收口-04.md` 执行 cleanup，不做导航业务验证。
- 父层新增稳定事实：
  1. 导航线 own dirty / untracked 已完成一轮白名单收口，提交 SHA：
     - `12ce0814`
  2. 本轮明确未触碰禁区：
     - `Primary.unity`
     - `GameInputManager.cs`
     - `TagManager.asset`
     - `项目文档总览` 尾账
  3. 收口过程中出现一次真实阻塞并已最小化处理：
     - 代码闸门因 `NavigationLiveValidationRunner.cs` 强依赖 `DebugIssueAutoNavClick` 失败；
     - 已在导航 own 文件内改为反射调用 + fallback 后通过闸门并完成 sync。
- 父层当前恢复点：
  - 父层仍有大量 foreign dirty，不是全局 clean；
  - 但导航线这轮“认领-清扫-白名单收口”目标已完成，可转入下一步裁定或停等新委托。

### 会话 52 - 2026-03-26

- 子工作区 `导航V2` 本轮首次建立，并按用户要求只做“两份 Gemini 锐评审核 + 自我审视”，没有继续导航业务实现。
- 父层新增稳定事实：
  1. `导航V2` 当前主要输入文件是：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\000-gemini锐评-1.0.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\000-gemini锐评-1.1.md`
  2. 当前审核结论分流为：
     - `000-gemini锐评-1.0.md` -> `Path B`
     - `000-gemini锐评-1.1.md` -> `Path C`
  3. 子工作区已正式生成：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\000-gemini锐评-1.1审视报告.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\memory.md`
  4. 父层本轮新增的最重要自省结论不是“Gemini 说错了什么”，而是：
     - 我不能再把“很锋利的架构诊断”直接偷换成“当前切片就该这么做”；
     - Sunset 当前仍应以 `006/007 + 当前 live 委托 + 真实代码热区` 为施工依据。
- 父层当前恢复点：
  - `导航V2` 可以继续作为“审核锐评 / 收口自省”的子工作区存在；
  - 后续若进入导航实现判断，不应把 `000-gemini锐评-1.1.md` 直接升格成上位施工蓝图。

### 会话 53 - 2026-03-26

- 子工作区 `导航V2` 本轮没有被批准直接进入导航实现，而是接受了一轮更窄的治理回拉。
- 父层新增稳定事实：
  1. 当前对 `导航V2` 的正式裁定是：
     - `继续发 prompt`
     - 但只允许继续做“开工准入与边界收口”，不允许直接开实现；
  2. `000-gemini锐评-1.1.md -> Path C` 的方向判断可以保留；
  3. `000-gemini锐评-1.0.md -> Path B` 目前仍缺正式边界补记，因此还不能直接作为“可吸收后就开工”的依据；
  4. `导航V2` 上一轮回执对线程记忆的报实存在边界混淆：
     - 现场实际存在 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
     - 但它回执把“线程记忆”写成了父线程 `导航检查/memory_0.md`
  5. 为此已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\001-导航V2开工准入与边界收口-01.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\000-gemini锐评-1.0边界补记.md`
- 父层当前恢复点：
  - 在 `1.0` 边界、线程记忆纠偏、开工准入条件三件事收稳之前，`导航V2` 仍停留在“审核 / 认知收口”阶段；
  - 下一轮只等 `导航V2` 按新 prompt 交最小回执，再决定是否允许进入实现施工。
