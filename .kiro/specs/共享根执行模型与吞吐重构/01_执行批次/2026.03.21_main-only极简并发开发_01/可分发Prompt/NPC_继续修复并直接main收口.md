# NPC｜继续修复并直接 main 收口

```text
当前规则已经更新，而且这条更新已经真正进入 `main`：
- `main-only + whitelist-sync + exception-escalation`
- 不再因为 shared root 里有 unrelated dirty，就默认拦住你自己的白名单提交
- 你不要再按旧口径说“现在不能提交到 main”

当前 shared root：
- `D:\Unity\Unity_learning\Sunset @ main`

你这轮按下面顺序做，直接干，不要回分支：

1. 先复核你当前已经在 `main` 现场里的 NPC dirty
   当前应主要落在：
   - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/Editor/NPCPrefabGeneratorTool.cs`
   - `Assets/222_Prefabs/NPC/001.prefab`
   - `Assets/222_Prefabs/NPC/002.prefab`
   - `Assets/222_Prefabs/NPC/003.prefab`
   - `.kiro/specs/NPC/`
   - `.codex/threads/Sunset/NPC/memory_0.md`

2. 这轮你要解决的真实目标只有这些
   - 气泡不要再糊脸、贴脸
   - 气泡位置和观感更自然
   - NPC 与 NPC 不再互相穿透、堆叠
   - 玩家与 NPC 不再互相穿透
   - 碰撞体改成更合理的下半身透视占位，不要整身硬占满

3. 如果你当前这批 dirty 已经足够构成交付面
   - 直接按白名单提交到 `main`
   - 不要再说“等治理线程代提”

4. 如果你检查后发现还差一刀
   - 只在 NPC 允许域内补到可交付
   - 补完后立刻白名单提交到 `main`

当前允许域：
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
- `.codex/threads/Sunset/NPC/`

这轮不要做：
- 不要再切 branch
- 不要只写结论不改业务
- 不要扩成新的大系统重构
- 不要碰 `spring-day1`、农田、导航、scene-build 的现场

只有命中下面情况你才停：
1. 你发现有人正在改同一批 NPC Prefab 或同一批 NPC 脚本
2. 你要进 Unity / MCP live 写，但已经有人在写
3. 你发现当前 dirty 里混进了明显不属于 NPC 的路径

回执不要再让项目经理手动建文件。
直接把最终回执追加到：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`

回执固定字段：
- 当前在改什么
- changed_paths
- 是否触碰 Unity / MCP live 写
- 是否撞到高危目标
- 当前是否已经提交到 `main`
- 提交 SHA
- 当前 `git status` 是否 clean
- blocker_or_checkpoint
- 一句话摘要
```
