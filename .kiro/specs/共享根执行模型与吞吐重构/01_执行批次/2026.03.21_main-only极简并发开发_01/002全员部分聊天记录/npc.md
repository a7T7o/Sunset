我：“.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\并发线程_当前版本更新前缀.md”

他：“这次口径我已经对齐了，按这个前缀回当前真实现场：

- 当前在改什么：NPC `main-only` 复工准备，主线只剩气泡二次美化和下半身碰撞体感二次修正
- 上次 checkpoint / 当前恢复点：上次 checkpoint 是 `363918a6`，已经把 `NPC main-only bubble-and-collider pass` 提交到本地 `main`；当前恢复点是 NPC 第一刀收口已完成，下一刀直接继续做气泡与碰撞精修
- 剩余任务清单：必须继续的是倒三角朝下尾巴、尾巴单独轻微跳动、气泡/字体适度放大、留白更自然、玩家更难顶开 NPC、NPC/NPC 不再轻易堆叠；可以直接判废的是这轮先不做攻击/工具命中/受击/新交互系统
- 本轮下一步最多推进到哪里：最多推进到“NPC 第二刀表现层+碰撞体感精修完成并形成新的 `main` checkpoint”
- changed_paths：当前无新增 NPC dirty；上次 checkpoint 涉及 `[memory_0.md](/D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/NPC/memory_0.md)` `[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/memory.md)` `[memory.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/1.0.0初步规划/memory.md)` `[001.prefab](/D:/Unity/Unity_learning/Sunset/Assets/222_Prefabs/NPC/001.prefab)` `[002.prefab](/D:/Unity/Unity_learning/Sunset/Assets/222_Prefabs/NPC/002.prefab)` `[003.prefab](/D:/Unity/Unity_learning/Sunset/Assets/222_Prefabs/NPC/003.prefab)` `[NPCPrefabGeneratorTool.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NPCPrefabGeneratorTool.cs)` `[NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)` `[NPCBubblePresenter.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs)`
- 是否触碰高危目标：否
- 是否需要 Unity / MCP live 写：当前这条回执不需要；下一刀做完后需要 live 验收
- 当前是否可以直接提交到 `main`：可以；而且上一刀已经提交进本地 `main`
- blocker_or_checkpoint：`checkpoint = 363918a6 已完成；当前无 NPC 提交阻塞，只有后续若进入 Unity / MCP live 写时需要看占用`
- 一句话摘要：NPC 已经从“不能动”切到“可以直接在 main 继续做第二刀”，现在就差按新口径把气泡和碰撞精修到底”