using System.IO;
using NUnit.Framework;

[TestFixture]
public class WorldStateContinuityContractTests
{
    private static readonly string ProjectRoot =
        Directory.GetParent(UnityEngine.Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();

    private static readonly string CropControllerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Farm/CropController.cs");

    private static readonly string PersistentBridgePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs");

    private static readonly string ChestControllerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/Placeable/ChestController.cs");

    private static readonly string WorldStateRunnerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PersistentWorldStateLiveValidationRunner.cs");

    private static readonly string SaveManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Data/Core/SaveManager.cs");

    private static readonly string SpringDay1DirectorPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs");

    private static readonly string DoorTriggerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/DoorTrigger.cs");

    private static readonly string PlacementManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Placement/PlacementManager.cs");

    private static readonly string HotbarSelectionServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs");

    private static readonly string AutoPickupServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/AutoPickupService.cs");

    private static readonly string InventorySortServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs");

    private static readonly string CraftingServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Crafting/CraftingService.cs");

    private static readonly string PlayerInteractionPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs");

    private static readonly string SaveDataDtosPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs");

    private static readonly string TreeControllerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Controller/TreeController.cs");

    private static readonly string OcclusionManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs");

    private static readonly string WorldItemPickupPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/WorldItemPickup.cs");

    private static readonly string WorldItemPoolPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/WorldItemPool.cs");

    private static readonly string ItemDropHelperPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Utility/ItemDropHelper.cs");

    [Test]
    public void CropController_ShouldGeneratePersistentIdBeforeRegistryAndSaveUse()
    {
        string cropText = File.ReadAllText(CropControllerPath);

        StringAssert.Contains("EnsurePersistentId();", cropText,
            "作物运行态至少应在启用和取 PersistentId 时补齐 GUID，不能再让新种出来的作物带空 ID 离场。");
        StringAssert.Contains("if (PersistentObjectRegistry.Instance != null)", cropText,
            "作物启用时应直接走注册表注册，不应再因为 persistentId 为空而跳过。");
        StringAssert.Contains("public string PersistentId", cropText);
        StringAssert.Contains("GeneratePersistentId();", cropText,
            "PersistentId 读取应能兜底生成 GUID，避免 continuity 捕获时直接被跳过。");
    }

    [Test]
    public void CropController_SaveAndLoad_ShouldUseCellCenterAndRestoreSeedData()
    {
        string cropText = File.ReadAllText(CropControllerPath);

        StringAssert.Contains("saveData.SetPosition(GetCellCenterPosition());", cropText,
            "作物离场快照必须保存格子中心，不应再把带视觉偏移的子节点位置写进 world-state。");
        StringAssert.DoesNotContain("saveData.SetPosition(transform.position);", cropText,
            "作物 world-state 不应再直接保存子节点 transform.position，否则回场时容易产生漂移。");
        StringAssert.Contains("seedData = ResolveSeedData(cropSaveData.seedId) ?? seedData;", cropText,
            "作物回场加载时必须从 seedId 恢复 seedData，否则下一次切场会被判成不可保存对象。");
        StringAssert.Contains("restorePosition = farmTileManager.GetCellCenterWorld(layerIndex, cellPosition);", cropText,
            "作物回场时应优先按耕地格心复位，而不是死信存档里的旧世界坐标。");
        StringAssert.Contains("instanceData.seedDataID = cropSaveData.seedId;", cropText,
            "作物 runtime 实例数据也应同步 seedId，避免恢复后数据壳与真实种子定义脱节。");
    }

    [Test]
    public void PersistentBridge_ShouldIncludeChestFarmAndCropInSceneSnapshots()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<FarmTileManager>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<CropController>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<ChestController>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<WorldItemPickup>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<TreeController>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<StoneController>(scene), snapshots, capturedGuids);", bridgeText);
        StringAssert.Contains("case \"FarmTileManager\":", bridgeText);
        StringAssert.Contains("case \"Crop\":", bridgeText);
        StringAssert.Contains("case \"Chest\":", bridgeText);
        StringAssert.Contains("case \"Drop\":", bridgeText);
        StringAssert.Contains("case \"Tree\":", bridgeText);
        StringAssert.Contains("case \"Stone\":", bridgeText);
    }

    [Test]
    public void PersistentBridge_ShouldTreatMissingSceneWorldObjectsAsAuthoritativeRemoval()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("sceneWorldSnapshotsByScene[sceneKey] = snapshots;", bridgeText,
            "离场快照即使为空也必须保留为权威状态，否则最后一棵树/最后一个掉落物被移除后仍会在回场时复活。");
        StringAssert.Contains("RemoveUnexpectedSceneWorldBindings(bindings, snapshotGuids);", bridgeText,
            "回场恢复必须清理所有不在快照里的可移除场景物，而不能只清掉落物。");
        StringAssert.Contains("private static bool IsSceneWorldRemovalAuthoritative(string objectType)", bridgeText,
            "可移除 world-state 类型应有明确白名单，避免误关全局管理器。");
        StringAssert.Contains("case \"Tree\":", bridgeText);
        StringAssert.Contains("case \"Stone\":", bridgeText);
        StringAssert.Contains("case \"Chest\":", bridgeText);
        StringAssert.Contains("case \"Drop\":", bridgeText);
        StringAssert.DoesNotContain("RemoveUnexpectedSceneDrops(bindings, snapshotGuids);", bridgeText,
            "旧逻辑只清理 Drop，会导致砍掉的树/清掉的石头跨场景复活。");
    }

    [Test]
    public void OffSceneSnapshots_ShouldRecordCaptureDayAndCatchUpTreesOnReturn()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);
        string treeText = File.ReadAllText(TreeControllerPath);
        string dtoText = File.ReadAllText(SaveDataDtosPath);

        StringAssert.Contains("public bool hasCapturedTotalDays;", dtoText,
            "off-scene scene 快照需要显式标记是否带离场天数，避免旧档缺字段时被误判成超长补票。");
        StringAssert.Contains("public int capturedTotalDays;", dtoText,
            "off-scene scene 快照必须记录离场那天，回场后才能知道该补几天树成长。");
        StringAssert.Contains("sceneWorldSnapshotCapturedTotalDaysByScene[sceneKey] = GetCurrentTotalDaysSafe();", bridgeText,
            "离场抓 world-state 时必须把 totalDays 一起抓住，否则树离场几天后回来仍会停在旧年龄。");
        StringAssert.Contains("hasCapturedTotalDays = hasCapturedTotalDays,", bridgeText,
            "正式存档导出 off-scene scene 时也必须把离场天数带出去，打包后读档才不会退化回旧问题。");
        StringAssert.Contains("ResolveSceneWorldSnapshotElapsedDays(sceneKey, out currentTotalDays);", bridgeText,
            "回场恢复前必须先算离场经过了几天，不能继续只把旧快照原样 Load 回来。");
        StringAssert.Contains("ApplySceneWorldCatchUp(binding.PersistentObject, elapsedDays, currentTotalDays);", bridgeText,
            "world-state 恢复后应给支持的对象补离场 catch-up。");
        StringAssert.Contains("treeController.ApplyOffSceneElapsedDays(elapsedDays, currentTotalDays);", bridgeText,
            "当前最小合同至少要先把树的离场天数补齐，不能只有当前所在场景的树会成长。");
        StringAssert.Contains("public void ApplyOffSceneElapsedDays(int elapsedDays, int targetTotalDays)", treeText,
            "TreeController 需要提供显式的 off-scene 补票入口，避免桥接层直接改它的内部状态。");
        StringAssert.Contains("ReconcileOffSceneSeasonAndWeatherState();", treeText,
            "树木离场补票不能只补成长阶段，回场后还需要和当前季节/天气状态重新对齐。");
    }

    [Test]
    public void PersistentBridge_ShouldFinalizeFarmAndCropCatchUpAfterWorldRestore()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);
        string farmText = File.ReadAllText(Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Farm/FarmTileManager.cs"));
        string cropText = File.ReadAllText(CropControllerPath);

        StringAssert.Contains("FinalizeDeferredSceneWorldCatchUp(scene, elapsedDays, currentTotalDays);", bridgeText,
            "world-state 全部恢复后，还需要补一轮农地/作物的离场跨天结算，不能继续只靠 Tree 的即时 catch-up。");
        StringAssert.Contains("cropController.ApplyOffSceneElapsedDays(elapsedDays, currentTotalDays);", bridgeText,
            "作物应先按离场天数补成长/缺水/过熟/换季，再让农地收自己的最终水状态。");
        StringAssert.Contains("farmTileManager.ApplyOffSceneElapsedDays(elapsedDays, currentTotalDays);", bridgeText,
            "农地应在作物恢复后再收空置寿命与浇水残留，避免把作物脚下耕地误判成空地。");
        StringAssert.Contains("public void ApplyOffSceneElapsedDays(int elapsedDays, int currentTotalDays)", farmText,
            "FarmTileManager 需要显式提供 off-scene catch-up 入口。");
        StringAssert.Contains("public void ApplyOffSceneElapsedDays(int elapsedDays, int targetTotalDays)", cropText,
            "CropController 也需要显式提供 off-scene catch-up 入口。");
    }

    [Test]
    public void PersistentBridge_ShouldCaptureInventoryAndHotbarFromResolvedRuntimeServices()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("InventoryService sceneInventory = ResolveRuntimeInventoryService(scene);", bridgeText,
            "离场快照抓背包时必须优先使用 bridge 已收口的 runtime inventory，不能再抓回场景副本。");
        StringAssert.Contains("HotbarSelectionService sceneHotbarSelection = ResolveRuntimeHotbarSelectionService(scene);", bridgeText,
            "离场快照抓 hotbar 时也必须走 bridge 真源，否则回场会把旧选中状态重新写回。");
    }

    [Test]
    public void ChestController_ShouldNotMoveRootTransformWhenAligningSpriteBottom()
    {
        string chestText = File.ReadAllText(ChestControllerPath);

        StringAssert.Contains("CaptureAuthoredVisualBaseline(_spriteRenderer);", chestText,
            "箱子启动时应先把场景 authored 的视觉 pose 记成基线，不能一进运行时就改写成新默认位置。");
        StringAssert.Contains("_authoredBaselineSprite = sourceRenderer.sprite;", chestText,
            "箱子必须记住 authored closed sprite，才能让 closed 态精确回到编辑器正式面。");
        StringAssert.Contains("bool sourceIsRootRenderer = sourceTransform == transform;", chestText,
            "箱子如果 authored sprite 就挂在根节点，运行时转成视觉子节点时必须先识别这个分支。");
        StringAssert.Contains("_authoredVisualLocalPosition = sourceIsRootRenderer ? Vector3.zero : sourceTransform.localPosition;", chestText,
            "根节点 sprite 迁到视觉子节点时，authoring 基线必须回到 root-local 零位，不能把场景摆位再偏移一遍。");
        StringAssert.Contains("_authoredVisualLocalRotation = sourceIsRootRenderer ? Quaternion.identity : sourceTransform.localRotation;", chestText);
        StringAssert.Contains("_authoredVisualLocalScale = sourceIsRootRenderer ? Vector3.one : sourceTransform.localScale;", chestText);
        StringAssert.Contains("private void ApplyAuthoredVisualPoseToRenderer(SpriteRenderer targetRenderer)", chestText,
            "箱子运行态改用视觉子节点后，仍必须按 authored pose 回放，而不是让子节点从零位重新起算。");
        StringAssert.Contains("targetTransform.localRotation = _authoredVisualLocalRotation;", chestText,
            "运行时视觉子节点必须继承 authored 旋转。");
        StringAssert.Contains("targetTransform.localScale = _authoredVisualLocalScale;", chestText,
            "运行时视觉子节点必须继承 authored 缩放。");
        StringAssert.Contains("targetRenderer.sprite != _authoredBaselineSprite", chestText,
            "只有非 authored baseline sprite 才应该参与底边补偿；closed 态应直接回到 authored pose。");
        StringAssert.Contains("localPos.y = _authoredBottomLocalY - spriteBottomOffset;", chestText,
            "开关状态切换应保持 authored 底边基线一致，而不是把每个 sprite 都重新贴到 root 原点。");
        StringAssert.DoesNotContain("localPos.y = -spriteBottomOffset;", chestText,
            "旧的裸贴底公式会直接覆盖场景摆位，必须彻底移除。");
        StringAssert.Contains("ComputeSpriteBottomLocalOffset(", chestText,
            "贴底补偿应显式复用同一套 sprite-bottom 计算，避免视觉和碰撞再分叉。");
        StringAssert.Contains("ApplyAuthoredVisualPoseToRenderer(visualRenderer);", chestText,
            "把 root SpriteRenderer 迁到视觉子节点时，必须立即复原 authored 视觉位置，不能让箱子先跳一次。");
        StringAssert.Contains("visualRenderer.transform.SetSiblingIndex(rootRenderer.transform.GetSiblingIndex());", chestText,
            "运行时视觉子节点还应继承 authored sibling 顺序，避免渲染层级和编辑器正式面脱节。");
        StringAssert.Contains("Transform visualTransform = _spriteRenderer.transform;", chestText);
        StringAssert.Contains("if (visualTransform == transform)", chestText,
            "箱子贴底逻辑必须先识别 SpriteRenderer 是否挂在根节点，避免继续污染真实世界坐标。");
        StringAssert.DoesNotContain("transform.localPosition = localPos;", chestText,
            "箱子贴底逻辑不应再直接改根节点位置，否则切场后会越漂越高。");
        StringAssert.Contains("visualTransform.localPosition = localPos;", chestText,
            "只有视觉子节点才允许做贴底偏移。");
        StringAssert.Contains("EnsureSpriteRendererUsesVisualChild();", chestText,
            "根节点 SpriteRenderer 无法在不移动真实坐标的情况下底部对齐，应运行时转成视觉子节点。");
        StringAssert.Contains("physicsShape[pointIndex] = TransformSpritePhysicsPointToChestLocal(physicsShape[pointIndex]);", chestText,
            "Collider 也必须吃完整的视觉子节点局部变换，不能只做平移补偿。");
        StringAssert.Contains("private Vector2 TransformSpritePhysicsPointToChestLocal(Vector2 spriteLocalPoint)", chestText,
            "箱子碰撞恢复应显式把 sprite-local 点转换回箱子 root local，避免运行后碰撞体跑飞。");
        StringAssert.DoesNotContain("physicsShape[pointIndex] += colliderOffset;", chestText,
            "旧的纯偏移补偿不够覆盖 authored rotation/scale 场景，必须移除。");
    }

    [Test]
    public void SaveManager_LoadAndRestart_ShouldRefreshInventoryToolbarAndSelection()
    {
        string saveManagerText = File.ReadAllText(SaveManagerPath);
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("FindObjectsByType<InventoryService>", saveManagerText,
            "读档/重新开始后应刷新所有 runtime 背包服务，而不是只找第一个可见 UI。");
        StringAssert.Contains("PersistentPlayerSceneBridge.RefreshActiveSceneRuntimeBindings();", saveManagerText,
            "读档/重新开始在真数据落稳后，还应再补一次 authoritative bridge rebind，不能停在旧 snapshot 先绑、真态后写。");
        StringAssert.Contains("PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot();", saveManagerText,
            "读档/重新开始在补 bridge rebind 前，必须先把当前 runtime 背包与选中态同步进 bridge snapshot，避免后续切场又把旧背包打回来。");
        StringAssert.Contains("inventories[index]?.RefreshAll();", saveManagerText,
            "背包数据恢复后必须主动发刷新事件，确保工作台、toolbar、背包面板事实源同步。");
        StringAssert.Contains("ReassertCurrentSelection(collapseInventorySelectionToHotbar: false, invokeEvent: true)", saveManagerText,
            "toolbar 选中状态在读档/重新开始后必须重新装备并广播，避免图标有但工具不可用。");
        StringAssert.Contains("inventoryPanel.EnsureBuilt();", saveManagerText,
            "背包面板在刷新前必须确保重绑到当前 runtime inventory。");
        StringAssert.Contains("toolbar.Build();", saveManagerText,
            "toolbar 在刷新前必须重新绑定槽位，避免仍读旧 InventoryService。");
        StringAssert.Contains("Canvas.ForceUpdateCanvases();", saveManagerText,
            "恢复后应强制 UI 布局刷新一帧，减少读档后显示和真实数据不同步。");
        StringAssert.Contains("public static void RefreshActiveSceneRuntimeBindings()", bridgeText,
            "bridge 应显式提供一条“当前 active scene 再做一次 authoritative rebind”的入口，供 load/restart 收尾使用。");
        StringAssert.Contains("RebindPersistentCoreUi(scene, sceneInventory, sceneHotbarSelection);", bridgeText);
        StringAssert.Contains("RebindSceneInput(scene, sceneInventory, sceneHotbarSelection);", bridgeText);
    }

    [Test]
    public void SaveManager_ShouldPromoteLegacyFarmTilesIntoCurrentCropWorldObjects()
    {
        string saveManagerText = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("NormalizeLoadedSaveData(saveData);", saveManagerText,
            "旧档兼容应在读档入内存的第一步统一归一化，而不是把 legacy 漏洞留给运行时多处猜。");
        StringAssert.Contains("PromoteLegacyFarmStateForLoad(", saveManagerText);
        StringAssert.Contains("PromoteLegacyRootFarmTilesIntoWorldObjects(", saveManagerText,
            "旧根字段 farmTiles 仍存在时，应先升格成 FarmTileManager world object。");
        StringAssert.Contains("BuildLegacyCropWorldObject(", saveManagerText,
            "旧农田里的 cropId/cropGrowthStage 也应补升成新的 Crop world object。");
        StringAssert.Contains("guid = $\"LegacyCrop_{tile.layer}_{tile.tileX}_{tile.tileY}\"", saveManagerText);
        StringAssert.Contains("objectType = \"Crop\"", saveManagerText);
        StringAssert.Contains("genericData = JsonUtility.ToJson(cropData)", saveManagerText);
    }

    [Test]
    public void WorldStateRunner_ShouldDispatchScenarioChainAfterBootstrap()
    {
        string runnerText = File.ReadAllText(WorldStateRunnerPath);

        StringAssert.Contains("TouchProgress(\"scenario_chain_dispatch\"", runnerText,
            "runner 过完 bootstrap 后应显式派发后续场景链，而不是停在 bootstrap_completed 后无声失联。");
        StringAssert.Contains("runCoroutine = StartCoroutine(RunScenarioSequence());", runnerText,
            "world-state live runner 应把后续场景链拆成独立 coroutine，避免同一条外层链在 bootstrap 后失联。");
        StringAssert.Contains("private IEnumerator RunScenarioSequence()", runnerText);
        StringAssert.Contains("private IEnumerator RunNestedEnumerator(IEnumerator routine)", runnerText,
            "runner 进入场景 case 前应手动推进内层 IEnumerator，并把异常直接落成 report。");
        StringAssert.Contains("Stack<IEnumerator> stack = new Stack<IEnumerator>();", runnerText,
            "runner 应递归手动推进多层 IEnumerator，不能把 WaitForScene 这类更深层协程再丢回 Unity 后无声失联。");
        StringAssert.Contains("TouchProgress(\"scenario_chain_started\"", runnerText);
        StringAssert.Contains("yield return RunNestedEnumerator(RunPrimaryHomePrimaryScenario());", runnerText);
        StringAssert.Contains("yield return RunNestedEnumerator(RunPrimaryTownPrimaryScenario());", runnerText);
        StringAssert.Contains("yield return RunNestedEnumerator(RunHomePrimaryHomeChestScenario());", runnerText);
        StringAssert.Contains("private void Update()", runnerText,
            "跨场景 live runner 还应有 Update 级 watchdog 兜底，不能只靠 coroutine 看门狗。");
        StringAssert.Contains("TriggerWatchdogTimeout();", runnerText);
        StringAssert.Contains("yield return null;", runnerText,
            "watchdog 和场景链至少要保留逐帧推进能力，避免只靠固定 WaitForSecondsRealtime 卡住。");
    }

    [Test]
    public void DirectSceneLoads_ShouldQueueBridgeBeforeSwitchingScenes()
    {
        string directorText = File.ReadAllText(SpringDay1DirectorPath);
        string doorTriggerText = File.ReadAllText(DoorTriggerPath);

        StringAssert.Contains("LoadSceneThroughPersistentBridge(TownSceneName);", directorText,
            "Day1 晚饭切 Town 前必须先走 bridge，不能再直切场景绕过 world-state 捕获。");
        StringAssert.Contains("LoadSceneThroughPersistentBridge(HomeSceneName);", directorText,
            "Day1 回家/强制睡眠前也必须先走 bridge，避免 Primary 的树和农地离场态直接丢失。");
        StringAssert.Contains("PersistentPlayerSceneBridge.QueueSceneEntry(sceneName, string.Empty, string.Empty);", directorText,
            "SpringDay1Director 的直载帮助方法必须先把当前场景 runtime state 交给 bridge。");
        StringAssert.Contains("PersistentPlayerSceneBridge.QueueSceneEntry(targetSceneName, string.Empty, string.Empty);", doorTriggerText,
            "老 DoorTrigger 也必须在 LoadScene 前先把离场快照交给 bridge，否则 Home/Primary 往返仍会回初始。");
    }

    [Test]
    public void RuntimeInventoryConsumers_ShouldPreferBridgeAuthoritativeServices()
    {
        string placementText = File.ReadAllText(PlacementManagerPath);
        string hotbarSelectionText = File.ReadAllText(HotbarSelectionServicePath);
        string autoPickupText = File.ReadAllText(AutoPickupServicePath);
        string inventorySortText = File.ReadAllText(InventorySortServicePath);
        string craftingText = File.ReadAllText(CraftingServicePath);
        string playerInteractionText = File.ReadAllText(PlayerInteractionPath);

        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()", placementText,
            "放置系统切场后必须优先绑定 bridge 背包真源，否则图标正常但播种/放置仍会跟旧背包打架。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()", placementText,
            "放置系统的 hotbar 选择也必须优先走 bridge 真源。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimePackageTabs()", placementText,
            "放置系统刷新包裹 UI 时不能再盲抓场景副本。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()", hotbarSelectionText,
            "HotbarSelectionService 自己补引用时也必须优先桥接到当前 authoritative inventory。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()", autoPickupText,
            "自动拾取要始终朝当前真实背包加东西，不能再把物品塞进场景副本。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()", inventorySortText,
            "背包整理必须整理当前真实背包，不然 Home/Town 的 Sort 还会像断联。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimePackageTabs()", craftingText,
            "工作台刷新材料时应优先复用当前 package runtime context。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()", craftingText,
            "工作台兜底到背包时也必须优先走 bridge 真源。");
        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()", playerInteractionText,
            "动作完成后重放 hotbar 选择时，PlayerInteraction 也必须优先对准 bridge 真源。");
    }

    [Test]
    public void DropContinuity_ShouldResetPooledIdentityAndPreserveSourceContracts()
    {
        string pickupText = File.ReadAllText(WorldItemPickupPath);
        string poolText = File.ReadAllText(WorldItemPoolPath);
        string dropHelperText = File.ReadAllText(ItemDropHelperPath);
        string treeText = File.ReadAllText(TreeControllerPath);

        StringAssert.Contains("public void PrepareForSpawn()", pickupText,
            "对象池复用掉落物时必须显式刷新逻辑身份，不能继续沿用旧 GUID。");
        StringAssert.Contains("UnregisterFromPersistentRegistry();", pickupText,
            "掉落物回池或改 GUID 前必须先从持久化注册表注销，避免旧身份继续挂着。");
        StringAssert.Contains("sourceNodeGuid = null;", pickupText,
            "掉落物回池时必须清空来源节点 GUID，避免玩家手动丢弃的物品继承旧资源节点来源。");
        StringAssert.Contains("persistentId = null;", pickupText,
            "掉落物回池后也必须清空旧 GUID，避免下一次复用直接撞上旧 world-state。");
        StringAssert.Contains("RegisterToPersistentRegistry();", pickupText,
            "掉落物从池里重新生成或读档复原后，必须重新注册到持久化注册表。");

        StringAssert.Contains("PrepareSpawnedItem(item, position);", poolText,
            "对象池重新生成掉落物时，应先把实例从池根切回当前场景，再设置世界位置。");
        StringAssert.Contains("item.PrepareForSpawn();", poolText,
            "对象池复用前必须刷新掉落物逻辑身份，不能让不同物品共用同一个旧 GUID。");
        StringAssert.Contains("SceneManager.MoveGameObjectToScene(item.gameObject, activeScene);", poolText,
            "活跃掉落物不能继续挂在持久对象池场景里，否则切场时会被错误地一起带走。");

        StringAssert.Contains("pickup.SetSourceNodeGuid(null);", dropHelperText,
            "玩家手动丢弃出来的物品必须显式清空来源节点语义，避免被重建链误认成资源节点掉落。");
        StringAssert.Contains("pickup.SetSourceNodeGuid(PersistentId);", treeText,
            "树木生成掉落物时也必须写入来源 GUID，这样回场/读档时才能正确做来源去重。");
    }

    [Test]
    public void OcclusionManagers_ShouldRebindToPersistentPlayerAfterSceneSwitch()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);
        string occlusionText = File.ReadAllText(OcclusionManagerPath);

        StringAssert.Contains("RebindSceneOcclusionManagers(scene);", bridgeText,
            "切场重绑时应显式刷新场景 OcclusionManager 的玩家引用，不能再让 Primary 吃旧玩家壳。");
        StringAssert.Contains("FindComponentsInScene<OcclusionManager>(scene)", bridgeText,
            "bridge 应遍历当前场景的 OcclusionManager 做统一重绑，而不是指望某个场景刚好序列化正确。");
        StringAssert.Contains("manager.RefreshRuntimePlayerBinding(persistentPlayerTransform);", bridgeText,
            "OcclusionManager 的玩家真源应跟 persistent player 一起切，不应停留在被桥接销毁的 scene player 上。");
        StringAssert.Contains("public void RefreshRuntimePlayerBinding(Transform runtimePlayer)", occlusionText,
            "OcclusionManager 需要一个明确的运行时重绑入口，供 bridge 在切场时统一刷新。");
        StringAssert.Contains("RefreshPlayerBindings(forceSearch: true);", occlusionText,
            "即使桥接没主动推送，OcclusionManager 自己也应能在运行时发现旧玩家引用失效后自愈。");
        StringAssert.Contains("RefreshRegisteredOccludersFromScene();", occlusionText,
            "Town 首次启动不能只赌 OcclusionTransparency 自己注册成功，OcclusionManager 也应主动补扫当前 scene 的遮挡物。");
    }

    [Test]
    public void StartupFallback_ShouldRunOneMoreRuntimeRefresh_ForFirstTownBootstrap()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("yield return new WaitForEndOfFrame();", bridgeText,
            "首屏 fallback 不能只等一帧；Town 首次启动仍需要等到这一帧后半段再补一次 runtime refresh。");
        StringAssert.Contains("RefreshSceneRuntimeBindingsInternal(activeScene);", bridgeText,
            "首屏 fallback 结束前应再补一轮轻量 runtime refresh，避免第一次进 Town 遮挡/输入/工具链没绑稳。");
    }

    [Test]
    public void NativeFreshRestart_ShouldSuppressTargetSceneWorldRestore_BeforeLoadingTown()
    {
        string saveText = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(NativeFreshSceneName);", saveText,
            "重新开始载入 Town 前必须先抑制 target scene 的旧 world snapshot，不能让 Town 先吃一口旧 continuity。");
        StringAssert.Contains("PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(NativeFreshSceneName);", saveText,
            "若 Town 载入失败，重开入口也必须把 suppress 状态取消掉，不能把后续 scene restore 一起挂死。");
    }

    [Test]
    public void WorldStateRunner_ShouldFallbackToHomeChestWhenPrimaryChestIsUnavailable()
    {
        string runnerText = File.ReadAllText(WorldStateRunnerPath);

        StringAssert.Contains("RunHomePrimaryHomeChestScenario()", runnerText,
            "world-state live runner 需要覆盖 Home 箱子往返，而不是继续被 Primary 箱子缺失卡死。");
        StringAssert.Contains("primary_chest_optional_bootstrap_skip", runnerText,
            "Primary 没有可用箱子时，runner 应退化成可继续验证农地与 Home 箱子的形态，而不是 bootstrap 直接失败。");
        StringAssert.Contains("home-primary-home-chest", runnerText);
        StringAssert.Contains("sceneName.ToLowerInvariant()", runnerText,
            "场景箱子缺失原因应按目标场景动态生成，至少能区分 Home / Primary 等不同场景。");
        StringAssert.Contains("WatchdogRun()", runnerText,
            "world-state live runner 至少要有独立看门狗，不能再出现切到 Home 后永远不写 report 的悬空状态。");
        StringAssert.Contains("watchdog_timeout", runnerText,
            "runner 就算卡住，也必须把失败原因写进 report，而不是一直无结果。");
    }
}
