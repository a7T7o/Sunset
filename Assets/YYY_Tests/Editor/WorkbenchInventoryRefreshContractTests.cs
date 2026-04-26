using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class WorkbenchInventoryRefreshContractTests
{
    private const string AssemblyCSharp = "Assembly-CSharp";
    private static readonly string ProjectRoot =
        Directory.GetParent(Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();

    private static readonly string WorkbenchOverlayPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs");

    private static readonly string UiLayerUtilityPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs");

    private static readonly string CraftingServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Crafting/CraftingService.cs");

    private static readonly string CraftingStationInteractablePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs");

    private static readonly string PersistentBridgePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs");

    [Test]
    public void PlayerInventoryData_SetLikeOperations_ShouldRaiseInventoryChanged()
    {
        Type playerInventoryDataType = RequireType("FarmGame.Data.Core.PlayerInventoryData");
        Type inventoryItemType = RequireType("FarmGame.Data.Core.InventoryItem");
        Type itemStackType = RequireType("ItemStack");

        object data = Activator.CreateInstance(playerInventoryDataType, new object[] { 36, null });
        int inventoryChangedCount = 0;
        int slotChangedCount = 0;

        playerInventoryDataType.GetEvent("OnInventoryChanged")
            .AddEventHandler(data, (Action)(() => inventoryChangedCount++));
        playerInventoryDataType.GetEvent("OnSlotChanged")
            .AddEventHandler(data, (Action<int>)(_ => slotChangedCount++));

        object runtimeItem = Activator.CreateInstance(inventoryItemType, new object[] { 1001, 0, 2 });
        object runtimeStack = Activator.CreateInstance(itemStackType, new object[] { 1002, 0, 1 });

        Assert.That(playerInventoryDataType.GetMethod("SetItem").Invoke(data, new[] { (object)0, runtimeItem }), Is.EqualTo(true));
        playerInventoryDataType.GetMethod("ClearItem").Invoke(data, new object[] { 0 });
        Assert.That(playerInventoryDataType.GetMethod("SetSlot").Invoke(data, new[] { (object)1, runtimeStack }), Is.EqualTo(true));

        Assert.That(slotChangedCount, Is.EqualTo(3), "SetItem / ClearItem / SetSlot 都应继续发出槽位变化事件。");
        Assert.That(inventoryChangedCount, Is.EqualTo(3), "箱子拖拽这类通过 SetItem / SetSlot 的路径也必须发出 InventoryChanged，工作台才能立即刷新材料状态。");
    }

    [Test]
    public void WorkbenchOverlay_ShouldSubscribeToSlotAndInventoryChanged()
    {
        string overlayText = File.ReadAllText(WorkbenchOverlayPath);

        StringAssert.Contains("_inventoryService.OnSlotChanged += HandleInventorySlotChanged;", overlayText,
            "工作台浮层应订阅背包槽位变化，避免只靠 InventoryChanged 才刷新。");
        StringAssert.Contains("_inventoryService.OnSlotChanged -= HandleInventorySlotChanged;", overlayText,
            "工作台浮层取消绑定时也应解除 OnSlotChanged，避免重复订阅。");
        StringAssert.Contains("private void HandleInventorySlotChanged(int _)", overlayText,
            "工作台浮层应保留槽位变化兜底处理方法，确保箱子拖进背包后材料状态即时刷新。");
    }

    [Test]
    public void CraftingService_ShouldExposeRuntimeInventoryRebind()
    {
        Type craftingServiceType = RequireType("CraftingService");

        Assert.That(craftingServiceType.GetMethod("ConfigureRuntimeContext"), Is.Not.Null,
            "CraftingService 必须提供运行时背包重绑入口，避免切场后工作台继续读取旧 InventoryService。");
        Assert.That(craftingServiceType.GetMethod("RefreshRuntimeContextFromScene"), Is.Not.Null,
            "工作台打开前需要能主动刷新当前场景/Package 的背包事实源。");

        string serviceText = File.ReadAllText(CraftingServicePath);
        StringAssert.Contains("FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include)", serviceText,
            "CraftingService 兜底查找背包时也应包含 inactive persistent UI / runtime 服务。");
    }

    [Test]
    public void WorkbenchOpen_ShouldRebindCraftingServiceBeforeCountingMaterials()
    {
        string overlayText = File.ReadAllText(WorkbenchOverlayPath);
        string interactableText = File.ReadAllText(CraftingStationInteractablePath);
        string bridgeText = File.ReadAllText(PersistentBridgePath);

        StringAssert.Contains("_craftingService.RefreshRuntimeContextFromScene();", overlayText,
            "工作台浮层打开时应先刷新 CraftingService 的背包事实源，再绑定事件和统计材料。");
        StringAssert.Contains("craftingService.RefreshRuntimeContextFromScene();", interactableText,
            "工作台交互入口拿到 CraftingService 后应立刻刷新运行时背包引用。");
        StringAssert.Contains("craftingService.ConfigureRuntimeContext(sceneInventory, database);", bridgeText,
            "切场重绑 persistent UI 时也应同步重绑 CraftingService，避免工作台读旧背包。");
    }

    [Test]
    public void CraftingService_ShouldSplitMaterialReservationFromCraftCompletion()
    {
        Type craftingServiceType = RequireType("CraftingService");

        Assert.That(craftingServiceType.GetMethod("TryReserveMaterials"), Is.Not.Null,
            "工作台点击即交料后，CraftingService 必须提供独立的预扣入口，不能继续借用 TryCraft 成功语义。");
        Assert.That(craftingServiceType.GetMethod("NotifyCraftCompleted"), Is.Not.Null,
            "工作台预扣模式下，真正完成一件时仍需要单独通知 success，避免 Day1 craftedCount 提前累计。");

        string serviceText = File.ReadAllText(CraftingServicePath);
        StringAssert.Contains("public CraftResult TryReserveMaterials(RecipeData recipe, int craftCount)", serviceText,
            "CraftingService 应显式暴露批量预扣材料入口。");
        StringAssert.Contains("public void NotifyCraftCompleted(RecipeData recipe)", serviceText,
            "CraftingService 应显式暴露“完成一件再记 success”的入口。");
    }

    [Test]
    public void WorkbenchOverlay_ShouldReserveOnClick_RefundOnCancel_AndAvoidDoubleCapacityPenalty()
    {
        string overlayText = File.ReadAllText(WorkbenchOverlayPath);

        StringAssert.Contains("_craftingService.TryReserveMaterials(recipe, requestedQuantity)", overlayText,
            "工作台点击开始制作 / 追加制作时应先预扣材料，再把数量写入队列。");
        StringAssert.Contains("_craftingService.NotifyCraftCompleted(recipe);", overlayText,
            "工作台完成一件时应单独通知 success，而不是在点击时提前记成功。");
        StringAssert.Contains("RefundPendingReservedMaterials(canceledEntry)", overlayText,
            "中断当前制作时应退回该 recipe 所有未完成件数的材料。");
        StringAssert.Contains("RefundPendingReservedMaterials(entry)", overlayText,
            "取消排队时也应退回该条 recipe 未完成件数的材料。");
        StringAssert.Contains("return maxCraftable;", overlayText,
            "改成点击即预扣后，数量上限不应继续减 queuedAlready，否则会双重扣减。");
    }

    [Test]
    public void WorkbenchOverlay_ShouldRefreshBoundSceneRuntimeContextOnSceneReturn()
    {
        string overlayText = File.ReadAllText(WorkbenchOverlayPath);

        StringAssert.Contains("private void RefreshBoundSceneRuntimeState()", overlayText,
            "工作台跨场景后回到绑定场景时，应补一次运行时状态刷新。");
        StringAssert.Contains("RefreshBoundSceneRuntimeState();", overlayText,
            "sceneLoaded / activeSceneChanged 回到绑定场景时应触发工作台运行时刷新。");
    }

    [Test]
    public void WorkbenchOverlay_ShouldPersistRuntimeStateAndPreferPersistentUiRoot()
    {
        string overlayText = File.ReadAllText(WorkbenchOverlayPath);
        string uiLayerText = File.ReadAllText(UiLayerUtilityPath);

        StringAssert.Contains("public static void FlushRuntimeStateToPersistence()", overlayText,
            "工作台浮层需要暴露统一 flush 入口，让正式存档前能主动把 runtime 状态写回长期持久层。");
        StringAssert.Contains("public static void NotifyPersistentStateReplaced()", overlayText,
            "正式读档替换长期态后，工作台浮层需要一个统一入口把本地 runtime 壳同步成新真值。");
        StringAssert.Contains("StoryProgressPersistenceService.StoreWorkbenchRuntimeState(BuildWorkbenchRuntimeSaveData(stationKey));", overlayText,
            "工作台 runtime 队列应该正式写入长期持久层，而不是继续只活在浮层内存里。");
        StringAssert.Contains("SuspendCraftRoutineForPersistence();", overlayText,
            "工作台离场/禁用前应先挂起当前协程，再把进度写回持久层。");
        StringAssert.Contains("CleanupTransientState(resetSession: false);", overlayText,
            "工作台禁用时不应再直接把 session 真值清空，否则切场回来会丢队列。");
        Assert.That(overlayText, Does.Not.Contain("StopCraftRoutine();\r\n            CleanupTransientState(resetSession: true);"),
            "工作台 OnDisable 不能再走“直接停工并清空 session”的旧链。");

        StringAssert.Contains("PersistentPlayerSceneBridge.GetPreferredRuntimeUiRoot()", uiLayerText,
            "工作台 overlay 的 runtime UI 父节点应优先走 persistent UI 根，避免切场后挂回 scene-local UI。");
    }

    private static Type RequireType(string fullName)
    {
        Type type = Type.GetType(fullName + ", " + AssemblyCSharp);
        Assert.That(type, Is.Not.Null, "必须能从 Assembly-CSharp 解析到类型: " + fullName);
        return type;
    }
}
