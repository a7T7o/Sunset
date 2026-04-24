请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_shared-root工具incident与资源根同步分发批次_04.md]

不要再继续改 `npcId:104` 的内容本身。

你当前唯一主线固定为：
把已经完成内容一致性的 `NpcCharacterRegistry.asset + 104 删除态图片` 这一刀，改成一次 `Assets/Resources/Story` 目录同步 blocker 处理；这轮只处理同步口，不再回去修 resident / roam / runtime contract。

你必须先继承并且不要推翻的当前真状态：
1. 上一刀 `prompt_03` 已经完成，不准再把这轮讲成“104 内容还没修完”。
2. 当前 [NpcCharacterRegistry.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/NpcCharacterRegistry.asset) 里 `npcId:104` 已经是：
   - `handPortrait: {fileID: 0}`
3. [104.png](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/NPC_Hand/104.png) 与 [104.png.meta](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/NPC_Hand/104.png.meta) 当前仍是删除态
4. 当前新的第一真实 blocker 已经不是内容问题，而是：
   - [Recipe_9102_Pickaxe_0.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset)
   - 让本轮 own root 折叠成 `Assets/Resources/Story` 后被 same-root foreign dirty 挡住
5. 当前 `thread-state = PARKED`

这轮唯一允许的范围固定为：
1. [NpcCharacterRegistry.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/NpcCharacterRegistry.asset)
2. [104.png](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/NPC_Hand/104.png)
3. [104.png.meta](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/NPC_Hand/104.png.meta)
4. `Assets/Resources/Story/**` 范围内与这次同步 blocker 直接相关的 foreign dirty 现场
5. 与这次 blocker 直接相关的 thread-state / ownership / memory 证据

这轮明确不准做的事：
1. 不准再改 `npcId:104` 之外的任何 NPC 内容
2. 不准顺手碰：
   - [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
   - [NPCMotionController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
   - [NpcResidentRuntimeContract.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeContract.cs)
   - [NpcResidentRuntimeSnapshot.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeSnapshot.cs)
   - [NpcResidentDirectorBridgeValidationMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs)
3. 不准因为目录 blocker 就把 `SpringDay1Workbench` 或其它 `Assets/Resources/Story` foreign 文件顺手吞进白名单

这轮必须按顺序执行：
1. 先判断：现有证据是否已经足够把 `Assets/Resources/Story` 这次 blocker 收成 exact 目录级阻断。
2. 如果证据已足够：
   - 不重跑同一上传
   - 直接把 blocker 边界、foreign 文件和下一步路由说清并停车
3. 如果证据还不够：
   - 只允许做最多 `1` 次围绕当前 `3` 文件白名单的最小复核
   - 目标只为确认 blocker 边界，不是再回去改 `104` 内容
4. 只有当你确认 `Assets/Resources/Story` 当前 foreign blocker 已自然消失时，才允许对白名单这 `3` 个文件做 `1` 次真实上传尝试。
5. 无论成功或失败，最终都必须把下面三层拆开说：
   - `104` 内容一致性已经完成的部分
   - 当前真正挡住 sync 的 `Assets/Resources/Story` foreign blocker
   - 下一步应归谁处理：`NPC` 自己、对应 foreign owner、还是治理/工具位

这轮完成定义只有两种：
1. 你把 `Assets/Resources/Story` 目录级 blocker 收成 exact 结论并停车
2. 或者在 foreign blocker 已自然消失的前提下，这 `3` 文件真实提交并 push 成功

最终回执必须额外明确：
1. `npcId:104` 的 `handPortrait` 一致性是否仍保持已补平
2. 这轮有没有重跑；如果没有，为什么现有证据已足够
3. 当前 `Assets/Resources/Story` 的第一真实 foreign blocker 到底是谁
4. 这轮有没有再次去改 `104` 内容或碰其它 NPC 尾账
5. 下一步这件事应继续归 `NPC` 处理，还是应转给 foreign owner / 治理位

[thread-state 接线要求｜仅在你真的继续上传时生效]
如果你这轮确认 foreign blocker 已消失，并决定继续做真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮只读补证或最终停车，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
