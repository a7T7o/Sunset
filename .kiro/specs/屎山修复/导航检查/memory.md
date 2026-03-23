# 导航检查 - 开发记忆

## 模块概述

本工作区承接 Sunset 导航系统从“局部补丁式修修补补”升级到“共享导航核心重构”的主线。  
目标不是再做一轮零散审计，而是建立一份可长期维护、可持续执行的统一重构主表，并据此推进后续代码与验证。

## 当前状态

- **完成度**: 10%
- **最后更新**: 2026-03-22
- **状态**: 进行中

## 会话记录

### 会话 1 - 2026-03-22

**用户需求**：
> 这里是你的工作区D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查，你要做的不是写什么分析需求设计任务五件套文档，你只需要一个，详细的阶段设计内容，每个阶段要做哪些事情，有哪些任务，你要做的是一个包含需求描述，然后主要是阶段性的设计与任务内容，详细到你可以一直依据这个来完成后续的工作不会迷失，不会忘记，当然这个是可维护的，比如你完成了就标记，然后新增和修订都可以，由你自己把控，然后当然需要遵守kiro工作区的memory文档规范，请你开始吧，文档先行

**当前主线目标**：
- 为导航系统建立“统一导航重构阶段设计与执行主表”，作为后续长期工作的唯一主文档入口。

**完成任务**：
1. 读取工作区现状，确认 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查` 当前为空目录。
2. 回读旧导航审计链：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`
3. 基于现状重新定性问题：
   - 当前不是一个统一导航系统
   - 现状是静态路径层 + 玩家补丁导航 + NPC 漫游补丁的分裂架构
   - 后续如果要支持玩家/NPC/怪物/牲畜/宠物共存，必须重构为共享导航核心
4. 创建本工作区主文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
5. 创建本工作区 `memory.md`
6. 创建父工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`

**解决方案**：
- 不再使用 requirements/design/tasks 拆分文档。
- 统一把需求、架构定性、阶段设计、执行清单、进度标记、修订记录放进一个主文档中。
- 阶段设计分为：
  - S0 基线再确认
  - S1 统一导航代理契约
  - S2 动态代理注册表
  - S3 共享局部避让求解器
  - S4 共享路径执行层
  - S5 玩家接入
  - S6 NPC 接入
  - S7 其他移动体接入
  - S8 全链路验证与旧逻辑下线

**涉及的代码文件**

### 当前分析确认的核心代码文件
| 文件 | 关系 | 说明 |
|------|------|------|
| `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs` | 核心读取 | 静态/准静态路径规划底座 |
| `Assets/YYY_Scripts/Service/Navigation/INavigationUnit.cs` | 核心读取 | 未来统一代理契约的现有起点 |
| `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` | 核心读取 | 当前玩家导航闭环 |
| `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` | 核心读取 | 当前 NPC 漫游闭环 |
| `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs` | 核心读取 | 玩家运动适配层 |
| `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` | 核心读取 | NPC 运动适配层 |
| `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` | 相关读取 | 玩家输入脑与导航意图发起层 |

**遗留问题**：
- [ ] 下一轮开始时，先按主文档从 S0 / S1 进入，而不是再回到零散分析。
- [ ] 后续代码重构时，必须继续遵守“一个主文档 + memory 追加”模式，不再铺 requirements/design/tasks 套件。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| `屎山修复/导航检查` 工作区只维护一个主文档，而不是五件套 | 用户明确要求减少文档散落，保留唯一执行入口 | 2026-03-22 |
| 将导航问题正式定性为“共享导航核心重构” | 现状已不是单一 bug，而是系统分裂 | 2026-03-22 |
| 后续默认按照阶段主表推进 | 避免继续迷失在局部补丁和旧口径里 | 2026-03-22 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md` | 本工作区唯一主文档 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\memory.md` | 旧导航审计工作区记忆 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\memory.md` | 旧父工作区记忆 |
| `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md` | 导航线程记忆 |

### 会话 2 - 2026-03-23（锐评 001 审核）

**用户需求**：
> `.kiro\specs\屎山修复\导航检查\锐评\001.md，走审核锐评`

**当前主线目标**：
- 审核 `统一导航重构阶段设计与执行主表.md` 对应的锐评文件，判断其结论应走 Path A / B / C 哪一条。

**完成任务**：
1. 读取锐评文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\锐评\001.md`
2. 读取评审规则：
   - `D:\Unity\Unity_learning\Sunset\.kiro\steering\code-reaper-review.md`
3. 回读主文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
4. 回读相关 live 代码证据：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCMotionController.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerMovement.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\INavigationUnit.cs`

**路径判断**：
- 结论：**Path B**

**审核结论**：
1. 锐评指出“最终运动语义不一致”这个问题是对的。
2. 但它把修正方向进一步绝对化为“必须把 `MovePosition` 全部改成 `linearVelocity` / 动力学移动”，这一步没有被当前项目代码和架构证据充分证明，不能原样采纳。
3. 锐评指出 S3 避让求解器缺少算法边界定义，这一点成立，应吸收。
4. 锐评指出当前阶段顺序偏瀑布流，这一点成立，应吸收。
5. 锐评指出动态代理需要 sleeping / moving 状态，这一点成立，应吸收。

**关键结论**：
- 这份锐评整体方向正确，不需要走 Path C。
- 但不能照单全收其“统一改成 `linearVelocity`”的具体技术方案。
- 后续应把它作为对主表的局部修订输入，而不是直接当成实现指令。

### 会话 3 - 2026-03-23（Path B 第一批共享导航核心落地）

**用户需求**：
> 请你按照路径b完成所有任务，直接一步到位完成所有的任务，直接开始全面落地

**当前主线目标**：
- 不再停留在锐评审核或纯文档层，而是直接把 Path B 成立的建议落成第一批共享导航核心代码。

**完成任务**：
1. 修订主文档：
   - 吸收锐评中成立的三类问题：
     - S1 / S2 增加 moving / sleeping 状态
     - S3 增加第一版算法边界与防抖要求
     - 执行顺序改成交织推进，不再读成瀑布流
     - S6 增加“最终运动语义必须显式收口”的任务，而不是直接武断定为 velocity-only
2. 扩展导航代理契约：
   - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\INavigationUnit.cs`
   - 新增：
     - `GetCurrentVelocity()`
     - `GetAvoidancePriority()`
     - `IsCurrentlyMoving()`
     - `IsNavigationSleeping()`
     - `ParticipatesInLocalAvoidance()`
3. 新增共享导航核心第一批基础设施：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentSnapshot.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAvoidanceRules.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAgentRegistry.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
4. 玩家侧接入共享核心：
   - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
   - 完成：
     - 注册/反注册到共享代理注册表
     - 生成统一代理快照
     - 通过共享 solver 进行动态代理避让
     - 动态阻挡持久化后触发路径重建
5. NPC 侧接入共享核心：
   - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
   - 完成：
     - 实现统一代理契约
     - 注册/反注册到共享代理注册表
     - 在 `TickMoving()` 中先走共享 solver，再执行原有运动
     - 持续阻挡后触发重建路径

**关键结论**：
- 这轮已经不再是“审视一下锐评然后继续想”，而是把第一批共享导航核心真的落进了代码里。
- 玩家和 NPC 现在开始共享：
  - 统一代理契约
  - 统一代理注册表
  - 统一局部避让求解器入口
- 但当前仍未完成的关键收口是：
  - 统一路径执行层还没抽出
  - NPC 最终运动语义是否继续保留 `MovePosition`，还是迁移到另一种统一语义，仍然是显式待决任务

**涉及的代码文件**

### 本轮新增文件
| 文件 | 操作 | 说明 |
|------|------|------|
| `Assets/YYY_Scripts/Service/Navigation/NavigationAgentSnapshot.cs` | 新增 | 共享代理快照 |
| `Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs` | 新增 | 共享让行与阻挡规则 |
| `Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs` | 新增 | 共享动态代理注册表 |
| `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs` | 新增 | 第一版局部避让求解器 |

### 本轮修改文件
| 文件 | 操作 | 说明 |
|------|------|------|
| `Assets/YYY_Scripts/Service/Navigation/INavigationUnit.cs` | 修改 | 扩展统一代理契约 |
| `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` | 修改 | 玩家接入共享代理与共享局部避让 |
| `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` | 修改 | NPC 接入共享代理与共享局部避让 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md` | 修改 | 吸收 Path B 结论并同步进度 |

**遗留问题**：
- [ ] 做最小静态验证，排掉当前新增共享核心代码的明显编译问题
- [ ] 做 Unity / MCP / Play 现场验证，确认玩家绕移动 NPC、NPC 绕玩家是否成立
- [ ] 进入下一步时优先继续统一路径执行层，而不是回到局部补丁

### 会话 4 - 2026-03-23（P0 失败后的共享规则修正）

**用户反馈**：
> 你的p0就已经彻底不合格了，现在我右键导航，路线上有npc，只会推着npc移动，这是不合格的

**当前主线目标**：
- 不再把“共享核心已接线”误当成行为通过，而是直接针对运行态失败现象修正共享规则。

**本轮排查结论**：
1. 玩家和 NPC 的脚本搭载是真实存在的，不是“代码写了但角色没挂上”。
2. 当前一处关键根因是共享规则默认把玩家视为最高优先级，这会让自动导航玩家面对移动 NPC 时更容易继续顶着走，而不是主动让行。
3. 当前 solver 在近距离动态阻挡时，前冲削弱还不够，容易导致玩家继续把 NPC 推着走。

**本轮完成**：
1. 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAvoidanceRules.cs`
   - 显式规定：自动导航玩家面对 moving NPC / Enemy 时默认让行
2. 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
   - 增强近距离动态阻挡时的减前冲与侧绕权重
   - 让第一版 solver 不只是“轻微偏转”，而是会主动退让
3. 新增：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`
   - 用来锁定“玩家遇到移动 NPC 必须让行”的规则

**关键结论**：
- 当前已经确认一件事：P0 失败不只是“没做 Unity 验证”，而是规则层确实有一处方向性错误。
- 这轮修正之后，才值得再次进入运行态验收。

**新的恢复点**：
- 下一轮优先做运行态复验，不再先补更多架构代码。
- 如果这轮规则修正后仍然推着走，就说明问题主因已经进入“共享路径执行层缺失”或“最终运动语义冲突”。

### 会话 5 - 2026-03-23（玩家推 NPC 的直接纠偏）

**用户反馈**：
> 你的p0就已经彻底不合格了，现在我右键导航，路线上有npc，只会推着npc移动，这是不合格的

**当前主线目标**：
- 针对“玩家自动导航仍然推着 NPC 走”的失败现象，直接修正共享规则，而不是继续补外围逻辑。

**完成任务**：
1. 重新核对角色脚本搭载事实：
   - 玩家在 `Primary.unity` 中确有 `PlayerMovement + PlayerAutoNavigator`
   - `001/002/003.prefab` 确有 `NPCAutoRoamController + NPCMotionController + Rigidbody2D + BoxCollider2D`
2. 重新核对共享代码实现，定位到一处关键根因：
   - 自动导航玩家当前在共享规则里并没有真正对 moving NPC 主动让行
3. 直接修正：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationAvoidanceRules.cs`
     - 增加“玩家自动导航遇到 moving NPC / Enemy 默认让行”的规则
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
     - 近距离动态阻挡时增加减前冲与更强侧绕
4. 新增最小回归测试：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`

**关键结论**：
- 这轮已经确认并修正了一处真实规则错误，不再只是“等待 Unity 验证”。
- 但修正后是否真正解决“推着走”，仍必须回到运行态复验。

**遗留问题**：
- [ ] 复验修正后是否仍然推着 NPC 走
- [ ] 若仍失败，则下一刀直接进入共享路径执行层或最终运动语义收口

### 会话 6 - 2026-03-23（测试装配编译止血）

**用户反馈**：
> `NavigationAvoidanceRulesTests.cs` 出现找不到 `NavigationAgentSnapshot / NavigationUnitType / NavigationAvoidanceRules / NavigationLocalAvoidanceSolver` 的编译错误

**当前主线目标**：
- 先把因为我新增测试导致的编译错误止血，恢复测试装配可编译。

**完成任务**：
1. 确认错误根因不是导航运行时代码本身，而是 `Assets/YYY_Tests/Editor/Tests.Editor.asmdef` 无法直接编译期引用主程序集中的全局类型。
2. 将 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs` 改写为反射式测试，不再直接在编译期引用：
   - `NavigationAgentSnapshot`
   - `NavigationUnitType`
   - `NavigationAvoidanceRules`
   - `NavigationLocalAvoidanceSolver`
3. 用最小代码闸门只对该测试文件做预检，确认 `Tests.Editor` 装配编译通过。

**关键结论**：
- 这组编译错误是我新增测试写法不对引起的，我已修正。
- 当前这组“类型/命名空间找不到”的测试装配错误已被止血。

### 会话 7 - 2026-03-23（基于 NPC 正式回执继续修复 P0）

**用户补充信息**：
> 你先来看看npc的回执文档吧……总的来说就是不打断你的修复进程

**当前主线目标**：
- 在不打断修复进程的前提下，吸收 NPC 线当前正式回执，并继续围绕“玩家仍然推着 NPC 走”做导航侧纠偏。

**本轮已确认的可直接利用信息**：
1. NPC 线当前承诺稳定：
   - `Moving / ShortPause / LongPause` 语义
   - `IsMoving` 判定语义
   - `rb.MovePosition(...)` 作为当前 NPC 最终位移语义
   - `001 / 002 / 003` 的刚体与碰撞配置
2. 这意味着当前联调阶段不应再优先怀疑“NPC 线在偷偷改地基”，而应该先继续从导航侧排查玩家执行层与共享规则。

**本轮进一步修复**：
1. 发现第二个关键根因：
   - `PlayerMovement.UpdateMovement()` 会把输入再次 `normalized`
   - 导致共享 solver 就算给出了减前冲 / 减速倾向，也会在最终运动层被吃掉
2. 修正：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerMovement.cs`
   - 将最终速度计算改为 `Vector2.ClampMagnitude(movementInput, 1f) * currentSpeed`
3. 同时修正：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
     - 增加 `SpeedScale`
     - 增加阻挡代理位置 / 半径 / 建议侧绕方向
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
     - 使用 `SpeedScale`
     - 增加临时动态绕行点
     - 避免“共享规则说要绕，但静态 path 仍然笔直穿过 NPC”时继续硬顶

**关键结论**：
- 现在已经明确：当前 P0 不合格并不是单一原因，而至少有两层问题同时存在：
  1. 让行规则一开始就偏了
  2. 玩家最终运动层把减速信息吃掉了
- 在 NPC 线当前地基稳定的前提下，这两刀都是我导航线必须优先修掉的内容。

### 会话 8 - 2026-03-23（Primary.unity 导航脏改卫生收口）

- 用户明确要求先停下非必要推进，优先处理 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 里由导航线留下的热文件脏改。
- 本轮 live 核查确认：
  - `Primary.unity` 中属于导航线的新增字段只有 `PlayerAutoNavigator` 上的 6 个序列化参数：
    - `dynamicObstaclePadding`
    - `dynamicObstacleRepathCooldown`
    - `dynamicObstacleVelocityThreshold`
    - `obstacleProbeBufferSize`
    - `sharedAvoidanceLookAhead`
    - `avoidancePriority`
  - 这些值与 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs` 中当前字段默认值一致，不属于必须保留在 scene 上的独立配置。
  - 同一份 `Primary.unity` diff 中还混有 `StoryManager`、对话测试状态等非导航内容，因此不应把整份 scene 一起当成导航 checkpoint 收口。
- 本轮动作：
  - 已从 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 移除上述 6 个导航序列化字段，释放导航线对热场景的字段级占用。
  - 保留并准备独立收口的代码 checkpoint：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- 当前恢复点：
  - `Primary.unity` 已不再包含导航线新增的那 6 个字段级脏改；
  - 后续继续修“玩家绕开 NPC”时，应优先围绕代码与验证推进，不再让 `Primary.unity` 承担导航侧中间态。

### 会话 9 - 2026-03-23（动态避让闭环失效根因补刀）

- 用户再次明确：主线不是“已经接入共享导航”，而是必须解决“玩家右键导航遇到 NPC 仍然推着走”，并要求补足控制台日志、彻底看清 NPC 构造与执行链后再继续落地。
- 本轮 live 复核事实：
  - 当前 live Git 现场起点：
    - `D:\Unity\Unity_learning\Sunset`
    - `main`
    - `4f76b1b87efb455dc0cc370988ca8b69afc601a3`
  - `Primary.unity` 中玩家 `PlayerAutoNavigator` 当前 `enableDetailedDebug = 1`，因此玩家侧可直接复用现有调试开关输出新日志。
  - `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 当前 live 基线仍是：
    - `isTrigger = false`
    - `mass = 6`
    - `linearDamping = 8`
    - `collisionDetection = 1`
  - `003.prefab` 上 `NPCMotionController` / `NPCAutoRoamController` 的 `showDebugLog` 当前仍为 `0`，所以本轮日志能力必须写进代码但不能依赖样本 prefab 已默认开启。
  - 当前会话没有可用的 Unity MCP resources / templates，说明本轮无法直接通过 MCP 做 live Play 取证，只能先完成代码层闭环修复与静态/编译闸门。
- 本轮确认的关键根因，不再只是旧结论重复：
  1. 玩家侧虽然已经有 `SpeedScale + detour`，但在“触发动态 repath 的那一帧”直接 `return`，没有先把旧速度清零，上一帧前冲会继续生效。
  2. NPC 侧虽然接了共享 solver，但 `NPCAutoRoamController.TickMoving()` 之前根本没有消费 `NavigationLocalAvoidanceSolver` 返回的 `SpeedScale`，导致 NPC/NPC 会车时没有真正减速层。
  3. 玩家和 NPC 两侧此前都缺少“近距离接触时禁止继续把前向速度压进阻挡代理”的最后一道执行仲裁，所以会出现逻辑上开始避让、物理上仍然把对方顶着走。
  4. 固定 `sharedAvoidanceLookAhead = 0.8` 对运行态偏短，当前需要按速度自适应扩大探测前瞻，而不是只靠固定半径。
- 本轮已落地代码：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
    - 新增 `GetRecommendedLookAhead(...)`
    - 新增 `CloseRangeConstraintResult`
    - 新增 `ApplyCloseRangeConstraint(...)`
    - 作用：在接近阻挡代理时主动剥离“继续向阻挡体前冲”的分量，并把速度进一步压低到可停住/可侧绕的级别
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
    - 改为按建议速度自适应 `lookAhead`
    - 动态 repath / detour 触发当帧先 `ForceImmediateMovementStop()`，不再沿用旧速度
    - 在最终 `SetMovementInput(...)` 前应用 close-range constraint
    - 新增共享避让运行日志，复用 `enableDetailedDebug`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
    - 现在正式消费共享 solver 的 `SpeedScale`
    - 动态 repath / hard stop 当帧先 `StopMotion()` + 清零刚体速度
    - 现在也应用 close-range constraint，而不是只改方向不减速
    - 新增共享避让运行日志，复用 `showDebugLog`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`
    - 新增 close-range constraint 反射测试
    - 补强类型解析，兼容嵌套类型反射
- 当前恢复点：
  - 先对白名单路径跑代码闸门与提交收口；
  - 收口完成后，下一轮第一优先级是做真实运行态验收：
    - 玩家绕移动 NPC
    - NPC/NPC 会车
    - NPC 绕玩家

### 会话 10 - 2026-03-23（NPC 代理中心点与负 clearance 脱离态修正）

- 用户再次明确要求“彻查、彻底解决”，并追问为什么不用 MCP 去直接核实 NPC 组件与现场事实。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-unity-validation-loop`
  - `unity-mcp-orchestrator`
  - `sunset-startup-guard` 仍因当前会话未显式暴露，改走手工等价 preflight
- 本轮 live / MCP 事实先行确认：
  - 当前会话 `list_mcp_resources` / `list_mcp_resource_templates` 仍然返回空。
  - 但 `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1` 已实测通过：
    - `C:\Users\aTo\.codex\config.toml` 只保留 `unityMCP`
    - `http://127.0.0.1:8888/mcp` 正在监听
    - `Library\\MCPForUnity\\RunState\\mcp_http_8888.pid` 存在且与监听进程一致
  - 因而这次“用不上 MCP”不是项目服务没起，而是**当前会话资源暴露层失效**；不能继续把责任甩给用户截图。
  - `Assets/222_Prefabs/NPC/001.prefab` / `003.prefab` 的 `BoxCollider2D` 当前都明确是：
    - `m_Offset: {x: 0, y: 0.46}`
    - `m_Size: {x: 0.88, y: 0.68}`
- 本轮锁定的新根因：
  1. 玩家共享避让位置一直用 `playerCollider.bounds.center`，但 NPC 共享代理位置此前却用 `rb.position / transform.position`，没有对齐到碰撞体中心；在 NPC collider 存在明显 Y 偏移时，阻挡体中心、法线方向、clearance 与 detour 落点都会被系统性算歪。
  2. `NavigationLocalAvoidanceSolver.ApplyCloseRangeConstraint(...)` 之前只在“仍然明显朝着 blocker 前冲”时才施加约束；一旦 `clearance < 0` 但 `forwardIntoBlocker` 已经降到很小，就会错误返回“未施加约束”，于是日志里才会出现“明明已经负 clearance，还在 `DetourMove` 慢慢蹭”的现象。
  3. `PlayerAutoNavigator.TryCreateDynamicDetour(...)` 之前只用单个前向候选点，没有要求 detour 点真正落到 blocker 接触壳层之外，所以 detour 虽然触发了，但仍可能把玩家留在接触区里磨蹭。
- 本轮已落地代码：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
    - `GetPosition()` 改为统一返回 `navigationCollider.bounds.center`
    - `TickMoving()` 中显式拆分：
      - 路径/位移基点：`rb.position`
      - 共享避让基点：`GetNavigationCenter()`
    - 避免再用 NPC 脚底点冒充 blocker 中心
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationLocalAvoidanceSolver.cs`
    - 对 `clearance <= 0` 的情况新增真正的“脱离接触区”逻辑
    - 即使 `forwardIntoBlocker` 已经很低，只要还在接触壳层内，也会继续给出 separation + tangential 的逃离方向，而不是直接放行成“未约束”
    - 保留“正面硬顶 blocker”时的 `HardBlocked` 语义
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
    - detour 候选改为多候选
    - 以 separation direction + sidestep 为主
    - 新增 detour 点必须落在 blocker 接触壳层之外的最小净空校验
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`
    - 新增“`clearance` 已为负、但朝向不再正冲 blocker 时，仍必须触发接触脱离约束”的回归测试
- 本轮验证结果：
  - 已运行：
    - `powershell -ExecutionPolicy Bypass -File D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread 导航检查 -IncludePaths ...`
  - 代码闸门通过：
    - `Assembly-CSharp`
    - `Tests.Editor`
  - `git diff --check` 通过
  - 但这仍然只是代码/编译闭环，**不等于行为已验收通过**
- 当前恢复点：
  - 下一轮真实 Editor / Play 验收时，优先观察两件事：
    1. `[NavAvoid]` 在 `clearance < 0` 时是否不再出现 `closeConstraint=False`
    2. `blockerPos` 是否比之前更接近 NPC 实际碰撞体中心，而不再像脚底点
  - 如果运行态仍然出现“推土机顶人”，下一刀优先怀疑玩家的最终物理语义：
    - `Rigidbody2D Dynamic`
    - `m_Mass = 1`
    - `m_LinearDamping = 0`
    - `Update` 中持续写 `linearVelocity`
    而不是再回到 tag / obstacleTags / prefab 挂载是否存在这种已经排除过的问题。

### 会话 11 - 2026-03-23（治理侧补发导航线高压 prompt）

- 用户当前已经明确不再接受“继续一点一点补导航 patch”的推进方式，而是要求导航线先正视一个系统级事实：十多轮迭代后，用户现场体感依旧没有根本变化，玩家面对动态 NPC 仍像推土机。
- 基于这条要求，本轮由治理侧在本工作区新增并写实：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-1.md`
- 该 prompt 的职责不是普通提醒，而是强制导航线先完成一次“当前落地偏移审计”，核心要求包括：
  - 重读主表、`memory.md`、`005-gemini-1.md`、`003-NPC-2.md`
  - 正面回答“当前问题是否已经属于系统级失败”
  - 把“文档承诺 / 代码落地 / Scene 挂载 / 用户现场体感”四层裂口写进主表
  - 先用 `unityMCP` 做 live Scene / 组件 / 挂载核查，再决定怎么改
  - 强制审计 `NavGrid2D` 当前挂在 `Systems` 上是否仍合理，必要时直接整理导航专属承载对象
  - 不再允许只补局部规则，而是正面判断是否必须直接推进共享路径执行层、最终运动语义收口与导航根对象整理
- 当前恢复点更新为：
  - 导航线下一步不该再直接沿旧补丁链继续，而应先按 `002-prompt-1.md` 的要求补齐“当前落地偏移审计”，然后再进入下一轮真正的结构性落地与终验。

### 会话 12 - 2026-03-23（002 prompt 当前落地偏移审计）

- 用户这轮明确要求：
  - 先完整读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-1.md`
  - 不要继续局部 patch
  - 先用 MCP 核查 live Scene / 组件 / 挂载
  - 先完成“当前落地偏移审计”，再决定后续结构性落地
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-scene-audit`
  - `unity-mcp-orchestrator`
  - `sunset-startup-guard` 继续手工等价
- 本轮已完整重读：
  - `统一导航重构阶段设计与执行主表.md`
  - `memory.md`
  - `005-gemini-1.md`
  - `003-NPC-2.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\steering\scene-modification-rule.md`
- 本轮 unityMCP live 审计结论：
  - active scene 确实是 `Primary`，当前 `isDirty = true`
  - `Primary/2_World` 当前只有一个子物体：`Systems`
  - `Primary/2_World/Systems` 当前同时挂有：
    - `NavGrid2D`
    - `WorldSpawnService`
    - `WorldSpawnDebug`
    - `GameInputManager`
  - `Player` 当前 live 同时挂有：
    - `PlayerMovement`
    - `PlayerAutoNavigator`
    - `Rigidbody2D`
    - `BoxCollider2D`
  - `NPCs/001`、`002`、`003` 当前 live 都挂有：
    - `NPCMotionController`
    - `NPCAutoRoamController`
    - `Rigidbody2D`
    - `BoxCollider2D`
  - 玩家与 3 个 NPC 的 `navGrid` 引用当前都指向同一个 `Systems/NavGrid2D`
- 本轮新增稳定判断：
  1. 当前问题必须正式定性为“系统级失败”，不能再当局部 bug 处理。
  2. 当前只落到了共享导航基础设施进入运行时，**没有**落到“玩家、NPC、未来动态移动体已经在同一个导航系统里运动”。
  3. 当前最关键的裂口是：
     - S1-S3 已有代码
     - S4 共享路径执行层没有落
     - 玩家 / NPC 仍各自保留私有路径执行主循环
     - Scene 承载仍把 `NavGrid2D` 混在 `Systems`
  4. 当前 `NavGrid2D` 继续留在 `Systems` 不合理；下一轮结构性落地应优先评估建立 `NavigationRoot`
- 本轮已完成：
  - 在 `统一导航重构阶段设计与执行主表.md` 中新增高优先级“当前落地偏移审计”段
  - 明确写出：
    - 系统级失败
    - 当前真实架构
    - 目标架构
    - 现场失败与文档承诺的裂口
    - 为什么十多轮后现场仍无本质变化
    - 下一轮必须替换或重排的执行层
- 本轮没有做的事：
  - 没有继续补导航 patch
  - 没有调整 `Primary.unity` 中导航相关挂载
  - 没有声称玩家绕移动 NPC / NPC 绕玩家 / NPC-NPC 会车已经通过 live 验收
- 当前恢复点：
  - 下一轮结构性落地前，先按 `scene-modification-rule.md` 补出 NavigationRoot 迁移方案的五段分析
  - 结构性实现优先级应改成：
    1. `NavigationRoot` / 导航承载整理
    2. S4 共享路径执行层
    3. 玩家 / NPC 最终运动语义统一收口
    4. live 场景终验
