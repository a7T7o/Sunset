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

### 会话 13 - 2026-03-23（002-prompt-2 结构落地执行令）

- 用户在收到“当前落地偏移审计”回执后，要求治理侧继续补第二轮 prompt，不再让本线停在审计阶段。
- 本轮已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-2.md`
- 该执行令明确要求本线从“审计完成”正式切换为“结构落地阶段”，核心硬顺序为：
  1. 在主表补齐 `NavigationRoot` 五段分析
  2. 除非有强证据证明现有承载合理，否则直接利用 unityMCP 落地 `NavigationRoot`
  3. 真正推进 S4 共享路径执行层
  4. 用 live 行为终验回答玩家绕移动 NPC、NPC 绕玩家、NPC-NPC 会车是否通过
- 本轮关键治理判断：
  - 上一轮审计 checkpoint 已经通过，但只通过到“前置审计完成”，不允许再停在分析层
  - 这条线当前真正的下一阶段不是再补主表，而是开始结构重排与终验
- 当前恢复点：
  - 下一步应按 `002-prompt-2.md` 直接进入 `NavigationRoot` 承载整理、S4 落地和 live 终验，而不是再补新的偏移说明。

### 会话 14 - 2026-03-23（002-prompt-2 结构落地 + live 终验）

- 当前主线目标：
  - 严格按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-2.md` 完成 `NavigationRoot` 承载裁决、S4 共享路径执行层落地，以及玩家 / NPC / NPC-NPC 的 live 终验。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-scene-audit`
  - `unity-mcp-orchestrator`
  - `sunset-unity-validation-loop`
  - `sunset-startup-guard` 继续手工等价
- 本轮结构落地：
  - 已在 `统一导航重构阶段设计与执行主表.md` 补齐 `NavigationRoot` 五段分析，并把本轮结构结果写回主表。
  - 已通过 unityMCP 在 `Primary/2_World` 下创建 `NavigationRoot`，采用“复制 `Systems` 再剥离非导航组件”的方式完成 live 迁移：
    - `NavigationRoot` 现在只保留 `Transform + NavGrid2D`
    - `Systems` 现在保留 `Transform + WorldSpawnService + WorldSpawnDebug + GameInputManager`
    - 玩家与 `001 / 002 / 003` 的 `navGrid` live 引用都已切到 `NavigationRoot/NavGrid2D`
    - 旧 `Systems/NavGrid2D` 已移除，`Primary.unity` 已保存，scene dirty 已清回 `false`
  - 已把 S4 共享路径执行层真正落到代码：
    - 新增 `Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs`
    - `PlayerAutoNavigator.cs` 改为使用共享 `ExecutionState / TryBuildPath / EvaluateWaypoint / EvaluateStuck / override waypoint`
    - `NPCAutoRoamController.cs` 改为接入同一套共享路径执行状态，并补了 `DebugMoveTo(Vector2 destination)`
  - 为 live 终验新增：
    - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
    - `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`
    - 其中终验入口后续改成“单场景 probe setup”菜单，避免 MCP 会话里 runtime runner 的后续 Update 日志缺失。
  - 为恢复 live 验证窗口，本轮还最小修正了一个外部编译阻塞：
    - `Assets/Editor/ChestInventoryBridgeTests.cs`
    - 把 `GetProperty("seedRemaining", 0)` 改成 `GetPropertyInt("seedRemaining", 0)`，避免 Play 期间自动重编译直接把终验打断。
- 本轮验证结果：
  - 代码层：
    - `refresh_unity` 后无本轮导航代码编译错误
    - 之前已跑过的 `NavigationAvoidanceRulesTests` 仍为 `4/4 Passed`
    - 本轮终验前 Unity 已恢复到可重新进入 Play Mode 的状态
  - live Scene 回读：
    - `NavigationRoot/NavGrid2D` live 配置有效，`obstacleTags` 含 `NPC`
    - 玩家和 3 个 live NPC 的 `navGrid` 确认都指向 `NavigationRoot`
  - 玩家绕移动 NPC：
    - probe setup 成功，日志显示 `npcMoveIssued=True`、`playerActive=True`、`npcMoving=True`，玩家建路成功 `2 个路径点`
    - 但 setup 后约 5 秒回读仍显示：
      - `Player` 位置仍是 `(-9.15, 4.45)`，`Rigidbody2D.linearVelocity = (0, 0)`，`PlayerAutoNavigator.IsActive = true`
      - `NPC 001` 位置仍是 `(-7.30, 3.60)`，`Rigidbody2D.linearVelocity = (0, 0)`，`NPCAutoRoamController.IsMoving = true`
    - 结论：失败，而且失败层级不是“绕得不好”，而是“执行态已启动但实际位移没有推进”
  - NPC 绕玩家：
    - probe setup 成功，日志显示 `npcMoveIssued=True`、`npcMoving=True`
    - 约 5 秒后回读：
      - 玩家仍停在 `(-7.25, 4.45)`
      - `NPC 001` 仍停在 `(-9.05, 4.45)`，`Rigidbody2D.linearVelocity = (0, 0)`
      - `NPCAutoRoamController.IsMoving = true` 且 `DebugPathPointCount = 11`，但 `NPCMotionController.IsMoving = false`
    - 结论：失败，本质仍是“有路径 / 有 moving 状态，但没有真实运动输出”
  - NPC-NPC 会车：
    - probe setup 成功，日志显示 `npcAMoveIssued=True`、`npcBMoveIssued=True`
    - 约 5 秒后回读：
      - `NPC 001` 仍停在 `(-9.05, 4.45)`
      - `NPC 002` 仍停在 `(-5.25, 4.45)`
      - 两者都保持 `NPCAutoRoamController.IsMoving = true`、`DebugPathPointCount = 11`，但 `Rigidbody2D.linearVelocity = (0, 0)`，`NPCMotionController.IsMoving = false`
    - 结论：失败，而且失败表现与前两类场景一致
- 本轮新增稳定判断：
  1. `NavigationRoot` 迁移和 S4 共享路径执行层已经真正落地，当前失败已不能再归咎于“Scene 承载没整理”或“玩家 / NPC 还没接到同一层执行状态”。
  2. 当前最高优先级 blocker 已收敛为：
     - 玩家 `PlayerAutoNavigator.IsActive = true` / NPC `NPCAutoRoamController.IsMoving = true`
     - 路径点数量正常
     - 但 `Rigidbody2D.linearVelocity` 与 Transform 位置不推进
     - 说明 live 失败现在落在“运行态位移执行没有真正落地”，不是局部规避参数。
  3. runtime 创建的 `NavigationLiveValidationRunner` 在 MCP 会话里菜单触发后能完成 setup，但它自己的后续 Update 证据不稳定；因此本轮终验结论最终以“单场景 probe setup + MCP 运行态回读”作为可信证据。
- 当前恢复点：
  - 下一轮不要再回头查 NPC Tag / `NavigationRoot` / S4 是否落地，这三件事本轮都已完成。
  - 下一优先排查应切到“为什么玩家导航 / NPC 漫游都进入 active/moving 状态，但实际位移始终为 0”：
    1. `PlayerAutoNavigator -> PlayerMovement` 的输入是否被别的运行态系统每帧清零
    2. `NPCAutoRoamController.TickMoving / FixedUpdate` 的位移结果是否被别的系统或 Rigidbody 语义吞掉
    3. 是否存在统一的运行态锁定、暂停、动作态或物理步进断裂

### 会话 15 - 2026-03-24（003-prompt-3 位移恢复后，继续锁接触裁决与 live 终验）

- 当前主线目标：
  - 严格按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\003-prompt-3.md` 继续推进；
  - 不再碰结构层，只围绕“位移未推进”后的真实 live blocker 继续修；
  - 用 unityMCP 再跑三类 live 场景，确认是否还存在“推土机”和新的第一责任点。
- 本轮子任务：
  - 基于上一轮已确认的 `runInBackground` 假冻结修复，继续处理“接触后仍持续推挤 / detour 恢复失效”的执行层问题；
  - 不再回头查 `NavigationRoot`、NPC Tag、Scene 挂载或 S4 是否落地。
- 本轮显式使用：
  - `skills-governor`
  - `unity-mcp-orchestrator`
  - `sunset-unity-validation-loop`
  - `sunset-startup-guard` 继续手工等价
- 本轮代码改动：
  - `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
    - 接触壳层内改成更强的 separation + 更低逃逸速度；
    - “非让行方但已进入近接触”也会被标成 blocking agent，避免只剩轻微 sidestep 却不进入 close-range 裁决。
  - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
    - `clearance <= 0` 且需要 repath 时，不再继续普通 `PathMove`，而是切 `ContactDetour / HardStop`；
    - 接触壳层内不再继续走环境碰撞微调；
    - close-range 裁决改用 `GetColliderRadius()`，修掉玩家侧 `dynamicObstaclePadding` 被重复计算导致的“明明有净空却长期判负 clearance”。
  - `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
    - 补了一条“NPC 即使保留优先级，只要进入近接触也必须被 solver 标记为 blocking agent”的回归测试；
    - 同步更新 close-range 慢速脱离断言。
- 本轮验证结果：
  - `Tests.Editor`：`118 / 118 Passed`
  - unityMCP live 全量重跑 `Tools/Sunset/Navigation/Run Live Validation`
  - `PlayerAvoidsMovingNpc`：
    - 已不再是“完全不动 / 直接推着 NPC 走”
    - 玩家和 NPC 都真实推进过，最大侧移已出现
    - 但仍以 `pass=False` 收尾，`minClearance=-0.092`，`playerReached=False`，`npcReached=False`
    - 控制台额外证据：
      - 已出现 `ContactDetour`
      - `[Nav] 卡顿 3 次，取消导航`
    - 当前判断：问题已从“推土机”切到“detour 后恢复推进 / stuck 收尾”。
  - `NpcAvoidsPlayer`：
    - `pass=True`
    - `npcReached=True`
    - `minClearance=-0.066`
    - 说明 NPC 侧在这轮 live 里已经能绕开静止玩家完成通过。
  - `NpcNpcCrossing`：
    - 仍 `pass=False`
    - `minClearance=0.513`，两者都真实移动过，但都没有到达各自终点
    - 日志显示两者会在 `Moving / ShortPause` 间切换后停在侧下方路径，不再是“位移未推进”，而是“执行收尾没闭环”。
  - 收尾现场：
    - 已退回 Edit Mode
    - `Primary.unity` 当前 `isDirty = false`
- 本轮关键判断：
  1. `003-prompt-3` 要求锁的“位移未推进”第一责任点已经不再是当前 blocker。
  2. 当前 live 已证明：角色位移链现在是真动了，`NPC Avoids Player` 已通过。
  3. 当前新的首要 blocker 已经换成“共享执行层在 detour / path-end / stuck 收尾时提前结束”，不再是“位移没写入 Rigidbody2D”。
  4. 从代码和 live 现象看，下一刀最像的具体责任点是：
     - `PlayerAutoNavigator.CheckAndHandleStuck()` 会在动态 detour / blocker 仍未完全脱离时过早取消导航；
     - `PlayerAutoNavigator.BuildPath()`、`NPCAutoRoamController.TryBeginMove()`、`NPCAutoRoamController.TryRebuildPath()` 当前都没有消费 `NavigationPathExecutor2D.BuildPathResult.ActualDestination`，导致 live 上出现“path end / short pause 已触发，但原目标并未真正到达”的嫌疑。
- 当前恢复点：
  - 下一轮不要再回头查“角色为什么不动”；
  - 直接改“detour 之后为什么提前收尾 / stuck cancel / path-end 提前落地”：
    1. 玩家：`CheckAndHandleStuck()` 与 detour 恢复点
    2. 玩家 / NPC：路径重建后对 `ActualDestination` 的消费
    3. NPC-NPC：`ReachedPathEnd` 触发短停前，先确认是否真的接近原目标
  - 本轮结束前 Unity 已退回 Edit Mode，没有把 Play 现场留给其他线程。

### 会话 16 - 2026-03-24（005-prompt-5 单场短窗续工：睡眠 blocker 参数抬高后，责任点前移到 NPC detour 恢复）

- 当前主线目标：
  - 严格按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-prompt-5-续工裁决入口与用户补充区.md` 收紧推进方式；
  - 只保留 `PlayerAvoidsMovingNpc` 单场短窗，不再回结构层、不再长时间 full-run；
  - 先吸收用户“已经出现一点绕开迹象，尝试把避让参数调高一点”的补充，再用 live 结果判断第一责任点是否继续留在 solver。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `unity-mcp-orchestrator`
  - `sunset-unity-validation-loop`
  - `sunset-startup-guard` 继续手工等价
- 本轮代码改动：
  1. `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
     - 只对 `treatAsBlockingObstacle && other.IsNavigationSleeping` / stationary blocker 这一支抬高侧绕权重；
     - 新增 sleeping/stationary blocker 的减前冲和更早降速；
     - 抬高 `repathLaneThreshold / repathClearanceThreshold / blockerForwardReach`，让 moving NPC 面对已到位玩家时更早转入 detour/repath。
  2. `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
     - 新增 `Solver_ShouldEscalateSleepingBlocker_EarlierForMovingNpc`；
     - 回归确认 sleeping blocker 现在会更早触发 `ShouldRepath`，并显式压低 `SpeedScale`。
  3. `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
     - 把 `Probe Setup/Player Avoids Moving NPC` 改成真正的单场收口；
     - `RunSingleSetup(...)` 不再在首场结束后串起 `NpcAvoidsPlayer / NpcNpcCrossing`，用于满足 005 prompt 的“单场短窗”要求。
- 本轮验证结果：
  - `Tests.Editor`：`119 / 119 Passed`
  - 本轮最小 live 窗口：
    - 先修复 `Probe Setup` 会自动串场的问题；
    - 再只跑 `PlayerAvoidsMovingNpc` 单场；
    - 结果：`pass=False timeout=6.50 minClearance=0.161 playerReached=True npcReached=False`
    - 说明：玩家已经稳定先到位，最小净空也转成了正值；当前失败不再表现为“推着 NPC 硬撞”。
  - heartbeat 关键证据：
    - 玩家在 `timer≈1.18s` 已 `playerActive=False`
    - `NPC 001` 之后长时间停留在 `(-6.3~-6.6, 4.1~4.3)` 一带来回摆动
    - 目标仍是 `(-7.30, 5.35)`，直到超时都保持 `npcAState=Moving`
    - 这表明失败形态已经变成“绕开后没有恢复到原目标收尾”，而不是“继续把玩家当推土机往前顶”
  - `NpcAvoidsPlayer / NpcNpcCrossing`
    - 本轮修参后，修 runner 前的一次短窗里两条都仍是 `pass=True`
    - runner 改成单场后未再次重跑，后续沿用这次最近 pass 基线
  - `Primary.unity`
    - 当前 live Scene `isDirty = false`
    - 磁盘上的 `git diff` 仍是 `StoryManager` 与 `CraftingStationInteractable` 的既有改动，不是本轮单场 probe 产生
- 本轮关键判断：
  1. 用户要求的“把避让参数调高一点”这刀已经真实落到了 sleeping/stationary blocker 分支，而且 live 结果证明这刀确实把失败形态从“接触推挤”压成了“正净空但收尾超时”。
  2. 因为玩家已到位、净空已转正、`NPC 001` 仍在 detour 区附近摆动，当前第一责任点已经不该再停在 `NavigationLocalAvoidanceSolver.Solve()` 的 sleeping blocker 裁决本身。
  3. 现在更具体的下一责任点应前移到：
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `TryHandleSharedAvoidance()`
     - `shouldAttemptDetour && TryCreateDynamicDetour(...)` 以及其后 `OverrideWaypoint` 清理/恢复原目标的分支
  4. 这条判断已经满足 005 prompt 要求的“更具体文件 / 方法 / 条件分支”，而且不是回结构层。
- 当前恢复点：
  - 下一轮不要再继续泛泛调 sleeping blocker 参数；
  - 直接锁 `NPCAutoRoamController.TryHandleSharedAvoidance()` 的 detour 恢复链：
    1. dynamic detour 何时真正清掉
    2. 清掉后何时恢复原目标路径
    3. 为什么 `NPC 001` 会在正净空状态下长期维持 `Moving` 却围着旧 detour 区摆动到超时

### 会话 17 - 2026-03-24（002-prompt-6 detour 恢复链锁死 + 同轮 fresh 复验）

- 当前主线目标：
  - 严格按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-6.md` 继续执行；
  - 不再泛调 solver，只锁 `NPCAutoRoamController.TryHandleSharedAvoidance()` 的 detour 恢复链；
  - 先修通 `PlayerAvoidsMovingNpc`，再补跑另外两条拿同轮 fresh 结果。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `unity-mcp-orchestrator`
  - `sunset-unity-validation-loop`
  - `sunset-startup-guard` 继续手工等价
- 本轮代码改动：
  1. `Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs`
     - `TryBuildPath(...)` 新增 `ignoredCollider`；
     - NPC 重建路径时可忽略自身碰撞体，不再把“自己压在目标点上”误判成终点不可走。
  2. `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - `IsWalkable(...) / TryFindNearestWalkable(...)` 新增忽略指定碰撞体的 live 查询分支；
     - 同步补 `ShouldIgnoreCollider(...)`，允许忽略同一根对象/同一刚体。
  3. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `DebugMoveTo / TryBeginMove / TryRebuildPath` 全部改成把 `navigationCollider` 传入共享建路；
     - 新增 `[NPCNavBuild]` 诊断日志，直接打印 detour 清理后恢复原目标时的真实终点。
  4. `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
     - 新增 `NavigationPathExecutor_ShouldKeepRequestedDestination_WhenIgnoringNpcSelfCollider`；
     - 锁“NPC 自己堵在目标点上时，忽略自身碰撞体后不得再把请求终点替换掉”。
- 本轮关键证据：
  - fresh log 已确认：
    - `001 DebugMoveTo => Destination=(-7.30, 5.35), PathCount=5`
    - `001 detour 清理后恢复原目标 => Requested=(-5.47, 3.48), Actual=(-5.47, 3.48), PathCount=14`
  - 当前 detour 恢复链已经不是“恢复了但把原目标改写成替代点”，而是能保住原目标。
- 本轮验证结果：
  - `Tests.Editor`：`121 / 121 Passed`
  - fresh live：
    - `PlayerAvoidsMovingNpc`
      - `pass=True`
      - `minClearance=0.145`
      - `playerReached=True`
      - `npcReached=True`
    - `NpcAvoidsPlayer`
      - `pass=True`
      - `minClearance=0.744`
      - `npcReached=True`
    - `NpcNpcCrossing`
      - `pass=False`
      - `timeout=6.57`
      - `minClearance=0.553`
      - `npcAReached=False`
      - `npcBReached=False`
  - 现场：
    - 本轮 live 收尾已退回 Edit Mode
    - `Primary.unity` 当前未留下本轮 probe 脏改
- 本轮关键判断：
  1. `002-prompt-6` 要锁的 detour 恢复链已经被进一步压实并修通；
  2. `PlayerAvoidsMovingNpc` 的第一责任点已经不再是 detour 清理/原目标恢复；
  3. 当前新的剩余 blocker 已转成 `NpcNpcCrossing` 会车中段停滞：
     - 两个 NPC 持续 `Moving`
     - `minClearance` 始终为正
     - 但会长期卡在 `x≈-8.2 / -6.8, y≈4.14` 一带，不继续推进到终点。
- 当前恢复点：
  - 下一轮不要回头再查 detour 恢复是否丢目标；
  - 直接锁 `NpcNpcCrossing` 在共享执行层里的中段停滞：
    1. 双 NPC 会车时的 waypoint 推进是否被 close-range / repath 节流卡住
    2. `NPCAutoRoamController.TickMoving()` 在双 moving 对称场景下为什么长期保留 `Moving` 却不继续落位
    3. 是否需要为双 NPC 会车补一条“正净空后必须恢复推进”的共享执行收尾约束

### 会话 18 - 2026-03-24（002-prompt-6 收口：双 NPC 会车恢复推进，三条同轮 fresh 全绿）

- 当前主线目标：
  - 继续 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\002-prompt-6.md`；
  - 不再回结构层、不再泛调 solver，只把 `NpcNpcCrossing` 的第一责任点锁死并补齐同轮 fresh 终验。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `unity-mcp-orchestrator`
  - `sunset-unity-validation-loop`
  - `sunset-startup-guard` 继续手工等价
- 本轮补记并核实的关键代码落地：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `TryHandleSharedAvoidance()` 保留了 `ShouldForceDynamicDetour(...)` 与 soft-deadlock detour 触发；
     - 决定性修复是去掉 `!avoidance.HasBlockingAgent` 分支上的“即时 `ClearDynamicDetourIfNeeded(...)`”；
     - 结果是 dynamic detour 不再一帧建、一帧拆，`NpcNpcCrossing` 不再陷入 clear/rebuild 风暴。
  2. `Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs`
     - 新增 `NPCAutoRoamController_ShouldForceDynamicDetour_ForSoftDeadlockPeerCrossing`
     - `InvokeStatic(...)` 已支持 `BindingFlags.NonPublic | BindingFlags.Static`
  3. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 外部编译 blocker 已止血，当前不会再阻断本线 EditMode 验证。
- 本轮验证结果：
  - `Tests.Editor = 123 / 123 Passed`
  - unityMCP 单实例：`Sunset@21935cd3ad733705`
  - 收尾前已回到 Edit Mode
  - 同轮 fresh：
    - `PlayerAvoidsMovingNpc`
      - `pass=True`
      - `minClearance=0.379`
      - `playerReached=True`
      - `npcReached=True`
      - `timeout=3.69`
    - `NpcAvoidsPlayer`
      - `pass=True`
      - `minClearance=0.989`
      - `npcReached=True`
      - `timeout=3.74`
    - `NpcNpcCrossing`
      - `pass=True`
      - `minClearance=0.632`
      - `npcAReached=True`
      - `npcBReached=True`
      - `timeout=3.40`
- 本轮关键判断：
  1. `NpcNpcCrossing` 的第一责任点不是继续调 solver 参数，而是 `NPCAutoRoamController.TryHandleSharedAvoidance()` 里 detour 被过早清理。
  2. 当前 `002-prompt-6` 已完成闭环：
     - detour 恢复链已通
     - 玩家绕移动 NPC 已通
     - NPC 绕玩家已通
     - NPC-NPC 会车已通
  3. 后续导航线如再出现回归，不要回头从结构层或 sleeping blocker 参数重新起跑；先守住这条“detour 不得单帧清理”的事实基线。
- 当前恢复点：
  - `002-prompt-6` 当前已收口完成；
  - 下一轮若继续导航线，应从新的治理入口或新的 live 回归现象出发，不再重复 detour 恢复链排查。

### 会话 19 - 2026-03-24（实现说明审计：当前导航不是 crowd sim，而是“静态路径 + 启发式局部绕行”）

- 当前主线目标：
  - 不继续 patch；
  - 回答用户审核问题：当前导航到底怎么实现、为什么会看起来像“NPC 外面套了个大圆形碰撞壳，玩家仍然像推土机”。
- 本轮完成：
  1. 回读核心实现：
     - `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
     - `Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs`
     - `Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
  2. 用 unityMCP 回读 live Scene 当前挂载与参数：
     - `Player/PlayerAutoNavigator`
     - `Player/BoxCollider2D`
     - `Player/PlayerMovement`
     - `NPCs/001/NPCAutoRoamController`
     - `NPCs/001/BoxCollider2D`
     - `Primary/2_World/NavigationRoot/NavGrid2D`
- 本轮新增稳定事实：
  1. 当前导航并不是连续 crowd simulation / RVO / ORCA；
     - 它本质上仍是“NavGrid 静态路径 + 共享局部避让 solver + 临时 override waypoint”。
  2. 当前系统对动态角色的几何近似是“圆半径”，不是按 BoxCollider 真实形状做 Minkowski/多边形投影：
     - 玩家 live BoxCollider bounds 半径约 `0.392`
     - 玩家 `AvoidanceRadius = playerRadius + 0.15 ≈ 0.542`
     - NPC live BoxCollider bounds 半径约 `0.374`
     - NPC `AvoidanceRadius = max(0.6, colliderRadius) = 0.6`
     - 因此玩家与 NPC 的基础 interaction radius 约 `1.142`
     - 玩家 close-range engage 壳层大约在 `1.29` 中心距附近启动
     - 玩家 dynamic detour 的 contact shell distance 约 `1.64`，候选绕行点会被投到更外层
  3. 因而用户看到“像外面包了一个大圆形壳”不是错觉，而是当前实现的真实特征。
  4. 当前 probe 通过的验证口径是受控脚本场景，不等于完整自由玩法口径：
     - `NavigationLiveValidationRunner` 会把玩家与 NPC 摆到固定位置，再调用 `SetDestination / DebugMoveTo`
     - 它证明的是“在这组受控交叉路径场景里，当前启发式链路可以过”
     - 还没有等价证明“真实右键点地 + 任意漫游 NPC + 全场景碰撞组合”都已经手感合格
- 本轮关键判断：
  1. 用户当前感受到的“推土机感”不是在胡说，和当前实现方式是对得上的。
  2. 真正造成这种体感的不是单一 bug，而是当前方案的结构特征：
     - 目标意图始终保持“继续去点击点”
     - 局部避让只是修正方向/速度，或临时插一个 override waypoint
     - 它没有把“礼让等待 / 长走廊式重规划 / 真实轮廓避障”变成第一原则
  3. 当前 live probe 全绿只能说明“功能链已经从彻底推撞退到了可控启发式避让”，不能直接外推成“手感已经达标”。
- 当前恢复点：
  - 如果下一轮继续做导航体验收口，应优先把“probe 通过”和“真实手感合格”拆成两个验收口径；
  - 同时正面决定是否继续接受当前“圆壳层 + 单 detour 点”这套启发式方案，还是上提到更强的体验级避让语义。

### 会话 20 - 2026-03-24（客观审核与锐评：功能过线，但架构仍是启发式混合方案）

- 当前主线目标：
  - 不继续 patch；
  - 站在用户验收视角，对“当前导航是不是合适、是不是最佳、最初框架是否就有偏差”给出独立判断。
- 本轮完成：
  1. 回读并交叉核对：
     - `NavigationLocalAvoidanceSolver`
     - `NavigationAvoidanceRules`
     - `NavigationPathExecutor2D`
     - `PlayerAutoNavigator`
     - `NPCAutoRoamController`
     - `NavigationAvoidanceRulesTests`
  2. 把代码实现、近期 live 回执与用户体感问题对齐，确认“为什么会像大圆壳、为什么仍有推土机感”。
- 本轮新增稳定事实：
  1. 当前导航主骨架是：
     - `NavGrid2D` 静态 A* 建路
     - `NavigationPathExecutor2D` 统一路径推进 / stuck / override waypoint
     - `NavigationLocalAvoidanceSolver` 启发式局部避让
     - 玩家 / NPC 在行为层各自包一层 detour / repath 策略
  2. 当前并不是时空级 crowd simulation，也不是 ORCA / RVO；
     - 它是“静态路径 + 半径壳层近接触约束 + 单 detour 点”的混合方案。
  3. 玩家和 NPC 虽然共享了一部分执行层，但没有从第一天就统一成同一套动态交通语义；
     - 真正统一的是路径状态机；
     - 没真正统一的是“遇到活体阻挡时该等待、该侧绕、该谁礼让、何时恢复原目标”的完整行为规则。
  4. 因此用户看到的“大圆壳感”和“推土机残影”与当前实现是对应的，不是错觉。
- 本轮关键判断：
  1. 方向不能判成“完全错了”；
     - 共享执行层、共享 registry、统一局部避让入口，这些方向本身是对的。
  2. 但起始抽象层级确实偏低；
     - 一开始更像是在“统一寻路执行 + 后补动态避让补丁”，而不是先定义统一动态交通规则。
  3. 所以这条线前面才会持续出现：
     - 推着走
     - 不推进
     - 慢蹭
     - detour 清不掉
     - detour 清掉后又恢复不到原目标
  4. 当前三条 fresh live 全绿，只能证明“受控 probe 场景已被补丁链拉到可通过”，还不能直接外推成“真实自由玩法手感已经优雅”。
- 当前恢复点：
  - 若后续继续做导航体验收口，应把验收拆成两条：
    1. 功能线：三条受控场景持续通过
    2. 体验线：真实右键导航面对任意漫游 NPC 时，不再明显呈现大圆壳 / 推土机体感
  - 如果用户的目标是“能用、能过受控终验”，当前方案已经接近可交付；
  - 如果用户的目标是“自然、稳定、像同一交通系统里的活体互让”，当前方案还不是最佳框架。

### 会话 21 - 2026-03-24（硬读 `005-gemini聊天记录-1.md` 后的稳定理解）

- 当前主线目标：
  - 不继续 patch；
  - 按用户要求把 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\005-gemini聊天记录-1.md` 整份硬读完，再回答“现在到底理解到了什么”。
- 本轮完成：
  1. 已完整通读全文 `305` 行，不跳段、不只看摘要；
  2. 已把它拆成 4 个层次理解：
     - 第一段：对当前导航现状的技术说明
     - 第二段：对当前架构原罪的更强烈锐评
     - 第三段：用户把目标从“修补能用”上提到“专业级、未来可扩展底座”
     - 第四段：Gemini 给出四层架构白皮书
- 本轮新增稳定事实：
  1. 这份文件里真正最重要的，不是它骂得多狠，而是它把用户的真实要求彻底说死了：
     - 用户不要“能用”
     - 用户要“极致贴合项目、未来怪物追击/宠物跟随也能接”的专业导航基础设施
  2. Gemini 的核心判断里，方向上最有价值的 4 点是：
     - 静态建路与动态避让必须断层
     - 动态活物不该同时扮演静态网格障碍和动态避让对象
     - 当前 detour 机制确实把微观避让反向污染到了宏观路径
     - 玩家 / NPC 的最终运动语义必须真正收口
  3. 但 Gemini 里面也有明显“说得太绝”的部分，不能直接当施工图：
     - 不是所有临时 detour 思想都天然错误，真正的问题是它被写进共享路径状态并引发生命周期风暴
     - “必须全部 `linearVelocity`”是一个强建议，不是唯一真理；关键是统一运动学语义，而不是机械迷信某个 API
     - “全面用真实 BoxBounds 替代一切圆形近似”方向是对的，但项目里更现实的表述应是“用脚底 footprint / shape-aware clearance”，而不是把它说成唯一实现形式
- 本轮关键判断：
  1. 这份文件不是单纯在骂当前实现，它实际上已经把导航议题从“修 bug”升级成“重做底层导航哲学”。
  2. 当前项目真正缺的不是再补一条 if-else，而是一份可执行的：
     - 需求边界
     - 验收分层
     - 宏观/微观责任划分
     - 玩家/NPC/怪物/宠物统一行为语义
  3. 因此这份 Gemini 文件对我最大的价值，不是“它提供了最后答案”，而是：
     - 它正确逼出了问题的真正层级；
     - 但它的白皮书仍需要结合 Sunset 当前代码资产做二次收敛，不能原样神化照搬。
- 当前恢复点：
  - 如果继续推进导航线，下一步最值得产出的不该是继续补 patch；
  - 而应是一份“结合当前 Sunset 代码现实的专业导航需求与架构设计稿”，把：
    1. 可继承的现有资产
    2. 必须推翻的旧语义
    3. 真正的最终验收口径
    全部明确写清。

### 会话 22 - 2026-03-24（正式落盘：导航从补丁修复转入专业底座设计）

- 当前主线目标：
  - 不再停留在聊天审视；
  - 把导航线真正落成一份工作区级正式设计稿，作为后续代码线程的唯一长期架构入口。
- 本轮完成：
  1. 新增正式文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\006-Sunset专业导航系统需求与架构设计.md`
  2. 文档已明确写清：
     - 用户真实目标不是“能用”，而是项目级专业导航底座
     - 当前旧系统的根问题与红线
     - 哪些现有资产保留、哪些重构、哪些语义废止
     - 新导航的 5 层架构
     - 功能线 / 体验线双验收
     - 后续迁移阶段
  3. 已同步更新：
     - `统一导航重构阶段设计与执行主表.md`
     - 让主表与新长文档建立“执行主表 / 长期设计正文”分工关系
- 本轮新增稳定事实：
  1. `导航检查` 工作区自此不再只有 prompt、锐评和临时主表；
     - 已拥有一份正式的长期架构设计正文。
  2. 后续导航线的主语已经从“统一导航旧系统修补”切成“专业导航底座迁移”。
- 本轮关键判断：
  1. 从现在开始，再继续只围绕旧 detour 或旧 solver 打补丁，已经不符合当前主线目标。
  2. 后续任何代码推进都必须先对齐 `006-Sunset专业导航系统需求与架构设计.md`，否则会重新退回“局部修好了，底层方向继续漂”的旧循环。
- 当前恢复点：
  - 下一轮若继续导航线，应优先拆出：
    1. 现有资产继承清单
    2. 必须废止的旧语义清单
    3. 第一阶段可执行迁移任务
