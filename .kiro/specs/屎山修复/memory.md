# 屎山修复 - 工作区记忆

## 2026-03-29（导航检查父线程复审 `V2 -15`：旧 blocker 已失效，动态线改发 `-16` 回到 fresh + 中心语义收口）

- 当前主线目标：
  - 导航主线继续由父线程做审回执与分发，`导航检查V2` 继续负责动态实现线；这轮的关键不是继续讨论旧 blocker，而是把 `V2` 从 stale blocker 停车位拉回到同一条 `PlayerAutoNavigator.cs` 完成语义链。
- 本轮已完成事项：
  1. 严格复审 `V2` 最新 `-15` 回执，确认：
     - scope 仍锁在 `PlayerAutoNavigator.cs`
     - 但“`SpringUiEvidenceMenu.cs` compile blocker 所以没法 fresh”已经过时
  2. 父线程现场事实：
     - `SpringUiEvidenceMenu.cs` 当前已补 `using FarmGame.Data;`
     - `Object.FindFirstObjectByType` 歧义已被 `UnityEngine.Object.FindFirstObjectByType` 消除
     - 当前 console 不再有 compile error，只剩 warning
  3. 正式裁定：
     - 接受 `-15` 为历史 checkpoint
     - 不接受继续拿它当当前停车位
  4. 新建并落盘：
     - `2026-03-29-导航检查V2-PlayerAutoNavigator-完成语义fresh复核与中心语义收口-16.md`
     - `2026-03-29-父线程验收清单-PlayerAutoNavigator-完成语义fresh复核与中心语义收口-16.md`
- 当前稳定结论：
  1. 动态线下一轮必须重新做 fresh compile / fresh live，不能再用旧 blocker 顶账
  2. 下一轮唯一主刀仍只允许在 `PlayerAutoNavigator.cs`
  3. 当前最该继续压窄的方向是：
     - `TryFinalizeArrival / HasReachedArrivalPoint / GetPlayerPosition`
     - 普通地面点导航与跟随交互目标的完成语义分离
- 当前恢复点：
  - 用户若继续放行 `导航检查V2`，直接转发 `-16`
  - 父线程下一轮先审 fresh truth，再审中心语义是否仍混用

## 2026-03-29（导航检查父线程完成全局警匪定责清扫第一轮自查，own 边界收窄为静态工具链与审核 docs）

- 当前主线目标：
  - 全局警匪定责清扫第一轮要求各线程先认 own / foreign / mixed / stale narrative；导航检查父线程这轮不再继续发动态实现 prompt，而是先把自己当前真正 own 的面重新说死。
- 本轮已完成事项：
  1. 导航检查父线程已正式自查并落盘：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\2026-03-29_全局警匪定责清扫第一轮回执_01.md`
  2. 当前边界已冻结：
     - own：`NavigationStaticPointValidationRunner.cs`、`NavigationStaticPointValidationMenu.cs`、父线程审核/分发 docs、父线程/工作区 memory
     - not-own：`PlayerAutoNavigator.cs` 当前动态 runtime active owner、`导航检查V2` memory、`Primary.unity` scene cleanup owner
  3. 父线程对 `Primary.unity` 的口径已降级为：
     - 只报 same-GUID / scene incident 现场
     - 不再冒认 scene cleanup
- 当前稳定结论：
  1. 导航检查父线程当前应被视为“静态 validation / 审核 docs owner”，而不是“动态 runtime owner”；
  2. 第二轮如果正式分配 cleanup，父线程只应继续 active 在自己的静态工具链与审核 docs 清扫上。
- 当前恢复点：
  - 等第二轮 cleanup 是否正式分配；
  - 在那之前，导航检查父线程不继续扩题，不继续静态 live，不继续替动态线裁 owner。
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

### 会话 54 - 2026-03-26

- 子工作区 `导航V2` 本轮最新回执已通过治理复核。
- 父层新增稳定事实：
  1. `000-gemini锐评-1.0.md` 的 `Path B` 边界已经真实写进正文，而不是继续停留在口头判断；
  2. `导航检查V2` 线程记忆边界也已明确纠偏：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
       才是实现线程自己的线程记忆；
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`
       只能继续作为父线程补同步；
  3. 因此 `导航V2` 这条审核支线当前已达到本阶段完成定义，不再继续占用实现入口；
  4. 当前治理正式批准把入口从 `导航V2` 切回 `导航检查V2`，并新增实现委托：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2复工准入后续工委托-06.md`
- 父层当前恢复点：
  - `导航V2` 当前分类为“无需继续发”；
  - `导航检查V2` 下一轮只围绕 `NpcAvoidsPlayer` 的 NPC 侧 fresh 证据继续施工，不回漂大架构争论。

### 会话 55 - 2026-03-26

- 子工作区 `导航检查` 本轮继续做治理总闸，不改导航实现本体，只审 `导航检查V2` 的首条 NPC 侧失败回执。
- 父层新增稳定事实：
  1. `导航检查V2` 这条失败回执有效，但当前仍是 `own clean = no`，因此不能直接把它当“这一刀已闭环”往下跳；
  2. 现场代码证据已把新的第一怀疑点继续收窄到：
     - `NPCAutoRoamController.TickMoving()` 中新增的
       `ClearedOverrideWaypoint -> StopMotion -> rb.linearVelocity = 0 -> return`
       硬停 early-return 链；
  3. 基于这个收窄，已新增续工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer释放硬停收口-07.md`
  4. 当前不另外给 `NPC/NPCV2` 发 cleanup prompt：
     - `Primary.unity` 与 3 个 TMP 字体当前虽 dirty，但仍是 mixed hot 面；
     - 用户刚有 Unity 使用，`Primary.unity` 也处于 `unlocked`；
     - 在 owner 未清前，不把这几项硬判成 `NPC` 线程 own cleanup。
- 父层当前恢复点：
  - `导航检查V2` 下一轮继续打 NPC 侧 release 硬停链，并要求把 own dirty 一并自收口；
  - 若后续 hot 面长期不消，再单独开 `NPC/NPCV2` 的 owner 报实 / hygiene 线程，不和导航主刀混做。

### 会话 56 - 2026-03-26

- 子工作区 `导航检查` 本轮继续做治理总闸，插入式处理 `NPCV2` 的最新汇报，但不改导航实现本体。
- 父层新增稳定事实：
  1. `NPCV2` 在提交 `24886aad` 中修掉 `NPCAutoRoamControllerEditor.cs` 的 `Play Mode -> MarkSceneDirty` 报错，这条汇报成立；
  2. 但这条提交只覆盖：
     - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
     - `NPCV2` 自己的 memory
     - `NPC` 工作区 memory
     不能外推成“当前所有 dirty 都归 NPC 线”；
  3. 当前 dirty 仍需拆 owner：
     - `Assets/000_Scenes/Primary.unity` 最近一次提交触碰来自 `65e1ee35`，但 working tree 仍是 mixed hot 面；
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 当前 dirty 仍归导航线；
     - 3 个 TMP 字体不归这轮 `NPCV2` editor 修复。
  4. 因此这轮不让 `NPCV2` 吞 broad cleanup，只新增极窄委托：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Primary归属报实与最小cleanup委托-06.md`
- 父层当前恢复点：
  - `导航检查V2` 继续围绕 `NPCAutoRoamController` 的 release / 执行链收口；
  - `NPCV2` 如需继续，只允许做 `Primary.unity` own residue 报实与最小 cleanup，不碰导航脚本、TMP 字体或 Unity live 写。

### 会话 57 - 2026-03-26

- 子工作区 `导航检查` 本轮继续做治理总闸，不改运行时代码，只根据用户最新补充把子线程 prompt 重新收紧。
- 父层新增稳定事实：
  1. `NPCV2` 在 `24886aad` 修掉的仍然只是：
     - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
     - `Play Mode -> MarkSceneDirty` 的 Inspector 报错；
     不能被误写成导航 runtime 已修。
  2. 当前 runtime 热点仍在导航子线程：
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 继续 dirty；
     - 因此 `导航检查V2` 的下一轮主刀仍锁执行层握手，不切给 `NPCV2`。
  3. 当前 3 份 dirty `DialogueChinese*` 字体最近提交历史来自 `spring-day1 / spring-day1V2`，不归 `NPCV2`；
     - `NPCV2` 最多只对它们做只读 owner 排除，不允许直接吞 cleanup。
  4. 本轮新增两份续工 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer执行链硬停与握手续工-08.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Editor修复后Primary与字体owner复核-07.md`
- 父层当前恢复点：
  - 导航主线继续维持“父子分线推进”：
    - `导航检查V2` 只打 `NPCAutoRoamController` 执行链；
    - `NPCV2` 只做 `Primary.unity + 字体` 的 owner / hygiene 报实。

### 会话 58 - 2026-03-26

- 用户进一步质疑“是不是已经进入一个新的分支，只剩下分配任务然后让两边继续做”。
- 父层新增稳定事实：
  1. 当前不是新的业务分支，而是父线程把同一主线拆成两个受控子切片：
     - 导航子线继续修 runtime 执行链；
     - NPC 子线继续修 `HomeAnchor` 的 Editor / Inspector 补口链。
  2. cleanup 不是当前主线，只是各线程在自己那一刀完成后必须自带的收尾纪律。
  3. 现在还不能对两条子线说“无事发生，随便继续”；
     - 只能说“各自继续，但严格只在各自刀口里继续”。
- 父层当前恢复点：
  - 父线程继续做总闸与解释，不把 mixed hot 面、runtime 导航、NPC editor 补口再次混成一锅；
  - 后续是否进入导航下一阶段，取决于 `导航检查V2` 的 fresh runtime 结果，而不是当前这轮分派动作本身。

### 会话 59 - 2026-03-26

- 用户进一步要求：不要沿用旧 prompt，要根据两条子线程已经聊出来的最新自述，重新对症下药。
- 父层新增稳定事实：
  1. `导航检查V2` 最新自述已经把问题缩成“先只读钉死第一责任点，再决定是否做最小 runtime 事务”；
  2. `NPCV2` 最新自述则明确表示：当前更该先修“运行中的 `Home Anchor` 还是空”，而不是先做 cleanup。
  3. 因此本轮 prompt 已更新为：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-26-导航检查V2-NpcAvoidsPlayer第一责任点钉死与最小事务-09.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-HomeAnchor运行中补口链续工-08.md`
- 父层当前恢复点：
  - 这轮之后，导航子线先收窄责任点，NPC 子线先恢复 `HomeAnchor` 可用性；
  - owner cleanup 与 mixed hot 面继续退居支撑位，不抢当前最用户可见的主线。

### 会话 60 - 2026-03-26

- 子工作区 `导航V2` 本轮不再只停在锐评审核，而是正式产出了一份面向后续开发的统一文档。
- 父层新增稳定事实：
  1. 已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\002-导航V2开发宪法与阶段推进纲领.md`
  2. 这份文档没有推翻 `006/007`，而是把它们翻译成当前 V2 可长期遵守的开发宪法；
  3. 文档中正式钉死了：
     - 三层文档关系：`006 = 目标蓝图`、`007 = 迁移路线`、`002 = V2 现行开发宪法`
     - V2 的 10 条硬约束
     - 当前阶段快照、accepted checkpoint、当前两条活跃子线
     - `P0 -> P1 -> P2 -> P3` 的发展顺序
  4. 因而 `导航V2` 从此不再只是“审核锐评工作区”，而是同时承担“开发宪法工作区”的职责。
- 父层当前恢复点：
  - 后续如果导航继续推进，除了 `006/007` 外，还应优先以这份 `002` 文档约束 prompt 与阶段判断；
  - 这样后续就不必再只靠聊天共识和散落 memory 推进。

### 会话 61 - 2026-03-26

- 当前主线目标：
  - 继续把导航线从“只靠聊天和 prompt 共识推进”提升到“有足够稳定的文档体系支撑后续施工”，但这轮只做中间讨论，不进入新实现。
- 本轮子任务：
  - 讨论 `导航V2` 现有文档体系是否已经够继续推进，以及是否还缺新的关键文档层。
- 本轮新增稳定事实：
  1. 当前已经形成可继续使用的三层文档基础：
     - `006`：目标蓝图
     - `007`：阶段路线
     - `002`：V2 现行开发宪法
  2. 这三层对当前推进已经够用，已经足以约束：
     - prompt 不能再自造上位法
     - 结构线 / 体验线不能再混说
     - 当前 `P0 -> P1 -> P2 -> P3` 的发展顺序
  3. 但它们还不是永久完备体系；未来真正该补的，不是再来一份泛大设计，而是：
     - “当前代码现实 vs `006/007` 目标”的偏差账本 / 状态图
     - 阶段切换准入与残留旧闭环账本
  4. 因而当前父层统一判断应表述为：
     - 短中期文档体系已经足够继续推进
     - 长期仍需在 `P0` 稳住后补一份状态账本型文档
- 父层当前恢复点：
  - 现阶段继续推进时，统一以上述三层文档为正式依据；
  - 等当前用户可见阻塞稳定后，再判断是否进入“第四类状态账本文档”的产出，而不是现在又回到空转大讨论。

### 会话 62 - 2026-03-26

- 当前主线目标：
  - 导航总主线不变，但这轮父层不碰实现，只负责读取新的 Gemini 2.0 锐评，并给 `导航V2` 下发一轮更窄的审核 / 纠偏委托。
- 本轮子任务：
  - 审 `000-gemini锐评-2.0.md` 是否值得吸收、该吸收到什么程度，以及它应不应该推动 `002` 做局部修订。
- 本轮新增稳定事实：
  1. `000-gemini锐评-2.0.md` 不是纯噪音，它点中了几个值得认真核的点：
     - “状态账本 / 偏差账本”是否不该等到 `P0` 稳住后才出现
     - 不能只靠 markdown 宪法
     - `P0` 退出条件是否需要客观依赖切断指标
  2. 但它也不能直接升格成上位法，因为它存在明显绝对化：
     - 当前仓库并不是“完全没有代码级隔离”，已存在 `INavigationUnit`、共享注册表、共享执行资产
     - 当前也不是“完全没有 Hysteresis 基准”，detour clear hysteresis / recovery cooldown 已有代码与测试
  3. 因而父层本轮最正确的动作不是直接判死刑，也不是直接照单全收，而是给 `导航V2` 发一轮新的审核委托，强制它把 `2.0` 审成对 Sunset 现状负责的结论。
  4. 本轮已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\003-导航V2审核000-gemini锐评-2.0与现行宪法纠偏-01.md`
- 父层当前恢复点：
  - 下一轮 `导航V2` 如继续，应先执行上述 `003` 委托；
  - 在这轮审核回执出来前，不应把 `000-gemini锐评-2.0.md` 直接当成新宪法，也不应提前转成实现委托。

### 会话 63 - 2026-03-27

- 当前主线目标：
  - 继续把导航线的“V2 审核工作区”从交接态推进到真正自治态；这轮只做治理分析，不发新 prompt、不进实现。
- 本轮子任务：
  - 结合 `导航V2` 最新回执、修订后的 `002 v1.1`，以及 `导航检查 -> 导航检查V2` 的 7 份交接正文，判断为什么 `导航V2` 现在还没有真正独立，以及后续该怎样接班。
- 本轮新增稳定事实：
  1. `导航V2` 现在仍更像“局部审核线程 / 文档编辑线程”，还不是“自持上下文的导航规范 owner”。
  2. 当前之所以还在交接态，有三层证据：
     - 最新回执虽完成了 `2.0 -> Path B` 与 `002 v1.1`，但没有体现它已经把 `00-06` 七件套交接正文当成自己的接班基础；
     - 它目前能做局部修宪，但还没有明确接住“角色、权限、入口、升级边界、运行循环”；
     - 它的白名单收口仍会被 `.kiro/specs/屎山修复` 同根 sibling dirty 阻断，说明操作层也未完全独立。
  3. 因而下一步不该直接让它写“最终文档终稿”，否则高概率只是再产一份好文档，而不是完成真正接班。
  4. 现在真正该补的是：
     - 一次接班认知统一
     - 一份 `导航V2` 的接班准入与自治规约
     - 一份真正 live 的偏差账本 / 状态账本入口
  5. 真正的“导航V2 独立”不等于它去写 runtime 代码，而是它要成为：
     - 导航线的规范 owner
     - 审核 owner
     - 调度 owner
     - 升级裁定 owner
- 父层当前恢复点：
  - 现阶段先不要急着让 `导航V2` 再写最终文档；
  - 更合理的顺序是先认知统一，再让它产出自治规约与状态账本入口，父线程确认后才算正式完成接班。

### 会话 64 - 2026-03-27

- 当前主线目标：
  - 继续把 `导航V2` 从交接态推进到自治态；这轮不发新 prompt，直接把讨论过的缺口落成正式文档资产。
- 本轮子任务：
  - 正式落地“接班准入与自治规约”以及“偏差账本入口”，并把它们挂接到 `002`。
- 本轮完成：
  1. 新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\004-导航V2接班准入与自治规约.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\005-导航V2偏差账本.md`
  2. 更新：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\002-导航V2开发宪法与阶段推进纲领.md`
     - 正式补入 `002 / 004 / 005` 的关系定义
  3. 两份新文档分别解决：
     - `004`：`导航V2` 到底是谁、能决定什么、何时必须上报
     - `005`：当前真实状态、偏差、冻结项、checkpoint 如何持续记账
- 本轮新增稳定事实：
  1. 现在导航 V2 不再只有“宪法”，已经补齐了“自治规约”和“状态账本入口”；
  2. 这三份文档一起，才第一次构成了可支持 `导航V2` 真正接班的最小自治体系；
  3. 后续判断重点不再是“还缺不缺文档”，而是 `导航V2` 能不能按这套体系真正自持运行。
- 父层当前恢复点：
  - 下一步如果继续，不应再回到“要不要再写文档”的讨论；
  - 应转向验证 `导航V2` 是否能基于 `002 + 004 + 005` 独立完成一次完整裁定 / 分发 / 记账循环。

### 会话 65 - 2026-03-27

- 当前主线目标：
  - 导航线已经完成文档层补强，这轮进一步明确“现在怎样收尾”和“父线程下一步到底要验证什么”。
- 本轮子任务：
  - 回答两个问题：
    1. 父线程希望 `导航V2` 在下一轮自发做什么
    2. V2 工作区还要不要继续写新的统一规约文档
- 本轮新增稳定事实：
  1. 父线程下一轮真正期待 `导航V2` 自发完成的，是一次完整自治闭环：
     - 主动读 `002 / 004 / 005`
     - 主动读 `00-06` 七份交接正文
     - 主动更新 `005`
     - 主动判断当前第一阻塞点和当前唯一主刀
     - 主动决定是给 `导航检查V2` 发实现委托，还是上报
  2. 这里的“自发”不是自由发挥，而是在 `004` 定义的自治边界里独立运转，不再依赖父线程逐句翻译。
  3. 文档层当前应判定为：
     - 已经够
     - 不应继续膨胀
  4. 当前真正的统一规约体系已经是：
     - `002`：宪法
     - `004`：自治规约
     - `005`：状态账本
  5. 因而后续不应再新增第四篇泛大设计或另一篇“统一规约”，否则会重新制造双源。
- 父层当前恢复点：
  - 现在的正确收尾不是“继续写新文档”，而是“开始测自治”；
  - 如果下一轮 `导航V2` 不能独立跑完一次完整循环，就修 `004`，而不是再造新文档。

### 会话 66 - 2026-03-27

- 当前主线目标：
  - 当前已经从“讨论如何让 `导航V2` 独立”正式切到“执行自治验收”。
- 本轮子任务：
  - 把专门验证 `导航V2` 是否已能独立接班的委托文件落盘。
- 本轮完成：
  1. 新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\006-导航V2自治验收委托-01.md`
  2. 该委托固定要求 `导航V2`：
     - 完整读 `002 / 004 / 005`
     - 完整读 `00-06` 七份交接正文
     - 真正更新 `005`
     - 在“分发 / 上报”之间二选一
     - 若判定分发，必须真的落一份给 `导航检查V2` 的窄委托文件
  3. 因而这轮之后，导航线针对 `导航V2` 的问题已经不再是“文档够不够”，而是“它会不会像 owner 一样独立运转一次完整循环”。
- 父层当前恢复点：
  - 下一步如继续，应直接把 `006-导航V2自治验收委托-01.md` 发给 `导航V2`；
  - 后续父层只审这次自治验收结果，不再替它提前翻译下一步。

### 会话 67 - 2026-03-27

- 当前主线目标：
  - 不再讨论 `导航V2` 应不应该自治，而是实测它能否基于 `002 + 004 + 005 + 00-06` 七份交接正文，独立跑完一次 owner 循环。
- 本轮子任务：
  - 执行 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\006-导航V2自治验收委托-01.md`，只做：
    - 读正文
    - 记账
    - 裁定
    - 分发或上报
- 父层新增稳定事实：
  1. `导航V2` 本轮身份已明确站稳在：
     - 规范 owner
     - 审核 owner
     - 调度 owner
     - 升级裁定 owner
     而不是实现线程。
  2. 本轮未命中 `004` 的 8 类上报阈值，因此不应继续把判断交还父线程或用户，而应自行完成分发。
  3. `005` 已完成本轮实时记账，当前单一第一阻塞点继续压窄为：
     - `TryReleaseSharedAvoidanceDetour(... rebuildPath:false)`
     - `-> TryHandleSharedAvoidance() return true`
     - `-> TickMoving()` 当帧直接 `return`
     这条 NPC 侧 release 后恢复窗口未成立的执行链。
  4. `导航V2` 已新增一份真正的自治分发文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\007-导航V2自治分发-导航检查V2-NpcAvoidsPlayer释放恢复窗口-01.md`
     - 继续只锁 `导航检查V2` 的 NPC 侧 release 后恢复窗口，不回漂 solver、大架构、scene 或 broad cleanup。
- 父层当前恢复点：
  - 当前可以把 `导航V2` 判为已完成一次有效自治循环样本；
  - 后续如继续，应直接审它下一轮的 owner 裁定与分发质量，而不是再替它翻译当前唯一主刀。

### 会话 68 - 2026-03-27

- 当前主线目标：
  - 父层已收到 `导航V2` 的自治验收回执与典狱长后续分发意见；这轮只判断“能不能直接接典狱长现有指令”，不再新造 prompt。
- 本轮子任务：
  - 复核 `导航V2` 的自治验收是否足够成立，以及典狱长提出“直接启用现有 `007`，不再造 `008`”是否正确。
- 本轮完成：
  1. 复核：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\007-导航V2自治分发-导航检查V2-NpcAvoidsPlayer释放恢复窗口-01.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\005-导航V2偏差账本.md`
     - `导航V2` 最新自治验收回执
  2. 明确裁定：
     - 这次自治验收结果可接受；
     - 它确实完成了“读正文 -> 记账 -> 裁定 -> 分发”的完整循环；
     - 因而当前可以直接采用现有 `007` 作为下发给 `导航检查V2` 的 live 入口，不需要再新造 `008`。
  3. 同时补充限定：
     - 这次接受只代表“自治循环成立”；
     - 不代表 `导航V2` 或相关 own root 已经 clean；
     - 后续 same-owner 文档尾账仍需单独收口，不能再混回当前 runtime 主线。
- 父层当前恢复点：
  - 现在可以直接按典狱长给出的现有 `007` 去推进 `导航检查V2`；
  - 后续父层只需继续审运行结果与尾账，不必回头否掉这次自治分发。

### 会话 63 - 2026-03-26

- 当前主线目标：
  - 导航总主线仍不变；这轮父层继续停在 `导航V2` 审核支线，把 Gemini 2.0 锐评审成对 Sunset 当前真实现场负责的结论，并把必要的宪法纠偏落到文档里。
- 本轮子任务：
  - 执行 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\003-导航V2审核000-gemini锐评-2.0与现行宪法纠偏-01.md`，只做审核与文档修正，不碰导航实现、不进 live、不扩到 `TrafficArbiter / MotionCommand`。
- 父层新增稳定事实：
  1. `000-gemini锐评-2.0.md` 应判 `Path B`，不是 `Path A`，也还不到 `Path C`：
     - 它对“状态账本必须前移”“不能只靠 markdown 宪法”“前门指标与结构退出指标要分层”这几条提醒成立；
     - 但“完全没有代码级隔离”“完全没有 Hysteresis 基准”与当前代码 / 测试事实不符。
  2. 代码级现实已重新核实：
     - 已存在 `INavigationUnit`
     - 已存在 `NavigationAgentRegistry`
     - 已存在 `NavigationPathExecutor2D`
     - 因而正确表述应是“已有部分代码级隔离资产，但还没强到足以把所有后续结构迁移都变成编译期硬约束”。
  3. 测试基线现实也已重新核实：
     - `NavigationPathExecutor2D` 已包含 detour clear hysteresis / recovery cooldown 元数据与执行逻辑；
     - `NavigationAvoidanceRulesTests` 已覆盖 `detour_clear_hysteresis`、`detour_recovery_cooldown`、`LastDetourRecoveryTime`；
     - 更准确的缺口是“未来交通裁决状态机测试仍未齐备”，而不是“完全没有基准”。
  4. 因而 `002` 已完成局部修订，核心纠偏为：
     - `P0` 明确收成“当前前门”，不是结构退出；
     - 最小状态账本 / 偏差账本必须从 `P0` 第一刀同步启动；
     - `P1` 才是把实时账本升级成正式中间态资产；
     - `P2` 若宣称结构阶段退出，必须补客观依赖切断指标，但不能偷换成失真总量口号；
     - 本文新增“不是代码级防火墙”的边界说明。
- 父层当前恢复点：
  - `导航V2` 这轮审核支线可以停在“已完成当前纠偏”；
  - 当前真实现行口径应改为：`006 + 007 + 修订后的 002(v1.1)`；
  - `000-gemini锐评-2.0.md` 继续保留为问题意识材料，不直接升格成导航实现蓝图。
  - Git 收口当前仍被同根 residual 阻断：
    - `.kiro/specs/屎山修复/导航检查/memory.md`
    - `.kiro/specs/屎山修复/导航检查/2026-03-26-导航检查V2-NpcAvoidsPlayer执行链硬停与握手续工-08.md`
    - `.kiro/specs/屎山修复/导航检查/2026-03-26-导航检查V2-NpcAvoidsPlayer第一责任点钉死与最小事务-09.md`
  - 因而本轮回执里的 `当前 own 路径是否 clean` 必须报 `no`，不能伪装成已同步完成。

### 会话 69 - 2026-03-27

- 当前主线目标：
  - `导航检查V2` 按 `导航V2-007` 自治分发继续施工，只锁 NPC 侧 detour release 后恢复窗口。
- 本轮子任务：
  - 在 `TryReleaseSharedAvoidanceDetour(... rebuildPath:false) -> TryHandleSharedAvoidance() -> TickMoving() 当帧 return` 链路上做最小补口，并尝试最多 1 条 `NpcAvoidsPlayer` fresh。
- 父层新增稳定事实：
  1. `NPCAutoRoamController.cs` 已下最小补口：
     - `detour.Cleared || detour.Recovered` 分支不再 `StopMotion + linearVelocity=0 + return true`；
     - 改为 `return false`，避免 release 当帧短路。
  2. 代码静态闸门 `git diff --check` 通过。
  3. 本轮 fresh 未能执行：
     - Unity MCP 报 `Unity session not available`；
     - baseline 检查虽为 pass，但 `mcpforunity://instances` 返回 `instance_count=0`。
- 父层当前恢复点：
  - 责任点仍保持在同一 release/recover 链，不存在回漂；
  - 下一刀仅需补齐 1 条 `NpcAvoidsPlayer` fresh，拿到 `scenario_end` 后立即 stop 清场。

### 会话 70 - 2026-03-27（全局盘点：文档已够，剩余集中在 P0 双阻塞与验证收口）

- 当前主线目标：
  - 统一回答“导航线是否已可开工、文档是否还欠缺、全局剩余内容”。
- 本轮子任务：
  - 基于 `006/007/002/004/005 + 导航检查V2 最新现场` 做全量余项裁定。
- 父层新增稳定事实：
  1. 文档层已满足当前开工需要：
     - `006 + 007` 负责目标与阶段路线；
     - `002 + 004 + 005` 负责现行宪法、自治规则与实时偏差记账；
     - 当前不应再新增泛总览文档。
  2. 全局实现剩余集中在 `P0` 两条：
     - `P0-A`：`NpcAvoidsPlayer` NPC 侧 detour release/recover 链仍待 fresh 过线证据；
     - `P0-B`：`HomeAnchor` 运行中补口链仍待闭环。
  3. 当前最大现实阻塞不是设计不清，而是验证现场：
     - Unity MCP baseline 虽 pass，但当前 session 不可用（`instance_count=0`），导致 fresh 无法补齐。
  4. 收口层仍未完成：
     - 实现线程 own 路径仍 dirty，不能宣称本刀已 fully clean。
- 父层当前恢复点：
  - 可以继续开工，但只允许按 `P0` 单切片推进；
  - 优先恢复 Unity session 并补齐 1 条 fresh，再决定后续补口或收口，不前跳大阶段。

### 会话 71 - 2026-03-27（导航检查V2：008 自主闭环冲刺结果）

- 当前主线目标：
  - 继续把 `P0-A / NpcAvoidsPlayer` 的 NPC 侧 detour `release / recover` failure 压到单条可继续下刀的同链责任点。
- 本轮子任务：
  - 在同一条 slice 内完成 live 恢复、最小补口和 fresh 复核，不回漂 solver/大架构/scene/字体。
- 父层新增稳定事实：
  1. 已恢复非 MCP 的 Unity live 取证路径：
     - 直接命中 Unity 原生菜单 `Tools/Sunset/Navigation/Probe Setup/NPC Avoids Player`
     - fresh 已不再受 `Unity session not available` 阻塞。
  2. `NPCAutoRoamController.cs` 本轮新增最小补口：
     - `ClearedOverrideWaypoint` 分支先补 `TryReleaseSharedAvoidanceDetour(...)`
     - `TryReleaseSharedAvoidanceDetour(...)` 允许消费一次“override 已清但尚未 recover”的 detour 上下文
     - recover 成功时恢复当帧硬停返回。
  3. patch 已通过最小闸门并编译进 Unity：
     - `git diff --check` 通过
     - `*** Tundra build success (3.61 seconds), 9 items updated, 862 evaluated`
  4. patch 后 fresh 仍失败：
     - `35610:[NavValidation] scenario_end=NpcAvoidsPlayer pass=False timeout=6.51 minClearance=0.832 npcReached=False`
  5. 因此本轮判定为 `结局 B`，不是 `结局 C`：
     - fresh 已拿到
     - 但 failure 未翻过来，只是责任点继续压窄。
- 父层当前恢复点：
  - 当前第一责任点已从“post-clear recover 是否接上”继续前移到 `TryHandleSharedAvoidance()` 内 release 窗口本身；
  - 下一刀仍只该继续打 `!avoidance.HasBlockingAgent -> TryReleaseSharedAvoidanceDetour(... rebuildPath:false)` 这条入口，不得回漂别线。

### 会话 72 - 2026-03-27（导航高速开发模式：P0 双阻塞同步前推）

- 当前主线目标：
  - 用户已把导航线切到“高速开发模式”；父层不再要求每一刀停等新 prompt，而是允许在 `P0` 范围内连续推进、自己记账、能测就测、测不了就继续做下一段。
- 本轮子任务：
  - 一次性前推当前两条全局 `P0` 阻塞：
    - `P0-A / NpcAvoidsPlayer`
    - `P0-B / HomeAnchor`
- 父层新增稳定事实：
  1. 已新增高速开发日志入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\009-导航V2高速开发执行日志-01.md`
  2. `P0-A` 已继续向前：
     - `NPCAutoRoamController.cs` 新增 release 稳定窗与同链调试计数；
     - `NavigationLiveValidationRunner.cs` 会在 `NpcAvoidsPlayer` 的结果里补 detour create/release/no-blocker 证据。
  3. `P0-B` 也已继续向前：
     - `NPCAutoRoamController.cs` 新增 runtime `HomeAnchor` 自动绑定/临时创建链，不再只依赖 `Editor` 补口。
  4. 当前 live 再次受阻，但阻塞源已更新：
     - 不再是 `Unity session not available`
     - 而是外部测试装配 `Assets\YYY_Tests\Editor\NPCToolchainRegularizationTests.cs` 对 `NPCRoamProfile / NPCDialogueContentProfile` 的编译错误，导致 `SetupNpcAvoidsPlayer` 只排到 `entering_play_mode`。
- 父层当前判断：
  1. 当前导航线的高速推进已经发生真实代码前进，不是空转解释；
  2. 但 `P0` 仍未退出，因为：
     - `NpcAvoidsPlayer` 还缺新的 fresh 结果；
     - `HomeAnchor` 还缺新的运行态非空验证；
  3. 这轮外部 blocker 不归导航 detour 热链 owned scope，不应把主线又拉去横向修整套测试线。
- 父层当前恢复点：
  - 下一步优先等待外部 blocker 解除后重跑 `NpcAvoidsPlayer`；
  - 如仍有空档，可继续沿 `009` 日志维护剩余队列，但不前跳 `P2+`。

### 会话 73 - 2026-03-27（追加收缩：测试装配 blocker 已排除，live 卡点继续前移）

- 当前主线目标：
  - 继续把导航高速模式下的 `P0-A` live 阻塞一层层压窄，不把“没拿到 fresh”含混归因。
- 本轮子任务：
  - 清掉会直接拦住 Unity 编译的 `NPCToolchainRegularizationTests.cs` 缺类型问题，并再触发一次 `NpcAvoidsPlayer`。
- 父层新增稳定事实：
  1. 原先外部测试装配 blocker 已被排除：
     - `NPCRoamProfile / NPCDialogueContentProfile` 的 `error CS0246` 不再出现；
     - Unity 已重新 `Tundra build success (2.70 seconds), 13 items updated, 862 evaluated`。
  2. 但当前 fresh 仍未形成：
     - 这次只走到 `queued_action=SetupNpcAvoidsPlayer entering_play_mode`；
     - 后续 `runtime_launch_request / scenario_start / scenario_end` 都没有接上。
- 父层当前判断：
  1. 当前导航线 live blocker 已进一步收缩成“play mode 入口之后的 runtime handoff”；
  2. 因此后续若继续 live 排查，不应再回头重修 `NPCToolchainRegularizationTests.cs`，而应直接盯 `NavigationLiveValidationMenu / Runner` 的启动接力。
- 父层当前恢复点：
  - 当前 `P0-A` 仍待下一轮 fresh；
  - `P0-B` runtime 自愈链已在位，可等待下一次运行态现场验证。

### 会话 74 - 2026-03-27（追加收缩：scene 仍只读，live 阻塞更新为无 Unity 实例）

- 当前主线目标：
  - 父层继续监督导航高速模式只在 `P0` 内埋头推进，不把 `Primary.unity`、大架构或 broad cleanup 混进这条线。
- 本轮子任务：
  - 给 `NpcAvoidsPlayer` 的 editor -> runtime handoff 增加兜底，并再次复核 `Primary.unity` 与 Unity live 现场。
- 父层新增稳定事实：
  1. `NavigationLiveValidationMenu.cs` 已补 editor 侧兜底：
     - 编译 / domain reload 后若 pending action 仍在，会自动续接 play 请求；
     - 进入 Play 后会补打 `editor_dispatch_pending_action=...`。
  2. `NavigationLiveValidationRunner.cs` 的 `scenario_end=NpcAvoidsPlayer` 现在会直接带 detour create/release/recover 核心计数。
  3. `Primary.unity` 最新现场：
     - `Check-Lock.ps1` 返回 `unlocked`
     - 但文件本体仍是 `M`
     - 仍不能把它当成“scene 可安全顺手写”的信号。
  4. MCP 当前不是编译炸，而是：
     - `unityMCP` 资源正常
     - 但当前 `instance_count = 0`
     - 没有可接管的 Unity 实例，因此这轮无法拿 live fresh。
- 父层当前判断：
  1. 当前不应把 `Primary.unity` 解读成可写窗口；
  2. 当前导航线的离线推进仍有效，因为 handoff 链和 detour 证据链都已继续前推；
  3. 下一次一旦 Unity 会话恢复，最小动作仍是 `NpcAvoidsPlayer` 一条 fresh，而不是扩窗。
- 父层当前恢复点：
  - 保持 scene 只读；
  - 等 Unity 实例恢复后，先验证 `editor_dispatch_pending_action / runtime_launch_request / scenario_end`；
  - 若 fresh 仍失败，再继续只打同一条 `release / recover` 链。

### 会话 75 - 2026-03-27（追加收缩：Unity 实例恢复，当前 blocker 收束回 release/recover 本身）

- 当前主线目标：
  - 父层继续监督导航高速模式在同一条 `NpcAvoidsPlayer` slice 内完成“会跑起来 -> 跑完 -> 看清失败点”的闭环。
- 本轮子任务：
  - 在 Unity 实例恢复后，重新执行 1 条 `NpcAvoidsPlayer` fresh，验证 handoff 是否已经恢复。
- 父层新增稳定事实：
  1. 当前 `unityMCP` 实例已恢复：
     - `instance_count = 1`
  2. 最新 fresh 已完整跑通：
     - `queued_action`
     - `runtime_launch_request`
     - `scenario_start`
     - `scenario_end`
  3. 最新核心结果：
     - `pass=False`
     - `npcReached=False`
     - `detourActive=True`
     - `detourCreates=11`
     - `releaseAttempts=164`
     - `releaseSuccesses=10`
     - `noBlockerFrames=6`
     - `recoveryOk=False`
  4. Play 结束后已主动退回 Edit Mode。
- 父层当前判断：
  1. handoff 不再是 blocker；
  2. 当前失败点已重新收束回 detour `release / recover` 本身；
  3. `Primary.unity` 仍保持 mixed hot 面只读口径，不随 Unity 实例恢复而自动转成“可写”。
- 父层当前恢复点：
  - 下一刀继续只锁 `release / recover` 链；
  - 不回头修 handoff；
  - 不扩到 scene。

### 会话 76 - 2026-03-27（导航高速模式：P0 双前门已拿到 runtime 实证）

- 当前主线目标：
  - 继续只在导航 `P0` 范围内推进，并把真实进展从“还在修”升级为“哪些前门已经过线、哪些边界仍不能放开”。
- 本轮子任务：
  - 用最短 live 继续验证 `NpcAvoidsPlayer` 是否稳定过线，并补 `HomeAnchor` 的运行态非空实证。
- 父层新增稳定事实：
  1. `NPCAutoRoamController.cs` 已补入 post-release recovery window + release 后路径恢复线；
  2. `NpcAvoidsPlayer` 在同链补口后连续拿到 3 条 `pass=True`：
     - `61458`
     - `62333`
     - `63640`
  3. `HomeAnchor` 在 Play Mode 下已回读：
     - `001 / 002 / 003` 的 `HomeAnchor` 都非空
     - 三者 `IsRoaming = true`
  4. `Primary.unity` 仍为 `M`，本轮继续保持只读。
- 父层当前判断：
  1. 当前不能再把 `P0-A / P0-B` 写成“仍未过线”的事实；
  2. 但也不能把这轮成功误写成：
     - `Primary.unity` 可写
     - 体验线已全部通过
     - shared root 已 clean
  3. 导航线下一步不该再盲目重复旧的 fail slice，而应重新选择新的最小用户可见问题或只保留回归 watch。
- 父层当前恢复点：
  - 优先把 `导航V2/导航检查/线程记忆` 与高速日志同步到新的 runtime 事实；
  - 继续保持 `Primary.unity`、字体、broad cleanup 在禁区；
  - 后续如再开新刀，应重新做最小切片选择。

### 会话 77 - 2026-03-27（导航高速模式：真实点击近身单 NPC 已压成 passive NPC 同链，failure 形态发生翻转）

- 当前主线目标：
  - 父层继续监督导航高速模式从“已经过线的 `NpcAvoidsPlayer / HomeAnchor`”切到下一条真实用户可见问题，而不是继续重复旧 slice。
- 本轮子任务：
  - 只围绕 `RealInputPlayerSingleNpcNear` 压缩玩家对 passive NPC 的过早绕行，并查清这条链到底是 path、detour 还是 solver 在主导。
- 父层新增稳定事实：
  1. 当前路径本身不是主锅：
     - heartbeat 已明确 `pathCount=1`
     - 当前 waypoint 就是目标点 `(-5.80, 4.45)`
  2. 当前 blocker 已被钉死为：
     - `npcAState=Inactive`
     - 即 passive / sleeping NPC blocker
  3. 这一轮 live 指标先后出现：
     - `blockOnsetEdgeDistance 1.114 -> 0.954 -> 0.737`
     - `maxPlayerLateralOffset 2.180 -> 1.723 -> 1.167`
  4. 最新 checkpoint 再次翻出新的 failure 形态：
     - `pass=False timeout=6.51`
     - `playerReached=False`
     - `minEdgeClearance=0.260`
     - `maxPlayerLateralOffset=0.054`
     - 说明当前已从“鬼畜大绕”进入“几乎不绕，但停住/取消”的新阶段。
- 父层当前判断：
  1. 当前不该再把导航状态描述成“还卡在旧的 detour release/recover fail”；
  2. 也不该把 `Primary.unity`、scene 或 `GameInputManager.cs` 拉进这个 slice；
  3. 现在真正该继续打的是：
     - `PlayerAutoNavigator`
     - `NavigationLocalAvoidanceSolver`
     - `NavigationLiveValidationRunner`
     这一条 passive NPC blocker 同链。
- 父层当前恢复点：
  - 下一刀目标改成“恢复近身后轻微可推进”，而不是继续追更少的侧绕；
  - 仍保持 `Primary.unity`、字体、broad cleanup 禁区不动；
  - 继续沿高速日志 `009` 记账，不回头用长聊天拼现场。

### 会话 78 - 2026-03-27（父层同步：导航验证面已 compile-clean，玩家侧 passive NPC checkpoint 定在 0.594）

- 当前主线目标：
  - 继续监督导航高速模式的最新 slice，但不让支撑性 compile blocker 把父层判断带偏。
- 本轮子任务：
  - 把 `RealInputPlayerSingleNpcNear` 这一轮新增的 compile 清障与 best-known stable checkpoint 同步到父层。
- 父层新增稳定事实：
  1. 为了恢复导航 fresh，已清掉两处同源 compile blocker：
     - `PlayerNpcNearbyFeedbackService.cs` 的不存在类型引用；
     - `NPCInformalChatInteractable.cs` 对 `PlayerNpcChatSessionService` 的强类型依赖。
  2. 清障后，玩家侧 passive NPC 最新可信 checkpoint 为：
     - `scenario_end=RealInputPlayerSingleNpcNear pass=False minEdgeClearance=-0.001 blockOnsetEdgeDistance=0.594 playerReached=True`
  3. 更激进的参数压缩已被证明确实会退回：
     - `HardStop + stuck cancel`
     - 或“起步即取消”
     - 当前这些回退实验都已撤回。
- 父层当前判断：
  1. handoff 和 compile 已不再是这条 slice 的主 blocker；
  2. 当前 remaining blocker 继续锁在玩家侧 passive NPC 的 onset 太早；
  3. 下一刀不应再机械继续压同一组半径参数，而应换到 `close-constraint / stuck cancel` 衔接条件。
- 父层当前恢复点：
  - 继续沿 `009` 与各层 memory 记账；
  - 下次续跑应从 `0.594 + playerReached=True` 的稳定 checkpoint 出发；
  - `Primary.unity`、字体、broad cleanup 继续禁入。

### 会话 79 - 2026-03-27（父层同步：close-constraint 主锅已收缩到“接手后压速过久”）

- 当前主线目标：
  - 继续监督导航高速模式的玩家侧 passive NPC 同链，不让 live 抖动把结论带偏。
- 本轮子任务：
  - 在 `0.594 + playerReached=True` 的稳定 checkpoint 上，继续确认 close-constraint 责任点到底收缩到了哪一层。
- 父层新增稳定事实：
  1. `PlayerAutoNavigator.cs` 已撤掉已知更差的 `ResetProgress(...)` 回退实验，并恢复 `PASSIVE_NPC_CLOSE_CONSTRAINT_EXTRA_RADIUS = 0.05f`。
  2. fresh 再次拿到：
     - `scenario_end=RealInputPlayerSingleNpcNear pass=False minEdgeClearance=-0.005 blockOnsetEdgeDistance=0.594 playerReached=True`
     - 说明 best-known stable checkpoint 未回退。
  3. 新增 `closeForward` 证据后，当前更精确的事实是：
     - close-constraint 接手期间，`closeForward` 已从约 `0.55` 持续降到 `0.09`
     - 但 `moveScale` 旧链里基本仍被压在 `0.18`
     - 当前主锅因此从“onset 太早”继续收缩成“接手后压速恢复太慢”。
  4. 第二条 fresh 未拿到 `scenario_end`，只出现：
     - `WebSocket is not initialised`
     - `runner_disabled`
     - `OcclusionTransparency ... 未找到OcclusionManager`
     - 当前应按 transport 抖动记账，不算项目逻辑回退。
- 父层当前判断：
  1. 不应再把这条 slice 的主锅写成 path / detour / compile；
  2. 下一刀更应该继续审：
     - `closeForward` 下降后何时恢复有效速度；
     - 而不是继续盲拧 solver/radius。
- 父层当前恢复点：
  - 继续沿 `009` 与各层 memory 记账；
  - 下次有效 fresh 优先看 `moveScale` 是否不再长期卡在 `0.18`；
  - `Primary.unity`、字体、broad cleanup 继续禁入。

### 会话 80 - 2026-03-27（父层同步：用户新增 crowd 慢卡反馈后，crowd 已有 baseline fail 与 fresh pass）

- 当前主线目标：
  - 在导航高速模式下同时覆盖：
    - single NPC 近身 onset
    - 多 NPC crowd 慢卡 / 乱跑
- 本轮子任务：
  - 把用户新增的 crowd 反馈转成可验证切片，并同步父层“哪条已过、哪条仍未过”。
- 父层新增稳定事实：
  1. crowd baseline 已被正式钉死：
     - `scenario_end=RealInputPlayerCrowdPass pass=False ... crowdStallDuration=4.900 playerReached=False`
  2. 针对多人 passive NPC pressure 的 detour 旁路补口后，crowd 已拿到一条 fresh pass：
     - `scenario_end=RealInputPlayerCrowdPass pass=True minEdgeClearance=-0.008 directionFlips=1 crowdStallDuration=0.356 playerReached=True`
  3. 与此同时，single NPC 当前 kept 版本复核为：
     - `scenario_end=RealInputPlayerSingleNpcNear pass=False minEdgeClearance=-0.005 blockOnsetEdgeDistance=0.594 playerReached=True`
  4. 本轮后段 Unity live 已被外部 `NPCValidation` 线程污染；
     - 因而后续 speculative tweak 未继续保留，只回到已验证版本。
- 父层当前判断：
  1. 用户说的 crowd 严重 bug 当前不能再写成“还没做”；
  2. 当前真正 remaining blocker 再次收缩成：
     - single NPC close-constraint onset 仍偏早；
  3. crowd 这条线现阶段可转为回归 watch，而不是继续主刀。
- 父层当前恢复点：
  - 后续导航实现主刀应回到 single NPC close-constraint 同链；
  - crowd 只保留回归 watch；
  - `Primary.unity`、字体、broad cleanup 继续禁入。

### 会话 81 - 2026-03-27（父层同步：crowd stale detour owner 与侧后方误计数已纳入恢复链）

- 当前主线目标：
  - 继续监督导航高速模式的新 crowd 尾症状，但不把主刀从 single NPC 同链挪走。
- 本轮子任务：
  - 把“后面没人了还像被拖住”的 crowd 口头症状，进一步收缩成玩家侧 detour owner / crowd pressure 边界问题。
- 父层新增稳定事实：
  1. `PlayerAutoNavigator.cs` 已新增两处 crowd 恢复补口：
     - 当前 blocker 切换时释放旧 detour owner；
     - crowd pressure 只统计前方通道内的 passive NPC blocker，不再把侧后方 crowd 继续算进 repath 压力。
  2. crowd fresh 继续保持 pass：
     - `scenario_end=RealInputPlayerCrowdPass pass=True minEdgeClearance=-0.005 directionFlips=2 crowdStallDuration=0.449 playerReached=True`
  3. single watch 本轮拿到：
     - `scenario_end=RealInputPlayerSingleNpcNear pass=False minEdgeClearance=-0.011 blockOnsetEdgeDistance=0.737 playerReached=True`
     - 但 heartbeat 全程 `detour=False detourOwner=0`，未命中本轮新增 crowd detour 分支。
  4. 因而当前 best-known stable single checkpoint 仍保留：
     - `0.594 + playerReached=True`
- 父层当前判断：
  1. crowd 这轮应继续留在回归 watch，而不是重新变成唯一主刀；
  2. 用户描述的 crowd 尾症状现在已经有明确代码责任点，不再只是泛泛说“可能是 crowd 太复杂”；
  3. 下一刀仍应继续 single NPC close-constraint，同步 watch crowd 是否再出现 stale-owner 拖偏。
- 父层当前恢复点：
  - 继续沿 `009` 与各层 memory 记账；
  - crowd watch 的重点更新为：
    - stale detour owner
    - 前方通道计数
  - `Primary.unity`、字体、broad cleanup 继续禁入。

### 会话 82 - 2026-03-27（父层同步：用户直接汇报改成人话层优先）

- 当前主线目标：
  - 继续维持导航高速开发主线，同时把用户刚刚明确点名的“汇报格式”上升为协作硬约束。
- 本轮子任务：
  - 将新的直接汇报顺序同步到父层，避免后续线程又退回技术 dump 开头。
- 父层新增稳定事实：
  1. 用户已明确规定：
     - 直接汇报必须先用 6 条人话层说明当前真实进度；
     - 然后才能补技术审计层。
  2. 以后如果仍先讲参数、checkpoint、changed_paths，而不先讲人话层，应直接判定为汇报不合格并重发。
  3. 这条要求已经同步到：
     - `导航V2`
     - `导航检查`
     - 当前线程记忆
- 父层当前判断：
  1. 这不是表达偏好，而是后续直接汇报的硬合同；
  2. 导航主线本身未换线，仍然是 single NPC 主刀、crowd 回归 watch。
- 父层当前恢复点：
  - 后续对用户的直接汇报统一先走 6 条人话层，再补技术审计层；
  - 导航实现主线继续不变。

### 会话 83 - 2026-03-27（父层同步：single 主线新增 detour release 同帧恢复补口）

- 当前主线目标：
  - 继续监督导航高速模式下的玩家侧 single 主刀；
  - 不让 crowd watch 或 external blocker 把主线叙事冲散。
- 本轮子任务：
  - 只同步这一刀的新增事实：
    - 玩家侧 detour release 恢复窗口补口
    - external compile blocker 报实
- 父层新增稳定事实：
  1. `PlayerAutoNavigator.cs` 已新增一刀更窄的恢复补口：
     - detour release 后不再先 `ForceImmediateMovementStop()` 再退出本帧；
     - 改成同帧继续回到主路径评估。
  2. 这刀的静态闸门通过：
     - `git diff --check -- PlayerAutoNavigator.cs`
     - `validate_script(PlayerAutoNavigator.cs) => errors=0 warnings=2`
  3. 本轮没有新的 single fresh，不是因为导航 own 代码红了，而是现场已被外部编译错误污染：
     - `Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs` 多条 `CS0246`
- 父层当前判断：
  1. 当前导航主线仍然可以继续，但 live 结论必须暂停在 external blocker 前；
  2. single kept baseline 仍保留：
     - `0.594 + playerReached=True`
  3. crowd 继续只做回归 watch，不重新升级为主刀。
- 父层当前恢复点：
  - 下一次 single 续跑前，必须先确认 Console 回到 `0 error`；
  - 然后再只跑 1 条 `RealInputPlayerSingleNpcNear` fresh；
  - `Primary.unity`、字体、broad cleanup 继续禁入。

### 会话 84 - 2026-03-27（父层同步：single 已拿到 0.120 pass，crowd watch 未回退）

- 当前主线目标：
  - 继续监督导航高速模式下的玩家侧 single 主刀；
  - 不让旧的 `0.594` 叙事继续滞后于当前现场。
- 本轮子任务：
  - 同步 single detour release 同帧恢复补口的真实 live 结果；
  - 并确认 crowd watch 未被这刀打坏。
- 父层新增稳定事实：
  1. 先前那组 `SpringDay1LateDayRuntimeTests.cs` `CS0246` 没有复现，当前判定为 stale console residue；
  2. single 这轮连续拿到：
     - `pass=False ... blockOnsetEdgeDistance=0.236 playerReached=True`
     - `pass=True ... blockOnsetEdgeDistance=0.120 playerReached=True`
  3. crowd watch retry 后继续 pass：
     - `scenario_end=RealInputPlayerCrowdPass pass=True ... crowdStallDuration=0.309 playerReached=True`
- 父层当前判断：
  1. `RealInputPlayerSingleNpcNear` 已从“仍未过线”推进到“已拿到首条明确 pass”；
  2. 旧的 `0.594 kept baseline` 现在只能算历史对照，不再是当前最佳基线；
  3. 下一步重点应转向稳定性确认，而不是再回到旧参数盲拧。
- 父层当前恢复点：
  - single 当前最新 best-known baseline 更新为：
  - `pass=True`
  - `blockOnsetEdgeDistance=0.120`
  - `playerReached=True`
- crowd 继续只做回归 watch；
- `Primary.unity`、字体、broad cleanup 继续禁入。

### 会话 85 - 2026-03-27（父层同步：single 近身切片已从自取消/超时推进到连续两条 pass）

- 当前主线目标：
  - 继续监督导航高速模式下玩家侧 single close-constraint 主刀；
  - 不让“这轮已经有连续 pass”继续停留在线程层、工作区层而未同步到父层。
- 本轮子任务：
  - 同步 `PlayerAutoNavigator.cs` 这一轮围绕 stuck / close-constraint / blocked-input 的最小闭环；
  - 并记录 crowd watch 未回退。
- 父层新增稳定事实：
  1. single 这轮新增的真实责任点不是 detour，而是：
     - `BlockedInput` 窗口被 `CheckAndHandleStuck()` 误判后直接 `Cancel()`
  2. 这轮在 `PlayerAutoNavigator.cs` 中新增的有效收口包括：
     - short-range avoidance stuck suppress
     - `NPC blocker` / `Passive NPC blocker` 识别拆分
     - avoidance 当前帧 clearance / move floor 直连
     - slight-overlap soft-overlap 放松
  3. 最新 live 结果：
     - single latest passes：
       - `scenario_end=RealInputPlayerSingleNpcNear pass=True ... blockOnsetEdgeDistance=0.100 playerReached=True`
       - `scenario_end=RealInputPlayerSingleNpcNear pass=True ... blockOnsetEdgeDistance=0.170 playerReached=True`
       - `scenario_end=RealInputPlayerSingleNpcNear pass=True ... blockOnsetEdgeDistance=0.170 playerReached=True`
     - crowd latest watch：
       - `scenario_end=RealInputPlayerCrowdPass pass=True minEdgeClearance=-0.012 directionFlips=1 crowdStallDuration=0.192 playerReached=True`
- 父层当前判断：
  1. 当前玩家侧 single 切片已经不再是“偶发一条侥幸 pass”；它已经重新回到连续 pass 窗口；
  2. crowd 这轮没有被 single 新补口打回去，继续维持回归 watch；
  3. 当前 remaining 风险已再次收缩成：
     - `0.23 ~ 0.35` 这类 residual watch 是否还会复发
- 父层当前恢复点：
  - 当前最新 best-known single baseline 更新为：
    - `pass=True`
    - `blockOnsetEdgeDistance=0.100`
    - `playerReached=True`
  - crowd 继续只做回归 watch；
  - `Primary.unity`、字体、broad cleanup 继续禁入。

## 2026-03-28（对子线程最新“已接近收口”叙事的父层纠偏）

- 复核范围：
  - `导航检查V2` 最新长回执
  - `NavigationLiveValidationRunner.cs`
  - `PlayerAutoNavigator.cs`
  - `SpringDay1WorldHintBubble.cs`
- 父层新增稳定结论：
  1. `single 0.462` 被解释成 validation false positive，这一点基本成立；
  2. 但这轮不能因此升格成“导航主链基本闭环，下一步只做 cleanup”；
  3. 真实原因不是导航又回到全盘失控，而是：
     - validation runner 已加入 interaction suppression，probe 保真度下降；
     - runtime 本体仍有一簇新的 passive NPC / stop radius 行为改动；
     - 还混入了 `SpringDay1WorldHintBubble.cs` 的 UI / 字体侧改动。
- 父层管理结论：
  - 当前对子线程的正确管理，不是让它直接 cleanup；
  - 而是先强制它做“validation 语义分层 + runtime 改动边界报实 + 跨域 UI 变更切分”，之后才允许谈 same-root cleanup / checkpoint。
- 父层恢复点：
  - 可以承认“现在已经不是鬼畜主故障都没找到的阶段”；
  - 但还不能承认“只剩工程收口”。

## 2026-03-28（父线程亲测导航 live 后的再裁定）

- 父层本轮新增事实：
  1. 已亲自给 `NavigationLiveValidationRunner` 增加 raw/suppressed click 对照与动作聚合计数，并跑了：
     - 整包 live 一次
     - `Raw Single` 两次
     - `Raw Crowd` 一次
     - `Raw Push` 一次
     - `NpcAvoidsPlayer` 一次
     - `NpcNpcCrossing` 一次
  2. 亲测后更准确的判断不是“导航根本没成效”，也不是“已经快收口”；
  3. 当前真实状态是：
     - crowd / push / NPC 两条都还能过；
     - single 也能过，但它是用大量 `HardStop` 和动作切换抖着过。
- 父层最新裁定：
  - 当前第一责任点不再是“detour owner 有没有接上”；
  - 也不是“cleanup same-root dirty”；
  - 而是玩家 single 近距避让体验：需要把 `HardStopFrames / actionChanges` 从当前这种明显难看的形态压下来。
- 父层恢复点：
  - 后续对子线程的管理，应该改成“单刀压玩家 single 近距避让抖动”，而不是继续放它往 cleanup-only 漂。

## 2026-03-28（父线程改用“纠正一次 + 放手让子线程自判实现分支”）

- 父层新增动作：
  - 已生成针对 `导航检查V2` 的续工 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-28-导航检查V2-玩家Single近距避让止抖-10.md`
- 父层当前管理口径：
  1. 不再越级给实现线程设计具体 patch；
  2. 只负责一次纠正：
     - 当前主刀是玩家 single 近距避让止抖
     - 不是 cleanup-only
     - 不是交互误触主导
  3. 之后让实现线程自己思考具体 runtime 分支，并继续按回执制推进。

## 2026-03-28（父线程再次校准导航主线：single 止抖不是终局，地面点锚点契约已上升为结构性问题）

- 本轮父层新增事实：
  1. 已复核 `导航检查V2` 最新回执与 `PlayerAutoNavigator.cs` 当前 diff，确认：
     - 它这轮确实把 `Raw SingleNpcNear` 的 `hardStopFrames` 从 `26` 压到 `2`，`actionChanges` 从 `9~10` 压到 `8`；
     - 因而“single 止抖有推进”可以接受为有效 checkpoint。
  2. 但父层同步做了代码与 Unity 现场只读核验后，确认当前更底层的问题没有进入它这轮主刀：
     - 普通地面点导航仍按 `Rigidbody2D.position / Transform.position` 语义请求与判定终点；
     - 跟随交互目标链才使用 `ClosestPoint + stopRadius`。
  3. Unity 当前玩家现场只读回读：
     - `Transform.position / Rigidbody2D.position = (-7.9073, 8.5603)`
     - `BoxCollider2D.bounds.center = (-7.9146, 9.7616)`
     - Y 轴稳定相差约 `+1.20`
     - 这与用户“点 A 却停在 A 上方”的现场观察完全一致。
  4. 验证层 `NavigationLiveValidationRunner` 仍主要使用脚底/刚体锚点做 `GetActorFootPosition(...)` 与 `playerReached` 判定，因此 probe 目前会天然放大“错锚点也算到达”的风险。
  5. 用户新增体感“moving NPC 太早太僵硬，static NPC 反而被推土机顶过去”，与 `PlayerAutoNavigator` 当前 passive blocker 延迟逻辑也能对上：
     - moving NPC 仍偏保守早绕；
     - passive/static NPC 更容易因延迟 repath 门槛而过晚进入有效避让窗口。
- 当前父层裁定：
  1. 现在不能再把导航整体表述成“残余抖动收尾期”；
  2. 更准确的阶段判断是：
     - 某个 single 子症状被明显压小
     - 但普通点导航终点契约和 static NPC 语义仍未站稳
  3. 当前已被用户认可、且父层也认同的契约必须正式作为后续导航线依据：
     - 普通地面点导航使用“玩家实际占位中心”语义
     - 跟随交互目标才使用 `ClosestPoint + stopRadius`
     - 两套不得继续混用
- 父层恢复点：
  - 若后续继续管理 `导航检查V2`，下一轮不该再只是继续刷 `actionChanges=8 -> 6`；
  - 必须先把“普通点终点锚点契约”升格为显式主问题，再去重新审 static / moving NPC 的回避语义。

## 2026-03-29（导航线拆成“父线程静态止血 + V2 动态续工”，并确认 shared Unity 外部菜单已成静态 fresh 第一阻塞）

- 当前父层新增事实：
  1. 父线程已亲自把普通地面点导航契约往“玩家实际占位中心”收了一刀：
     - `PlayerAutoNavigator.GetPlayerPosition()`
     - `PlayerAutoNavigator.GetPathRequestDestination()`
     - `PlayerAutoNavigator.CompleteArrival()`
  2. 父线程已独立搭起静态验证链：
     - `NavigationStaticPointValidationRunner.cs`
     - `NavigationStaticPointValidationMenu.cs`
     - 以及 marker file 触发口径 `Library/NavStaticPointValidation.pending`
  3. fresh 静态证据已经证实：
     - `accepted_case_count=2`
     - `case_start` 中 `navTarget == target`
     - 说明“普通点请求终点仍然带旧 offset”这条旧契约已被实质修正
  4. 但 fresh live 同时也证实：
     - shared Unity 当前会被外部 `MCP ExecuteMenuItem` 抢占
     - 已实录 `SpringUiEvidenceMenu` 抢占静态 case 窗口
     - 所以当前静态线最新第一阻塞已经不是样本筛选错误，而是 live 窗口被外部菜单污染
- 当前父层管理动作：
  1. 新建父线程专用双线验收清单：
     - `2026-03-29-父线程验收清单-导航静态止血与动态并行-12.md`
  2. 新建对子线程的中间约束 prompt：
     - `2026-03-29-导航检查V2-动态线续工并冻结静态live触点-12.md`
  3. 从这轮起，导航线的并行边界固定为：
     - 父线程：静态点导航止血与静态 fresh 证据
     - `导航检查V2`：动态线代码/矩阵/责任点继续压窄
- 当前父层结论：
  1. 静态线不是“没动”，也不是“已验收”；
  2. 更准确的阶段判断是：
     - 静态契约修正已进代码
     - 静态验证工具已从黑盒变成可解释
     - 最终静态 fresh 仍需独占 Unity live 窗口
  3. 动态线当前不该再碰父线程静态触点，否则父子两线会在同一热区互相污染。

## 2026-03-28（父线程把下一轮切成“高保真测试矩阵接管”，不再继续 patch 竞猜）

- 本轮父层新增动作：
  1. 新建对子线程的重型接管 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-28-导航检查V2-导航高保真测试矩阵与契约收口-11.md`
  2. 新建父线程自己的下一轮验收清单：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-28-父线程验收清单-导航高保真测试矩阵-11.md`
- 本轮父层裁定：
  1. 当前最正确的管理动作，不是继续逼 `导航检查V2` 去单点抛光 `actionChanges=8`；
  2. 而是先让它完成一次高保真测试矩阵接管，把：
     - 普通点导航停偏
     - static NPC 推土机
     - moving NPC 提前僵硬
     这三类坏相测成可信证据。
  3. 从这一轮开始，导航线下一步最重要的产出不是“又修了一点”，而是：
     - 可信测试底座
     - 锚点契约收口
     - 新的第一责任点
- 父层恢复点：
  - 后续若继续，先转发 `-11` prompt 给 `导航检查V2`；
  - 下一轮父层统一按验收清单审，不再让 chat 回执主导判断。

## 2026-03-28（导航检查V2 已完成高保真矩阵接管，runtime 第一责任点改判为 passive/static NPC blocker）

- 当前父层新增事实：
  1. `导航检查V2` 已交出正式报告：
     - `2026-03-28-导航检查V2-导航高保真测试矩阵报告-01.md`
  2. 普通地面点导航：
     - raw / suppressed 都显示 `Collider.bounds.center` 可稳定对点；
     - `Transform/Rigidbody` 仍稳定下偏约 `1.10 ~ 1.24`
  3. `SingleNpcNear raw ×3 + suppressed ×1`：
     - 已稳定证明当前坏相是“推土机式顶静止 NPC”，不是交互误触；
     - `npcPushDisplacement≈2.29`
     - `playerReached=False`
     - `detourMoveFrames=0`
  4. `MovingNpc raw ×3` 与 `Crowd raw ×3`：
     - 当前也都 fresh fail；
     - moving 呈现过早 onset + 大侧偏 + 未到点；
     - crowd 呈现长时间 pathMove 蹭行 + 未到点
  5. NPC 自身两条护栏：
     - `NpcAvoidsPlayer ×2 pass`
     - `NpcNpcCrossing ×2 pass`
- 当前父层裁定：
  1. 普通地面点导航当前主要是“契约/验收锚点错层”，不是下一刀的 runtime 第一火点；
  2. 当前下一轮实现主刀应重新收回：
     - `PlayerAutoNavigator.cs`
     - passive/static NPC blocker 响应链
  3. 历史 `pass` 口径需要分层：
     - ground 的 `pass` 只代表中心对点成立
     - 动态 player 场景的旧 `pass` 不能继续直接当现状结论

## 2026-03-29（父线程已把导航 V2 的下一刀从“继续矩阵”收成“只打 passive/static NPC blocker 响应链”）

- 当前父层新增动作：
  1. 正式审完 `2026-03-28-导航检查V2-导航高保真测试矩阵报告-01.md`；
  2. 新建对子线程的下一轮单刀 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-29-导航检查V2-PlayerAutoNavigator-passive静态NPC-blocker响应链-13.md`
  3. 新建父线程下一轮验收清单：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-29-父线程验收清单-PlayerAutoNavigator-passive静态NPC-blocker响应链-13.md`
- 当前父层结论：
  1. 高保真矩阵 checkpoint 可以接受，但不能冒充“体验已接近收口”；
  2. ground 契约、静态 runner/menu、moving/crowd 泛调都不是下一刀主刀；
  3. 下一刀唯一主刀正式固定为：
     - `PlayerAutoNavigator.cs`
     - passive/static NPC blocker 命中后仍停留在 `PathMove` 主路径、没有进入有效响应的那条链
- 当前父层恢复点：
  - 若用户继续让我管理 `导航检查V2`，下一步直接转发 `-13`；
  - 后续父线程只按 `SingleNpcNear raw` 的推土机签名是否被打破来审，不再让“矩阵已完成”顶替 runtime 收口。

## 2026-03-29（父线程静态线 fresh 已真实拿到 case_end/all_completed，新的阻塞已改判为 validation runner 本身）

- 当前父层新增事实：
  1. 父线程已拿到独占 Unity live 窗口，并让静态 runner 真正跑到：
     - `case_end`
     - `all_completed`
     - 而不是只停在 `runner_started / case_start`
  2. `StaticPointCase1`：
     - `centerDistance=0.080`
     - `rigidbodyDistance=1.204`
     - `transformDistance=1.204`
     - `pass=False`
     - 当前 fail 的直接原因是 timeout，而不是中心没到点
  3. `StaticPointCase2`：
     - 当前不是被外部菜单污染；
     - 而是第一案走完后新 origin 下重算 offset，被 `path_probe_not_open_ground` 直接 skip。
  4. 本轮还补了最小观测日志到：
     - `Assets/YYY_Scripts/Service/Navigation/NavigationStaticPointValidationRunner.cs`
     - 用来确认 `case_tick / runner_disabled / runner_destroyed`
- 当前父层结论：
  1. 静态线当前已经不是“无法闭环取证”；
  2. 当前新的最窄阻塞从“外部菜单污染”继续收缩成：
     - validation runner 的 timeout / settle 关系
     - 多 case 编排时 origin 漂移导致的第二案 skip
  3. 因而静态线下一刀若继续，不该再回到普通点 runtime 契约，而应直接修静态 validation runner 自己的判定与 case 编排。
- 当前父层恢复点：
  - 动态线仍继续由 `导航检查V2` 负责；
  - 父线程若继续静态线，应把主刀切到 `NavigationStaticPointValidationRunner.cs`，而不是再去碰 `PlayerAutoNavigator` 的普通点契约。

## 2026-03-29（父线程已把静态 validation runner 自身的 3 处口径补齐，剩余阻塞改判为共享 Unity 独占窗口）

- 当前父层新增动作：
  1. 在 `Assets/YYY_Scripts/Service/Navigation/NavigationStaticPointValidationRunner.cs` 内继续收口：
     - case 目标从“重算 offset”改为“固定目标点”
     - timeout 逻辑允许“中心已到点且已停住”直接算完成
     - conflict 判定从“场上存在 live runner”收紧到“live runner 真的 busy”
     - 增加固定 `ValidationStartCenter`，每案开跑前强制回到同一起点并清旧导航状态
  2. 继续用 live 验证复核，确认：
     - 只要 `导航检查V2` 在同一 Unity 实例里继续跑动态 `NavigationLiveValidationRunner`
     - 静态 runner 现在会按设计 abort，不再偷偷混跑
- 当前父层结论：
  1. 静态线当前代码层已经没有明显遗漏的自我收口项；
  2. 当前剩余最大阻塞已不再是静态 runner 自己的逻辑，而是 shared Unity 没有真正独占窗口；
  3. 因而父线程这一刀更准确的状态是：
     - `代码收口完成`
     - `最终独占 fresh live 仍待窗口`
- 当前父层恢复点：
  - 若用户后续让动态线先停，再给静态线独占窗口，父线程下一步直接复跑静态 menu；
  - 在那之前，父线程不需要再继续改静态 runner 代码。

## 2026-03-29（导航检查V2：passive/static NPC 已从纯推土机推进到 detour 失活残差）

- 当前新增进展：
  1. `导航检查V2` 已按 `-13` 只锁 `PlayerAutoNavigator.cs` 补刀；
  2. `SingleNpcNear raw` 的旧坏相：
     - `npcPushDisplacement≈2.29`
     - `detourMoveFrames=0`
     - `blockedInputFrames=0`
     - `hardStopFrames=0`
     - `actionChanges=1`
     已被打破；
  3. 当前 fresh `SingleNpcNear raw x3` 统一改成：
     - `npcPushDisplacement=0.077 / 0 / 0`
     - `detourMoveFrames=14 / 15 / 15`
     - `actionChanges=3`
     - 但仍 `playerReached=False`
- 当前父层应更新的阶段判断：
  1. 这条线已经不再是“完全没响应的 pure PathMove bulldozer”；
  2. 当前剩余责任点仍在 `PlayerAutoNavigator.cs` 内，但已从“进不去响应”改成：
     - detour / rebuild 进去后，导航在未到点时过早掉成 `Inactive / pathCount=0`
- 当前护栏状态：
  - `MovingNpc raw x1`：仍 fail（既有失败态）
  - `Crowd raw x1`：仍 fail（既有失败态）
  - `NpcAvoidsPlayer x1`：pass
  - `NpcNpcCrossing x1`：pass
- 当前恢复点：
  - 后续若继续导航检查 V2，实现主刀仍旧只该停留在 `PlayerAutoNavigator.cs`；
  - 不应回漂到 solver、大架构或静态线。

## 2026-03-29（父线程接受“已到可用地步”但不接受收口，导航线下一刀继续锁终点前过早失活）

- 当前新增事实：
  1. 用户真实手测已明确确认：
     - 单个静止 NPC 的体验比以前“好了非常多”，已到“可以用”的地步；
     - 但 crowd 仍会挤在中间过不去；
     - 终点有 NPC 停留时，玩家仍会在终点附近反复避让/顶撞。
  2. 结合 `导航检查V2` 最新回执与父线程只读核查，当前最窄责任点继续留在：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
     - `detour/rebuild` 已进入后，未到点却掉成 `Inactive/pathCount=0` 的终点前过早失活链。
- 本轮父层动作：
  1. 接受上一轮 `导航检查V2` 为有效 checkpoint，但明确不接受“已接近收口”的口径；
  2. 新增对子线程的下一轮 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-29-导航检查V2-PlayerAutoNavigator-detour后过早失活与终点blocker语义-14.md`
  3. 新增父线程验收清单：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-03-29-父线程验收清单-PlayerAutoNavigator-detour后过早失活与终点blocker语义-14.md`
- 当前父层判断：
  1. 导航主线当前已经不是“完全不能用”的阶段；
  2. 但它仍不是“可收口”的阶段；
  3. crowd 挤住与终点 NPC 反复避让，下一轮必须先判是否同属当前这条终点前失活链，不能直接开成新的大责任簇。

## 2026-03-29（父线程静态线实跑后，阻塞已从“等独占窗口”升级为“当前 scene 基线错误”）

- 当前新增事实：
  1. 父线程已按用户要求立即重跑静态 menu，不再停在“只差独占窗口”的旧口径上；
  2. 这次静态 fresh 已真实跑到：
     - `case_end`
     - `all_completed=False passCount=1 caseCount=2`
  3. 但 fresh 结果显示：
     - `StaticPointCase1` 从异常 origin `(-16.33, 15.96)` 出发，最终 `centerDistance=13.001`
     - `StaticPointCase2` 正常通过，`centerDistance=0.024`
  4. 父线程进一步只读核对发现：
     - 当前 active scene 是 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
     - 不是此前默认为导航验证基线的 `Assets/000_Scenes/Primary.unity`
     - 且当前 working tree 下后者实体已不存在
- 当前父层判断：
  1. 静态线当前不能再沿用“只差独占窗口再复跑”的说法；
  2. 这条线已经真正跑出了 fresh 结果，但结果证明当前 scene 基线本身不对；
  3. 因而静态线的新第一阻塞已改判为：
     - `scene baseline mismatch`
     - 而不是单纯的 Unity live 窗口问题。

## 2026-03-29（父线程继续只读核后，修正静态线判断：scene 迁移异常仍在，但 `origin=-16.33` 更像 runner 绑错对象）

- 当前父层新增事实：
  1. 当前 active scene 与 Unity 内存态 Build Settings 解析结果都指向：
     - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
     - 但磁盘字面的 `ProjectSettings/EditorBuildSettings.asset` 仍写旧路径 `Assets/000_Scenes/Primary.unity`
     - 同一 GUID 仍是 `a84e2b409be801a498002965a6093c05`
  2. 父线程继续只读搜索 scene 内容后确认：
     - 当前 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 内确实有 `NavigationRoot`、`Player`、`001/002/003_HomeAnchor`
     - 所以它是“完整主场景搬家后的异常路径面”，不是“小 UI scene 误入 Build Settings”
  3. `Editor.log` 里的静态 validation 轨迹继续证明：
     - `origin=(-16.33, 15.96)` 不是所有 run 的常态
     - 同一 runner 在其它 fresh run 也能从 `(-8.16, 7.38)` 正常起跑
  4. 结合代码热区，当前最窄技术责任点更像：
     - `NavigationStaticPointValidationRunner.cs` 的 `EnsureBindings()`
     - 在 scene / backup / domain reload 后，如果 `playerNavigator` 变了但 `playerRigidbody / playerCollider` 没同步重绑，就会污染 `GetActorPositionForCenter()` 与 `ResetPlayerToRunStart()`
     - 这比“普通点导航本体稳定飞到错误远点”更符合当前日志
- 当前父层判断修正为：
  1. scene 路径/GUID 异常仍然是真的，且仍属于需要治理报实的 high-risk 搬家事故；
  2. 但把静态技术 blocker 一句压成 `scene baseline mismatch` 已经不够准确；
  3. 当前静态线更合理的拆法是：
     - `scene incident`：`Primary` 主场景路径/GUID/资产位置异常
     - `runner incident`：static validation 自身在 reload 后可能拿到混合引用，导致 `origin=-16.33` 这类巨偏假象
- 当前父层恢复点：
  - 如果父线程后续继续静态线，最小技术切片应先锁 `EnsureBindings()` 的一致性，而不是直接把全部火力打到 scene 去留；
  - 动态导航主线仍保持不变，继续由 `导航检查V2` 在 `PlayerAutoNavigator.cs` 内收终点前过早失活链。

## 2026-03-29（父线程已把 static runner 的绑定一致性补口落地，但 fresh live 被外部编译红灯卡住）

- 当前父层新增动作：
  1. 父线程已按上一轮诊断，直接在 `NavigationStaticPointValidationRunner.cs` 内把 `EnsureBindings()` 收紧为：
     - 每次都从当前 `playerNavigator` 同步重绑 `Rigidbody2D / Collider2D`
     - 不再允许 reload 后沿用旧组件引用
  2. 最小静态校验通过：
     - `validate_script => errors=0 warnings=2`
     - `git diff --check` 通过
  3. 但在准备进入 compile-clean 后 fresh live 时，Unity 现场撞到了外部 compile blocker：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs(1266,36)`
     - `CS0103: GetDetailFlowFloorTop does not exist in the current context`
     - 该文件当前已是 dirty，不属于父线程这轮静态 runner 触点
- 当前父层判断：
  1. 静态线这轮已经从“只读诊断”推进到“最小补丁已落地”；
  2. 但它还没有进入“fresh live 已确认补丁有效”的阶段；
  3. 当前新的第一外部阻塞不是 scene，也不是 runner 逻辑，而是 shared Unity compile-red 现场；
  4. 所以下一步若继续静态线，前提先清外部编译红灯，再复跑 static menu 看 `origin=-16.33` 是否消失。

## 2026-03-29（动态导航 `-14`：`PlayerAutoNavigator` 终点前完成时机已继续补口，但本轮 live 被外部 PromptOverlay 编译红错截断）

- 当前父层新增事实：
  1. `导航检查V2` 这轮继续只锁 `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`。
  2. 当前补口继续压在：
     - `TryFinalizeArrival(...)`
     - `HasReachedArrivalPoint(...)`
     - `CompleteArrival()`
     - detour clear / recover 后的短 settle 保持条件
  3. 新增 runtime 护栏包括：
     - detour 仍 active 时不允许直接点导航完成
     - detour clear / recover 后给一个短 settle 完成窗口
     - `CompleteArrival()` 打出 `Resolved / Requested / Transform / Collider` 证据
  4. 目标脚本最小代码闸门通过：
     - `validate_script(PlayerAutoNavigator.cs) => errors=0 warnings=2`
     - `git diff --check` 通过
  5. 但 shared root 当前 fresh compile 已被别线新红错卡住：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - `PageRefs.pageCurlImage` 缺失
     - 当前 console 有 3 条 `CS1061`
- 当前父层判断：
  1. 动态导航主线这轮没有漂移，责任点仍压在 `PlayerAutoNavigator` 的终点前完成语义；
  2. 但这轮不能 claim 新的 fresh live 结论，因为 compile blocker 不在允许 scope 内，且会阻断新补口真正进 runtime；
  3. 所以下一轮若继续动态线，优先条件不是“再多想一点 crowd”，而是先等 shared root 回到 compile-clean，再重拿 `Single/Crowd/Push/NpcAvoids/NpcNpc` 最小 fresh。

## 2026-03-29（父线程复审 `-14` 后，正式把动态下一轮改判为“fresh compile/live 裁定 + blocker 报实”）

- 当前父层新增事实：
  1. 父线程已对照 `-14` prompt、`-14` 验收清单和 `PlayerAutoNavigator.cs` 热区复核，确认：
     - `TryFinalizeArrival / ShouldDeferActiveDetourPointArrival / TryHoldPostAvoidancePointArrival` 这一簇确实已落代码
     - `-14` 这轮没有 scope 漂移
  2. 但父线程同时核到：
     - `SpringDay1PromptOverlay.cs`
     - `SpringDay1WorkbenchCraftingOverlay.cs`
     当前脚本级校验都是 `errors=0`
     - console 也没有保留 `-14` 回执里那组 `pageCurlImage CS1061`
     - 因而 `-14` 把“external blocker”直接表述成当前停止理由，并不够报实
  3. 父线程已正式新建：
     - `2026-03-29-导航检查V2-PlayerAutoNavigator-完成语义fresh裁定与blocker报实-15.md`
     - `2026-03-29-父线程验收清单-PlayerAutoNavigator-完成语义fresh裁定与blocker报实-15.md`
- 当前父层判断：
  1. `-14` 的结构补口 checkpoint 可以保留；
  2. 但下一轮最重要的已经不是“再多讲一点 still fail 的理论”，而是先把：
     - 当前 compile 是否真的红
     - 当前 blocker 是否真是最新 blocker
     - compile clean 后 fresh 最小 live 到底怎么判
     这三件事做实
  3. 所以下一轮动态主刀仍锁同一条 `PlayerAutoNavigator` 完成语义链，但提示词已经收紧成：
     - fresh compile / console truth
     - fresh 最小 live
     - 若 still fail，再继续只在同一簇内压责任点

## 2026-03-29（动态导航 `-15` 已把 fresh compile truth 钉死：当前最新 blocker 不是旧 `PromptOverlay`，而是 untracked 的 `SpringUiEvidenceMenu`）

- 当前父层新增事实：
  1. `导航检查V2` 已按 `-15` 的先后顺序先做 fresh compile / console 报实，没有再拿旧 blocker 顶账。
  2. 在 Unity 实例 `Sunset@21935cd3ad733705` 上，针对当前 active scene `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 做了两轮完全 fresh 的：
     - `clear console -> request compile -> wait -> read_console`
     两轮结果一致。
  3. 当前最新 compile blocker 已明确改判为：
     - `Assets/Editor/Story/SpringUiEvidenceMenu.cs(78,37) CS0104`
     - `Assets/Editor/Story/SpringUiEvidenceMenu.cs(154,52) CS0104`
     - `Assets/Editor/Story/SpringUiEvidenceMenu.cs(160,35) CS0104`
     - `Assets/Editor/Story/SpringUiEvidenceMenu.cs(191,39) CS0246`
  4. 当前 `SpringDay1PromptOverlay.cs / SpringDay1WorkbenchCraftingOverlay.cs` 的脚本级校验仍是 `errors=0`，说明父线程上一轮对 stale blocker 的担心是对的：
     - 旧的 `pageCurlImage` 口径不是这轮最新事实
  5. 当前 blocker 文件是 `?? Assets/Editor/Story/SpringUiEvidenceMenu.cs`，不属于动态导航这轮 own 路径，也不在允许 scope 内。
- 当前父层判断：
  1. `-15` 这轮已经完成了“把最新 compile truth 报实”这半刀；
  2. 但由于当前 shared Unity 仍 compile-red，本轮还不能要求 `导航检查V2` 继续拿 fresh live 矩阵；
  3. 对 `PlayerAutoNavigator` 完成语义链的技术判断暂不改写：
     - 一旦 compile-clean 恢复，下一 fresh 仍应优先继续审
       `HasReachedArrivalPoint / GetPlayerPosition / TryFinalizeArrival`
       这条“未到点却先 `Inactive/pathCount=0`”的完成判定链。

## 2026-03-29（全局警匪定责清扫第一轮：`导航检查V2` 自查已把 still-own / mixed / foreign 收窄）

- 当前新增结论：
  1. `导航检查V2` 当前 active still-own 已明确收窄为：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
     - 本线程自己的回执 / memory
  2. `NavigationLiveValidationRunner.cs / NavigationLiveValidationMenu.cs / NavigationLocalAvoidanceSolver.cs`
     当前更准确的定位是：
     - `历史碰过但现在不该继续 claim 为当前主刀`
  3. `GameInputManager.cs` 已被 `导航检查V2` 明确认定为：
     - `mixed hot dependency`
     - 当前不认 active own touchpoint
  4. `NavigationStaticPointValidationRunner.cs / Assets/Editor/NavigationStaticPointValidationMenu.cs`
     已明确归父线程 `导航检查` 的静态线；
     `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 则继续按 cross-thread same-GUID 场景事故面处理。
  5. `导航检查V2` 必须撤回的旧叙事已钉死：
     - `PromptOverlay` blocker
     - `Workbench` blocker
     - `SpringUiEvidenceMenu` blocker 停车位
     - 把 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 继续当成自己 fresh proof 现场
- 当前恢复点：
  - 第二轮若继续清扫，应先按这轮定责结果分发，而不是继续让 `导航检查V2` 凭历史范围顺手吞 runner / static / scene / GameInput。

## 2026-03-31（父层补记：清扫轮次开始前的导航全局阶段已重新统一口径）

- 当前新增结论：
  1. 清扫轮次开始前，导航线的真实阶段不是“已经只差 cleanup”，也不是“前面都白做”；
  2. 更准确的父层口径应是：
     - 运行时功能已经从失控态推进到部分可用；
     - 一些常规 probe 与整包 `RunAll` 曾连续拿到绿态；
     - `single 0.462` 也已被确认是 validation false positive；
     - 但高保真矩阵与用户真实手测共同证明，ground 契约、静止 NPC、moving NPC、crowd、终点前 blocker 这些体验层问题仍未过线；
     - 同时 same-root dirty 与 mixed-root 依赖仍在阻断安全归仓。
  3. 因而后续之所以同时出现：
     - `导航检查V2` 继续窄修 `PlayerAutoNavigator`
     - 父线程 / 子线程进入全局警匪定责清扫
     不是主线漂移，而是功能残差与工程收口在同一阶段并存。
- 当前恢复点：
  - 后续父层对外汇报“清扫前全局进度”时，统一使用上述三段式表述：
    - 有进展
    - 未过线
    - 也未具备安全收口条件

## 2026-03-31（父层补记：清扫前双线开发落点已拆清）

- 当前新增结论：
  1. 清扫前的导航开发实际上分成两条真实 active 线：
     - 父线程自己的静态点导航 / static validation runner 线
     - `导航检查V2` 的动态 `PlayerAutoNavigator` runtime 线
  2. 父线程线当时已推进到：
     - 普通点契约改到中心语义
     - static runner 多轮收口
     - `EnsureBindings()` 绑定一致性补丁已落
     - 只差 compile-clean 条件下重跑 static menu 做 fresh 复核
  3. `导航检查V2` 线当时已推进到：
     - pure bulldoze 被打破
     - 剩余责任点改判为 `detour/rebuild` 后未到点先 `Inactive/pathCount=0`
     - 父线程已连续准备好 `-14/-15/-16` 这一串同链 prompt
  4. 2026-03-29 的 cleanup 插入点，发生在“下一轮开发 prompt 已准备好、但用户尚未转发完”的时刻；因此后续看起来像“突然改做清扫”，本质上是开发链被治理批次中断，不是没有下一步。

## 2026-03-31（父层补记：基于 `导航检查V2` 新反省，已重新把下一步收成双线 `-17`）

- 当前新增结论：
  1. `导航检查V2` 的最新反省，父层可接受的部分是：
     - 它终于承认清扫前没做完的是 `PlayerAutoNavigator` 完成语义闭环，而不是“整条导航已经做完”
  2. 父层不接受的部分是：
     - 它仍有把“工程线已抬头”偷换成“对它来说下一步自然应切根接盘”的倾向
  3. 因而父层这轮已正式把下一步重拆成两份硬切片 prompt：
     - `导航检查V2`：`2026-03-31-导航检查V2-PlayerAutoNavigator-完成语义续工与fresh闭环-17.md`
     - 父线程自己：`2026-03-31-父线程自工单-静态点导航fresh复核与runner绑定闭环-17.md`
  4. 当前正式口径变为：
     - `导航检查V2` 继续只做动态完成语义闭环
     - 父线程继续只做静态 fresh 复核
     - 两边都不再拿“历史阶段分析”替代当前施工刀口

## 2026-03-31（父层补记：父线程静态线第一次执行 `-17` 停在 Unity 实例接入层 blocker）

- 当前新增结论：
  1. 父线程已真实开始执行自己的 `-17` 自工单，而不是继续停留在 prompt / 历史讨论；
  2. 当前静态线第一 blocker 不是 `NavigationStaticPointValidationRunner.cs` 代码本身，也不是 compile red；
  3. 最新现场是：
     - `unityMCP` 服务层已恢复可握手
     - 但 `mcpforunity://instances` 返回 `instance_count=0`
     - 所以当前没有 Unity 实例真正接入 server
  4. 因而父线程静态线当前准确状态应写成：
     - compile truth / static fresh 暂不可取
     - 外部 blocker 在 Unity 实例接入层

## 2026-03-31（父层补记：`导航检查V2` 已用 `-17` 收掉玩家点导航 premature inactive 链）

- 当前新增结论：
  1. `导航检查V2` 这轮已经把被 cleanup 打断的 `PlayerAutoNavigator.cs` 完成语义一刀真实续完；
  2. 首轮 fresh 已证实：
     - 玩家点导航会在 `Collider.center` 靠近终点后提前 `CompleteArrival()`
     - 当时 `Transform` 仍明显未到点
     - 并带出 `Inactive/pathCount=0`
  3. 它随后只在 `GetPlayerPosition()` 做了普通点导航 vs 跟随目标的语义拆分：
     - 点导航走 `Rigidbody/Transform`
     - 跟随目标仍走 `Collider.center`
  4. 同矩阵复核后：
     - `SingleNpcNear / MovingNpc / 终点 NPC 代理` 全部翻到 `playerReached=True`
     - `Inactive/pathCount=0` 签名消失
     - `Crowd raw` 仍 fail，但已改成独立的 crowd stall 残留
- 当前恢复点：
  - 从父层视角看，`导航检查V2` 的当前第一未完项已不再是完成语义，而是 crowd / passive blocker 的后续体验链；
  - 后续任何续工 prompt 都不应再把 `PlayerAutoNavigator` 的 premature inactive 旧锅写成当前主锅。

## 2026-03-31（父层补记：父线程静态线已把 fresh verdict 跑出来，普通点中心语义重新成为动态线当前主刀）

- 当前新增结论：
  1. 父线程自己的静态点导航切片，已经从“Unity 实例未接入，fresh 暂不可取”推进到“fresh static menu 已真实跑完并给出裁定”；
  2. 父线程这轮在 own scope 内补了两刀：
     - `NavigationStaticPointValidationMenu.cs`：validation 期间临时关闭并恢复 `Console Error Pause`
     - `NavigationStaticPointValidationRunner.cs`：每次 reset 时现算 `runStartActorPosition`
  3. 这两刀之后：
     - `case_start origin` 已恢复到 `(-8.16, 7.38)`
     - static runner 不再停在 `case_start`
     - `all_completed` 已能正常打出
  4. 最新 fresh 同时也把新的 runtime 责任点钉死了：
     - `StaticPointCase1 / 2` 都仍 `pass=False`
     - 但失败签名已经稳定变成：
       - `centerDistance≈1.04~1.12`
       - `rigidbodyDistance≈0.155~0.161`
       - `transformDistance≈0.155~0.161`
     - 且 `resolved target` 正确
  5. 这说明当前普通地面点导航的核心残差，不再是 static runner 或 fresh 工具链，而是：
     - `PlayerAutoNavigator.cs` 对普通点导航仍按 `Transform / Rigidbody` 收口
     - 没有按玩家实际占位中心收口
  6. 父线程已据此新建 `导航检查V2` 下一轮 prompt：
     - `2026-03-31-导航检查V2-普通地面点中心语义与static-fresh闭环-18.md`
- 当前恢复点：
  - 父线程静态线接下来不该继续扩题，保持为“runtime 修后再复跑 static fresh”的验证位；
  - `导航检查V2` 的当前主刀应从 crowd 暂时收窄回：
    - `PlayerAutoNavigator.cs`
    - 普通地面点导航中心语义
    - 用 static fresh 直接验。

## 2026-03-31（父层补记：重试复核后确认 `-18` 仍是 live 入口）

- 当前新增结论：
  1. 父线程已重新核对 `导航检查V2` 当前线程记忆，确认它最新已完成的是 `-17`，尚未越过 `-18`；
  2. 因而父层此前写出的
     `2026-03-31-导航检查V2-普通地面点中心语义与static-fresh闭环-18.md`
     仍是当前有效 live 入口，而不是过期 prompt；
  3. 当前这条线不需要再生新 prompt，也不需要父线程继续改 static runner；最正确的动作仍是把 `-18` 直接发给 `导航检查V2`。
- 当前恢复点：
  - 父线程继续保持静态验证位；
  - 动态线继续只收 `PlayerAutoNavigator.cs` 的普通点中心语义，修后再回父线程复跑 static fresh。

## 2026-04-01（父层补记：`-17` 不能被升格成体验主线完成；当前双线已重新改判）

- 当前新增结论：
  1. `导航检查V2` 最新回执里，真正成立的只有一层：
     - premature inactive / `Inactive(pathCount=0)` 旧链已被 fresh 收掉；
  2. 但它把下一步直接改判成 crowd-only，这个父层不接受；
  3. 当前最新用户真实体验说明，仍更高优先的是：
     - 普通点击点对齐仍可感知地不稳
     - 静止 NPC 贴近路过时仍会被顶着走
     - 两个近距 NPC 之间会摆动、卡顿、甚至无必要后撤
  4. 因而父层当前已重新把主线拆成：
     - `导航检查V2`：近距静止 NPC / 双 NPC 通道避让体验纠偏
     - 父线程：静态点契约回归与点击点上偏复核
  5. 对 `导航检查V2` 当前回执里两个说法的正式裁定：
     - `npcPushDisplacement≈1.0` 的代理 pass 不得升格成体验 pass
     - `targetTransform == null -> Rigidbody/Transform` 不得被默认为普通点导航最终口径
- 当前恢复点：
  - 后续父层不再接受“crowd-only 就是唯一主刀”的说法；
  - 继续推进时，先发 2026-04-01 这组双线 `-19`。

## 2026-04-01（父层补记：父线程并行 before-baseline 已经把 static / 点击点契约判成当前拒收）

- 当前新增结论：
  1. 父线程已在不改 runtime 的前提下，先跑出了一轮 `导航检查V2 -19` 开工前的 before-baseline；
  2. 最新 static verdict 不是“勉强还能接受”，而是当前明确拒收：
     - `StaticPointCase1 pass=False centerDistance=1.600 rigidbodyDistance=1.995 transformDistance=1.995`
     - `StaticPointCase2 pass=False centerDistance=1.120 rigidbodyDistance=0.081 transformDistance=0.081`
  3. 这说明两件事同时成立：
     - 点击点上偏/错层没有被证明已经消失；
     - 普通点契约甚至出现了比上一轮更坏的 case，当前不能放行
  4. 因而父层当前统一口径应更新为：
     - `导航检查V2` 继续修近距静止 NPC / 双 NPC 通道体验链
     - 父线程已经拿到一份可直接做前后对照的拒收 baseline
     - 等 `导航检查V2 -19` 回来后，父线程不再“重新理解”，而是直接对照裁定。
- 当前恢复点：
  - 父线程当前 thread-state 已合法 `PARKED`；
  - 下一个最小动作就是等 `导航检查V2 -19` 回执回来后，直接做 acceptance / rejection。

## 2026-04-01（父层补记：用户最新实机把动态主刀改判为 NPC 漫游异常中断；`导航检查V2` live 入口已升级到 `-20`）

- 当前新增结论：
  1. 用户最新截图和现场反馈表明，当前最灾难级坏相已经不再只是玩家近距 corridor 避让，而是：
     - roam 语义下 NPC 与玩家 / NPC 与 NPC 在同一点附近互挤、互卡、鬼畜不止；
  2. 用户给出的新设计裁定已明确：
     - 当前漫游导航只要出现一次明确异常，就应取消本次漫游段；
     - 并且必须把中断原因向上抛出，留给后续 NPC 逻辑接手；
  3. 因而 `导航检查V2` 的当前 live 主刀已从 `-19` 的玩家近距体验纠偏，进一步升级成：
     - `NPCAutoRoamController.cs`
     - roam move 的异常即中断
     - roam interruption reason / hook
  4. 对整体盘面的统一口径同步更新：
     - static / 点击点上偏：仍未解决，继续保持拒收
     - 玩家 premature inactive 旧链：已收掉，但不能再当当前主刀
     - 新第一主刀：NPC roam fail-fast interrupt
- 当前恢复点：
  - `导航检查V2` 若继续，直接转发：
    - `2026-04-01-导航检查V2-NPC漫游异常中断与鬼畜止血-20.md`
  - 父线程继续作为 acceptance / 拒收位，不参与这轮 runtime 写入。

## 2026-04-01（父层补记：复审 `导航检查V2` 昨夜回执后，`-20` 已再升级成 `-21`）

- 当前新增结论：
  1. `导航检查V2` 昨夜那份回执不能按 `-20` 审，因为它实际还没收到 `-20`；
  2. 所以父层对它昨夜回执的正式裁定应是：
     - 承认 `-19` 上 single 静止 NPC 推挤有部分进展
     - 但同时明确 `-19` 未闭环，不能继续拖成长窗主线
  3. 因而父层把 live 入口进一步升级成：
     - `2026-04-01-导航检查V2-冻结19并转NPC漫游异常中断-21.md`
  4. `-21` 相比 `-20` 的关键新增，是：
     - 明确要求先冻结 `-19` 当前 partial checkpoint
     - 再切到 `NPCAutoRoamController.cs` 的 roam fail-fast interrupt
     - 不再允许它一边承认 `-19` 未闭环，一边继续无限补 single/corridor 样本
- 当前恢复点：
  - 当前应转发 `V2 -21`；
  - `V2 -20` 保留为阶段中间稿，不再视为最新 live 入口。

## 2026-04-01（父层补记：static 拒收报告与 `V2 -21` 验收尺已经落成文档）

- 当前新增结论：
  1. 父线程不再只有零散 memory，而是已经把当前验收层收成正式文档：
     - `2026-04-01-父线程-static拒收报告与V2-21验收尺-22.md`
  2. 这份文档当前固定了三层父层事实：
     - static / 点击点偏上仍拒收
     - `V2 -21` 回来时该怎么审
     - `unityMCP` 当前 listener 缺失，属于工具层真实 blocker
  3. 因而当前整体盘面更清楚了：
     - 实现线：`V2 -21`
     - 验收线：`-22` 文档
     - 工具线：先恢复 `unityMCP` listener，再做下轮 live 验收
- 当前恢复点：
  - 父线程后续不再需要重新组织验收思路；
  - 直接等 `V2 -21` 回来，按 `-22` 文档裁定。

## 2026-04-01（父层补记：等待 `V2 -21` 期间又完成了一轮关键预审，`NPCAutoRoamController` interruption 已不是“从零创建”问题）

- 当前新增结论：
  1. 父线程继续只读扫盘后，确认当前 workspace 中的 `NPCAutoRoamController.cs` 已经存在 interruption 骨架：
     - reason
     - snapshot
     - event
     - debug 暴露
     - 多个调用点
  2. 因而父层后续对 `V2 -21` 的验收重点已经重新收窄：
     - 不再问“有没有从零创建 interruption contract”
     - 而是问“这套现有 contract 有没有真正打中实际 roam 互卡坏相”
  3. 同时已明确：
     - `NpcAvoidsPlayer / NpcNpcCrossing` 目前只走 `DebugMoveTo(...)`
     - 它们是 guardrail，不是 roam live 证明
  4. 这轮预审已单独收成文档：
     - `2026-04-01-父线程-V2-21预审风险清单与验证入口盘点-23.md`
- 当前恢复点：
  - 父层现已同时拥有：
    - `-22` 拒收 / 验收尺
    - `-23` 风险清单 / 验证入口边界
  - 等 `V2 -21` 回执回来后，不再重新理解大盘，直接按这两份文件裁定。

## 2026-04-01（线程侧补记：`-19` 中途停车时的真实进度，供后续回溯责任点）

- 当前新增事实：
  1. 在父层把 live 入口升级到 `-20` 之前，`导航检查V2` 实际已经按 `-19` 推进到一半，并拿到了一组 fresh 事实；
  2. `-19` 期间 baseline 先证实：
     - `SingleNpcNear raw ×3` 仍有 `npcPushDisplacement=1.017 / 0.796 / 1.018`
     - `Crowd raw ×1` 仍是 `pass=False / directionFlips=2 / crowdStallDuration=0.726`
  3. 线程侧在允许热区内只保留了 1 个最终补口：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
     - 收紧 `ShouldBreakSinglePassiveNpcPathMoveBulldoze(...)` 对“持续顶人时仍不升级”的门槛
  4. 线程侧曾试探性改过 `NavigationLocalAvoidanceSolver.cs`，但 fresh live 证明 crowd 代理恶化，已全部回退，不留最终 diff；
  5. 只看最终保留代码时，`SingleNpcNear raw` 已拿到改善样本：
     - `npcPushDisplacement=0.328`
     - `npcPushDisplacement=0.149`
     但也出现了 1 条异常超时样本：
     - `playerReached=False / timeout=6.51 / npcPushDisplacement=0.000`
- 当前阶段判断：
  - `-19` 没有闭环完成；
  - 但它确实把“单静止 NPC 推挤”的第一责任点进一步压到了 `PlayerAutoNavigator.ShouldBreakSinglePassiveNpcPathMoveBulldoze(...)`
  - 同时也反证：双 NPC / corridor 侧的 solver 泛调不是这轮最小可靠补口
- 当前恢复点：
  - 这段记录只用于后续回溯 `-19` 到底做到哪里；
  - 不覆盖父层已经改判到 `-20` 的最新 live 入口。

## 2026-04-01（工作区补记：用户要求中途阶段汇报时，`-19` 的最终保留代码与停车状态）

- 当前新增事实：
  1. 用户在 `-19` 尚未闭环时要求直接阶段汇报，因此线程未继续推进到新主刀 `-20`，而是先把当前 slice 停在可回溯状态；
  2. 停车当下，最终保留代码仍只有：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
  3. `NavigationLocalAvoidanceSolver.cs` 已回退到基线，不留最终 diff；
  4. 当前 `unityMCP` 服务层在停车前再次掉线：
     - `check-unity-mcp-baseline.ps1 => listener_missing`
  5. 线程状态已按 live 规则执行：
     - `Begin-Slice` 已跑
     - `Park-Slice` 已跑
     - 当前 state=`PARKED`
- 当前阶段判断：
  - 这不是一轮“已完成待 sync”的 checkpoint；
  - 当前只能判为：
    - 单静止 NPC 推挤已有改善样本
    - 单场景稳定性仍未收干净
    - 双近距 NPC 通道仍未闭环
    - 需要下一轮按最新裁定继续，而不是现在 claim done

## 2026-04-01（导航检查V2：`-21` 已把 roam interruption 契约落地，但真实 roam fresh 仍缺 1 组）

- 当前新增事实：
  1. `-19` 已正式被收缩成 carried partial checkpoint：
     - 仍只保留 `PlayerAutoNavigator.cs` 的 single 推挤改善事实；
     - 但这轮主刀已切走，不再继续扩它。
  2. `导航检查V2` 当前在 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 内，已经把普通 roam move 的异常即中断口真正接上：
     - stuck cancel
     - stuck recovery fail
     - shared avoidance repath fail
     - shared avoidance recover / clear
     都会进入同一套 interruption snapshot / debug 读口。
  3. 受控 guardrail fresh 已重新转绿：
     - `NpcAvoidsPlayer pass=True`
     - `NpcNpcCrossing pass=True`
  4. 但“真实 roam 互卡”这条最新 live 主证据，这轮短窗仍未抓到：
     - 自然 roam Play 窗口里没有出现新的 `roam interrupted =>` 日志
- 当前阶段判断：
  - 当前不是零进展；
  - 也不是可以收工；
  - 而是已经完成 roam fail-fast 代码落地与 guardrail 复核，但还差 1 组真正的 roam fresh 才能把 `-21` claim done。
- 当前恢复点：
  - 后续若继续 `-21`，只需继续围绕同一 controller 的真实 roam 复现与新 debug 口取证；
  - 不需要再回到 `-19` 的 player 单静止 NPC 微调。

## 2026-04-01（父层补记：等待 `V2 -21` 回执期间，又新增一条只读高风险预审结论）

- 当前新增结论：
  1. 父线程在等待 `V2 -21` 回执期间继续做了只读中途预审，没有去抢 runtime 主刀；
  2. 当前 `导航检查V2` 的 active 现场仍合规：
     - `status=ACTIVE`
     - `owned_paths` 只锁在 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  3. 只读 diff 后，父层新增钉死了一个比“有没有 interruption”更细的新风险：
     - 当前 `TryReleaseSharedAvoidanceDetour(...)` 在 `detour.Cleared || detour.Recovered` 时，会直接触发
       `TryInterruptRoamMove(RoamMoveInterruptionReason.SharedAvoidanceRecovered, ...)`
     - 这高概率会把“正常绕开后恢复主路”也误判成异常中断
  4. 父线程已把这条新增风险单独收成补充文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-01-父线程-V2-21中途预审补充-SharedAvoidanceRecovered过宽风险-24.md`
- 当前恢复点：
  - 后续父层对 `V2 -21` 的收件，除了 `-22` 和 `-23`，还必须带上 `-24` 一起审；
  - 换句话说，下一次不只问“是不是会中断”，还要问“是不是只在真正异常时才中断”。

## 2026-04-01（父层补记：`V2 -21` 最新回执已复审，当前 live 入口已继续推进到 `-25`）

- 当前新增结论：
  1. `V2 -21` 最新回执不能放行为 done，但也不是打回重做；
  2. 父层当前正式接受：
     - `-19` 已被冻结
     - `NPCAutoRoamController.cs` 的 interruption contract 已落
     - `NpcAvoidsPlayer / NpcNpcCrossing` 当前未被它带坏
  3. 父层当前正式不接受：
     - 在没有真实 roam fresh 的前提下 claim `-21` 完成
     - 在没有正面处理 `SharedAvoidanceRecovered` 过宽风险的前提下 claim interruption 边界已成立
  4. 因而父层已把下一轮 live 主刀进一步收窄为：
     - `SharedAvoidanceRecovered / Clear` 到底是不是异常 interruption
     - 以及“异常会中断、正常恢复不会误伤”的成对 fresh 证据
  5. 对应新 prompt 已落盘：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-01-导航检查V2-收窄Recovered边界并拿真实roam中断证据-25.md`
- 当前恢复点：
  - 若继续放行 `导航检查V2`，现在应转发 `-25`
  - 父层后续收件时，要把 `-24` 的过宽风险和 `-25` 的成对证据要求一起带上裁。

## 2026-04-01（屎山修复父层补记：`导航检查V2 -25` 已完成边界收窄与 roam 入口补口，但 fresh live 被外部 compile blocker 卡住）

- 当前新增事实：
  1. `导航检查V2` 这一刀已把 `NPCAutoRoamController.cs` 中 `SharedAvoidanceRecovered / Clear` 从“广义异常中断”撤回为正常恢复主路链；
  2. 同时只在 `NavigationLiveValidationRunner.cs` 新增了最小真实 roam 证据入口，没有回漂 menu / scene / player / solver；
  3. 本地白名单代码闸门已通过，但 Unity fresh compile 当前被外部 Editor 测试装配阻断：
     - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
     - `Sunset / TMPro / SpringDay1ProximityInteractionService` 三处 `CS0246`
  4. 因为这条外部 compile 红面，`RunNpcRoamRecoverWindow` 的 `PendingAction` 当前仍未被消费，说明本轮 Play 没有真正进入新增 roam probe
- 当前阶段判断：
  - `导航检查V2 -25` 不是空转；
  - 也不是 fresh 已完成；
  - 正确状态是：
    - `代码边界与 live 入口已成立`
    - `成对 roam fresh 仍待拿`
    - `第一真实 blocker 已从“自然 roam 不稳定”升级为“外部 Editor 测试装配 compile 红面”`
- 当前恢复点：
  - 父层若后续继续裁定 `-25`，应先把这条外部 compile blocker 明确报实；
  - 清掉它之后，`导航检查V2` 不需要重开题，只需直接跑新增的两个 roam probe 拿成对 fresh。

## 2026-04-01（屎山修复父层补记：`导航检查V2 -25` 已拿到成对 roam fresh，但同轮 latest guardrail fresh 转红）

- 当前新增事实：
  1. `导航检查V2 -25` 已用 fresh compile + fresh live 纠正了上一版“外部 compile blocker 卡住”的旧判断；
  2. 当前已拿到两条关键 fresh：
     - `NpcRoamRecoverWindow pass=True`
       - `detourCreates=1`
       - `releaseSuccesses=1`
       - `interruption=False`
     - `NpcRoamPersistentBlockInterrupt pass=True`
       - `reason=StuckCancel`
       - `trigger=StuckCancel`
       - `blockerKind=NPC`
       - `blockerId=-2162`
  3. 这说明父层 `-24 / -25` 真正要收的两件事已经成立：
     - `SharedAvoidanceRecovered / Clear` 不再广义误伤成异常 interruption；
     - 真实异常 interruption 也确实还能被抓到。
  4. 但同轮最新 guardrail fresh 已转红：
     - `NpcAvoidsPlayer pass=False`
     - `NpcNpcCrossing pass=False`
- 当前阶段判断：
  - `导航检查V2 -25` 不是零进展；
  - 但也不能 claim done；
  - 正确状态是：
    - `核心边界与成对 fresh 已成立`
    - `guardrail 回归失败，当前仍有 blocker`
- 当前恢复点：
  - 父层后续若继续裁定这一线，不该再要求它重做 `Recovered/Clear` 成对证据；
  - 而应只追最新两个 guardrail 为什么在最新 runtime 现场变红。

## 2026-04-01（屎山修复父层补记：导航 static 偏上仍 open，且 `导航检查V2` 当前 active slice 未覆盖该问题）

- 当前新增事实：
  1. 父线程已只读核对 `导航检查V2` 当前 `thread-state`：
     - 状态=`ACTIVE`
     - 切片=`收窄Recovered边界并拿真实roam中断证据-25`
     - 当前认领路径只包含：
       - `NPCAutoRoamController.cs`
       - `NavigationLiveValidationRunner.cs`
       - own docs / thread memory
  2. 这说明 `导航检查V2` 现在继续推进的是 NPC roam / guardrail 线，
     不是用户此刻最痛的 static 点偏上线。
  3. 父线程只读核对 `PlayerAutoNavigator.cs + Primary.unity` 后确认：
     - 点导航仍按 `Rigidbody/Transform` 收口；
     - 玩家碰撞体中心仍相对根节点上偏约 `1.2`
     - 所以“点击点在脚下 / 角色中心却停在更上面”的现场仍然存在
- 当前阶段判断：
  - `导航检查V2 -25`：
    - 不是空转；
    - 但也没有把导航整体收完
  - 整体导航：
    - 动态/NPC roam 线仍在施工；
    - static 点偏上线仍未关闭
- 当前恢复点：
  - 后续若继续治理裁定，不允许再把“`V2` 还在 active”偷换成“static 也在跟着推进”；
  - 若用户要先止住最基础可感知问题，需单独拉起 static / 点击点契约切片。

## 2026-04-01（屎山修复父层补记：`导航检查V2` 最新回执只能证明 `-25` slice 收口，不足以证明整体右键导航可用）

- 当前新增事实：
  1. `NavigationLiveValidationRunner.cs` 的最新 diff 的确表明：
     - `-25` 这轮补的是 managed roam probe 的参数 snapshot / restore；
     - 这能合理解释前一轮 latest guardrail 红是 probe 污染，而非 `NPCAutoRoamController` runtime 语义回退
  2. 但与此同时：
     - `PlayerAutoNavigator.cs` 仍处于 dirty；
     - static 点偏上这条线没有新的关闭证据；
     - 用户真实右键手测明确给出“还是胡闹”
- 当前阶段判断：
  - `导航检查V2 -25`：
    - 现在最多只能判为 `targeted probe / 局部验证` 过线
  - 整体导航体验：
    - 仍不能 claim `真实入口体验` 过线
- 当前恢复点：
  - 后续治理口径必须明确区分：
    - `-25` 局部 slice 过线
    - 整体右键导航仍未过线
  - 不允许再把局部 probe 绿面偷换成玩家真实体验已成立。

## 2026-04-01（屎山修复父层补记：已向 `导航检查V2` 下发 `-26`，主线正式回拉到玩家右键真实入口）

- 当前新增事实：
  1. 父层已正式生成并下发：
     - `2026-04-01-导航检查V2-强制回拉玩家右键真实入口主线-26.md`
  2. 这意味着导航当前治理口径已明确改成：
     - `-25` = 局部 checkpoint
     - `-26` = 玩家真实右键入口主线
- 当前阶段判断：
  - 导航整体仍未过线；
  - 但治理位已经把下一刀从“继续讲局部 roam”改回了“直接打用户正在骂的主问题”。
- 当前恢复点：
  - 后续只按 `-26` 收件与裁定；
  - 不再接受把 `-25` 的局部绿面写成整体导航闭环。

## 2026-04-01（屎山修复父层补记：`-26` 最新回执当前只能放行为“继续停在 Player 线”，不能放行根因收敛）

- 当前新增事实：
  1. `导航检查V2` 最新 `-26` 回执已经明确承认：
     - crowd / 双近距 NPC 通道仍未过真实入口体验线
     - 当前线程已 `PARKED`
  2. 这部分可以接受；
     说明它没有再把本轮说成 done。
  3. 但代码与 prompt 对照后，父层当前不接受它把 remaining 问题继续压成“只剩 slow-crawl 一个点”：
     - 终点 NPC dedicated case 仍缺
     - corridor 识别、arrival blocker、late finalize 三簇仍同时可疑
- 当前阶段判断：
  - 导航整体仍在 Player 主线施工中；
  - 当前正确管理方式不是放它收口，而是继续钉 Player 线多簇共同责任。
- 当前恢复点：
  - 如果下一轮继续治理分发，应继续压：
    - dedicated 终点 NPC case
    - corridor / arrival / finalize 三簇共同排查
  - 不接受“只剩一个 crowd slow-crawl 条件”的过早收束。

## 2026-04-01（屎山修复父层补记：已向 `导航检查V2` 下发 `-27`，治理重点改成 dedicated 终点 NPC case + 多簇共同责任）

- 当前新增事实：
  1. 父层已下发 `-27`；
  2. `-27` 明确禁止它继续把问题写成单点 slow-crawl；
  3. `-27` 强制补 dedicated 的“终点有 NPC 停留” case。
- 当前阶段判断：
  - 导航整体仍在 Player 主线施工中；
  - 当前治理位已经把下一刀从“继续压 slow-crawl 指标”改成“先把 dedicated case 和多簇责任钉死”。
- 当前恢复点：
  - 后续只按 `-27` 收件；
  - 不再接受只拿 crowd 代理长期替代 dedicated 终点 NPC case。

## 2026-04-01（屎山修复父层补记：`导航检查V2 -25` latest guardrail 转红已证实是 runner probe 污染，不是 NPC roam runtime 语义重新坏掉）

- 当前新增事实：
  1. `导航检查V2` 已继续只锁：
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
  2. latest guardrail 转红的第一责任点已经重新压出来：
     - 不是 `Recovered/Clear` 又误伤；
     - 也不是 `NPCAutoRoamController` runtime release/recover 语义回退；
     - 而是 `NavigationLiveValidationRunner` 的 managed roam probe 会直接改 `NPCAutoRoamController` 运行参数，且旧版本没有在下一条 scenario 前恢复默认值。
  3. 现已在 runner 内补上 controller tuning snapshot / restore 收口：
     - `PrepareScene(...)`
     - `FinishRun()`
     - `AbortRun()`
     都会恢复 managed roam probe 的临时调参。
  4. fresh 结果已经闭回绿面：
     - `NpcRoamPersistentBlockInterrupt pass=True`
     - 同一 Play 会话紧接着：
       - `NpcAvoidsPlayer pass=True`
       - `NpcNpcCrossing pass=True`
     - 回到 `Edit Mode` 后再跑：
       - `NpcRoamRecoverWindow pass=True`
- 当前阶段判断：
  - `导航检查V2 -25` 当前已经不是“核心证据绿但 guardrail 红”的 blocker 状态；
  - 现阶段正确口径改为：
    - `core roam + same-play guardrail 都已 fresh 通过`
    - `可进入 Ready-To-Sync / sync 收口`
- 当前恢复点：
  - 父层后续不应再要求它回头重证 `Recovered/Clear`；
  - 若继续治理裁定，重点应转到：
    - 这刀是否已真实归仓
    - 以及 static 点偏上线是否另起 slice。

### 2026-04-01 追加尾注（收口态）

- `导航检查V2 -25` 当前 runtime 与 fresh 证据已闭环；
- 但 `Ready-To-Sync` 被 broad own roots 阻断，当前 thread-state 已改为 `PARKED`；
- 后续若继续治理，不该再问“功能是不是还没修好”，而应只处理：
  - 同根他线 dirty/untracked 怎么归仓/认领
  - 以及 `导航检查V2` 何时能重新拿到可 sync 的窄白名单

## 2026-04-01（屎山修复父层补记：`导航检查V2 -26` 右键玩家主线已把 static contract 与 single 线拉正，但 crowd 仍停在 player-side 晚段 completion 链）

- 当前新增事实：
  1. `导航检查V2` 已真实转到 `-26` 的玩家右键入口主线，当前只锁：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
     - 只读借用 `NavigationLiveValidationRunner.cs` 取证
  2. 普通点导航 contract truth 已 fresh 重新钉死：
     - `Ground raw matrix pass=True`
     - `accurateCenterCases=6/6`
     - `positiveCenterBiasCases=0/6`
     - 说明点导航真实口径已回到 `Collider(center)`，
       不再让 `Transform/Rigidbody` 假绿掩掉“停在点上方”的问题
  3. `SingleNpcNear` 与 `MovingNpc` 也都保持 green：
     - `SingleNpcNear pass=True`
     - `MovingNpc pass=True`
     - 说明玩家单 NPC 近距线没有被 crowd 修法带坏
  4. crowd / 双近距 NPC 通道这条线现在的真实进度是：
     - 旧坏相：
       - 起步秒进 detour
       - `directionFlips=4`
       - `crowdStallDuration≈0.6`
     - 新坏相：
       - 基本不再 early detour / 来回倒转
       - `directionFlips` 已压到 `1`
       - 但仍剩 `crowdStallDuration≈0.77~1.40`
       - 最新 blocker 改成“通过 corridor 后，终点前 lingering blocked-input 慢堵”
- 当前阶段判断：
  - `导航检查V2 -26` 不是原地踏步；
  - 玩家入口主线已经从“ground+single 也不稳”推进到：
    - `ground 过线`
    - `single 过线`
    - `moving guardrail 过线`
    - `crowd 显著改善但仍 fail`
- 当前恢复点：
  - 父层后续若继续裁定这一线，不该再把它回拉去 NPC roam；
  - 正确下一刀仍是：
    - `PlayerAutoNavigator.cs`
    - `TryGetPointArrivalNpcBlocker(...) / TryFinalizeArrival(...) / crowd late completion`
  - 也就是继续只打玩家侧晚段 lingering blocker 语义，而不是重开 solver 或 scene。

## 2026-04-01（屎山修复父层补记：`导航检查V2 -26` 续跑后只站住一刀 detour 执行语义改善，crowd 仍未过真实入口体验线）

- 当前新增事实：
  1. `导航检查V2` 本轮继续只锁：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
  2. 本轮最终只保留 1 个有效补口：
     - `ShouldUseBlockedNavigationInput(...)` 不再把 `_hasDynamicDetour` 直接视为必进 `BlockedInput`
     - crowd 最新 fresh 结果因此从“末段 `BlockedInput` 慢堵”压成“有真实 `DetourMove`，但整体仍慢”
  3. 当前最新 fresh 证据：
     - `Ground raw matrix pass=True`
     - `SingleNpcNear raw ×3 pass=True`
     - `MovingNpc raw ×1 pass=True`
     - `Crowd raw ×3` 仍 `pass=False`
       - `directionFlips=2 / 1 / 1`
       - `crowdStallDuration=1.385 / 1.462 / 1.469`
       - `detourMoveFrames=24 / 24 / 25`
       - `blockedInputFrames=0 / 2 / 3`
  4. 线程本轮还试过两次“更早 crowd repath”：
     - 都会把 `directionFlips` 和 `blockedInputFrames` 明显拉高
     - 已在本轮真实撤回，不留在当前代码里
- 当前阶段判断：
  - `导航检查V2 -26` 仍然只能算 partial checkpoint，不能 claim 玩家入口体验过线
  - 但相较上一 checkpoint，它已经把 crowd 的末段主坏相从“`BlockedInput` 慢蹭”压成了“detour 可接手，但 slow-crawl 仍太长”
- 当前恢复点：
  - 父层后续若再看这条线，不应再回到 `-25` 或 NPC roam；
  - 正确下一刀仍只应回到 `PlayerAutoNavigator.cs`
  - 当前最窄剩余责任仍在：
    - `ShouldDeferPassiveNpcBlockerRepath(...)`
    - `HasPassiveNpcCrowdOrCorridor(...)`
    - `ShouldBreakSinglePassiveNpcPathMoveBulldoze(...)`
    - crowd 晚段完成前的 `TryFinalizeArrival(...)`

## 2026-04-01（屎山修复父层补记：已把“终点有 NPC 停留”专案从 crowd 代理里拆出，形成 dedicated case 的最小设计与验收尺）

- 当前新增事实：
  1. 父线程这轮没有下场改 `PlayerAutoNavigator.cs`，而是继续做并行只读支撑；
  2. 经过只读盘点，当前 `NavigationLiveValidationRunner` 虽已有：
     - `GroundPointMatrix`
     - `SingleNpcNear`
     - `CrowdPass`
     - `MovingNpc`
     - `NpcAvoidsPlayer`
     - `NpcNpcCrossing`
     但仍 **没有** dedicated 的“终点有 NPC 停留” player-side case；
  3. 父线程已把这条缺口正式单独落成文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-01-父线程-终点NPC专案预审与验收尺盘点-28.md`
  4. 文档中已明确：
     - dedicated case 的最小正确落点应是：
       - `NavigationLiveValidationRunner.cs`
       - `NavigationLiveValidationMenu.cs`
     - 不应扩到：
       - `NavigationStaticPointValidationRunner.cs`
       - `Primary.unity`
       - `NavigationLocalAvoidanceSolver.cs`
       - `NPCAutoRoamController.cs`
  5. 文档同时把后续审件最关键的分类口径也先写死了：
     - `InteractionHijack`
     - `Bulldoze`
     - `Oscillation`
     - `Linger`
     - `Reached`
     - `StableHoldPending`
- 当前阶段判断：
  - 这不是新的 runtime 修复 checkpoint；
  - 这是父线程对“终点 NPC 专案”补出的 dedicated 证据设计与验收尺；
  - 它的价值在于：
    - 以后 `Crowd raw` 不再被允许长期代理这条终点占位坏相。
- 当前恢复点：
  - 后续父层若继续裁 `V2` 的 Player 主线，会直接拿 `-28` 追问：
    - dedicated case 有没有补
    - raw click 是否仍是真导航而非 interaction 劫持
    - lingering / 死避让到底被分到了哪一类

## 2026-04-02（屎山修复父层补记：`V2` dedicated endpoint 当前不是缺案子，而是假绿；已继续下发 `-29`）

- 当前新增事实：
  1. `V2` 这轮确实补出了 dedicated 的 `RealInputPlayerEndpointNpcOccupied`；
  2. 但父层复审后确认，这条专案当前的 `pass=True` 口径不可接受：
     - 它把
       - `点击点 point-arrival 成立`
       - 和
       - `被终点 blocker shell 挡住`
       混成了同一个 green；
  3. 代码现场实锤是：
     - `endpointArrivalTolerance = combinedRadius + 0.35`
     - `playerReached = !IsActive && (reachedByCenter || reachedByBlockedShell)`
     - 这使得 `playerCenterDistance ≈ 1.06 ~ 1.17` 仍可被报成 pass；
  4. 同时，当前 `NavigationLiveValidationMenu.cs` 里仍没有 dedicated endpoint 的标准菜单入口 / `PendingAction` / `ExecuteAction(...)` 分发，说明工具链也还没完整接回。
- 当前阶段判断：
  - 这条 dedicated endpoint 专案当前不是“未实现”；
  - 而是“已实现到 targeted probe 层，但 green 定义假，不能拿去代表真实入口体验成立”。
- 当前恢复点：
  - 父层已继续下发：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-导航检查V2-拒收dedicated终点NPC假绿并强制补真口径矩阵-29.md`
  - 下一轮如果 `V2` 继续 claim endpoint 过线，必须先证明：
    1. blocker shell hold 已从 green 中剥离；
    2. `pass=True` 已回到真实 point-arrival 合同；
    3. dedicated endpoint 已补上标准 toolchain 入口与最小 fresh matrix。

## 2026-04-02（屎山修复父层补记：已把 `-29` 的后续收件顺序与假绿拒收逻辑固化成 `-30`）

- 当前新增事实：
  1. 父线程在 `-29` 已经发出的基础上，没有停在“等回执再说”，而是继续补完了下一次收件时的固定验收单；
  2. 新文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-父线程-V2-29验收尺与假绿拒收清单-30.md`
  3. `-30` 里已经把父层下一次的收件顺序写死成：
     - 先审 green 定义
     - 再审 outcome 分类
     - 再审 raw click 是否被 interaction 劫持
     - 再审 toolchain 是否补齐
     - 最后才审 fresh matrix
  4. 也就是说，后续 `V2` 再回来时，父层不会再只看“你是不是多跑了几条”，而是先看它有没有把 fake green 真正剔掉。
- 当前恢复点：
  - 现在父层对这条 dedicated endpoint 专案已经有两份互补文档：
    - `-29` = 给 `V2` 的继续施工指令
    - `-30` = 父线程自己的固定拒收/放行清单

## 2026-04-01（屎山修复父层补记：`导航检查V2 -27` 已把 dedicated 终点 NPC 专案补成 fresh `raw ×3`，当前先停在 partial checkpoint）

- 当前新增事实：
  1. `导航检查V2` 已在 `NavigationLiveValidationRunner.cs` 内补出 dedicated 的：
     - `RealInputPlayerEndpointNpcOccupied`
     并保留 `pending_action.txt` 最小启动口，不需要再借 crowd 代理这条专案；
  2. 本轮 fresh `raw ×3` 已全部转为：
     - `pass=True`
     - `npcPushDisplacement=0`
     - `directionFlips=0`
     - `blockedInputFrames=0`
     - 说明当前 dedicated endpoint 专案已经不再表现为“推着 NPC 走 / 原地抖停 / 6.5 秒 lingering 假失败”
  3. 这轮补的不是 solver 或 scene，而是 endpoint case 自己的真实 throughline：
     - 真身 setup 已去掉旁观 NPC 干扰
     - runner 通过语义已收成 blocker-aware arrival shell
  4. 当前这条专案对三簇的裁定已经变清楚：
     - `HasPassiveNpcCrowdOrCorridor(...)`：在单 blocker endpoint case 里基本排除，不是主因
     - `TryGetPointArrivalNpcBlocker(...)`：仍是共同责任点
     - `TryFinalizeArrival(...) / ShouldHoldPostAvoidancePointArrival(...)`：仍是共同责任点
- 当前阶段判断：
  - `导航检查V2 -27` 不是 done；
  - 但它已经不再缺 dedicated endpoint 专案，也不再只有 crowd 代理；
  - 当前更像：
    - endpoint case 已站住
    - full matrix 还没按 `-27` 全量重跑
    - 所以先报 partial checkpoint，而不是 claim 玩家主线整体过线
- 当前恢复点：
  - 后续父层如果再审这条线，不该再追问“终点 NPC 专案有没有 dedicated case”；
  - 正确下一刀应继续只留在 Player 主线，判断：
    - endpoint blocker shell 完成语义
    - 与 crowd 晚段 lingering 的关系

## 2026-04-02（屎山修复父层补记：`导航检查V2 -29` 已把 dedicated endpoint 假绿剔掉，并用 fresh matrix 证明当前是真 lingering）

- 当前新增事实：
  1. `NavigationLiveValidationRunner.cs` 已把 dedicated endpoint 的通过定义纠正为：
     - 只有 `outcome=ReachedClickPoint`
     - 且 `playerCenterDistance <= 0.35`
     - 才允许 `pass=True`
  2. `NavigationLiveValidationMenu.cs` 已补回 dedicated endpoint 的标准菜单入口、`PendingAction` 与 `ExecuteAction(...)` 分发；
  3. fresh 最小矩阵已经补完，guardrail 结果是：
     - `Ground raw ×1`：`pass=True`
     - `SingleNpcNear raw ×1`：`pass=True`
     - `NpcAvoidsPlayer ×1`：`pass=True`
     - `NpcNpcCrossing ×1`：`pass=True`
  4. 关键 dedicated endpoint 主证据已从假绿改为真坏相：
     - `EndpointNpcOccupied raw ×3` 全部 `pass=False`
     - `caseValid=True`
     - `pendingAutoInteractionAfterClick=False`
     - `npcPushDisplacement=0`
     - `blockedInputFrames=0`
     - `outcome` 全部稳定为 `Linger`
     - `playerCenterDistance=1.019 / 1.031 / 1.073`
     - 说明现在不是交互劫持，也不是 blocker shell 被误算成到点，而是 dedicated endpoint 仍在“未到点击点就提前失活”
  5. `Crowd raw ×1` 仍 `pass=False`，当前 crowd 坏相与 dedicated endpoint 一样继续停在“未到点就提前 Inactive”的现实问题上，而不是被 `-29` 误修好。
- 当前阶段判断：
  - `导航检查V2` 这条 Player 主线现在已经完成了：
    - dedicated endpoint case 的 toolchain 收口
    - fake green 纠偏
    - fresh 真实坏相重报
  - 但还不能 claim endpoint 过线；
  - 当前最窄第一责任点已经重新压回 `PlayerAutoNavigator.cs` 完成语义链，而不再是 runner/tooling。
- 当前恢复点：
  - 下一刀如果继续，只应留在：
    - `TryFinalizeArrival(...)`
    - `ShouldHoldPostAvoidancePointArrival(...)`
    - `TryGetPointArrivalNpcBlocker(...)`
  - 不该再回漂到：
    - crowd 代理口径
    - NPC roam
    - solver
    - scene / static runner
## 2026-04-02（屎山修复父层补记：`导航检查` 已复审 `V2 -29` 新回执，并把下一刀压成 `-31`）

- 当前新增事实：
  1. 父层已对照代码现场确认：
     - dedicated endpoint fake green 确实已经从 runner 口径里剔掉
     - dedicated endpoint 的 menu/toolchain 也确实已接回
  2. 但父层没有接受：
     - “现在已经实锤就是提前失活真因”
     - 因为 `V2` 最新回执还没把 endpoint / crowd 失败瞬间的：
       - `IsActive / pathCount / pathIndex / DebugLastNavigationAction`
       - 以及具体 PAN 分支
       钉成 fresh 证据
  3. 父层也明确把“右键停位偏上”的体验问题重新压回现场：
     - center-only 结构绿不能再顶替用户可视停位真值
     - 用户最新真人反馈仍是“右键停位体感怪、像落在点击点上方”
- 当前新增文档：
  1. `-31`
     - 只允许 `V2` 回 `PlayerAutoNavigator.cs`
     - 先补 endpoint / crowd linger 真因
     - 再补 ground 可视停位真值，不准再假关闭
  2. `-32`
     - 父线程自己的固定验收尺
     - 下次优先审真因证据与右键停位偷换，再看 fresh matrix
- 当前阶段判断：
  - 导航 Player 主线仍未过线；
  - 当前从 `-29` 往下的唯一主刀，继续是：
    - `PlayerAutoNavigator.cs` 完成语义 / blocker 语义
  - runner / menu 已从 active root 降成 carried checkpoint
- 当前恢复点：
  - 后续若再审 `V2`，直接按 `-32` 收件；
  - 当前父线程已 `PARKED`，等待 `V2 -31` 回执。

## 2026-04-02（屎山修复父层补记：`导航检查` 已复审 `V2 -31` 回执，并把下一刀放大到 `-33`）

- 当前新增事实：
  1. `V2 -31` 所报 external compile gate 在 `Editor.log` 中确有痕迹；
  2. 但父层同时核到当前工作树里的 `InventorySlotUI.cs / ToolbarSlotUI.cs` 已含对应方法本体；
  3. 所以这条 gate 当前必须先塌缩真伪，不能再被宽泛当成默认停车位。
- 当前新增文档：
  1. `-33`
     - 强制先塌缩 compile gate 真相
     - gate 一旦 cleared / stale，就直接同窗继续做 `PlayerAutoNavigator.cs` 大刀闭环
  2. `-34`
     - 父线程新的固定验收尺
     - 不再接受“小 blocker checkpoint”
- 当前阶段判断：
  - 导航 Player 主线仍未过线；
  - 但从父层治理口径上，下一刀已经从“先钉小真因”放大到“必须尝试同窗完成 endpoint + crowd + ground 至少两条体验改善”。

## 2026-04-02（屎山修复父层补记：`导航检查` 已复审 `V2 -33` 回执，并把下一刀前置成 `-35`）

- 当前新增事实：
  1. `V2 -33` 所报 compile gate 继续有 `Editor.log` 证据；
  2. 但父层进一步确认，这两份 UI 源文件本身干净，常见源码侧解释已基本排除；
  3. 因此下一刀不应继续停在“等外部文件修”，而应先做 Unity 编译态 / 导入态自清恢复。
- 当前新增文档：
  1. `-35`
     - 强制先做自清恢复动作
     - gate cleared 后必须同窗继续 PAN 大闭环
  2. `-36`
     - 父线程新的固定验收尺
     - 不再接受“没做完自清恢复就停”的 blocker 回卡
- 当前阶段判断：
  - 导航 Player 主线仍未过线；
  - 父层当前把下一刀重新压成：
    - 先自清 Unity 编译态
    - 再继续 `PlayerAutoNavigator.cs`
    - 而不是继续空等 UI 文件 owner

## 2026-04-02（屎山修复父层补记：已并行补出 UI compile gate 的 owner incident 备用材料）

- 当前新增事实：
  1. `rapid_incident_probe` 指向的责任家族仍是：
     - `农田交互修复V2`
  2. 但当前带着相关 UI dirty 的 active thread 是：
     - `农田交互修复V3`
  3. 且它当前 `thread-state` own paths 不含 UI 子根，形成 owner / white-list 失配现场。
- 当前新增文档：
  1. `-37`
     - UI compile gate 急诊定责与 owner 失配说明
  2. `-38`
     - 若 `V2 -35` 自清失败时，直接升级给 `农田交互修复V3` 的备用接盘 prompt
- 当前阶段判断：
  - 这两份文档不改变 `V2 -35` 的优先级；
  - 它们是父线程并行准备的升级材料，避免后续再次空转。

## 2026-04-02（屎山修复父层补记：`导航检查V2 -31` 首轮续跑被他线 compile gate 截断，当前只能停在 blocker checkpoint）

- 当前新增事实：
  1. `导航检查V2` 已真实回读 `-31 / -32`，并继续只锁：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
  2. 线程本轮没有再改 runner/menu/solver/NPC roam/scene；
     只完成了 PAN 热区静态核对、compile truth 报实和 live 尝试。
  3. 当前 fresh compile truth 不是 `PlayerAutoNavigator.cs` 自己红，而是他线 compile gate：
     - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
       - `CS0103: TickStatusBarFade / ApplyStatusBarAlpha`
     - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
       - `CS0103: TickStatusBarFade / ApplyStatusBarAlpha`
  4. 线程已尝试执行：
     - `Tools/Sunset/Navigation/Run Raw Real Input Endpoint NPC Occupied Validation`
     但 Unity 没有进入 Play，也没有新的 scenario 日志；
     因此 `-31` 要求的 fresh matrix 这轮没有真正启动。
- 当前阶段判断：
  - `导航检查V2 -31` 这轮不能算真因已钉实；
  - 也不能把“没有新 live”偷换成“继续沿用旧 live 就行”；
  - 当前最准确的定性是：
    - `compile truth 已报实`
    - `runtime slice 未漂移`
    - `fresh live 被外部 compile gate 截断`
    - `本轮只能算 blocker checkpoint`
- 当前恢复点：
  - 后续若继续，不是立刻追 PAN 第二刀，而是先等外部 compile gate 清掉；
  - gate 清掉后，再按 `-31` 原顺序补：
    - `EndpointNpcOccupied raw ×3`
    - `Crowd raw ×1`
    - `Ground raw ×1`
    - `SingleNpcNear raw ×1`
    - `NpcAvoidsPlayer ×1`
    - `NpcNpcCrossing ×1`
  - 当前 `thread-state` 已从原 `ACTIVE` 切片退回：
    - `PARKED`

## 2026-04-02（屎山修复父层补记：`导航检查V2 -33` 已把 compile gate 塌缩成 active 真 blocker，当前不是 stale 停车位）

- 当前新增事实：
  1. `导航检查V2` 已按 `-33` 先做 compile gate 真伪塌缩，而不是继续直接 claim runtime blocker；
  2. 只读事实已确认：
     - `InventorySlotUI.cs`
     - `ToolbarSlotUI.cs`
     当前工作树里都真实存在：
     - `TickStatusBarFade()`
     - `ApplyStatusBarAlpha()`
     方法本体；
     同时两份文件都处于 dirty 状态；
     仓库内未发现第二份同名类在抢编译。
  3. 但最新强制 recompile 后，`Editor.log` 仍稳定重现：
     - `InventorySlotUI.cs(173/444/524) CS0103`
     - `ToolbarSlotUI.cs(141/298/355) CS0103`
     - 并伴随：
       - `*** Tundra build failed (...)`
       - `## Script Compilation Error for: ... Assembly-CSharp.dll`
  4. 更关键的是：
     - 在这次强制 recompile 后，
       `editor_state` / `read_console` 持续 not ready，
       `Run Raw Real Input Endpoint NPC Occupied Validation` 也直接 timeout，
       说明 Unity 没恢复到可稳定进 Play 的状态。
- 当前阶段判断：
  - 这条 compile gate 现在已经被父层接受为：
    - `active`
    - `不是 stale`
    - `当前活 blocker`
  - 因此 `-33` 本轮合法收口落入第二种：
    - “最新 recompile 仍稳定红，且 Unity 无法进入 Play”
  - 不是线程偷停，也不是继续拿小 blocker 糊弄。
- 当前恢复点：
  - 后续若继续 `-33`，前提不再是“先回 PAN 第二刀”，而是：
    - UI 线 compile gate 真正清掉
    - Unity 恢复到可读 `editor_state / read_console / execute_menu_item`
  - 只有那之后，`导航检查V2` 才能继续同窗大刀推进：
    - endpoint
    - crowd
    - ground 可视停位真值

## 2026-04-02（新增子工作区：树石修复，先完成 Tree/Stone controller 编辑态预览与运行态状态修复）

- 当前主线目标：
  - 新增 `树石修复` 子工作区，专门处理 `TreeController / StoneController` 的编辑态预览与运行态状态一致性问题；
  - 这轮不是导航支线延续，而是 `屎山修复` 父层新增一条资源节点稳定性子线。
- 本轮已完成事项：
  1. 已在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\树石修复\memory.md` 建立子工作区记忆；
  2. 已修 `TreeController.cs`：
     - 补上无 `SeasonManager` 时的编辑态季节回退；
     - 把显示、碰撞体、派生状态拉成同一刷新入口；
     - 进入 Play 首帧先同步可交互状态，不再等 `SeasonManager` 慢一拍；
  3. 已补 `StoneController.cs` 的编辑态组件自动缓存和预览兜底；
  4. 脚本级校验已过，当前未见 `TreeController / StoneController` 自身新编译红。
- 当前稳定结论：
  1. `TreeController` 这条线的真问题不是单点 bug，而是“预览链、碰撞链、派生状态链”过去各走各的；
  2. `StoneController` 没有暴露出同等级 fatal runtime 链断，但确实存在编辑态组件引用依赖过强的问题，已顺手补稳。
- 当前 blocker：
  - Play 级终验仍被外部 Unity 编译红阻断：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `SetConversationBubbleFocus` 缺失
- 当前恢复点：
  - 父层后续若继续收这条子线，先看外部 compile blocker 是否 cleared；
  - cleared 后，再看 `树石修复` 子线的 Play 级复测回执，而不是重新从脚本静态排查开始。

## 2026-04-02（屎山修复父层补记：`导航检查V2 -35` 进行中现场已显示 compile gate 曾被清掉，当前争议重新收缩到“局部 runtime 改善 vs 用户可视停位仍未站住”）

- 当前新增事实：
  1. 从最新 `Editor.log` 看，`导航检查V2` 已经不再停在 compile gate：
     - 日志里先后出现了多次 `*** Tundra build success (...)`
     - 其后已继续跑起：
       - `RealInputPlayerEndpointNpcOccupied`
       - `RealInputPlayerGroundPointMatrix`
       - `RealInputPlayerCrowdPass`
       - `RealInputPlayerSingleNpcNear`
       - `RealInputPlayerAvoidsMovingNpc`
  2. 当前 fresh live 里已经能看到：
     - dedicated endpoint 从一组 `Linger pass=False` 继续推进到一组 `ReachedClickPoint pass=True`
     - `CrowdPass` 从 `crowdStallDuration=1.874 pass=False` 推进到 `0.268 pass=True`
     - `SingleNpcNear / MovingNpc` 也都继续保持 `pass=True`
  3. 但 `GroundPointMatrix` 最新仍只是：
     - `accurateCenterCases=6/6`
     - `positiveCenterBiasCases=0/6`
     - 与此同时 `Transform/Rigidbody` 仍稳定比点击点低约 `1.09~1.24`
     - 所以这条证据当前依旧只能证明 `center-only` 合同，不足以证明用户可视停位已经自然
- 当前阶段判断：
  - `导航检查V2 -35` 现在至少已越过：
    - `compile blocker checkpoint`
  - 当前真正该防的是另一种偷换：
    - 把 live 局部转绿说成整体体验过线
    - 把 `GroundPointMatrix pass=True` 说成“右键停位偏上已关闭”
- 当前恢复点：
  - 下次父层正式收 `V2` 回执时，不能再只问 compile gate；
  - 必须继续追问：
    1. gate 在哪一步 cleared
    2. 哪些 live case 已重跑
    3. 为什么 current ground truth 仍不能反驳用户的“停位偏上/停位怪”体感
    4. endpoint / crowd 的局部转绿是否已足够代表真实右键入口体验

## 2026-04-02（屎山修复父层补记：`V2 -35` 正式回执已被改判为“有真实推进，但旧新样本混算 + 新 blocker 未 fresh 钉实”，已继续下发 `-40`）

- 当前新增事实：
  1. `V2` 正式回执里已经明确承认：
     - 原始 UI gate 已 `cleared`
     - 也明确写了：
       - `右键停位偏上 = 不能关闭`
  2. 但它当前对 endpoint / crowd 的汇报口径仍有一个新问题：
     - 用多轮改动过程中的 `fail -> pass` 累计样本，叙述成“当前最终代码已稳定转绿”
  3. 同时它又拿：
     - `PlayerNpcChatSessionService.cs / SetConversationBubbleFocus`
     当最终停车理由；
     但父层当前手上的最新现场并未把这条新 gate fresh 钉成稳定活 blocker
- 当前阶段判断：
  - 这条线现在不能再停在“runtime 中段 partial checkpoint”；
  - 它下一轮更该做的是：
    - 先塌缩新 blocker 真伪
    - 再把 **当前最终代码** 的稳定矩阵从零开始补满
  - 而不是继续用旧样本和新样本混讲一个“总体在变好”的故事。
- 当前新增文档：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-导航检查V2-禁止混算旧新样本并补满当前最终代码稳定矩阵-40.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-父线程-V2-40验收尺与旧新样本混算拒收清单-41.md`
- 当前恢复点：
  - 后续父层若继续收件，先看：
    1. 新 compile gate 是否 fresh 塌缩为 `cleared / stale / active`
    2. 当前最终代码矩阵是否真的跑满
    3. `Ground raw` 是否仍被偷换成“用户可视停位已关闭”

## 2026-04-02（屎山修复父层补记：`导航检查V2 -35` 已把 PAN 玩家主线重新推进到 fresh matrix 过线，但当前被新的外部 Service/Player compile gate 截停）

- 当前新增事实：
  1. `导航检查V2` 这轮没有再停在旧的 compile gate checkpoint，而是真正回到 `PlayerAutoNavigator.cs` 继续施工与 live 取证；
  2. 线程先完成了 Unity 编译态自清，并成功起跑玩家主线 fresh live；
  3. 只改 `PlayerAutoNavigator.cs` 后，目前已经拿到同一批次 fresh player-side 样本：
     - `EndpointNpcOccupied raw`：`pass=True`
     - `Crowd raw`：`pass=True`
     - `SingleNpcNear raw`：`pass=True`
     - `MovingNpc raw`：`pass=True`
     - `Ground Point Matrix raw`：`pass=True`
  4. 其中最关键的实质推进有两条：
     - endpoint 旧的 `BlockedInput/Linger` 坏链已翻绿；
     - crowd 旧的慢爬/卡顿坏相已通过更早切 detour 被压到过线范围内（`crowdStallDuration` 已掉到 `0.268`）
  5. 当前新的停车原因已不再是旧 UI gate，而是 fresh 编译中出现新的 unrelated blocker：
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `CS0103: SetConversationBubbleFocus`
- 当前阶段判断：
  - `导航检查V2` 玩家主线不是“完全没做成”，也不是“还停在 compile gate 原地”；
  - 当前更准确的父层判断是：
    - PAN 主线已拿到一批 fresh 过线证据
    - 当前继续扩 live 被外部 `Service/Player` compile gate 截断
    - 这条线本轮属于“有效推进后合法停车”
- 当前恢复点：
  - 后续恢复这条线时，不要再把第一步写成“重新猜 endpoint/crowd 真因”；
  - 第一前提先看：
    - `PlayerNpcChatSessionService.cs` 的 external compile gate 是否 cleared
  - cleared 后，再回 `导航检查V2` 的玩家 PAN 矩阵稳定性复核，而不是重做上一轮已经过的责任点拆解

## 2026-04-02（父层补记：`-35` 回看结果应按“部分完成但未整包收口”定性）

- 当前新增结论：
  1. `导航检查V2 -35` 不能再被回写成“只停在 UI compile gate checkpoint”；
  2. 也不能被夸成“整个 `-35` 已完整完成”；
  3. 父层当前最准确的定性应是：
     - write scope 没漂
     - 自清恢复做成了
     - PAN 玩家主线已有实质推进和多条 fresh 过线样本
     - 但 `SingleNpcNear ×2` 与 `NpcAvoidsPlayer / NpcNpcCrossing` 这部分没有收满，因此仍属于 `-35` 的 partial completion

## 2026-04-02（屎山修复父层收尾补记：`V2 -40` 已发出，当前治理现场已结到等待回执）

- 当前新增事实：
  1. `V2 -40 / -41` 已形成父层最新治理闭环：
     - `-40` = 下一轮唯一施工 prompt
     - `-41` = 下一次收件固定验收尺
  2. 本轮父层已补齐 skill 审计：
     - `STL-20260402-054`
     - canonical duplicate groups=`0`
  3. `导航检查` thread-state 已重新 `Park-Slice`，当前不再保留 `ACTIVE` 假现场。
- 当前阶段判断：
  - 这条线目前不是继续由父线程追加新刀；
  - 而是等待 `导航检查V2` 交回一份符合 `-40/-41` 的 fresh 回执。
- 当前恢复点：
  - 下次继续先看：
    1. `PlayerNpcChatSessionService` 新 gate 是否 fresh 塌缩
    2. 当前最终代码矩阵是否真的从零开始重跑补满
    3. 用户可视停位偏上是否仍被偷换成 `center-only` 结构绿

## 2026-04-02（屎山修复父层补记：`V2 -40` 被拒收，原因升级为“假 Tool_002 blocker + 假 queued-only blocker”）

- 当前新增事实：
  1. 父层已接受 `SetConversationBubbleFocus` 这条旧 blocker 现在是 `stale`；
  2. 但 `V2` 新报的 `Tool_002_BatchHierarchy.cs` blocker 当前不能直接接受，因为：
     - 同一份最新 `Editor.log` 里先有 `CS0136`
     - 后面又已有 `Tundra build success`
     - 且磁盘上的对应代码行已不是 `parent`
  3. `queued_action-only` 这条 live 口径也不能接受，因为父层当前只读现场已看到：
     - `scenario_start`
     - `scenario_setup`
     - `scenario_observe_start`
     - heartbeat
- 当前阶段判断：
  - 这条线当前不该继续停在 blocker 解释层；
  - 下一轮更该被逼回：
    - 当前最终代码矩阵必须真正重新起跑
- 当前新增文档：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-导航检查V2-拒绝把已清Tool002与已dispatch探针继续当blocker并立刻补满当前最终代码矩阵-42.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-父线程-V2-42验收尺与假Tool002阻塞拒收清单-43.md`
- 当前治理现场：
  - `导航检查` 已重新 `Park-Slice`
  - 当前等待物已更新为：
    - `waiting-for-v2-42-receipt`
    - `awaiting-proof-that-current-final-code-matrix-has-really-restarted`

## 2026-04-02（屎山修复父层补记：`V2` 当前已进入 crowd 单红面阶段，已继续下发 `-44`）

- 当前新增事实：
  1. `V2` 最新一轮已把“假 Tool_002 blocker / 假 queued-only blocker”都清掉；
  2. 当前最终代码矩阵也已被 fresh 跑满；
  3. 现在父层接受的最新 runtime 事实是：
     - endpoint / ground / single / moving / npc guardrails 都绿
     - 只剩 `Crowd raw ×3` 三连红
- 当前阶段判断：
  - 当前最合理的下一刀已被压成：
    - 只收 `PlayerAutoNavigator.cs` 的 crowd 同簇三连红
  - 但即使下一刀 crowd 绿了，也仍然不能把“右键停位偏上”写成已关闭。
- 当前新增文档：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-导航检查V2-只收PAN-crowd同簇三连红并维持其余矩阵绿面-44.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-父线程-V2-44验收尺与crowd唯一红面拒收清单-45.md`
- 当前治理现场：
  - `导航检查` 已重新 `Park-Slice`
  - 当前等待物已更新为：
    - `waiting-for-v2-44-receipt`
    - `awaiting-proof-that-pan-crowd-cluster-is-green-without-regressions`

## 2026-04-02（屎山修复父层补记：`V2 -44` 再改判为“先纠偏 crowd 测试语义”，已继续下发 `-46`）

- 当前新增事实：
  1. `V2` 这次不只是报 crowd 仍红，还承认了更根本的问题：
     - 旧 crowd case 本身就是错题
  2. 这意味着父层不能再简单地下达“继续修 crowd runtime”；
  3. 更合理的下一刀已经改成：
     - 先纠偏 crowd 验证语义
     - 再裁定最近两刀 PAN crowd 补口该留还是该回
- 当前新增文档：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-导航检查V2-先纠偏crowd测试语义再裁定PAN crowd补口留回-46.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-02-父线程-V2-46验收尺与crowd语义纠偏拒收清单-47.md`
- 当前治理现场：
  - `导航检查` 已重新 `Park-Slice`
  - 当前等待物已更新为：
    - `waiting-for-v2-46-receipt`
    - `awaiting-crowd-semantics-correction-before-any-more-pan-runtime-tuning`

## 2026-04-02（屎山修复父层补记：树石修复子线已把树批量状态工具搭到可烟测态）

- 当前新增事实：
  1. `树石修复` 子线在上一刀 `TreeController / StoneController` 编辑态预览修复的基础上，又补了一把 Editor 工具刀；
  2. 这轮新增了 `Assets/Editor/TreeBatchStateTool.cs`，目标是让用户在 `Hierarchy` 里选中树父物体后，直接批量修改：
     - `treeID`
     - `当前阶段`
     - `当前状态`
     - `当前季节`
  3. 这把工具不是只写字段，而是走 `TreeController.ApplyBatchEditorState(...)`，批量应用时会一并刷新：
     - Sprite 预览
     - 派生血量
     - 非法树桩状态修正
     - `PolygonCollider2D`
  4. 为了降低入口摩擦，`TreeControllerEditor` 的“当前状态”区也补了：
     - `选中父物体并打开批量树工具`
- 当前阶段判断：
  - 这条子线当前不是“树工具还没写”，而是已经进入“代码可烟测、等待 Unity 实机点一下”的阶段；
  - 现阶段最大 blocker 不是逻辑再次失焦，而是 `unityMCP` 仍无法连到 `http://127.0.0.1:8888/mcp`，导致这轮没有拿到脚本校验和编辑器烟测证据。
- 当前恢复点：
  - 下次恢复这条子线时，先不要重写工具；
  - 第一动作应该是直接进 Unity 做四项烟测：
    1. 工具能否正确抓取当前选择下的 `TreeController`
    2. 批量改 `stage/state/season`
    3. 批量改 `treeID`
    4. 树的显示、碰撞体、可砍伐状态是否同步刷新

## 2026-04-02（屎山修复父层补记：树批量工具已并到 `Tool_004` 菜单）

- 当前新增事实：
  1. `树石修复` 子线又补了一刀入口并轨，不再只把树工具挂在 `Tools/Sunset/Tree/...`；
  2. 当前新的主菜单入口已经对齐到现有批量工具组：
     - `Tools/004批量 (Tree状态)`
  3. 工具脚本命名也已并轨为：
     - `Assets/Editor/Tool_004_BatchTreeState.cs`
- 当前阶段判断：
  - 这条子线当前不再卡在“工具藏得太深、用户看不到菜单”的层面；
  - 剩下的主要不确定性回到 Unity 编辑器里的真实打开与批量应用手感，而不是入口组织方式。

## 2026-04-03（屎山修复父层补记：Rock/C1~C3 的真实问题是 prefab 本体仍为 NUL，当前已拉回 HEAD 止血版）

- 当前新增事实：
  1. `树石修复` 这轮没有扩回 Tree/Stone 系统逻辑，而是严格只做了 `Rock/C1/C2/C3.prefab`；
  2. 真实根因不是 `StoneController` 配置链断，而是 working tree 里的 `C1/C2/C3.prefab` 文件内容仍是整文件 `NUL` 空字节；
  3. 这 3 份文件当前已经被直接重写回 `HEAD` 的正常 YAML 文本；
  4. `Assets/222_Prefabs/Rock/` 当前只有这 3 份 `.prefab`，最小同类体检未发现额外同类 prefab。
- 当前阶段判断：
  - 这轮属于“HEAD 止血版在本地真正落盘并被确认恢复正常文本结构”；
  - 不是另起一轮 Rock 结构改造，更不是脚本层重写。
- 当前恢复点：
  - 后续只需在 Unity 当前现场确认一次：`C1/C2/C3` 是否不再继续报 `Unknown error occurred while loading ...`
  - 如果已不再报，这条 incident 可以按 prefab 本体损坏已止血关闭。

## 2026-04-03（屎山修复父层补记：Stone 批量状态工具已并到 `Tool_005`）

- 当前新增事实：
  1. `树石修复` 子线又补了一把石头专用的批量状态工具；
  2. 当前主菜单入口为：
     - `Tools/005批量 (Stone状态)`
  3. 这把工具不是纯复制树工具，而是额外处理了石头专属差异：
     - `OreType` 改动时同步切 `spriteFolder/spritePathPrefix`
     - 不勾血量时按阶段/含量自动派生 `currentHealth`
- 当前阶段判断：
  - 这条子线现在已经同时具备 `Tool_004 Tree` 与 `Tool_005 Stone`；
  - 剩下的不确定性只在 Unity 里的真实使用手感，不在工具结构层。

## 2026-04-03（屎山修复父层补记：Tool_005 首个编译红已热修）

- 当前新增事实：
  1. `Tool_005_BatchStoneState.cs` 首次落地后立刻暴露出一个纯命名空间级低级红：
     - 少了 `using FarmGame.Data;`
  2. 这不是工具设计错，而是 `StoneStage / OreType` 所在命名空间未引入；
  3. 当前该红已直接补掉。

## 2026-04-02（屎山修复父层补记：导航检查V2 `-40` 当前合法停车位已改判为 external `Tool_002_BatchHierarchy` gate）

- 当前新增事实：
  1. `导航检查V2 -40` 已先 fresh 塌缩掉：
     - `PlayerNpcChatSessionService.cs / SetConversationBubbleFocus`
     - 当前不再能写成活 blocker，定性更接近 `stale`
  2. 最新 fresh compile 里真正活着的 gate 已改判为：
     - `Assets/Editor/Tool_002_BatchHierarchy.cs(387,27): error CS0136`
     - `Assets/Editor/Tool_002_BatchHierarchy.cs(406,27): error CS0136`
  3. `V2` 又做了 1 次受控 endpoint raw 起跑探针，但日志只到：
     - `queued_action=RunRawRealInputEndpointNpcOccupied entering_play_mode`
     - 没有 `editor_dispatch_pending_action / scenario_start / scenario_end`
     - 最终状态回到 `EnteredEditMode`
  4. 因此这轮没有形成任何“当前最终代码连续重跑”的新样本；
     `-35` 历史样本不能被并入 `-40`
- 当前阶段判断：
  - `V2 -40` 目前符合 prompt 允许的第二种合法收口：
    - 新 blocker 已 fresh 塌缩
    - 但当前最终代码稳定矩阵被新的 external compile/live gate 截停
  - 当前仍不能把：
    - `右键停位偏上`
    写成：
    - `已关闭`
- 当前恢复点：
  - 下次继续这条线时，先看：
    1. `Tool_002_BatchHierarchy.cs` gate 是否 cleared
    2. cleared 后再从零开始重跑 `-40` 全矩阵

## 2026-04-02（屎山修复父层补记：导航检查V2 `-42` 已把假 Tool002 / 假 queued blocker 清掉，并重跑出当前最终代码矩阵）

- 当前新增事实：
  1. `导航检查V2 -42` 已 fresh 证明：
     - `Tool_002_BatchHierarchy` 当前是 `cleared`
     - `queued_action-only` 口径已失效，最新干净 probe 已 `scenario_start/setup/observe`
  2. `V2` 已按要求从零开始连续重跑当前最终代码矩阵：
     - `EndpointNpcOccupied raw ×3`：全 `pass=True`
     - `Crowd raw ×3`：全 `pass=False`
     - `Ground raw matrix ×1`：`pass=True`
     - `SingleNpcNear raw ×2`：全 `pass=True`
     - `MovingNpc raw ×1`：`pass=True`
     - `NpcAvoidsPlayer ×1 / NpcNpcCrossing ×1`：全 `pass=True`
  3. 当前剩余第一责任点已重新压回：
     - `PlayerAutoNavigator.cs` 的 crowd 同簇
     - 而不再是 compile / dispatch blocker
- 当前阶段判断：
  - `导航检查V2` 现在已经越过 blocker 裁定阶段；
  - 当前最准确的阶段是：
    - “当前最终代码矩阵已报实，只剩 crowd raw ×3 三连红”
- 当前恢复点：
  - 下次继续这条线时，先不再发 blocker 塌缩 prompt；
  - 直接围绕 `PlayerAutoNavigator.cs` 的 crowd 同簇补口与复测

## 2026-04-02（屎山修复父层补记：`-44` 本轮不是“crowd 已绿”，而是被用户现场纠正为测试语义错位）

- 当前新增事实：
  1. `导航检查V2` 这轮确实只改了 `PlayerAutoNavigator.cs`，并且只留在 PAN crowd 同簇：
     - 先补了跨 blocker 的 crowd 慢爬累计
     - 又补了 crowd blocker 振荡计数
  2. 线程拿到的 fresh crowd 样本并未稳定转绿；
     目前仍存在：
     - `directionFlips`
     - `crowdStallDuration`
     - 少量 `minEdgeClearance<0`
     的混合红面
  3. 更重要的是，用户已明确纠正当前 crowd case 的体验语义：
     - 不能把“前面是一堵 NPC 墙，还继续硬撞/挤穿”当成正确测试目标
- 当前阶段判断：
  - 这条线现在不能再按“继续把 crowd raw 旧 probe 压绿”推进；
  - 更准确的定性是：
    - 代码层已有两刀 PAN crowd 最小补口
    - 旧 crowd probe 仍未稳定过线
    - probe 自身又被用户判定为体验口径错误
- 当前恢复点：
  - `导航检查V2` 已合法 `PARKED`
  - 下一轮应先做 crowd 测试矩阵/玩家语义纠偏，再决定当前两刀代码如何取舍

## 2026-04-02（屎山修复父层补记：`Primary.unity` 当前 dirty 现场不能继续误算给 `导航检查V2`）

- 当前新增事实：
  1. 用户在 crowd 线追问“是不是把 `Primary.unity` 撤回/清扫了”后，已完成一次只读热场景追责；
  2. 结果显示：
     - `Primary.unity` 当前仍是大 dirty
     - `导航检查V2` 当前 `PARKED`，own path 只有 `PlayerAutoNavigator.cs`
     - 另有 active 线程 `019d4d18-bb5d-7a71-b621-5d1e2319d778` 明确把 `Assets/000_Scenes/Primary.unity` 挂在 `a_class_locked_paths`
- 当前阶段判断：
  - `导航检查V2` 这条线当前真正要背的账，仍然只是 `PlayerAutoNavigator.cs` 的 crowd 线补口与测试语义错位；
  - `Primary.unity` 的 active dirty 现场不能继续默认算作这条线刚刚清扫或回退造成
- 当前恢复点：
  - 后续若再追 `Primary.unity` 责任，应直接转向 active scene owner / root-integrator；
  - `导航检查V2` 继续保持 `PARKED`，不写 scene，不做越权处置

## 2026-04-02（屎山修复父层补记：`导航检查V2 -46` 已把 crowd 语义纠偏落到 runner/menu，但当前最终代码在新语义下仍双红）

- 当前新增事实：
  1. `导航检查V2 -46` 已完成 crowd 语义纠偏的结构动作：
     - 旧 `Crowd raw` 已降级为 `legacy blocked-wall stress`
     - 新建：
       - `PassableCorridor`
       - `StaticNpcWall`
     - 本轮只动 runner/menu，没有再改 `PlayerAutoNavigator.cs`
  2. 当前最终代码上的 fresh 结果：
     - `PassableCorridor ×3`：全红，且 3 条全是 `Oscillation`
     - `StaticNpcWall ×3`：全红，其中 2 条 `ReachedBlockedTarget`、1 条 `Oscillation`
  3. 最小 raw 护栏仍绿：
     - `EndpointNpcOccupied raw ×1`
     - `Ground raw matrix ×1`
     - `SingleNpcNear raw ×1`
     - `MovingNpc raw ×1`
  4. compile / Editor 可用性本轮 clean：
     - 最新 `Editor.log` 尾段未见 runner/menu 新引入的 compile red
     - Unity 已退回 `Edit Mode`
- 当前阶段判断：
  - 这条线已经不再是“旧 crowd probe 是否还算数”的口径争论；
  - 当前更准确的阶段是：
    - **新语义已落、fresh 已跑、但当前最终代码在 `PassableCorridor / StaticNpcWall` 两边都没站住**
- 当前恢复点：
  - 父层当前接受的裁定是：
    - 下轮优先考虑回退最近两刀 `PlayerAutoNavigator.cs` crowd 补口
  - 继续时不要再回旧 `Crowd raw` 主 acceptance，也不要再把 `Ground raw` 结构绿偷换成“右键停位偏上已关闭”

## 2026-04-02（屎山修复父层补记：导航总进度已压到最后 2 个未闭环点）

- 当前新增事实：
  1. 用户要求直接回答“导航为什么还没完、现在到底还差多少”，因此父层重新把最新导航现场压成总图，而不再沿用长链回执口径。
  2. 当前玩家导航这条线，题目已经纠正完成：
     - 旧 `Crowd raw` 只算 `legacy blocked-wall stress`
     - 新 crowd 主语义改为：
       - `PassableCorridor`
       - `StaticNpcWall`
  3. 当前最终代码在新语义下仍未闭环：
     - `PassableCorridor ×3`：全红，且 `3/3 = Oscillation`
     - `StaticNpcWall ×3`：全红，其中 `2/3 = ReachedBlockedTarget`、`1/3 = Oscillation`
  4. 其余 player-side 关键矩阵已站住：
     - `EndpointNpcOccupied`
     - `Ground raw matrix`
     - `SingleNpcNear`
     - `MovingNpc`
     - `NpcAvoidsPlayer / NpcNpcCrossing`
  5. 但 `Ground raw matrix` 当前仍只能证明：
     - `center-only` 结构合同成立
     - 不能证明“右键停位偏上已关闭”
- 当前阶段判断：
  - 导航主线已经不再是“到处都没做完”的状态；
  - 更准确地说，现在只剩 2 个未闭环点：
    1. `PlayerAutoNavigator.cs` crowd 同簇的 runtime 响应
    2. 右键停位偏上的可视体验真值
- 如果按“整个导航大盘”看，还应另外记 2 条尾账：
  1. `Primary` 穿越/边界/切场这条线，代码侧已 compile-clean，但仍待用户回当前打开场景做真实入口复测；
  2. 较早的 NPC roam probe / fail-fast 线仍是 carried partial checkpoint，不是当前主阻塞，但尚未被正式关案。
- 当前恢复点：
  - 若继续导航主线，父层下一刀不应再放大 scope；
  - 仍应围绕：
    - 回退最近两刀 `PlayerAutoNavigator.cs` crowd 补口
    - 用新 `PassableCorridor / StaticNpcWall` 语义 fresh 复判
    - 并继续诚实维持“stop-bias 未关闭”的口径

## 2026-04-02（屎山修复父层补记：Primary traversal 快修不再手赌 scene YAML，而是转成 Editor 现场自动补齐）

- 当前新增事实：
  1. 用户继续追责 `Primary` 的 `Water/Props` 可穿越、可走出 tilemap、切场失败与现场找不到 `SceneTransitionTrigger`；
  2. 这轮没有继续向 `Assets/000_Scenes/Primary.unity` 追加硬写，而是新增 Editor binder，把当前打开的 `Primary` 场景实例当成真实修复目标；
  3. 运行时侧已补：
     - `NavGrid2D` 显式障碍 tilemap 占位判定
     - 玩家手动移动的导航边界硬拦截
     - Editor PlayMode 按 scene path 切场 fallback
  4. Editor 侧已补：
     - `PrimaryTraversalSceneBinder`
     - 负责把 `Water/Props` collider、`NavGrid2D` 阻挡源、`boundsPadding=0`、`CameraDeadZoneSync`、`SceneTransitionTrigger` 实时补进当前打开的 `Primary`
- 当前阶段判断：
  - 这条线现在已经从“磁盘 scene 修改”改成“运行时逻辑 + 当前打开 scene 实例自动修复”的组合口径；
  - 更准确的阶段是：
    - 代码与 editor auto-bind 已 compile-clean
    - 等用户回到 Unity 现场做真实入口复测
- 当前恢复点：
  - 若用户反馈仍有未挡住的水域或 props 区块，下一轮应继续围绕 binder 实际命中的 tilemap / collider 现场收窄，不要重新回到泛 scene YAML 盲改。

## 2026-04-02（屎山修复父层补记：Primary traversal 失效根因已压到“空层误命中 + 玩家中心探针过高”）

- 当前新增事实：
  1. 用户对 `Primary` 的最新复测明确否掉了：
     - `Water` 仍可进
     - `Props` 仍可穿
     - tilemap 外边界仍可越
  2. 这次重新压实后，已确认上轮 binder / auto-detect 命中的两个层：
     - `Layer 1 - Props_Porps`
     - `Layer 1 - Farmland_Water`
     都是空 tilemap；
     所以上轮“看起来接上 obstacle source”其实没有真正接到用户眼前的可见阻挡层。
  3. 当前 `Primary` 里真实有 tile 且应参与 traversal obstacle 的层已收窄为：
     - `Layer 1 - Wall`
     - `Layer 2 - Base`
     - `Layer 1 - Props_Ground`
     - `Layer 1 - Props_Background`
     - `Layer 1 - Props_Base`
  4. 玩家手动拦截此前继续用碰撞盒中心点判定；
     由于玩家 `BoxCollider2D` 中心明显偏上，导致“脚先踩进 water / props / map outside，但中心点还没进障碍”的漏判成立。
- 本轮父层接受的修正：
  - `PrimaryTraversalSceneBinder` 与 `NavGrid2D` 都已改成只收真实非空阻挡层；
  - `PlayerMovement` 已改成脚底三点探针，不再用中心点做唯一拦截依据。
- 当前阶段判断：
  - 这条 `Primary traversal` 子线已不再停留在“也许现场没同步”；
  - 当前更准确的阶段是：
    - 真实根因已压实并已做代码返修
    - 剩余只差 Unity 现场重测 3 个入口 case
- 当前恢复点：
  - 若用户继续反馈仍可穿越，下一步应直接进入 Unity scene instance 现场看上述 5 个 obstacle tilemap 的实际覆盖，而不是回退到旧的空层名义口径。

## 2026-04-03（屎山修复父层补记：Primary traversal 这条线已按用户要求收窄为脚本契约 owner）

- 当前新增事实：
  1. 用户已明确重置分工：
     - 通用工具线不再归这条线程
     - `Town.unity` 基础内容转化不再归这条线程
     - `Primary.unity` / `Town.unity` / `PrimaryTraversalSceneBinder` 都不再是这条线的 own
  2. 这条线当前 exact-own 已收窄为：
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
  3. 当前脚本侧已完成的收口是：
     - `NavGrid2D` 不再把 `Primary` 的 obstacle 命名硬编码进基础逻辑；
     - `PlayerMovement` 现在有显式 `NavGrid2D` 接线入口和清楚的脚底采样参数；
     - `SceneTransitionTrigger2D` 现在有稳定的 target scene 契约，不再依赖 binder 或 scene 自动补丁。
- 当前阶段判断：
  - 这条线现在不应再被理解成“Primary/Town 场景 owner”；
  - 更准确的定位是：
    - **traversal / blocking / scene transition 的基础脚本 owner**
  - 场景 owner 后续需要自己把这些脚本接回 `Primary / Town`。
- 当前恢复点：
  - 后续若继续问这条线，先按“脚本逻辑 / 契约 / 接线说明”回答；
  - 不再默认让这条线继续 claim scene 实写或通用工具维护。

## 2026-04-03（屎山修复父层补记：导航检查V2并行验收侧已把 final acceptance pack 与当前红绿总图钉死）

- 当前新增事实：
  1. `导航检查V2` 这轮不是继续主刀 `PlayerAutoNavigator.cs`，而是只收：
     - `NavigationLiveValidationRunner.cs`
     - `NavigationLiveValidationMenu.cs`
     - final acceptance pack
     - 父线程最新 PAN 工作树上的 fresh live 复判
  2. runner/menu 侧现在已经有固定入口：
     - `Tools/Sunset/Navigation/Run Final Player Navigation Acceptance Pack`
  3. final acceptance pack 已在当前工作树上 fresh 跑完 12 条，当前总图已压实为：
     - 红：
       - `PassableCorridor ×3`
       - `StaticNpcWall ×3`
     - 绿：
       - `EndpointNpcOccupied ×1`
       - `GroundRawMatrix ×1`
       - `SingleNpcNear ×1`
       - `MovingNpc ×1`
       - `NpcAvoidsPlayer ×1`
       - `NpcNpcCrossing ×1`
  4. `导航检查V2` 这轮没有再碰：
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
  5. 当前仍不能把：
     - `右键停位偏上 / 玩家可视停位偏上`
     写成：
     - `已关闭`
     因为 `GroundRawMatrix` 仍只算结构层绿，不等于玩家可视停位体验成立
- 当前阶段判断：
  - 导航父线现在已经不是“验收矩阵没跑真”；
  - 更准确的状态是：
    - **验收矩阵入口与 fresh 结果已齐**
    - **剩余 PAN 主红面已被压到 `PassableCorridor / StaticNpcWall` 同簇**
  - 但这轮并未直接 sync 成功；
    - `Ready-To-Sync` 被同一 `Assets/YYY_Scripts/Service/Navigation` 根下的 foreign remaining dirty 截停
    - 当前更准确的收口状态是：
      - **验收事实已完成**
      - **白名单归仓被 Navigation 同根残留阻断**
- 当前恢复点：
  - 父线程后续若继续玩家右键导航主线，应直接围绕 `PlayerAutoNavigator.cs` 里的 crowd / wall 同簇责任点继续收；
  - 不需要再回到“是不是该先做 final pack / 是不是还缺 runner/menu 入口”的旧问题上。

## 2026-04-03（屎山修复父层补记：导航验收分账已定稿，accepted navigation 与 Primary traversal 不再混报）

- 当前新增事实：
  1. `导航检查V2` 已按 `-54` 完成最终验收交接定性：
     - 当前玩家导航版本：**用户已认可**
     - `PassableCorridor / StaticNpcWall`：仍保留红面，但只算 targeted probe / 后续 polish 诊断项
     - `FinalPlayerNavigationAcceptancePack`：结构与验收工具层已完成
  2. 从现在起，导航父层必须把两件事彻底拆开：
     - accepted player navigation version
     - `Primary traversal` 剩余闭环
     二者不再允许混成“导航整体还没做完/已经全做完”的单结论
  3. `Primary traversal` 当前仍是父线程独立切片：
     - 不属于 `导航检查V2` 这轮 handoff 已完成范围
     - 也不能拿它去否掉“当前玩家导航版本已被认可”的顶层事实
  4. 当前仍不能写成已关闭或已全线完成的点保持不变：
     - `右键停位偏上已关闭`
     - `final acceptance pack 全绿`
     - `整个导航系统全线完成`
- 当前阶段判断：
  - 导航线现在的正确全局口径是：
    - 玩家右键导航当前版本：可接受
    - targeted probe：仍有两个诊断红面
    - `Primary traversal`：父线程独立收口中
  - 这比旧口径“导航还是一口大锅”更接近真实状态
  - 当前归仓状态则要单独看：
    - `导航检查V2` 的用户验收事实已完成
    - 但白名单 sync 仍被 own-root remaining dirty 合法阻断
- 当前恢复点：
  - 父线程后续如果继续汇报，必须优先按这三层分账讲清；
  - 不允许再把 `Primary traversal` 混进 `导航检查V2` 的 accepted navigation handoff 里。

## 2026-04-03（屎山修复父层补记：`导航检查V2` 已按 `-58` 进入冻结停车态，不再自行 reopen）

- 当前新增事实：
  1. `导航检查V2` 已接受新的停车裁定：
     - 不再重跑 live
     - 不再碰 runtime
     - 继续保持 `PARKED`
     - 冻结 accepted navigation handoff truth
  2. 从现在起，`导航检查V2` 只有在“专用 sync-cleanup slice”被明确下发时，才允许重新开工；
     - 否则即使 same-root 还有尾账，也不得自己 reopen
  3. 父层对导航三线的当前总判断进一步稳定为：
     - accepted player navigation version：保持成立
     - `导航检查V2`：冻结停车，等待专用 cleanup
     - `Primary traversal`：继续是父线程独立业务主线
- 当前阶段判断：
  - 当前导航线已经不是“谁看见 blocker 就继续动一刀”的状态；
  - 更准确的是：
    - `导航检查V2` 进入冻结 truth 的停车态
    - 业务主线只留给父线程 `Primary traversal`
- 当前恢复点：
  - 后续如果没有明确新的 cleanup slice，就不要再唤醒 `导航检查V2`

## 2026-04-03（屎山修复父层补记：用户已认可当前导航版本；剩余 targeted probe 红面转为后续 polish 诊断项）

- 当前新增事实：
  1. 用户刚刚已明确认可当前导航版本：
     - “避让有点走得太多，但完全可以接受”
     - 当前版本被用户真实入口体验层接受
  2. 父线程在此之前曾短暂尝试一刀更激进的 corridor crowd 判定扩张；
     - 目标是更早触发 detour；
     - 单样本 fresh 结果没有优于当前认可版本，反而重新落成 `Oscillation`；
     - 这刀已撤回，没有保留。
  3. `导航检查V2` 的 final acceptance pack 事实仍成立：
     - 红面仍是：
       - `PassableCorridor ×3`
       - `StaticNpcWall ×3`
     - 但它们现在只能继续被定性为：
       - **targeted probe / 后续 polish 诊断项**
     - 不再作为“当前导航版本不可接受”的证据。
  4. 已为 `导航检查V2` 新建新的收口 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-03-导航检查V2-用户已认可当前导航版本仅收最终验收交接与可归仓判定-52.md`
- 当前阶段判断：
  - 导航主线当前已不再是“继续追红面直到 pack 全绿”；
  - 更准确的总图是：
    - **真实入口体验：用户已认可**
    - **targeted probe：仍有两簇诊断红面**
    - **工具/验收侧：final acceptance pack 已成型**
- 当前恢复点：
  - 父线程默认停止继续增加 runtime 风险；
  - `导航检查V2` 下一刀只收最终验收交接与可归仓判定，不再继续修 PAN；
  - 当前仍不能把“右键停位偏上”写成已关闭。

- 当前父线程状态：
  - `导航检查` 已合法 `PARKED`
  - blockers：
    - `user-accepted-current-navigation-version-as-baseline`
    - `waiting-for-v2-final-acceptance-handoff-or-sync-ruling`

## 2026-04-03（屎山修复父层补记：工具-V1新分工已接入，导航三方正式改成“父线程收 Primary traversal / V2收最终验收 / 工具-V1只保留脚本契约”）

- 当前新增事实：
  1. 工具-V1最新分工已经 fresh 压实：
     - 只保留 3 个脚本契约 owner：
       - `NavGrid2D.cs`
       - `PlayerMovement.cs`
       - `SceneTransitionTrigger2D.cs`
     - 不再 own：
       - `Primary.unity`
       - `Town.unity`
       - `PrimaryTraversalSceneBinder.cs`
       - 通用工具 / scene live-apply
  2. 导航父线程新的 prompt 已把剩余 `Primary traversal` 问题单独拆成独立切片：
     - 只收：
       - `Props` 阻挡
       - `Water` 阻挡
       - tilemap 外边界阻挡
     - 并明确要求与：
       - `PAN crowd runtime`
       - `runner/menu final acceptance`
       分账
  3. `导航检查V2` 新的 prompt 已改判为：
     - 不再继续修 runtime
     - 只收最终验收交接、已认可版本的分层报实、以及 sync / own-root 判定
  4. 用户已认可当前玩家导航版本，仍是本工作区当前最高层事实；
     - `PassableCorridor / StaticNpcWall` 红面只能继续作为 targeted probe / 后续 polish 诊断项
     - 不能再被写成当前版本不可用
- 当前阶段判断：
  - 当前导航工作区已经不再是“谁继续修 crowd runtime”；
  - 更准确的最新分账是：
    - 父线程 `导航检查`：收 `Primary traversal` 独立闭环
    - `导航检查V2`：收最终验收交接与可归仓判定
    - 工具-V1：只保留 3 个脚本 contract support
- 当前恢复点：
  - 后续所有导航汇报都必须把：
    - 已认可的玩家导航版本
    - `Primary traversal` 剩余闭环
    - runner/menu 验收与归仓
    分开报实；
  - 当前仍不能把“右键停位偏上”写成已关闭。

## 2026-04-03（屎山修复父层补记：这轮 prompt 重排已停车，等待父线程执行 `-53` 与 `导航检查V2 -54` 回执）

- 当前新增事实：
  1. `导航检查` 已对本轮 prompt 重排切片执行合法 `Park-Slice`
  2. 当前 blockers 已切换为：
     - `waiting-for-parent-primary-traversal-slice-execution`
     - `waiting-for-v2-54-receipt-and-final-review`

## 2026-04-03（屎山修复父层补记：工具-V1三脚本线被改判为“边界守住但已混入 runtime change”，当前不允许它自己清 same-root dirty）

- 当前新增事实：
  1. 工具-V1最新回执经抽查后，边界报实基本成立：
     - 没碰 `Primary/Town` scene
     - 没碰 binder
     - 没碰通用工具
  2. 但它当前修改并不等于纯 contract support：
     - `NavGrid2D.cs` 已改到 obstacle / nearest-walkable 语义
     - `PlayerMovement.cs` 已改到真实移动边界约束
  3. 因此当前最准确的定性是：
     - **三脚本边界守住**
     - **same-root Ready-To-Sync blocker 为真**
     - **但 slice 已经是 contract + runtime behavior change 混合态**
  4. 已生成正式裁定文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-03-工具V1-三脚本contract回执与停工裁定-55.md`
- 当前阶段判断：
  - 工具-V1这条线当前不是“继续自己收治理尾账”；
  - 更准确的是：
    - 先停在 `PARKED`
    - 等父线程 / scene owner 消化这刀里哪些 runtime 变化该保、该 trim、或该继续点名
- 当前恢复点：
  - 后续如果再审工具-V1，优先检查它有没有继续把 runtime change 冒充成 pure contract support；
  - 当前不允许把 same-root cleanup 继续压回工具-V1自己处理。

## 2026-04-03（屎山修复父层补记：导航当前最终分账已压实，真正剩余只剩 Primary traversal）

- 当前新增事实：
  1. `导航检查V2` 回执已审完：
     - 它当前成立的是：
       - accepted navigation version 的最终验收交接
       - 不是 legal-sync
  2. 工具-V1最新 `prompt_03` 已确认与 `-55` 一致：
     - 工具-V1继续 `PARKED`
     - 不再自己清 same-root dirty
  3. 当前总分账已正式落盘：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-03-导航最终分账与当前剩余总图-56.md`
- 当前阶段判断：
  - 当前导航不是“整体还没做完所以一锅红”；
  - 更准确的是：
    - 已认可玩家导航版本：成立
    - `导航检查V2` 最终交接：成立但未归仓
    - 工具-V1三脚本：成立但已停车
    - 真正剩余业务主线：只剩父线程 `Primary traversal`
- 当前恢复点：
  - 以后继续对用户汇报时，默认直接按 `-56` 分账；
  - 当前不要再把 targeted probe 红面和 `Primary traversal` 混成“整个导航没完”。

## 2026-04-03（屎山修复父层补记：总分账已转成三线程下一轮 prompt，后续默认按 57/58/59 分发）

- 当前新增事实：
  1. `-56` 总分账已经被翻译成三份具体续工/停车 prompt：
     - `-57` 父线程施工
     - `-58` `导航检查V2` 停车
     - `-59` 工具-V1停车
  2. 后续默认分发基线已经不再是：
     - “三条线程都继续看导航”
     而是：
     - 只有父线程继续业务施工
     - 其余两条线只保留停车 truth
- 当前恢复点：
  - 如果用户继续要转发 prompt，默认直接给 `-57 / -58 / -59`；
  - 如果以后还要开新 prompt，必须建立在这三条分工已经先被消费完的基础上。

## 2026-04-03（屎山修复父层补记：用户直接重开工具-V1脚本侧新切片，目标改成 inspector-driven 接线底座）

- 当前新增事实：
  1. 用户没有让工具-V1回去清 same-root dirty，也没有让它碰 `Primary/Town` scene；
  2. 用户新的直接裁定是：
     - 做一个挂在空物体上的 manager；
     - 由用户自己在 Inspector 里拖 Water / Props / Border / NavGrid / Player 等引用；
     - 先解决 traversal blocking、场景外越界、以及转场黑屏卡顿掩蔽；
     - 不再赌 scene 硬写。
  3. 工具-V1已实际落下这套脚本底座：
     - `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
- 当前阶段判断：
  - 这不是 `导航检查V2` 的解冻，也不是 scene integration 完成；
  - 更准确的是：
    - **脚本接线底座已落地**
    - **scene 仍待 owner 手动挂载 / 拖引用 / 实际验收**
- 当前恢复点：
  - 后续如果继续推进这一面，应该优先由 scene owner 把 manager 接回 `Primary` 或其他实际 scene；
  - 当前不要再把“脚本底座落地”写成“玩家现在已经一定不能下水 / 一定不能出图”。

## 2026-04-03（屎山修复父层补记：工具-V1新切片当前已停在“等 scene 接线 + 手测”）

- 当前新增事实：
  1. 工具-V1这轮脚本改动已经落地，但没有继续尝试 legal-sync；
  2. 当前它已经重新 `Park-Slice`；
  3. 它此刻真正等待的不是治理 cleanup，而是：
     - scene owner / 用户把 manager 挂进 scene
     - 然后做真实 Play 验证
- 当前恢复点：
  - 后续如果用户继续追这条线，优先先问：
    - manager 有没有挂进 scene
    - Water / Props / Border / NavGrid / PlayerMovement 有没有拖齐
    - Play 复测结果是什么

## 2026-04-03（屎山修复父层补记：桥面问题已被改判为“walkable override 缺口”，不再归因于水碰撞本身）

- 当前新增事实：
  1. 用户反馈的新问题不是“水没拦住”，而是：
     - 水已经拦住了
     - 但桥面没有把水上的这段重新判回可走
  2. 工具-V1已补完脚本侧解法：
     - `NavGrid2D` 新增显式可走覆盖源
     - `TraversalBlockManager2D` 新增桥面 `walkable override` 拖拽槽位
  3. 当前正确口径是：
     - Water 仍然禁行
     - Bridge 作为覆盖层单独声明为可走
- 当前恢复点：
  - 后续不要再从“排序层高低”推断能不能走；
  - 统一改成：
    - 阻挡源 = Water / Props / Border
    - 可走覆盖源 = Bridge / 其它跨水通路

## 2026-04-03（屎山修复父层补记：父线程已把工具-V1的 TraversalBlockManager2D 接回 Primary binder，但 live 又卡在 Unity busy）

- 当前新增事实：
  1. 父线程没有去改工具-V1 own 的三脚本，而是只改了：
     - `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
  2. 这轮真正推进的是：
     - 把工具-V1新补的 `TraversalBlockManager2D` 接回 `Primary` 的 scene/binder 侧；
     - 正式把 `blocking tilemaps` 和 `bounds tilemaps` 分离，不再继续把 `Layer 2 - Base` 混成 obstacle。
  3. 父线程当前只读压实的 `Primary` 结构真相：
     - `Layer 1 - Farmland_Water` 与 `Layer 1 - Props_Porps` 当前都是空 tilemap；
     - 非空 blocking tilemap 真实集中在 `Wall + Props_*`；
     - 非空 bounds tilemap 真实集中在 `Layer 1 - Base + Layer 2 - Base`。
  4. live 入口新结论：
     - 命令桥不是死的，`Assets/Refresh` 请求能被 Unity 吃掉并归档；
     - 但 Unity 随后又卡在 `isCompiling=true`，导致 fresh Play 验证窗口暂时拿不到。
- 当前阶段判断：
  - 当前 `Primary traversal` 没有漂回 crowd / runner / 工具 cleanup；
  - 也没有再把球扔回工具-V1；
  - 更准确的阶段是：
    - **scene/binder 已前进到 manager-driven integration**
    - **真实入口体验验证仍被 Unity busy/compiling 阻断**
- 当前恢复点：
  - 先等 Unity 从本轮 `Assets/Refresh` 后的 busy 状态恢复；
  - 再由父线程继续 fresh 验 `Props / Water / tilemap 外边界 / NPC同语义`；
  - 当前还不到把“Primary traversal 已闭环”写成完成的时候。

## 2026-04-03（屎山修复父层补记：Primary 自动回写根因已从 Editor 侧切断）

- 当前新增事实：
  1. 用户最新现场已确认“TraversalBlockManager2D 参数总被改回去”的第一嫌疑不是手动误拖，而是 Editor 自动回写。
  2. 导航检查线已只改两处：
     - 删除 `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
     - 从 `Assets/000_Scenes/Primary.unity` 的 `NavigationRoot` 上移除旧 `TraversalBlockManager2D`
  3. 这意味着：
     - `Primary` 不再有自动 binder 在 `delayCall / hierarchyChanged / sceneOpened` 上偷偷重写 traversal 现场；
     - 用户自己放在 `2_World` 下那份 manager，终于不再被旧 `NavigationRoot` manager 抢配置。
- 当前阶段判断：
  - `Primary traversal` 当前从“scene/binder 自动补丁持续搅局”推进到了“用户手动 manager 配置终于可稳定保存”的阶段；
  - 但桥面与水体的最终可走语义仍待后续 live 验。
- 当前恢复点：
  - 后续优先回到“用户自己的 manager 配置 + 当前脚本 contract 是否足够”这条线；
  - 不再回头把 `PrimaryTraversalSceneBinder` 当作临时补丁器恢复。

## 2026-04-03（屎山修复父层补记：放置成功卡顿已定性为精确 contract 问题，不升级导航父线程）

- 当前新增事实：
  1. 只读复核 `NavGrid2D / ChestController / TreeController` 后，当前“placeable 成功那一下卡顿”的最硬链路已压实为：
     - `ChestController.Start()`
     - `RequestNavGridRefreshDelayed()`
     - `NavGrid2D.OnRequestGridRefresh`
     - `RefreshGrid()`
     - `RebuildGrid()`
  2. `NavGrid2D` 当前没有区域刷新入口，运行时刷新 contract 仍是整图重建；
  3. `TreeController` 这轮已不是主嫌疑，因为树苗 `Stage 0` 已通过当前展示/碰撞同步条件规避掉旧的误刷路径。
- 当前阶段判断：
  - 这件事与导航有关，但不是“导航父线程回来接大包”的级别；
  - 更准确地说，它是：
    - **当前三脚本精确响应位应接的 contract 粒度问题**
    - 不是 `Primary traversal` 父线程主线问题。
- 当前恢复点：
  - 后续若要真修，优先以 `NavGrid2D contract + ChestController caller` 的最小 reopen 处理；
  - 不把这件事再泛化成导航父线程整包返工。

## 2026-04-03（屎山修复父层补记：放置卡顿已在三脚本线内直接落地局部刷新，不再等待导航父线程）

- 当前新增事实：
  1. 导航检查线没有再发导航父线程 prompt，而是直接在 own 路径里完成了最小代码修复：
     - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
     - `Assets/YYY_Scripts/Controller/TreeController.cs`
  2. 当前修法不是“异步糊一下”或“删掉刷新”：
     - 而是给 `NavGrid2D` 补了 bounds 级局部刷新 contract；
     - 再把箱子/树的运行时 obstacle 变化切到这个局部入口。
  3. 现在 placeable 成功后的卡顿责任边界更明确：
     - 不是导航父线程整包问题；
     - 也不是 scene/binder 问题；
     - 是已被当前三脚本线精确收掉的一刀运行时 contract 修复。
- 当前阶段判断：
  - 这部分已经从“只读判责”进入“脚本侧已落地，待 Unity live 复测”的阶段；
  - 还不能把它写成最终体验完成，因为没有 live 体感证据。
- 当前恢复点：
  - 后续优先做用户 Unity 复测；
  - 如果 residual 卡顿仍在，再继续查是否还有别的 placeable caller 或 grid bounds 规模问题，而不是把父线程再次整包唤醒。

## 2026-04-03（屎山修复父层补记：残余放置卡顿又压实到放置验证热路径，不再只归因导航）

- 当前新增事实：
  1. 用户复测后明确表示“放置还是卡”；
  2. 导航检查线继续实查后发现，放置成功后 `PlacementManager` 会立刻恢复预览并重跑 `PlacementValidator`；
  3. 而 `PlacementValidator` 之前在运行时会用 `FindObjectsByType<TreeController/ChestController>` 扫全场，这本身就是新的热路径嫌疑。
  4. 当前这层也已经被直接改掉：
     - `ChestController / TreeController` 维护活动实例表；
     - `PlacementValidator` 改读活动实例表，不再每次放置验证全场扫描。
- 当前阶段判断：
  - 放置卡顿现在已经不是“只剩导航这一刀”；
  - 更准确地说，当前线程已经连续收掉两段热路径：
    - `NavGrid2D` 整图刷新
    - `PlacementValidator` 全场扫描
  - 但最终体验是否过线，仍要看用户 live 复测。
- 当前恢复点：
  - 先让用户再测一次体感；
  - 如果仍卡，再继续查 `PlacementManager` 即时预览恢复或放置特效/实例化链，而不是把问题回退成“导航父线程没处理”。

## 2026-04-03（屎山修复父层补记：桥面不可通行与树苗残余卡顿已继续在脚本侧补强）

- 当前新增事实：
  1. 只读审计 `Primary.unity` 后，桥面当前不是“配置完全没接”，而是：
     - 水是阻挡源；
     - `桥_底座` 已被挂进 walkable override；
     - 但旧的 override 判定太窄，靠桥边时仍会被水边 collider 误吞。
  2. 当前已直接在 `NavGrid2D.cs` 把 walkable override / obstacle tilemap 的命中从点采样改成面积查 tile，先用脚本侧把窄桥误判兜住。
  3. 树苗残余卡顿继续被拆出第二段重复热路径：
     - `PlacementManager` 在树苗落地时多做了一次 `SetStage(0)`；
     - `TreeController` 在 `SeasonManager` 已存在时还会多跑一次延迟初始化显示。
  4. 当前也已直接在脚本侧收掉：
     - `InitializeAsNewTree()` 只保留唯一 ID 与树苗基态归一；
     - 树苗放置链不再紧接着补一轮 `SetStage(0)`；
     - `TreeController.Start()` 仅在 `SeasonManager` 缺席时才延迟补初始化。
  5. `002批量-Hierarchy` 工具窗口失焦即关的根因，也已明确落到打开方式：
     - 现在文件里已经改回普通 `GetWindow<T>() + Show() + Focus()` 常驻窗口路径。
- 当前阶段判断：
  - 这轮新增的是脚本侧补强，不是 scene 最终收口；
  - 真实体验仍待用户 live 复测，不能写成最终完成。
- 当前恢复点：
  - 先让用户复测 3 件事：
    - 树苗是否还明显卡顿
    - 桥面是否能正常通过
    - `002批量-Hierarchy` 是否还能失焦即关
  - 如果桥仍然不过，再进入 scene 五段式审计，不直接扩成导航父线程整包返工。

## 2026-04-03（屎山修复父层补记：桥面 contract 已继续补到玩家实体 soft-pass）

- 当前新增事实：
  1. 只读 scene 审计继续压实后，桥现在最像是“两层问题叠加”：
     - 导航层需要 bridge override 更稳地命中；
     - 实体层还需要让玩家在桥 override 命中时，不再被 `Water` 的实体 collider 硬顶住。
  2. 当前这层也已经直接在脚本侧收掉：
     - `NavGrid2D` 新增公开 `HasWalkableOverrideAt(...)`
     - `PlayerMovement` 新增桥面命中时的 traversal soft-pass
     - `TraversalBlockManager2D` 把当前 traversal blocking colliders 一并绑定给玩家运行时
  3. 因此桥这条现在不再只是“理论上 A* 可走”，而是脚本 contract 上已经补到“玩家实体也应该能过桥”。
- 当前阶段判断：
  - 这仍然是脚本侧成立，不是 scene 最终完成；
  - 用户 live 复测仍然是最后一层真值。
- 当前恢复点：
  - 让用户先直接进 Unity 过桥；
  - 如果还被挡，再做 scene 五段式审计，不回退成“导航父线程整包缺席”的泛问题。

## 2026-04-03（屎山修复父层补记：树石批量工具新增提示已静音）

- 当前新增事实：
  1. 用户直接要求把最近新增的树/石批量工具提示全部关掉，不要再显示；
  2. 本轮收缩范围只锁在我新加的提示层，不扩到 `TreeControllerEditor.cs` / `StoneControllerEditor.cs` 里原本就存在的旧 warning；
  3. 已把 `Tool_004_BatchTreeState.cs` / `Tool_005_BatchStoneState.cs` 里的 `HelpBox`、`DisplayDialog`、`ShowNotification` 清掉；
  4. 已把树/石 Inspector 里“打开批量工具”按钮前的新增引导 `HelpBox` 清掉，仅保留按钮。
- 当前阶段判断：
  - 树/石批量工具现在回到“只给字段和按钮，不给新增提示”的轻量态；
  - 这仍是编辑器 UX 收口，不是 Tree/Stone 业务逻辑再返工。
- 当前恢复点：
  - 用户现在可直接在 Unity 里继续批量改树和石头；
  - 如果后续还要改，只应继续锁在树石编辑器工具层，不扩回其他系统。

## 2026-04-04（屎山修复父层补记：树石批量工具状态参数已按钮化）

- 当前新增事实：
  1. 用户明确要求树和石头批量工具里的参数都改成按钮，不再保留输入框/下拉；
  2. 当前已把树工具的 `treeID / 当前阶段 / 当前状态 / 当前季节` 改成按钮式选择；
  3. 当前已把石头工具的 `当前阶段 / 矿物类型 / 含量指数 / 当前血量` 改成按钮式选择；
  4. 数值型字段没有继续保留输入框，而是统一改成“预设按钮 + 步进按钮”。
- 当前阶段判断：
  - 树石批量工具现在已经进入“状态参数纯按钮选择”的编辑器交互态；
  - 这仍是工具层 UX 收口，不是 Tree / Stone 运行时逻辑返工。
- 当前恢复点：
  - 用户现在可直接回 Unity 使用新的按钮式批量参数面板；
  - 如果后续继续调，只需继续锁在 `Tool_004/005` 的排版和可点性，不扩题。

## 2026-04-04（屎山修复父层补记：Tree 批量窗口空引用导致的 GUI 报错已确认归我并热修）

- 当前新增事实：
  1. 用户贴出的 `MissingReferenceException` 直接命中 `Tool_004_BatchTreeState.DrawSelectionSummary()`，属于我这轮编辑器工具代码的问题；
  2. 后续 `Invalid GUILayout state` / `GUIClips` 报错是同一异常把 `OnGUI` 布局栈打断后的连带结果；
  3. 当前已在 `Tool_004/005` 两个批量窗口都补上 stale-reference 清理和 scope 化布局保护。
- 当前阶段判断：
  - 树石批量工具当前已从“对象销毁后可能炸窗口”回到更稳的编辑器态；
  - `UnityEditor.Graphs.Edge.WakeUp()` 暂不归到这条树石工具线上。
- 当前恢复点：
  - 用户现在可继续复测树/石批量窗口；
  - 若仍出现非 `Tool_004/005` 栈内错误，再按新栈单独收窄。

## 2026-04-05（屎山修复父层补记：树石批量工具按钮交互层已从 Toggle 改成显式 Button）

- 当前新增事实：
  1. 用户反馈 `005批量-Stone状态` 工具按钮点了不更新、界面像卡住；
  2. 当前最可疑实现点不是 `StoneController` 底层，而是批量工具 UI 仍在用 `GUILayout.Toggle(..., "Button")` 做伪按钮组；
  3. 当前已把 `Tool_004/005` 两个批量工具都改成显式 `GUILayout.Button` 选中逻辑，并在点击/读值/应用后强制刷新窗口。
- 当前阶段判断：
  - 树石批量工具现在已从“伪按钮组交互不稳定”切回更明确的按钮交互态；
  - 是否完全恢复，要以用户再次实机点击为准。
- 当前恢复点：
  - 用户现在直接回 Unity 再测按钮交互；
  - 若仍异常，下一刀就继续往应用链和编辑器事件链压缩，不再停留在样式层。

## 2026-04-03（屎山修复父层补记：NPC 已开始并行吃桥/水/边缘 contract）

- 当前新增事实：
  1. 用户已口头认可玩家当前桥 / 水 / 边缘体验，这条父层主线不再回头重修玩家；
  2. 本轮导航检查已把 `NPCAutoRoamController` 和 `TraversalBlockManager2D` 接到 NPC 等价 contract：
     - NPC 桥面中心脚底 probe
     - 基于 walkable override 的 water soft-pass
     - NPC nav bounds enforcement
  3. 当前没有 scene 实写，也没有改 binder / traversal Inspector 配置。
- 当前阶段判断：
  - 这轮已把“NPC 还像走旧接线”收成脚本侧补口；
  - 但父层还不能把 NPC 桥面体验写成最终关闭，因为真实 live 过桥真值还没补到。
- 当前恢复点：
  - 如果后续继续，只应补 NPC 最小 live probe；
  - 若 probe 仍失败，再精确点名是脚本 gap 还是 scene source gap，不准偷改 scene 混过去。

## 2026-04-03（屎山修复父层补记：NPC 接线已有 runtime probe）

- 当前新增事实：
  1. 本轮已补最小 runtime probe；
  2. `TraversalBlockManager2D` 在 Play 中已真实报出 `npcBindings=3，npcBounds=on`；
  3. 说明 NPC contract 绑定已跑进运行时，但仍未直接证明“桥上体验完全过线”。

## 2026-04-03（屎山修复父层补记：NPC bridge / water / edge targeted acceptance 已过）

- 当前新增事实：
  1. 本轮没有回碰玩家 traversal，也没有 scene 实写；
  2. `Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs` 已改成 `DebugMoveTo` 直达 probe，并在起跑前自动吸到最近可走格中心；
  3. fresh Play 结果已明确拿到：
     - `bridge_probe_pass final=(-9.10, 1.42) sawBridgeSupport=True inWater=False`
     - `edge_probe_pass position=(8.02, 2.25) inBounds=True`
     - 总结 `PASS bridge+water+edge`
  4. 本轮为继续验收，顺手清掉了一条外部 compile 门：
     - `SpringDay1PromptOverlay.cs` 的 `Object` 二义性
     - 仅是限定名修正，不是 UI 行为返工。
- 当前阶段判断：
  - NPC 当前已经有“结构接线 + targeted acceptance”两层证据；
  - 这足以证明 NPC 已能接入玩家当前认可的桥 / 水 / 边缘 contract；
  - 但如果后续还要追“自然 roam 体验是否同样稳定”，仍需额外 live case，不应把 targeted probe 误写成完整体验终验。
- 当前恢复点：
  - 用户若只关心“NPC 是否已经吃到等价 traversal contract”，当前可以按已成立处理；
  - 若后续要继续，只应补更贴近自然 roam 的体验验证，不要再回头重修玩家版。

## 2026-04-03（屎山修复父层补记：NPC 自然过桥也已拿到 fresh 正样本）

- 当前新增事实：
  1. 本轮继续补了一刀更贴近真实漫游的验证，不再只停在 `DebugMoveTo`；
  2. `Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs` 新增了自然桥入口：
     - `Tools/Codex/NPC/Run Natural Roam Bridge Probe`
  3. fresh 查明并修掉了一处 probe 自身接线错误：
     - 旧逻辑把 `homeAnchor` 误写成吸附后的起点
     - 导致 `StartRoam` 的 roam center 回到桥西侧，报出假 `ShortPause` 失败
  4. 修正后 fresh 结果：
     - `bridge_natural_probe_pass ... final=(-9.13, 1.42) sawBridgeSupport=True inWater=False state=Moving`
- 当前阶段判断：
  - NPC 这条线现在已经不只是“结构接线 + targeted probe”；
  - 还额外拿到了一条“自然 StartRoam 过桥”正样本；
  - 这让当前 bridge contract 的可信度进一步上升。
- 当前恢复点：
  - 如果继续补强，下一步应只考虑“自然 roam 下 edge / 长时间体验”；
  - 不应再回头重修玩家 bridge / water / bounds 方案。
## 2026-04-03（屎山修复父层补记：camera confiner 回归已止血，镜头重新跟随玩家）

- 当前新增事实：
  1. 用户直接报出“镜头不动、不跟着玩家走”，本轮先按回归事故处理，只碰 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`，不碰 `Primary.unity`。
  2. live 取证先确认：`CinemachineCamera` 的 `Follow` 没丢，真问题是 confiner 把镜头提前钉死。
  3. 修法已落在脚本侧：
     - 自动 bounds 不再只吃一张最窄的 exact base tilemap，而是会联合 exact base 候选；
     - 对当前 legacy 默认排除词做软化，避免把 `water / props / farmland` 一刀切排出可见场景边界；
     - 自动 bounds 现在会额外吸收 world layer 下的 `SpriteRenderer` 可见范围，补上房屋/桥面这类非 tilemap 可见内容；
     - 新增宽屏保护：超宽画面时自动收窄 camera viewport，避免全屏或超宽窗口把镜头直接锁死。
  4. fresh Play runtime 证据：
     - 修前：`Main Camera` 被强制修正到右边界，`PositionCorrection.x=-8.166666`
     - 修后：`Player` 与 `CinemachineCamera` 世界坐标重新对齐，`PositionCorrection=(0,0,0)`
     - 新 `WorldBounds` 读数约为 `center=(-13.625, 16.0)`、`size=(56.25, 65.0)`
  5. fresh screenshot 已补，镜头画面重新回到跟随态；Unity 也已退回 `Edit Mode`。
- 当前阶段判断：
  - `结构证据`：成立
  - `compile 证据`：成立
  - `真实体验`：这轮至少已拿到“镜头重新跟随玩家”的 live 证据，但用户自己的全屏/长路径终验仍待补。
- 当前恢复点：
  - 让用户优先复测：
    - 普通移动时镜头是否继续跟随
    - 双击 `Game` 全屏后是否还会出现“镜头不动”
    - 是否仍会拍到 scene 外
  - `SceneTransitionTrigger2D` 这轮没有继续改 runtime 行为；只保留上一轮黑幕异步加载和 scene path 兼容版本，若要最终收口仍需再补一次 end-to-end 转场终验。

## 2026-04-03（屎山修复父层补记：camera 左右残留蓝边继续收窄到“真实占用格子”）

- 当前新增事实：
  1. 用户继续反馈：镜头跟随已经恢复，但最左和最右仍会露出一条很窄的深蓝色场景外边。
  2. 本轮仍然只碰 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`，没有回碰 `Primary.unity`、binder、scene sync，也没有改用户当前 traversal Inspector 配置。
  3. `TryGetTilemapWorldBounds()` 已从直接使用 `tilemap.localBounds`，改成遍历 `cellBounds` 内真实 `HasTile` 的格子来收世界边界；空白但仍落在旧 bounds 里的列/行不再被当成可见世界宽度。
  4. `ComparePreferredTilemaps()` 与 `ShouldIncludeTilemapInAutoBounds()` 也同步改成同一口径，避免排序和筛选阶段还沿用旧的空白列面积。
  5. 这轮拿到的验证证据：
     - `validate_script(CameraDeadZoneSync.cs)`：`0 error / 2 warning`
     - `git diff --check -- Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`：仅 CRLF/LF 提示，无 diff 结构错误
     - Unity `refresh_unity` 已请求脚本编译，随后两次 `read_console(error)` 都是 `0 error`
- 当前阶段判断：
  - `结构 / checkpoint`：成立
  - `targeted probe / 局部验证`：成立到“脚本编译和控制台无新红错”
  - `真实入口体验`：尚未最终确认，仍待用户亲自复测左右蓝边是否完全消失
- 当前恢复点：
  - 如果用户复测仍能看到边缘蓝边，下一步优先继续查 `Camera.rect` 宽屏保护与当前 scene base 内容真实宽度之间是否还存在 1 格以内残差；
  - 但在用户给出 fresh 画面前，不能把这轮写成“camera 体验完全过线”。

## 2026-04-04（屎山修复父层补记：仅运行时左侧白边，继续收 runtime viewport / confiner 单侧残差）

- 当前新增事实：
  1. 用户最新补充：现在不是编辑态也有问题，而是“只有运行时左侧还会出现镜头溢出边界”；右侧和上下已经对了。
  2. 这条信息把根因进一步收窄为：不只是左侧地形不规则，更像 `Play` 时才会生效的 viewport / confiner 残差。
  3. 本轮仍然只碰 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`，没有改 scene、没有改 binder。
  4. 新落的脚本修复点：
     - `RefreshBounds()` 改成先应用最终 runtime viewport，再让 `CinemachineConfiner2D` 重算缓存；
     - `LateUpdate()` 里如果屏幕尺寸变了，只有在 viewport rect 真发生变化时才重新 `InvalidateBoundingShapeCache()`；
     - 新增 `snapViewportClampToPixelGrid`，把 runtime `Camera.rect` 吸到像素网格，降低只在 `Play` 里出现的一侧细白边/细缝。
  5. 本轮额外做的 live 取证：
     - 进过一次短 `Play`
     - 读到 runtime `CinemachineCamera`、`CameraDeadZoneSync`、`_CameraBounds` 组件值
     - runtime `WorldBounds` 约为 `center=(-14.45,16)`、`size=(54.6,65)`
     - 当前 `PositionCorrection=(0,0,0)`，说明不是“confiner 继续把镜头钉死”的旧问题
  6. 本轮验证：
     - `validate_script(CameraDeadZoneSync.cs)`：`0 error / 2 warning`
     - Unity 脚本编译请求后，连续两次 `read_console(error)`：`0 error`
     - Unity 已主动退回 `Edit Mode`
- 当前阶段判断：
  - `结构 / checkpoint`：成立
  - `targeted probe / 局部验证`：成立到 runtime 组件值和编译/控制台证据
  - `真实入口体验`：仍待用户复测左侧白边是否完全消失
- 当前恢复点：
  - 如果用户这次复测仍看到左侧白边，下一步优先继续查：
    - 用户实际全屏/窗口比例下的 `Camera.rect`
    - 和 confiner 在该比例下的窗口尺寸是否还有 1px 级别偏差
  - 但在用户 fresh 反馈前，不能把这轮写成“镜头最终体验已完全过线”。

## 2026-04-04（屎山修复父层补记：命中 exact base tilemap 后不再让 runtime Sprite 把相机边界外扩）

- 当前新增事实：
  1. 用户要求我先把自己这条线能负责的问题彻底修掉；本轮继续只碰 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`。
  2. 这轮前置核查已补齐：
     - `skills-governor`
     - `preference-preflight-gate`
     - `global-preference-profile.md`
     - 手工等价 `sunset-startup-guard`
     - `sunset-no-red-handoff`
  3. 根因继续收窄为：
     - `Edit` 下不溢出、`Play` 下左侧才溢出；
     - 说明比起地形本身，更像运行时参与边界计算的内容把一侧 world bounds 撑宽；
     - 当前 `TryCalculateAutoBounds()` 在已经命中 exact base tilemap 后，仍会继续把 `SpriteRenderer` bounds 纳入世界边界。
  4. 本轮新落脚本修复：
     - `SelectAutoBoundsTilemaps(out bool usingExactBaseTilemaps)`
     - 当已命中 `Layer 2 - Base / Layer 1 - Base` 这类 exact base tilemap 时，`TryCalculateAutoBounds()` 不再继续吸收 `SpriteRenderer` bounds；
     - `SpriteRenderer` 只保留给“没有 exact base tilemap 时”的 fallback。
  5. 本轮验证：
     - `validate_script(CameraDeadZoneSync.cs)`：`0 error / 2 warning`
     - Unity 编译请求后，`read_console(error)` 没有新增 `CameraDeadZoneSync` 相关红错
     - 短 Play probe 里，`WorldBounds` 已收紧到 `center=(-14,16)`、`size=(53,65)`
     - 运行时截图已补：`Assets/Screenshots/camera-runtime-check.png`
     - Unity 已退回 `Edit Mode`
- 当前阶段判断：
  - `结构 / checkpoint`：成立
  - `targeted probe / 局部验证`：成立到“bounds 已进一步收紧 + 编译无新红”
  - `真实入口体验`：仍待用户把玩家走到最左边界后终验，当前不能把这轮写成体验彻底过线
- 当前恢复点：
  - 等用户做真实左侧贴边复测；
  - 若仍有残边，下一步只继续查玩家贴左边时的相机位置与 `Camera.rect`，不再把 runtime Sprite 重新并入 exact base world bounds。

## 2026-04-04（屎山修复父层补记：用户改口为三层并集，同时修掉 frustum warning 的主因）

- 当前新增事实：
  1. 用户最新明确改口：镜头范围不要再按 `base-only`，而应该按 `LAYER 1 / LAYER 2 / LAYER 3` 三个 world layer 里的内容并集来算。
  2. 用户同时追问 `Screen position out of view frustum`；这轮已把它和相机需求一起处理。
  3. 这条 warning 的主因已锁到输入换算，而不是 camera confiner 本身：
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:1013`
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:1876`
     - 两处都把 `Input.mousePosition` 以 `z=0` 直接喂给 `Camera.ScreenToWorldPoint(...)`
     - 对当前 2D 相机来说，`z` 这里代表“离相机多远”，不是“世界坐标 z”；给 `0` 就会触发 Unity 的 frustum warning
  4. 为了不留下同类坑，debug 生成脚本也顺手一起改了：
     - `Assets/YYY_Scripts/World/WorldSpawnDebug.cs`
  5. 本轮落的修复：
     - `CameraDeadZoneSync.cs`
       - `SelectAutoBoundsTilemaps()` 改回直接收 `worldLayerNames` 下所有可用 Tilemap
       - `ShouldIncludeTilemapInAutoBounds()` 不再把 `water / props / farmland` 这类 tilemap 从三层并集里排掉
       - `SpriteRenderer` 现在只在“三层里一个 Tilemap 都没找到”时才当 fallback
     - `GameInputManager.cs`
       - 新增 `ScreenToGameplayWorld(...)`
       - 右键自动导航取鼠标世界坐标、通用 `GetMouseWorldPosition()` 都改成用“相机到世界平面 z=0 的距离”，不再用 `z=0`
     - `WorldSpawnDebug.cs`
       - 同样改成先算正确的世界平面深度，再做 `ScreenToWorldPoint`
  6. 本轮验证：
     - `git diff --check`：无结构错误，仅 `CameraDeadZoneSync.cs` 的 CRLF/LF 提示
     - Unity `Editor.log` 里在本轮最后几次 `Reloading assemblies after forced synchronous recompile` 之后，没有再看到新的 `Screen position out of view frustum`
     - `Editor.log` 里能看到的 `error CS0246` 来自既有 `Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs`，不是本轮新增
- 当前阶段判断：
  - `camera`：已按用户新口径切回“三层并集”
  - `frustum warning`：主因补丁已落到常驻输入链与 debug 链
  - `真实入口体验`：仍待用户自己进游戏确认两件事
- 当前恢复点：
  - 让用户复测：
    - 镜头边界现在是否按三层并集工作，不再只守 base
    - 那条 `Screen position out of view frustum` 是否还会在正常游玩里出现
  - 如果 warning 仍复现，下一步优先继续追剩余同类调用点，而不是再回头怀疑 confiner。

## 2026-04-04（屎山修复父层状态快照：历史需求剩余项重新盘点）

- 当前盘点结论：
  1. `player` 侧的桥 / 水 / 边缘行走问题，用户此前已经明确认可“玩家现在这个版本我也认可了”，所以这块不再算当前主 blocker。
  2. `npc` 过桥与同套 traversal 接入，仍在 `导航检查` 线程 active 收口，当前不是我这条线在做。
  3. `camera` 这条线现在剩的不是“继续大改逻辑”，而是：
     - 复测“三层并集”口径是否符合用户实际视觉预期
     - 复测 `Screen position out of view frustum` 是否已在正常玩法链消失
  4. `scene transition` 历史需求仍没有完全画句号：
     - `SceneTransitionTrigger2D` 的黑幕异步加载和 scene path / Build Profile 兼容脚本版本已存在
     - 但 end-to-end 的真实切场体验仍待单独终验，当前不能写成“转场最终体验已收口”
  5. 树苗放置卡顿已明确转交 `农田交互修复V3` 线程，不再是我这条线 own。
  6. `Tool_002_BatchHierarchy` 当前不是 active blocker；对应线程是 `scene-build-5.0.0-001`，状态是代码已落、等用户在 Unity 里继续验“确认选取 + 持久化”。
- 当前最该提醒用户的事实：
  - 从“历史原始大包”看，现在真正还悬着的主要是 4 块：
    1. `camera` 终验
    2. `scene transition` 终验
    3. `npc traversal` 终验
    4. `sapling stutter` 农田线程收口
  - 这 4 块里，只有第 1 块是我这条线刚刚还在直接动的。

## 2026-04-05（屎山修复父层补记：Town 相机 / 输入 / frustum 只收脚本契约，运行态未再见 frustum，但 Town 真实载入探针受限）

- 当前主线目标：
  - 只在允许范围内把 `Town` 进入链上的相机 / 输入 / `frustum` 问题推进到基础设施闭环
  - 不回到 `Primary/Town` scene wiring、UI、导航或通用工具
- 本轮子任务：
  - 继续收 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`
  - 继续收 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - 继续收 `Assets/YYY_Scripts/World/WorldSpawnDebug.cs`
  - 必要时补最小契约 `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
- 本轮实际做成了什么：
  1. `CameraDeadZoneSync.cs`
     - `sceneLoaded` 后会先重抓当前 scene 的 `CinemachineCamera / Main Camera`
     - 若 `Main Camera` 已切换，会重置默认 `Camera.rect` 捕获，再重新 `SetupConfiner + ValidateReferences + RefreshBounds`
  2. `GameInputManager.cs`
     - `Awake()` 的初始相机绑定改成 `ResolveWorldCamera()`
     - 订阅 `SceneManager.sceneLoaded`，在新场景下一帧重绑 `worldCamera`
     - `HandleRightClickAutoNav()` / `GetMouseWorldPosition()` 改成统一走 `ScreenToGameplayWorld(...)`
     - 该换算现在会：
       - clamp 到当前 `camera.pixelRect`
       - 使用“相机到 `z=0` 世界平面”的正确深度，不再用 `z=0`
       - 在 `SceneTransitionRunner.IsBusy` 时直接短路，避免切场窗口继续做屏幕转世界
  3. `WorldSpawnDebug.cs`
     - 同样改成 `sceneLoaded` 后重绑相机
     - 所有鼠标到世界坐标换算统一走正确深度与 `pixelRect` clamp
  4. `SceneTransitionTrigger2D.cs`
     - `SceneTransitionRunner` 在黑幕期间会缓存并关闭 `GameInputManager` 输入
     - `fade-in` 结束后再恢复原输入状态
     - 这条补口的目的不是改 Town scene，而是切断“旧输入链在切场窗口继续跑”的剩余风险
- 本轮验证：
  - `manage_script validate`
    - `GameInputManager`：`clean`
    - `SceneTransitionTrigger2D`：`clean`
    - `CameraDeadZoneSync`：`0 error / 2 warning`
    - `WorldSpawnDebug`：`0 error / 1 warning`
  - `git diff --check`
    - 本轮 4 个脚本无结构错误；仅保留既有 `CRLF/LF` 提示，不是新的 diff 断裂
  - Unity / MCP 低负载探针
    - `clear console -> enter Play -> read_console`
    - 本轮没有再读到新的 `Screen position out of view frustum`
    - 但 `manage_scene` 在 PlayMode 下不能直接载入 `Town`，工具自己返回：
      - 需要改走 `SceneManager.LoadScene()/LoadSceneAsync()`
    - 所以“真实 `Primary -> Town` 切场链已终验”这句话当前仍不能写
  - 本轮探针同时暴露的外部红面：
    - `Primary` 运行时存在 `The referenced script (Unknown) on this Behaviour is missing!`
    - `Primary` 运行时存在 `There are no audio listeners in the scene`
    - 还有 Unity 编辑器 `Graphs` 侧空引用与 MCP websocket 噪音
    - 这些都没有指向我本轮 own 的 4 个脚本
- 当前阶段判断：
  - `脚本契约`：成立
  - `Town 进入链上的相机 / 输入 / frustum` 主嫌疑：仍在我 own 范围内，而且这轮已补到更稳的口径
  - `真实 Town end-to-end 切场终验`：尚未闭环，原因不是 scene 没改，而是当前可用探针无法在 PlayMode 下直接把 `Town` runtime load 进去
- 当前恢复点：
  - 如果用户继续压这条线，下一步只该做 2 选 1：
    1. 用真实 `SceneTransitionTrigger2D` 手走一次 `Primary -> Town` 终验
    2. 或补一个只读 / 临时的 runtime probe 入口，用 `SceneManager.LoadSceneAsync()` 直接验证 Town 进入窗口
  - 不该再回头碰 `Town.unity / Primary.unity` 实写，也不该扩回导航或 UI
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - 当前 blocker：
    - `town-runtime-load-probe-blocked-by-manage_scene-playmode-limitation`
    - `external-primary-console-noise-missing-script-and-audio-listener`
## 2026-04-04｜NPC 自然漫游静态审计与补口：先收 bounds 归一化 + 被动 NPC 堵墙改线

- 当前主线目标：
  - 不重修玩家已认可版本；
  - 先在纯静态层把 NPCAutoRoamController.cs 里最像导致 NPC 撞墙卡住 / warning 连发的上层缺口补掉；
  - 然后再向用户申请占用 Unity 做 runtime 验证。
- 本轮子任务与服务关系：
  - 先确保当前准备改的导航代码已有可回退 checkpoint：7e06c2e6；
  - 再做 NPCAutoRoamController + NPCMotionController 静态审计；
  - 本轮实际只修改 NPCAutoRoamController.cs。
- 本轮实际完成：
  1. 重新压实责任链：
     - 玩家和 NPC 共享同一份 NavGrid2D + soft-pass + bounds enforcement 底层 contract；
     - 但 NPC 仍走独立的 roam / avoidance / recovery 上层链，不是“另一张静态导航”，而是“同底层、上层更脆”。
  2. 在 NPCAutoRoamController.cs 收了两刀纯静态补口：
     - 所有 roam 采样点 / requestedDestination / rebuildDestination 统一先经过 NormalizeDestinationToNavGridBounds(...)，不再把越出 nav world bounds 一点点的点直接喂给 TryFindNearestWalkable(...)；
     - TryBeginMove() 与多处 ResetMovementRecovery(...) 改用 GetNavigationCenter()，不再混用 	ransform.position 与 b.position 做导航恢复基准；
     - 当共享避让遇到“静止 NPC 挡墙”且 detour / rebuild 都失败时，先尝试 TryBeginMove() 改线，不再立刻掉成 SharedAvoidanceRepathFailed -> interrupt warning。
  3. 一次性只读子智能体 Sagan 已收结果并关闭。
- 关键判断：
  - 这轮最像的静态真问题，不是 NPCMotionController 基础运动桥接器本身坏掉；
  - 而是 NPCAutoRoamController 的“采样点越界 + 被动 NPC 堵墙 fallback 太硬”让 NPC 更容易撞墙、抖动、掉 warning。
- 验证结果：
  - git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs 通过；
  - alidate_script（含 --skip-mcp）与直接 CodexCodeGuard.dll 在当前环境都超时，未形成可靠结果；
  - 已人工复查新增 helper 与调用点，未发现明显语法级结构错误；
  - 当前尚未进入 Unity live，符合用户“先静态、后申请”的顺序要求。
- 当前阶段：
  - 静态补口已完成，线程已 Park-Slice；
  - 下一步应只做一件事：向用户申请占用 Unity，专门复测 NPC 自然漫游撞墙 / warning / bridge-water-edge runtime 真值。
- changed_paths：
  - D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs
- thread-state：
  - Begin-Slice=已跑
  - Ready-To-Sync=未跑（本轮未收口 sync）
  - Park-Slice=已跑
  - 当前状态：PARKED
- 时间：2026-04-05 00:29:33 +08:00

## 2026-04-05（屎山修复父层补记：Town 镜头不跟随已补成脚本自愈，最新 `frustum` 红确认为 Unity Tilemap 编辑器外部噪音）

- 当前新增事实：
  1. 用户最新现场先给出两条连续反馈：
     - 进入 `Town` 后不再爆旧的 runtime 红屏，但镜头不跟着玩家走
     - 随后又再次看到 `Screen position out of view frustum`
  2. 静态核实 `Town.unity` 后，`CinemachineCamera.Target.TrackingTarget` 仍然是空引用；
     - 所以“Town 相机不跟随”这一级问题，确实还在脚本 / 相机契约这条线上，而不是 layer 或 UI。
  3. 本轮只改了 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`，补成：
     - `Start()` 就会按 active scene 刷一次 scene references
     - `sceneLoaded` 后短 retry 数帧，自动把 `CinemachineCamera.Follow` 重绑回真正的玩家根
     - 玩家解析优先按 `PlayerMovement`，并用 `Rigidbody2D` / scene 优先级避开 `Tool` 这类同 tag 干扰
     - `LateUpdate()` 额外加了 Follow 丢失自愈兜底
  4. 用户随后贴出的最新 `frustum` 红，这轮已经通过 `Editor.log` 追到真实堆栈：
     - `UnityEditor.Tilemaps.GridEditorUtility:ScreenToLocal`
     - `UnityEditor.Tilemaps.PaintableSceneViewGrid`
     - 说明这次红属于 Unity Tilemap 画笔 / SceneView 编辑器态，不是当前 Town runtime 相机 / 输入链。
- 当前阶段判断：
  - `Town` 镜头不跟随：脚本侧补口已落，等待用户重新走一次 `Primary -> Town` 真正复测
  - 最新 `frustum` 红：当前不应再算到工具-V1线程自己的 runtime 问题上；应按 Unity 编辑器 Tile Palette / SceneView 状态噪音处理
- 当前恢复点：
  - 如果用户下次复测看到的是“镜头仍不跟随”，这条线继续只查 Town runtime 到底绑定到了哪个 `PlayerMovement`
  - 如果只再看到 `PaintableSceneViewGrid` 这条 warning，则不该继续让工具-V1线程背 runtime 锅

## 2026-04-05（屎山修复父层补记：Town `Main Camera` 静态缺 `CinemachineBrain`，工具-V1 已改成 runtime 自动补挂）

- 当前新增事实：
  1. 用户 fresh 反馈为：上一轮补了 Town 的 `Follow` 重绑后，镜头仍然不跟随。
  2. 继续静态核查 `Town.unity` 后，确认 `Main Camera` 当前只有：
     - `Transform`
     - `Camera`
     - `AudioListener`
     - 没有 `CinemachineBrain`
  3. 因而这条 Town 相机问题的真实链条已经进一步压实为两级：
     - `CinemachineCamera.Target.TrackingTarget` 静态为空
     - `Main Camera` 静态又缺 `CinemachineBrain`
  4. 工具-V1 本轮继续只改 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`：
     - 新增 `EnsureCinemachineBrain()`
     - `Awake()` 与每次 `RefreshSceneReferences(...)` 时若发现主相机没脑子，就 runtime 自动补挂并启用
- 当前阶段判断：
  - Town 相机不跟随这条线，目前脚本侧两级主嫌疑都已经被补上；
  - 接下来最关键的不再是继续静态猜，而是用户重新走一次 `Primary -> Town` 看是否已经恢复跟随。
- 当前恢复点：
  - 若仍失败，下一步只该继续查 runtime 下真正 active 的 `Main Camera / CinemachineCamera / PlayerMovement` 绑定现场；
  - 不该回到 scene wiring 泛修，也不该把刚确认是 `PaintableSceneViewGrid` 的编辑器 warning 再算回 runtime 链。

## 2026-04-05｜只读排查：当前 Console 里的 NPC oam interrupted 不是编译报错，而是自然漫游卡住 warning

- 当前主线目标：
  - 只读查明用户截图里的 NPC 导航告警到底是什么、是否仍然活着、现在主要坏在哪一层。
- 本轮子任务与服务关系：
  - 不进施工、不占 Unity 写现场；
  - 只读对齐 Editor.log + status.json + NPCAutoRoamController.cs，把 warning 源头和现状钉实。
- 只读查明的事实：
  1. 这批不是 compile error，而是运行时 Debug.LogWarning：
     - 触发点在 NPCAutoRoamController.cs:2412
     - 进入来源是 CheckAndHandleStuck(...) 的 progress.ShouldCancel -> TryInterruptRoamMove(StuckCancel, ...)
  2. 当前 Unity 仍在 Play Mode：
     - status.json = isPlaying=true
     - isCompiling=false
     - 所以这不是编辑器红编译，而是游戏跑着时 NPC 自然漫游不断卡住。
  3. 当前 warning 的主型是：
     - Reason=StuckCancel
     - 不是 SharedAvoidanceRepathFailed
  4. 统计 Editor.log 当前样本：
     - BlockerKind=None 远多于 BlockerKind=NPC
     - 说明多数 warning 不是“明确识别到某个 blocker 然后报错”，而是 NPC 长时间没有足够位移，被 stuck 检测直接取消当前漫游。
  5. 热点 NPC 很集中：
     -  03 次数最高，其次是 201 / 102 / 101 / 202 / 103
     - 多条日志里同一个 NPC 的 Current=... 基本不变，但 Requested=... 在变，说明它们是在同一位置附近反复短停、重选目标、再卡住。
- 当前判断：
  - 这能证明两件事：
    1. 用户截图里的 warning 是真的还活着，不是 stale；
    2. 当前第一 runtime 问题已经不是桥 / 水 / 边缘 contract 没接上，而是正式场景下 NPC 自然漫游在局部 choke point / crowd 位点里反复进 StuckCancel 循环。
- 当前边界：
  - 这轮仍是只读；
  - 还没有进入新的 targeted runtime 取证或修复；
  - 所以当前不能 claim “warning 已解决”。
- 下一步最小动作：
  - 如果继续，应直接针对高频点  03 / 201 / 101 / 202 做一次定点 runtime 取证：看它们各自在什么场景位置被卡、周围是否是 NPC 墙 / 静态碰撞 / 狭窄通道，再决定修 stuck recovery、采样选点 还是 shared avoidance。
- 时间：2026-04-05 01:34:32 +08:00

## 2026-04-05（屎山修复父层补记：Town 相机跟随已拿到用户 fresh 通过）

- 当前新增事实：
  1. 工具-V1线程前面把 `Town` 相机链压到两级主嫌疑：
     - `CinemachineCamera.Follow / TrackingTarget` 需要 runtime 重绑
     - `Main Camera` 缺 `CinemachineBrain` 时需要 runtime 自愈补挂
  2. 用户 fresh 回执已明确：`我测试了，没有任何问题`
- 当前阶段判断：
  - `Town` 相机跟随这条主线现在可以按 `用户已测通过` 收口；
  - 它不再是屎山修复父层的活跃 blocker。
- 当前恢复点：
  - 后续若再出现 Town 相机问题，应按新回归重新立案，不再把这条旧线挂作未闭环。

## 2026-04-05（屎山修复父层补记：Town 镜头边界仍比 Primary 更松，结构主因已定位到 `CameraDeadZoneSync` 的 bounds 选源实现）

- 当前新增事实：
  1. 用户最新重新点名的不是 `Town` 跟随，而是：
     - `Town` 里镜头仍会照到不该照到的区域
     - 需要把 `Town` 的摄像头边界做到和 `Primary` 一致
  2. 静态对比 `Primary.unity / Town.unity` 后，`CameraDeadZoneSync` 序列化字段在两边基本一致：
     - `autoDetectBounds=1`
     - `worldLayerNames = LAYER 1/2/3`
     - `explicitBoundsTilemaps=[]`
     - `explicitBoundsColliders=[]`
     - 所以不是“Primary / Town 挂了两套完全不同参数”
  3. 真正的结构主因在 `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`：
     - `SelectAutoBoundsTilemaps()` / `ShouldIncludeTilemapInAutoBounds()` 当前会把 world layers 下所有 tilemap 都纳入 bounds；
     - 代码没有真正利用 `preferredExactTilemapNames / preferredAutoBoundsKeywords` 去收敛到 `Base`；
     - 也没有对 tilemap 路径应用 `water / props / farmland / old` 排除；
     - 因而当前 auto bounds 本质上是“全部 tilemap 并集”，不是“Base 并集”。
  4. `Town.unity` 恰好比 `Primary.unity` 更容易把这个 bug 放大：
     - `Town` 在 world layers 下存在更多 `Water / Props / Grass / Farmland_*` tilemap；
     - 这些都可能被并进 `_worldBounds`，让 confiner 边界比真正可见 Base 更松。
  5. 进一步的放大器在同一脚本里也已查明：
     - `UpdateBoundingCollider()` 最终把 `_worldBounds` 压成 4 点矩形；
     - 不是按真实 Base 轮廓做 shape；
     - 所以 `Town` 一旦边缘更不规则，就比 `Primary` 更容易出现漏边。
  6. 另一个次级差异也已确认：
     - `Primary` 的 `Main Camera orthographic size = 10.5`
     - `Town` 的 `Main Camera orthographic size = 5`
     - 但 `Town` 的 `CinemachineCamera Lens.OrthographicSize` 仍是 `10.5`
     - `ApplyWideScreenViewportClamp()` 读的是 `mainCamera.orthographicSize`
     - 这会让 `Town` 的宽屏保护比 `Primary` 更依赖运行时时序。
- 当前阶段判断：
  - 这轮已经形成可行动的结构级结论；
  - 但还没有 live 读到 `Town` runtime 的最终 `_worldBounds` 数值，因此当前不能把它说成体验已过线。
- 当前恢复点：
  - 若后续 reopen，这条线最值得先做的是：
    1. 让 auto bounds 真正只取 `Base`
    2. 再决定是否要从矩形 confiner 升级成更贴合 Base 轮廓的 shape

## 2026-04-05（屎山修复父层补记：用户已纠正前提，Town 左侧漏边当前更像 runtime bounds 未稳定刷新，而不是“必须改成 Base 选源”）

- 当前新增事实：
  1. 用户明确纠正了分析前提：
     - 这里不是要求“只取 Base”
     - 而是允许继续按 `world tilemap` 并集作为相机边界源
  2. 在这个新前提下重新对比后，静态上最硬的异常变成：
     - `Primary` 与 `Town` 的 `_CameraBounds` 多边形点位完全相同
     - 都是 `(-41,-17) / (-41,49) / (13,49) / (13,-17)`
     - 说明 `Town` scene 资产里没有一个明显已经落成的“Town 自己的边界”
  3. 同时 `Town` 相机链仍明显比 `Primary` 更脆弱：
     - `Town Main Camera orthographic size = 5`
     - `Town CinemachineCamera Lens.OrthographicSize = 10.5`
     - `Primary` 这两者对齐为 `10.5`
     - `Town` 的 `TrackingTarget` 静态为空，而 `Primary` 静态上已绑玩家
     - `Town Main Camera` 还是 `SolidColor` 清屏，所以只要边界漏一点，蓝边会更明显
  4. `CameraDeadZoneSync.UpdateBoundingCollider()` 运行时仍然会把最终 `_worldBounds` 压成矩形；
     - 因此当前更像的实因不再是“选源必须换 Base”
     - 而是 `Town` runtime 没有稳定把自己的 world-tilemap 并集刷新进 `_worldBounds`
     - 或刷新时机被 Town 这条更脆弱的相机链干扰
- 当前阶段判断：
  - 上一条“必须收敛到 Base”的判断对用户当前要求来说不够准，现已在父层修正；
  - 现在更可信的结构结论是：
    - `Town` 左侧漏边 = runtime bounds 没稳定落成 Town 自己的结果
- 当前恢复点：
  - 如果后续继续查，最值钱的证据将是 Town runtime 下 `_worldBounds` 与 `_CameraBounds` 实际数值，而不是继续只做静态猜测

## 2026-04-05｜导航检查V2：NPC 自然漫游 stuck recovery 静态补口已完成，运行验证被外部 compile red 阻断

- 子线：
  - 导航检查V2
- 当前目标：
  - 修正式场景里 NPC 自然漫游反复 StuckCancel / 撞墙 / 堵塞 warning。
- 本轮子线结论：
  1. NPCAutoRoamController.cs 已完成一轮更强恢复链补口：零推进与共享 hard-stop 不再一味停住；静态/被动堵塞会进入 blocked-advance recover；terminal stuck 会优先 long-pause/restart，再决定是否 interrupt warning。
  2. NavGrid2D.cs 额外清掉一条导航静态 dead-code warning（CS0162），不改运行逻辑。
  3. own 改动已提交本地 checkpoint：7cd57279。
  4. 最新 Unity compile 证据显示：导航 own 文件没有新编译红面；当前 external red 在 BoxPanelUI.cs 与一组 NPC 测试脚本，不在本轮 own 路径。
- 当前状态：
  - 该子线已 PARKED；
  - 停车原因是 external compile blocker，不是导航 own 红错未清。
- 恢复点：
  - 外部 red 清掉后，直接回到 NPC 自然漫游 live 复测，不需要回头重做这轮静态补口。

## 2026-04-05（屎山修复父层补记：树石批量工具按钮选中色已改成显式着色）

- 当前新增事实：
  1. 用户确认按钮功能已恢复，但指出选中颜色没有显示，属于视觉层问题；
  2. 当前已把树/石批量工具的选中按钮改成显式 `GUI.backgroundColor` + `GUI.contentColor` 着色，不再依赖 Unity 皮肤 active 背景。
- 当前阶段判断：
  - 树石批量工具当前已从“功能对但选中态看不见”推进到“功能 + 选中视觉都补上”的状态；
  - 终验仍以用户现场观感为准。
- 当前恢复点：
  - 用户现在直接回 Unity 继续看按钮选中色；
  - 若还不满意，只继续调颜色/对比度，不扩到别的逻辑。

## 2026-04-05｜导航检查V2：NPC 当前未提交 diff 的只读审计已压实为“一条真风险 + 一条 diff hygiene”

- 子线：
  - 导航检查V2
- 当前目标：
  - 只读判断当前 NPC 导航未提交 diff 里，是否还留有会导致撞墙卡住 / 原地乱翻向 / 误把指令速度当真实速度的静态漏洞。
- 本轮子线结论：
  1. 高置信真风险只剩 1 条：
     - `NPCAutoRoamController.cs` 新加的 move-command oscillation 检测没有在“已经发生真实前进”时清零计数，可能把仍在缓慢前进的 `A/B/A` 校正误打成 blocked recover。
  2. `NPCMotionController.cs` 当前没有新的高置信 active 运行漏洞；
     - 但 `ReportedVelocity` 与 `CurrentVelocity/IsMoving` 语义已分裂，属于 latent API footgun。
  3. 当前 NPC 导航未提交 diff 里混入了非导航内容：
     - ambient chat / story bubble suppression。
- 当前状态：
  - 只读结论已形成；
  - 尚未进入新施工切片。
- 恢复点：
  - 若后续继续，先修 oscillation reset，再拆非导航 diff，之后才值得重新进 runtime 验证。

## 2026-04-05｜导航检查V2：NPC 疯转修复已形成双 checkpoint，当前停在 Town live 外部噪声 blocker

- 子线：
  - `导航检查V2`
- 当前目标：
  - 修 `NPC` 正式场景里的撞墙 / 原地乱翻向；
  - 不改玩家已认可版本，只收 NPC 底座 bug。
- 本轮子线新增事实：
  1. 已先为目标代码落本地回退点：
     - `c29a80a2` `npc-spin-debug-checkpoint`
  2. 后续真正的导航修复提交为：
     - `592705f8` `npc-spin-hardstop-fix`
  3. 这轮补的是 3 个纯 bug：
     - `NPCMotionController` 不再把长期没有真实位移的刚体意图速度当成移动事实；
     - `NPCAutoRoamController.NoteSuccessfulAdvance(...)` 会清掉 oscillation 计数；
     - `ShouldResetSharedAvoidanceStuckProgress(...)` 不再把 `HardBlocked` 误当 progress。
  4. 脚本级静态验证通过，`Editor.log` 也已有 fresh `Tundra build success`。
  5. 但 `Town` live 仍被外部噪声截断：
     - missing behaviour
     - `OcclusionManager` timeout
     - 因而还没拿到能站住的“正式场景 NPC 疯转已消失”新样本。
  6. 另外同文件仍有 foreign/非本刀改动残留在 `NPCAutoRoamController.cs`：
     - `ambient bubble helper / StoryPhase`
- 当前状态：
  - `PARKED`
- 恢复点：
  - 外部现场清稳后，直接回 `Town` live 做 targeted capture；
  - 不需要再回头重做这轮静态补口。

## 2026-04-05｜导航检查V2：Primary live 快测已拿到 NPC bridge pass，当前主风险转成长时 roam soak

- 子线：
  - `导航检查V2`
- 当前目标：
  - 在不改玩家版本的前提下，确认 NPC 是否已经接上桥 / 水 / 边缘 traversal contract。
- 本轮新增事实：
  1. `Primary` live quick probe 已跑：
     - `Tools/Codex/NPC/Run Natural Roam Bridge Probe`
  2. fresh console 结论：
     - `[CodexNpcTraversalAcceptance] PASS natural-roam-bridge`
     - `npc=002`
     - `sawBridgeSupport=True`
     - `inWater=False`
     - `state=Moving`
  3. 现场抽样的 3 个主 NPC：
     - `001`：`IsRoaming=True`
     - `002`：bridge probe 到点后稳定停住
     - `003`：`IsRoaming=True` 且 `IsMoving=True`
  4. 本轮没有新的 NPC 导航 error / warning，也没再看到 `bridge_probe_fail timeout`。
  5. 但 `SpringDay1NpcRuntimeProbe` 仍暴露聊天链 pair timeout；这属于 NPC 其它 runtime 问题，不是 traversal contract 本身。
- 当前状态：
  - `Primary` 的 NPC traversal contract 已有 fresh pass 证据；
  - 现阶段最该补的是更长时 roam soak / 墙边卡住复现，而不是再回头怀疑桥接线没吃进去。

## 2026-04-05｜导航检查V2：NPC 建路不动已修通，bridge/water/edge 当前 fresh 过线

- 子线：
  - `导航检查V2`
- 本轮新增事实：
  1. `NPCAutoRoamController` 已补两刀 bugfix：
     - 动态刚体首跳不再被 `MoveCommandNoProgress` 提前误杀；
     - 边界约束失败时，会尝试近邻 walkable fallback，而不是直接钉回原地。
  2. `CodexNpcTraversalAcceptanceProbeMenu` 补了更细的 runtime 诊断口径，便于后续真失败时定位。
  3. 先经过 `STOP -> Assets/Refresh -> PLAY`，`Editor.log` 出现 `Tundra build success` 后再跑 probe。
  4. fresh live：
     - `PASS natural-roam-bridge`
     - `PASS bridge+water+edge`
  5. 收尾时 Unity 已回 `Edit Mode`，fresh console `errors=0 warnings=0`。
- 当前判断：
  - NPC 当前已经吃到玩家已认可的 bridge / water / edge traversal contract；
  - 本轮主 bug `pathCount>0 但实体不动` 已从 active blocker 变成已修复；
  - 后续若继续 NPC 线，重点应转向长时 roam / 墙边卡住 / 聊天链，而不是再回头修这条 bridge 起跑链。

## 2026-04-06｜导航统一执行内核方向已立：后续只抽共享 traversal core

- 子线：
  - `导航检查V2`
- 本轮新增事实：
  1. 已新增一份方向锚定文档，明确后续“统一导航接口”的唯一正确边界：
     - 统一的是 `traversal 执行内核`
     - 不统一整个玩家 / NPC controller
  2. 已压实当前三层结构：
     - A：目标来源层
     - B：统一 traversal 执行层
     - C：运动落地层
  3. 已压实后续顺序：
     - 先抽窄 core
     - 先让 NPC 接
     - 玩家最后再切
- 当前判断：
  - 这条路现在不是“是否要统一”的问题，而是“按什么边界统一”的问题；
  - 正确边界已经明确，后续真正开工时不应该再回到“大重构 / 大合并”的错误路线。

## 2026-04-06｜导航统一执行内核的安全归档已补齐

- 子线：
  - `导航检查V2`
- 本轮新增事实：
  1. 新增方向锚定提交：
     - `1f10a107` `anchor navigation unified traversal direction`
  2. 新增安全 tag：
     - `nav-unification-anchor-20260406-01`
  3. 新增可脱离工作区恢复的 bundle：
     - `D:\Unity\Unity_learning\Sunset\.codex\archives\navigation\nav-unification-anchor-20260406-01.bundle`
- 当前判断：
  - 现在已经具备“方向已锚定 + Git 已提交 + 本地可离线恢复”的三层保险；
  - 后续如果开始统一共享 traversal core，可以在这个锚点上放心开刀。

## 2026-04-06｜导航统一执行内核第一刀已验证：共享 traversal core 接通玩家与 NPC

- 子线：
  - `导航检查V2`
- 本轮新增事实：
  1. 已新增共享执行内核：
     - `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs`
  2. 已完成第一批接线：
     - `PlayerMovement.cs`
     - `NPCAutoRoamController.cs`
  3. 为了压掉 NPC 假速度 / 假起跑问题，又补了三处 bugfix：
     - `NPCMotionController.cs`：新增 `ResetMotionObservation()`，并让 `StopMotion()` 同步重置 `_lastPosition`
     - `CodexNpcTraversalAcceptanceProbeMenu.cs`：起点 occupiable 解析 + 瞬移后重置 motion + queued play dispatch
     - `NavigationLiveValidationRunner.cs`：`PlaceActor(...)` 后重置 NPC motion 观测
  4. fresh live 结果：
     - NPC：`PASS natural-roam-bridge`
     - NPC：`PASS bridge+water+edge`
     - 玩家：`GroundPointMatrix pass=True`
  5. final fresh console：
     - `errors=0 warnings=0`
     - Unity 已回 `Edit Mode`
- 当前判断：
  - “统一 traversal core 第一刀”已经拿到最小可用 runtime 证据，当前不是纸面设计。
  - 仍未覆盖长时 roam / 墙边卡住 / 全量 acceptance，但桥 / 水 / 边界合同已用共享内核跑通。
