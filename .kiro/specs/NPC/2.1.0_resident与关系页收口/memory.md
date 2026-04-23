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

## 2026-04-18｜NPC_Hand 新增 202 对话胸像
- 用户目标：
  - 基于 `Assets/Sprites/NPC/202.png` 的角色设计，补一张可直接落入 `NPC_Hand` 链的人物对白胸像；
  - 强约束是：
    - 单人
    - 右向 3/4
    - bust-up
    - 粉发 + 白色女仆头带 + 黑白女仆装
    - 纯白实底
- 本轮方式：
  - 真实施工前已跑 `Begin-Slice`；
  - 只新增：
    - `Assets/Sprites/NPC_Hand/202.png`
    - `Assets/Sprites/NPC_Hand/202.png.meta`
  - 未进入 `Ready-To-Sync`；
  - 落盘后已跑 `Park-Slice`。
- 本轮完成：
  1. 已新建 `202` 的对白头像成品：
     - `Assets/Sprites/NPC_Hand/202.png`
  2. 已补一份可被 Unity 识别为 Sprite 的导入 `.meta`：
     - `Assets/Sprites/NPC_Hand/202.png.meta`
  3. 头像构图当前已收成：
     - 单人右向胸像
     - 头部、颈部、肩部完整在框内
     - 粉发 bob + 白色头带 + 黑白女仆服
     - 纯白不透明背景
- 静态验证：
  - `202.png` 当前尺寸：
    - `512x512`
  - alpha 范围：
    - `255~255`
  - 四角像素均为：
    - `(255,255,255,255)`
  - 已做本地可视抽查，当前成品不是“从行走帧直接裁大”的失败路径，而是一张独立胸像。
- 当前没做的事：
  - 还没进 Unity 触发真正的 import / `NpcCharacterRegistryHandPortraitAutoSync`
  - 还没在 `DialogueUI` / 关系页里做 runtime 侧显示核验
  - 因此这轮只能报：
    - 资产文件已落地
    - 静态自验已过
    - Unity 接链验证待补
- 当前恢复点：
  - 如果下一轮继续 resident / 头像真源线，最值钱的下一步是：
    1. 打开 Unity 让 `202.png` 完成 import；
    2. 复核 `NpcCharacterRegistry` 是否已吃到 `202`；
    3. 再看对白框显示里是否需要继续做表情版或更强 Ram 神态版。

## 2026-04-18｜`202` 对话胸像被用户退回后已按 `NPC_Hand` 现有风格重做
- 用户纠正：
  - 上一版是胡闹，主要错在三点：
    1. 没有先学习 `Assets/Sprites/NPC_Hand` 现有内容
    2. 错把背景做成了实色
    3. 成品里还有坏点 / 坏条纹
- 本轮方式：
  - 继续真实施工前已重新 `Begin-Slice`；
  - 先读取并比对：
    - `NPC_Hand/001.png`
    - `NPC_Hand/104.png`
    - `NPC_Hand/201.png`
    - 当前错误版 `NPC_Hand/202.png`
  - 先确认现有规律：
    - 透明底
    - 角色主体完整保留
    - 不额外铺实色背景
    - `201` 是本轮最接近的 maid 风格母版
  - 然后才重做 `202.png`。
- 本轮完成：
  1. 已用 `201` 的正式头像风格做低风险重构母版，重做 `Assets/Sprites/NPC_Hand/202.png`；
  2. 已把背景改回透明：
     - alpha 当前为 `0~255`
     - 四角像素均为透明
  3. 已把画布规格改成与当前成品一致的透明胸像尺寸：
     - `536x700`
  4. 已同步修正 `.meta` 中的 Sprite rect：
     - `536x700`
- 当前更稳结论：
  - 这次已经修正了“背景口径错 + 坏点坏条纹 + 不学现有资产”的问题；
  - 但它仍然只站住：
    - `NPC_Hand` 资产重做完成
    - 静态透明度和尺寸自验成立
  - 还没站住：
    - Unity import
    - `NpcCharacterRegistryHandPortraitAutoSync`
    - `DialogueUI` / 关系页 runtime 显示
- 当前恢复点：
  - 下一步不该再先画第三张；
  - 应先把这张新 `202.png` 走完 Unity 接链验证，再决定是否还要继续做更强 Ram 神态版。
