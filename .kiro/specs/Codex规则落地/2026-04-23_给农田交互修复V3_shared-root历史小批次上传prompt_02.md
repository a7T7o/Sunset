请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md]

这轮不要继续修农田/箱子/放置链，也不要回到第一波“docs-only 上传完先停”的口径。

你当前唯一主线固定为：
只按过去实际开发历史，给 `农田交互修复V3` 再还原 `1` 个最小历史小批次上传尝试；这轮只允许这一小批，撞 blocker 就停车，不换第二批。

你必须先继承并且不要推翻的当前真状态：
1. 你第一波已真实 push：
   - `46069df4` `2026.04.23_农田交互修复V3_01`
   - `22b5a1e9` `2026.04.23_农田交互修复V3_02`
   - `b72dbb6f` `2026.04.23_农田交互修复V3_03`
2. 你当前 `thread-state = PARKED`
3. 第一波 docs-only own 面已 clean
4. 当前真实剩余 blocker 仍在：
   - `StoneController.cs`
   - `StoneControllerEditor.cs`
   - `Tool_005_BatchStoneState.cs`
   - `C1.prefab / C2.prefab / C3.prefab`
   - `PlacementGridCalculator.cs / PlacementGridCell.cs / PlacementNavigator.cs / PlacementPreview.cs / PlacementValidator.cs`
   - `ChestControllerEditor.cs / ChestAuthoringBatchSelectWindow.cs / ChestAuthoringSerializationTests.cs / ChestInventoryBridgeTests.cs`
   - `.codex/tmp_sapling1200_crop.png`
   - `.codex/tmp_sapling1200_crop_correct.png`
   - `.codex/tmp_sapling1201_crop.png`
   - `.codex/tmp_wateringcan_crop.png`
5. 用户最新改判是“按小历史批次慢慢传”，不是“把剩余 controller / placement / editor / prefab 一起清”。

这轮唯一允许的小批次固定为：
只拿“石头链”这一组历史小批做一次真实上传尝试，不准扩到 placement、chest authoring 或 `.codex` 证据图：

- `Assets/YYY_Scripts/Controller/StoneController.cs`
- `Assets/Editor/StoneControllerEditor.cs`
- `Assets/Editor/Tool_005_BatchStoneState.cs`
- `Assets/222_Prefabs/Rock/C1.prefab`
- `Assets/222_Prefabs/Rock/C2.prefab`
- `Assets/222_Prefabs/Rock/C3.prefab`

这轮必须按顺序执行：
1. 先确认这组石头链是不是一个真实历史小批，而不是你为上传临时拼的包。
2. 只对白名单这一组跑真实上传尝试。
3. 如果这组一进白名单就仍然把 own roots 扩进 `Assets/Editor` 或 `Assets/YYY_Scripts/Controller` 的 mixed 脏面，立刻停车。
4. 不准因为被卡就顺手吞：
   - `Assets/YYY_Scripts/Service/Placement/*`
   - `Assets/Editor/Chest*`
   - `.codex/tmp_sapling*`
   - `.codex/tmp_wateringcan_crop.png`
5. 这轮不准继续第二个小批次。

完成定义只有两种：
1. 这组石头链小批真实提交并 push 到 `origin`
2. 或者你把这一个小批次的 exact blocker 报死

最终回执必须额外明确：
1. 这组石头链是不是独立历史批次
2. 第一真实 blocker 到底是 own-root 扩根、mixed，还是别的
3. 这轮没有去动 placement / chest / `.codex` 第二组尾账

[thread-state 接线要求｜本轮强制]
如果你这轮会继续真实上传尝试，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住或收口，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
