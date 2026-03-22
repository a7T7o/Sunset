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
