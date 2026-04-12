# NPC - 活跃入口记忆

> 2026-04-10 起，旧根母卷已归档到 [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/memory_0.md)。本卷只保留 NPC 主线分流与恢复点。

## 当前定位
- 本工作区不再把 resident、关系页、头像真源、自漫游、避让恢复继续堆回一条根卷。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `2.1.0_resident与关系页收口`
  - `2.2.0_自漫游与避让收口`

## 当前稳定结论
- NPC 当前真正活着的是两条线：
  1. resident 化、`NPC_Hand` 真源、关系页与人物简介结构
  2. 自漫游、避让响应、障碍自动收集与 motion 收薄

## 当前恢复点
- 关系页、头像、resident、人物介绍问题进 `2.1.0`
- roam、avoidance、motion、obstacle 问题进 `2.2.0`
- 查旧阶段全量过程时，再看 `memory_0.md` 与 `2.0.0进一步落地/memory_0.md`

## 2026-04-13｜NPC 当前可归仓内容先收成最小 resident checkpoint
- 用户目标：
  - 用户要求我按 NPC 自己的历史边界，把现在真能合法提交的内容先提交掉，不继续扩功能。
- 本轮完成：
  - 已把 resident / 关系页线里当前唯一稳定可拆出的最小代码包提交到 `main`：
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs.meta`
  - 提交 SHA：
    - `66dadf93cdd9e3b29d67162b192804daec9757ac`
- 当前阶段判断：
  - NPC 线程现在不是“完全没有可提交内容”，但也远没到“大包都能一起交”的状态；
  - 当前能安全归仓的是孤立且编译闸门通过的 `NpcCharacterRegistry`；
  - 其余 resident / bubble / runtime / tests / assets 仍被 mixed roots、remaining dirty、shared blocker 卡住。
- 当前恢复点：
  - 如果后续继续做“能提的就提”，优先找这种：
    - own root 干净
    - `preflight=True`
    - CLI `validate_script` 至少不是 `own_red`
    的最小白名单切片；
  - 不再把大块 mixed roots 当成“顺手一起提”的候选。
