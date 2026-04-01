# 2026-04-01 典狱长｜NPC｜继续 PARKED，只守 NPC 体验边界

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\NPC.json`
- `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\spring-day1V2.json`

## 当前已接受基线

1. 这轮“最近交互唯一提示 / 唯一 E / 视觉归属一致”主刀，当前改判优先恢复给 `spring-day1V2`
2. 你这条线当前保留的是：
   - NPC 闲聊 / 气泡 / 会话体验守门
3. 当前三条相关线都已 `PARKED`

## 这轮唯一要求

**不要恢复这刀实现。继续 `PARKED`。**

你这轮只需要守住两件事：

1. 不主动继续碰下面 3 个共享文件：
   - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
   - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
   - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
2. 如果后续 `spring-day1V2` 实现动到了 NPC 闲聊 / 气泡体验红线，你再作为守门位指出 blocker

## 明确禁止

1. 不恢复 `ACTIVE`
2. 不继续写这刀共享文件
3. 不 `sync`

## 固定回复格式

请最小回复即可：

- 当前是否保持 `PARKED`
- 你当前继续守的 NPC 体验红线是什么
- 如果后续 `spring-day1V2` 碰到哪类改动你会认为越界
