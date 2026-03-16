# NPC 1.0.0 初步规划 tasks

## 已完成

- 完成 NPC 固定模板生成链路的首轮落地：
  - 单张 PNG 输入
  - 自动切片
  - 生成 Idle / Move 动画
  - 生成 AnimatorController
  - 生成可拖入场景的 NPC Prefab
- 完成 NPC 运行时基础组件：
  - `NPCAnimController`
  - `NPCMotionController`
  - `NPCAutoRoamController`
  - `NPCBubblePresenter`
  - `NPCRoamProfile`
- 完成 NPC 第二阶段自动漫游策略：
  - 随机间隔移动
  - 0.5 秒到 3 秒短停
  - 3 次到 5 次短停后触发长停
  - 长停时优先尝试附近聊天，否则自言自语气泡
- 完成真实验证：
  - `Primary` 场景 001/002/003 可自动漫游
  - 再次抓到附近聊天正样本
  - 当前无 NPC 项目级编译红错

## 当前待做

- 根据后续脑暴继续扩展 `npc规划001.md` 中更细的行为层设计。
- 如用户仍希望进一步压缩工具 UI，可把 `NPCPrefabGeneratorTool` 再收束为更极简的固定入口。
- 如要继续扩展聊天系统，需要先判断是否会触及 A 类热文件 `DialogueUI.cs`。

## 当前最小恢复点

- 代码、预制体、动画和默认 RoamProfile 已恢复，项目已回到“可继续做 NPC 行为迭代”的状态。
