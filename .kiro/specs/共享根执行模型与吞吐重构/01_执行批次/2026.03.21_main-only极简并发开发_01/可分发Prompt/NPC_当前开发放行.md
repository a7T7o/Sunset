# NPC - 当前开发放行

```text
你现在直接开始真实开发，不再先走 branch / grant / return-main。
当前唯一真实基线：
- `D:\Unity\Unity_learning\Sunset @ main`

你这轮直接修 NPC 的真实问题，不要再停在“先验证我能不能开始”：
- 气泡位置仍然糊脸、贴脸的问题
- 气泡观感不自然的问题
- NPC 与 NPC 互相穿透
- 玩家与 NPC 互相穿透
- 碰撞体尺寸和透视关系不合理的问题

你可以直接修改的允许域：
- `Assets/YYY_Scripts/Controller/NPC/`
- `Assets/YYY_Scripts/Anim/NPC/`
- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
- `Assets/Editor/NPCPrefabGeneratorTool.cs`
- `Assets/Editor/NPCAutoRoamControllerEditor.cs`
- `Assets/222_Prefabs/NPC/`
- `Assets/111_Data/NPC/`
- `Assets/100_Anim/NPC/`
- `Assets/Sprites/NPC/`
- `.kiro/specs/NPC/`

这轮不要做的事：
- 不要先搞 branch
- 不要只写结论不改业务
- 不要顺手扩成大系统重构

只有命中下面情况你才停：
1. 发现有人已经在改同一批 NPC Prefab 或同一批 NPC 脚本
2. 你要进 Unity / MCP live 写，但已经有人在写
3. 你已经把 NPC 现场写坏了，需要先收口

聊天只回：
- 当前在改什么
- changed_paths
- 是否触碰 Unity / MCP live 写
- 是否撞到高危目标
- blocker_or_checkpoint
- 一句话摘要
```
