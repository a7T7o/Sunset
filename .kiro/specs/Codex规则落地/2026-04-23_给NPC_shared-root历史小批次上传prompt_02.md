请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md]

这轮不要继续修 resident / roam / runtime contract，也不要回到第一波“能交的就整包交”的口径。

你当前唯一主线固定为：
只按过去实际开发历史，给 `NPC` 再还原 `1` 个最小历史小批次上传；这轮只允许这一小批，撞 blocker 就停车，不换第二批。

你必须先继承并且不要推翻的当前真状态：
1. 你第一波 own 收口回执已成立，`035b4549` 已 push
2. `8f1909da` 已把你一部分 A 面数据/头像/registry/editor tool 先一步带进远端
3. 你当前 `thread-state = PARKED`
4. 当前真实剩余 blocker 仍在：
   - `Assets/Sprites/NPC_Hand/104.png`
   - `Assets/Sprites/NPC_Hand/104.png.meta`
   - `Assets/Editor/NPC/*`
   - `Assets/YYY_Scripts/Controller/NPC/*`
5. 用户最新改判是“按小历史批次慢慢传”，不是“再做一轮 broadly-own 上传”。

这轮唯一允许的小批次固定为：
只处理 `104` 头像删除与引用一致性这一小批；不要顺手扩到 editor tool 或 runtime controller。

这轮必须按顺序执行：
1. 先只核：
   - `Assets/Sprites/NPC_Hand/104.png`
   - `Assets/Sprites/NPC_Hand/104.png.meta`
   - 当前仓库内对 `104` 的剩余引用是否已经自然一致
2. 如果这组删除本身是独立历史小批，就只对白名单这组做真实上传尝试。
3. 如果你发现这组删除一旦提交就会牵出新的 shared/mixed 引用修复：
   - 立刻停车
   - 只报 exact blocker
4. 这轮不准改去吞：
   - `NPCAutoRoamController.cs`
   - `NPCMotionController.cs`
   - `NpcResidentRuntimeContract.cs`
   - `NpcResidentRuntimeSnapshot.cs`
   - `NpcResidentDirectorBridgeValidationMenu.cs`
   - 其它 `Assets/Editor/NPC/*`
5. 这轮不准继续第二个小批次。

完成定义只有两种：
1. `104` 删除这组小批真实提交并 push 到 `origin`
2. 或者你把这一个小批次的 exact blocker 报死

最终回执必须额外明确：
1. `104` 删除是不是独立历史批次
2. 当前引用是否已经自然一致
3. 如果失败，第一真实 blocker 到底是什么
4. 除这组之外，其它 NPC 尾账先不动

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
