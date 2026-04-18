using System.IO;
using NUnit.Framework;

[TestFixture]
public class InventoryRuntimeSelectionContractTests
{
    private static readonly string ProjectRoot =
        Directory.GetParent(UnityEngine.Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();

    private static readonly string HotbarSelectionServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs");

    private static readonly string InventoryPanelPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs");

    private static readonly string InventorySlotUiPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs");

    private static readonly string BoxPanelPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs");

    private static readonly string PackagePanelTabsPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs");

    private static readonly string InventorySortServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs");

    private static readonly string InventoryServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Inventory/InventoryService.cs");

    private static readonly string PersistentBridgePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs");

    private static readonly string GameInputManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Controller/Input/GameInputManager.cs");

    private static readonly string ToolbarSlotUiPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs");

    private static readonly string ToolbarUiPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs");

    private static readonly string PlayerInteractionPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs");

    private static readonly string InventoryInteractionManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs");

    private static readonly string InventorySlotInteractionPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs");

    [Test]
    public void BackpackSelection_ShouldStayAsPanelState_WhenNotSyncingHotbar()
    {
        string hotbarText = File.ReadAllText(HotbarSelectionServicePath);
        string inventoryPanelText = File.ReadAllText(InventoryPanelPath);
        string boxPanelText = File.ReadAllText(BoxPanelPath);

        StringAssert.Contains("public void SetPanelSelectionIndex(int index)", hotbarText,
            "HotbarSelectionService 应提供仅同步面板选中、不触发真实手持切换的入口。");

        StringAssert.Contains("if (syncHotbarSelection && slotIndex < InventoryService.HotbarWidth)", inventoryPanelText,
            "背包面板只有在 hotbar 区点击时，才应把选择同步进真实手持。");
        StringAssert.Contains("selection.SetPanelSelectionIndex(slotIndex);", inventoryPanelText,
            "点击非 hotbar 背包格时，应只更新面板选中态，不要偷偷改真实手持。");

        StringAssert.Contains("if (syncHotbarSelection && slotIndex < InventoryService.HotbarWidth)", boxPanelText,
            "箱子页 Down 区点击也必须遵守同一套 hotbar 真语义。");
        StringAssert.Contains("_hotbarSelection.SetPanelSelectionIndex(slotIndex);", boxPanelText,
            "箱子页背包格点击应只更新面板选中态，不要直接切世界手持。");
    }

    [Test]
    public void InventorySlotUi_ShouldNotMixBoxAndInventorySelectionsWithOr()
    {
        string slotText = File.ReadAllText(InventorySlotUiPath);

        StringAssert.Contains("bool slotOwnedByBoxPanel = boxPanel != null && boxPanel.gameObject;", slotText);
        StringAssert.Contains("bool slotOwnedByInventoryPanel = inventoryPanel != null && inventoryPanel.gameObject;", slotText);
        StringAssert.Contains("if (slotOwnedByBoxPanel)", slotText);
        StringAssert.Contains("else if (slotOwnedByInventoryPanel && resolvedInventoryPanel != null)", slotText);
        StringAssert.DoesNotContain("resolvedBoxPanel.IsInventorySlotSelected(index) ||", slotText,
            "箱子页与背包页的选中态不应再做 OR 混合，否则会制造假高亮。");
    }

    [Test]
    public void InventorySort_ShouldUseWholeBackpackAndRuntimeAuthoritativeContext()
    {
        string sortText = File.ReadAllText(InventorySortServicePath);
        string inventoryServiceText = File.ReadAllText(InventoryServicePath);

        StringAssert.Contains("private const int SortStartIndex = 0;", sortText,
            "玩家背包整理现在应按整包 36 格统一语义，而不是只整理 hotbar 下面几行。");
        StringAssert.Contains("InventoryService preferredRuntimeInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService();", sortText,
            "Sort 前应优先重绑到 bridge 当前背包真源，避免跨场景后还整理旧 InventoryService。");
        StringAssert.Contains("for (int i = SortStartIndex; i < inventory.Capacity; i++)", sortText);
        StringAssert.Contains("int slotIndex = SortStartIndex;", sortText);
        StringAssert.Contains("while (slotIndex < inventory.Capacity)", sortText);
        StringAssert.Contains("database = inventory.Database;", sortText,
            "整理时数据库应跟随当前 runtime inventory，一起避免跨场景后查旧数据库。");

        StringAssert.Contains("List<InventoryItem> runtimeItems = new List<InventoryItem>();", inventoryServiceText);
        StringAssert.Contains("for (int index = 0; index < inventorySize; index++)", inventoryServiceText,
            "InventoryService.Sort() fallback 也必须改成整包语义，不能再和 InventorySortService 打架。");
        StringAssert.Contains("MergeRuntimeItemsForSort", inventoryServiceText);
        StringAssert.Contains("SetInventoryItem(slotIndex++, runtimeItem);", inventoryServiceText,
            "fallback sort 也必须逐槽回写 runtime item，确保 hotbar/toolbar 真源一起刷新。");
    }

    [Test]
    public void InventoryTrash_ShouldDiscardHeldItemInsteadOfDroppingAtPlayerFeet()
    {
        string interactionManagerText = File.ReadAllText(InventoryInteractionManagerPath);
        string slotInteractionText = File.ReadAllText(InventorySlotInteractionPath);

        StringAssert.Contains("private void DiscardHeldItem()", interactionManagerText,
            "背包垃圾桶需要独立的删除语义，不能继续复用地面掉落逻辑。");
        StringAssert.Contains("DiscardHeldItem();", interactionManagerText,
            "管理器里的垃圾桶点击、拖到垃圾桶、held 点击垃圾桶都应走真正删除。");

        StringAssert.Contains("private void DiscardItemFromContext()", slotInteractionText,
            "SlotDragContext 拖到垃圾桶时也应是真删除，而不是继续掉地。");
        StringAssert.Contains("DiscardItemFromContext();", slotInteractionText);
    }

    [Test]
    public void SceneRebind_ShouldRefreshInputAndSortRuntimeContext()
    {
        string bridgeText = File.ReadAllText(PersistentBridgePath);
        string inventoryPanelText = File.ReadAllText(InventoryPanelPath);

        StringAssert.Contains("sceneInputManager.ResetPlacementRuntimeState(\"PersistentPlayerSceneBridge 场景重绑后重置输入运行态\");", bridgeText,
            "场景重绑后应再次清空输入运行态，避免剧情/Home 往返后残留旧的 queue/held/preview 状态。");
        StringAssert.Contains("sortService.RebindRuntimeContext(sceneInventory, database);", bridgeText,
            "场景重绑后应显式刷新 InventorySortService 的运行时上下文，避免整理背包仍拿旧引用。");
        StringAssert.Contains("RestoreSceneInventoryState(sceneInventory);", bridgeText,
            "统一 runtime rebind 入口也必须把最新 bridge snapshot 再打回当前场景背包，不能只重绑 UI 不收回真数据。");
        StringAssert.Contains("RebindHotbarSelection(sceneHotbarSelection, sceneInventory);", bridgeText,
            "统一 runtime rebind 入口也必须一起恢复 hotbar/背包真实选中态，避免 4/8 与交互链继续飘。");
        StringAssert.Contains("panel.ConfigureRuntimeContext(", bridgeText,
            "统一切场重绑时，背包页也必须走正式的 runtime context 入口，而不是只靠字段硬塞。");
        StringAssert.Contains("interactionManager.ConfigureRuntimeContext(", bridgeText,
            "统一切场重绑时，背包交互管理器也必须同步到当前真背包，否则显示刷新了、点击链仍可能指向旧对象。");
        StringAssert.Contains("panel.RefreshAll();", bridgeText,
            "场景重绑后应强制刷新背包页显示，避免引用已换但 UI 还停在旧状态。");
        StringAssert.Contains("InvalidateSlotBindings();", inventoryPanelText,
            "背包页只要重新配置 runtime context，就必须强制失效旧槽位绑定，不能再因为引用对象刚好没换而跳过逐格重绑。");
        StringAssert.Contains("EnsureBuilt();", inventoryPanelText,
            "背包页 RefreshAll 前也必须先补一轮 EnsureBuilt，避免切场后继续拿旧槽位绑定刷新。");
        StringAssert.Contains("sceneInventory.RefreshAll();", bridgeText,
            "切场重绑后应补发真实背包刷新事件，不能只改字段不通知订阅者。");
        StringAssert.Contains("sceneHotbarSelection.ReassertCurrentSelection(", bridgeText,
            "切场重绑后应重新广播当前真选中态，不能让 Home 能恢复、Town/Primary 往返却靠运气。");
        StringAssert.Contains("activeBoxPanel.RefreshUI();", bridgeText,
            "如果箱子 UI 仍开着，切场统一强刷新也要把它一并拉齐。");
        StringAssert.Contains("SetInputEnabled(true);", File.ReadAllText(GameInputManagerPath),
            "切场/重开硬复位必须解除旧输入门控，否则会出现画面恢复了但工具和切栏仍被卡死。");
        StringAssert.Contains("interactionManager?.Cancel();", File.ReadAllText(GameInputManagerPath),
            "场景重绑硬复位应一并清掉旧的背包拖拽/held 状态，避免箱子下半区像断联。");
        StringAssert.Contains("InventorySlotInteraction.ResetActiveChestHeldState();", File.ReadAllText(GameInputManagerPath),
            "切场硬复位后也要清掉 chest-held 残留，不能只在读档时处理。");
        StringAssert.Contains("lockManager?.ForceUnlock();", File.ReadAllText(GameInputManagerPath),
            "切场硬复位必须强解工具动作锁，否则旧锁态会跨场景残留。");
        StringAssert.Contains("if (preferredInventory != null) inventory = preferredInventory;", inventoryPanelText,
            "背包页重新打开时也应主动吃当前 runtime inventory，而不是只在字段为 null 时才补。");
        StringAssert.DoesNotContain("if (WasSceneAlreadyRebound(scene))", bridgeText,
            "sceneLoaded 的正式重绑入口不能再按 scene.handle 直接短路，否则往返同场景时可能整条跳过 runtime rebind。");
    }

    [Test]
    public void RuntimeReassertions_ShouldPreserveInventorySelectionPreference()
    {
        string inputText = File.ReadAllText(GameInputManagerPath);
        string toolbarSlotText = File.ReadAllText(ToolbarSlotUiPath);
        string playerInteractionText = File.ReadAllText(PlayerInteractionPath);
        string bridgeText = File.ReadAllText(PersistentBridgePath);
        string inventoryPanelText = File.ReadAllText(InventoryPanelPath);
        string boxPanelText = File.ReadAllText(BoxPanelPath);

        StringAssert.Contains("hotbarSelection?.ReassertCurrentSelection(collapseInventorySelectionToHotbar: false, invokeEvent: true);", inputText,
            "切场重绑、面板恢复等 runtime reassert 不应再把背包真实选槽强压回 hotbar。");
        StringAssert.Contains("selection.ReassertCurrentSelection(collapseInventorySelectionToHotbar: false, invokeEvent: true);", toolbarSlotText,
            "Toolbar 拒绝切槽后应恢复当前真选中，而不是偷偷抹掉背包偏好槽。");
        StringAssert.Contains(".ReassertCurrentSelection(collapseInventorySelectionToHotbar: false, invokeEvent: true);", playerInteractionText,
            "工具动作缓存应用失败时应保住当前背包偏好槽，不要再折回 hotbar。");
        StringAssert.Contains("hotbarInventorySelectionSnapshot = sceneHotbarSelection.selectedInventoryIndex;", bridgeText,
            "切场快照必须把 selectedInventoryIndex 一起记住，不能只存 hotbarIndex。");
        StringAssert.Contains("restoredInventoryIndex", bridgeText,
            "切场恢复时必须把背包偏好槽单独恢复，不能再与 hotbarIndex 共用同一个值。");
        StringAssert.Contains("selection.selectedInventoryIndex", inventoryPanelText,
            "背包页打开时应按 selectedInventoryIndex 恢复面板选槽，不要再强压成 selectedIndex。");
        StringAssert.Contains("_hotbarSelection.selectedInventoryIndex", boxPanelText,
            "箱子页 Down 区也应按 selectedInventoryIndex 恢复选槽，否则切场后会看起来像背包回弹。");
        StringAssert.Contains("followHotbarSelection = selection.selectedInventoryIndex < InventoryService.HotbarWidth;", inventoryPanelText,
            "背包页 runtime context 重绑后也应重新对齐 followHotbarSelection，不能沿用旧场景的 UI 本地壳。");
        StringAssert.Contains("_followHotbarSelection = _hotbarSelection.selectedInventoryIndex < InventoryService.HotbarWidth;", boxPanelText,
            "箱子页 Down 区 runtime context 重绑后也应重新对齐 followHotbarSelection。");
    }

    [Test]
    public void RuntimeHeldEntryPoints_ShouldPreferResolvedPlacementSelection()
    {
        string inputText = File.ReadAllText(GameInputManagerPath);

        StringAssert.Contains("private bool TryGetCurrentRuntimeHeldItem(out int slotIndex, out ItemStack slot, out ItemData itemData)", inputText,
            "GameInputManager 应收敛一个统一 helper，把当前运行态真实物品来源先走 placement-safe 解析，再回退 hotbar。");
        StringAssert.Contains("if (TryGetCurrentPlacementModeSelection(out slotIndex, out slot, out itemData))", inputText,
            "当前运行态真实物品来源必须优先吃 selectedInventoryIndex 对应的 placement-safe 槽位。");
        StringAssert.Contains("if (!TryGetCurrentRuntimeHeldItem(out _, out ItemStack slot, out ItemData itemData))", inputText,
            "预览刷新和左键主分发不应再直接把来源压回 selectedIndex。");
        StringAssert.Contains("if (!TryGetCurrentRuntimeHeldItem(out int slotIndex, out ItemStack slot, out ItemData _))", inputText,
            "远距离锄地/浇水的快照也必须记住真实运行态槽位，否则导航回来后会拿错物品来源。");
        StringAssert.Contains("if (TryGetCurrentRuntimeHeldItem(out int heldSlotIndex, out ItemStack heldSlot, out ItemData _))", inputText,
            "交互上下文里的 HeldSlot 也应跟运行态真实来源对齐，不能继续只看 hotbar 第一排。");
    }

    [Test]
    public void ToolbarBuild_ShouldBindBySiblingOrderInsteadOfNameParsing()
    {
        string toolbarUiText = File.ReadAllText(ToolbarUiPath);

        StringAssert.Contains("for (int resolvedIndex = 0; resolvedIndex < bindCount; resolvedIndex++)", toolbarUiText,
            "toolbar 重绑应直接按 Grid 的 sibling 顺序绑定 0..11，而不是继续靠名字推断槽位索引。");
        StringAssert.Contains("将按 sibling 顺序绑定", toolbarUiText,
            "如果 authored Grid 槽位数量异常，toolbar 应报实并继续按 hierarchy 顺序绑定，而不是静默错绑。");
        StringAssert.DoesNotContain("ResolveToolbarSlotIndex(", toolbarUiText,
            "toolbar 不应再保留名字解析索引这条脆弱链，否则 4/8 这类固定槽位坏相会继续反复。");
    }

    [Test]
    public void DialogueLock_ShouldClosePackageAndBoxPanelsTogether()
    {
        string inputText = File.ReadAllText(GameInputManagerPath);

        StringAssert.Contains("CloseBlockingPanelsForDialogueLock();", inputText,
            "剧情接管时应统一关闭背包与箱子，不应只关箱子留下 Package 面板残留。");
        StringAssert.Contains("tabs.ClosePanelForExternalAction();", inputText,
            "剧情接管的面板清理应走 PackagePanelTabsUI 的统一外部关闭入口。");
    }

    [Test]
    public void BoxPanel_ShouldSharePackageRuntimeContextForLowerInventorySlots()
    {
        string boxPanelText = File.ReadAllText(BoxPanelPath);
        string packagePanelText = File.ReadAllText(PackagePanelTabsPath);

        StringAssert.Contains("public InventoryService RuntimeInventoryService => runtimeInventoryService;", packagePanelText,
            "PackagePanelTabsUI 应暴露当前运行时背包事实源，供箱子页 Down 区复用同一引用。");
        StringAssert.Contains("packageTabs.RuntimeInventoryService", boxPanelText,
            "BoxPanelUI 打开时应优先吃 PackagePanelTabsUI 的运行时上下文，避免 Home/Town 切场后 Down 区绑定旧背包。");
        StringAssert.Contains("SyncInventoryInteractionManagerRuntimeContext();", boxPanelText,
            "箱子页 Down 区刷新时应同步 InventoryInteractionManager 的运行时引用，避免显示已刷新但点击链仍指向旧对象。");
        StringAssert.Contains("sortService.RebindRuntimeContext(_inventoryService, _database);", boxPanelText,
            "箱子页 Down 区整理前也应先重绑到当前 runtime 背包，避免跨场景后 sort 仍整理旧库存。");
    }
}
