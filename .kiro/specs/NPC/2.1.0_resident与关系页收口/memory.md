# 2.1.0 - resident 与关系页收口

## 模块概述
- 承接 NPC 的内容层与资料层：
  - resident 化
  - `NPC_Hand` 真源
  - 关系页头像
  - 人物简介结构
  - 玩家 / 旁白 `000` 特殊头像链

## 当前稳定结论
- `NPC_Hand` 已经从“手工挂图”推进到 editor 自动同步 + runtime 字典缓存真源
- 关系页不是百科，而是“当前剧情阶段里你对这个人的记录”
- 正式对白、关系页、人物资料的头像来源必须继续保持统一

## 当前恢复点
- 后续所有 resident / 关系页 / 简介 / 头像 canonical 问题统一先归这里

## 2026-04-13｜NpcCharacterRegistry 最小可提交包已真实归仓
- 用户目标：
  - 用户要求我不要再泛分析，而是按当前历史记忆和白名单规则，把 NPC 线程现在真能合法提交的内容先提交掉。
- 本轮方式：
  - 先对白名单最小包做真实 `preflight`；
  - 确认 `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs(.meta)` 可以独立收口后，再走 `Ready-To-Sync -> sync`；
  - 本轮未扩写新功能，只做归仓。
- 本轮完成：
  - 已真实提交并推送：
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs.meta`
  - 提交：
    - `66dadf93cdd9e3b29d67162b192804daec9757ac`
    - `2026.04.13_NPC_01`
- 本轮证据：
  - `sunset-git-safe-sync.ps1 -Action preflight` 对上面两条路径返回 `True`
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs --count 20 --output-limit 8`
    - `assessment=no_red`
    - `owned_errors=0`
    - `external_errors=0`
    - `unity_red_check=pass`
- 当前更稳结论：
  - `NpcCharacterRegistry` 现在已经不是“只存在于脏工作树里的未归仓实现”，而是 resident / 关系页 / 头像同源链里第一个已经真实回到 `main` 的最小代码包。
- 当前恢复点：
  - 头像真源、关系页真值、对白 speaker 解析这条链后续继续以 `NpcCharacterRegistry` 为 canonical 代码入口；
  - 但和它同批的 UI / runtime / tests / assets 仍在更脏的 mixed roots 里，不能假装这一刀已经把整条 resident 线都提交完了。
