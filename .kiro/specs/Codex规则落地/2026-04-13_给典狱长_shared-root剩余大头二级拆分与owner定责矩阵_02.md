# 2026-04-13｜shared-root 剩余大头二级拆分与 owner 定责矩阵

## 一句话结论
- 截至这次复核，`git status --porcelain=v1 -uall` 仍有 `236` 条。
- 当前 shared-root 已经不再是“资产海”，而是 `6` 组可直接认领的代码/配置/工具链簇。
- 这轮之后，其他线程不该再靠文件名猜 owner；应直接按本表认领自己的簇。

## 当前 6 组

| 责任组 | 数量 | 精确覆盖 | 当前裁定 | 说明 |
|---|---:|---|---|---|
| `spring-day1` | `48` | `Assets/Editor/Story/*SpringDay1*`、`DialogueDebugMenu.cs`、`SpringUiEvidenceMenu.cs`、`Assets/YYY_Scripts/Story/Directing/SpringDay1*`、`Assets/YYY_Scripts/Story/Managers/SpringDay1*`、`DialogueManager.cs`、`CraftingStationInteractable.cs`、`SpringDay1ProximityInteractionService.cs`、`Assets/YYY_Tests/Editor/SpringDay1*` | `继续发 prompt` | 仍是 live 主线的 Story/导演/运行时桥接尾账 |
| `NPC` | `47` | `Assets/Editor/NPC/*`、`Assets/YYY_Scripts/Controller/NPC/*`、`Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`、`Assets/YYY_Scripts/Data/NPC*.cs`、`Assets/YYY_Scripts/Story/Interaction/NPC*`、`NpcInteractionPriorityPolicy.cs`、`Assets/YYY_Tests/Editor/Npc*` | `继续发 prompt` | NPC runtime、resident continuity、bubble/interaction 仍是成组尾账 |
| `UI` | `19` | `Assets/YYY_Scripts/Story/UI/*`、`Assets/YYY_Scripts/UI/Tabs/*`、`Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`、`Assets/YYY_Tests/Editor/PackagePanel*`、`PlayerNpcConversationBubbleLayoutTests.cs`、`PlayerThoughtBubblePresenterStyleTests.cs`、`Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset` | `继续发 prompt` | 这组已不只是 `InteractionHintOverlay`，还包含地图页/关系页/UI kit 和字体材质尾巴 |
| `树石修复` | `8` | `StoneControllerEditor.cs`、`TreeControllerEditor.cs`、`Tool_004_BatchTreeState.cs(.meta)`、`Tool_005_BatchStoneState.cs(.meta)`、`StoneController.cs`、`TreeController.cs` | `继续发 prompt` | 仍需整组处理 same-root remaining dirty |
| `Codex规则落地 own：Town/Home/Primary 基线链` | `76` | `Assets/Editor/Home/*`、`Assets/Editor/Town/*`、`TownCameraRecoveryMenu.cs`、`TownFoundationBootstrapMenu.cs`、`ScenePartialSync*`、`ScenePrimaryBackupScratchDryRunMenu*`、`NavigationStaticPointValidationMenu.cs`、`Chest*`、`PlacementManagerAdjacentIntentTests*`、`CodexMcpHttpAutostart*`、`TilemapSelectionToColliderWorkflow.cs`、`TilemapToColliderObjects.cs`、`FarmAnimalPrefabBuilder*`、`Assets/YYY_Scripts/Farm/*`、`Service/Navigation/*`、`Service/Placement/*`、`Service/Player/*`、`Service/Rendering/*`、`TimeManager.cs`、`CraftingService.cs`、`PlayerToolHitEmitter.cs`、`StoryManager.cs`、`DialogueChineseFontRuntimeBootstrap*`、`SaveLoadDebugUI.cs`、对应 Editor tests` | `治理线程自己继续` | 这是 Town/Home/Primary runtime 基线与编辑工具的混合尾账，不适合甩给 `spring-day1 / NPC / UI / 树石` |
| `Codex规则落地 own：工具链/配置` | `38` | `.kiro/xmind-pipeline/*` `36` 条 + `ProjectSettings/EditorBuildSettings.asset` + `ProjectSettings/QualitySettings.asset` | `治理线程自己继续` | 其中 `.kiro/xmind-pipeline` 可独立验证与提交；`ProjectSettings` 仍需保守处理 |

## 代表样本

### `spring-day1`
- `Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs`
- `Assets/Editor/Story/DialogueDebugMenu.cs`
- `Assets/Editor/Story/SpringUiEvidenceMenu.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
- `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
- `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`

### `NPC`
- `Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs`
- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeContract.cs`
- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
- `Assets/YYY_Tests/Editor/NpcSceneTransitionContinuityTests.cs`

### `UI`
- `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
- `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`

### `树石修复`
- `Assets/Editor/Tool_004_BatchTreeState.cs`
- `Assets/Editor/Tool_005_BatchStoneState.cs`
- `Assets/YYY_Scripts/Controller/StoneController.cs`
- `Assets/YYY_Scripts/Controller/TreeController.cs`

### `Codex规则落地 own：Town/Home/Primary 基线链`
- `Assets/Editor/Home/PersistentPlayerSceneRuntimeMenu.cs`
- `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
- `Assets/Editor/ScenePartialSyncTool.cs`
- `Assets/Editor/ChestControllerEditor.cs`
- `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`

### `Codex规则落地 own：工具链/配置`
- `.kiro/xmind-pipeline/src/config/topic-blueprints.ts`
- `.kiro/xmind-pipeline/src/lib/extract-markdown.ts`
- `.kiro/xmind-pipeline/tests/xmind-pipeline.test.ts`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/QualitySettings.asset`

## 当前更硬的判断
- `TextMesh Pro/DialogueChinese V2 SDF.asset` 不再适合被治理线程当“剩余纯资产”顺手吞掉；它现在更像 `UI/字体链` 的一部分。
- `DialogueDebugMenu.cs`、`SpringUiEvidenceMenu.cs`、`DialogueManager.cs`、`CraftingStationInteractable.cs`、`SpringDay1ProximityInteractionService.cs` 应归到 `spring-day1`，而不是继续挂在“未分类 Story”。
- `TilemapSelectionToColliderWorkflow.cs`、`TilemapToColliderObjects.cs`、`FarmAnimalPrefabBuilder.cs` 更接近治理线程 own 的 scene/tooling 基线，不应误发给 `树石修复`。
- `.kiro/xmind-pipeline` 这组当前已通过本轮 `npm run smoke` 与 `npm run test`，可以视作治理线程 own 的独立安全 checkpoint 候选。
- `ProjectSettings/EditorBuildSettings.asset` 与 `ProjectSettings/QualitySettings.asset` 仍不能和 `.kiro/xmind-pipeline` 混成一个“顺手全收”的包。

## 本轮建议动作
1. 把 `spring-day1 / NPC / UI / 树石修复` 全部切到各自的 `02` 版专属 prompt。
2. 治理线程自己先收 `.kiro/xmind-pipeline`，不要继续把它和 `ProjectSettings` 捆绑。
3. `ProjectSettings` 暂留为治理线程 own blocker，不对外甩。
