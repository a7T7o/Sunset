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
