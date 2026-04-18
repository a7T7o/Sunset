using System.IO;
using NUnit.Framework;

[TestFixture]
public class ChestProximityInteractionSourceTests
{
    private static readonly string ProjectRoot =
        Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", ".."));

    private static readonly string ChestControllerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/Placeable/ChestController.cs");

    private static readonly string ProximityServicePath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs");

    private static readonly string GameInputManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Controller/Input/GameInputManager.cs");

    private static readonly string UiLayerUtilityPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs");

    [Test]
    public void ChestController_ShouldReportThroughDay1ProximityService()
    {
        string scriptText = File.ReadAllText(ChestControllerPath);

        StringAssert.Contains("private void ReportProximityInteraction(InteractionContext context)", scriptText);
        StringAssert.Contains("SpringDay1ProximityInteractionService.ReportCandidate(", scriptText);
        StringAssert.Contains("proximityInteractionKey", scriptText);
        StringAssert.Contains("showWorldIndicator: false", scriptText);
    }

    [Test]
    public void ChestController_ProximityTrigger_ShouldReuseOnInteractAndRespectUiSuppression()
    {
        string scriptText = File.ReadAllText(ChestControllerPath);
        string uiLayerText = File.ReadAllText(UiLayerUtilityPath);

        StringAssert.Contains("() => OnInteract(context)", scriptText, "近身 E 键触发必须复用 ChestController.OnInteract()，不能再长第二套开箱逻辑。");
        StringAssert.Contains("SpringDay1UiLayerUtility.IsBlockingPageUiOpen() && !sameChestOpen", scriptText, "箱子近身提示必须尊重全局阻塞页状态，但同一个箱子已打开时要允许 toggle 例外。");
        StringAssert.Contains("allowWhilePageUiOpen: sameChestOpen", scriptText, "同一个箱子已打开时，应继续把 toggle 候选交给统一 proximity 服务。");
        StringAssert.Contains("bool boxOpen = BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen;", uiLayerText, "箱子页打开时，应被 Day1 阻塞页判定链接住。");
    }

    [Test]
    public void ChestController_SameChestInteract_ShouldToggleCloseInsteadOfIgnoring()
    {
        string scriptText = File.ReadAllText(ChestControllerPath);

        StringAssert.Contains("if (BoxPanelUI.ActiveInstance.CurrentChest == this)", scriptText, "同一个箱子已打开时，仍应命中同一个真入口来处理 toggle。");
        StringAssert.Contains("BoxPanelUI.ActiveInstance.Close();", scriptText, "同一个箱子再次交互时应直接关闭，而不是继续保持无响应。");
        StringAssert.Contains("return \"关闭箱子\";", scriptText, "同箱已打开时应切到关闭语义，避免底部提示继续误导成“打开箱子”。");
        StringAssert.Contains("? \"按 E 关闭箱子\"", scriptText, "同箱已打开且仍在交互距离内时，detail 应明确提示 toggle 关闭。");
    }

    [Test]
    public void MouseChestNavigation_ShouldStillUseExistingAutoNavigateInteractPath()
    {
        string inputText = File.ReadAllText(GameInputManagerPath);

        StringAssert.Contains("private bool TryGetChestInteractableAtWorld(Vector2 world, out ChestController chest)", inputText);
        StringAssert.Contains("HandleInteractable(interactable, nodeGO.transform, playerCenter);", inputText, "右键点击箱子仍应复用现有 HandleInteractable -> 自动走近 -> 交互主链。");
        StringAssert.Contains("BeginPendingAutoInteraction(interactable, interactionTarget, interactDist);", inputText, "右键远距离点击箱子时，仍应保留自动走近后再交互。");
        StringAssert.Contains("TryInteract(interactable);", inputText, "近距离右键点击箱子时，仍应沿用统一 IInteractable 交互入口。");
    }

    [Test]
    public void ProximityService_ShouldKeepSingleSharedETargetArbitration()
    {
        string proximityText = File.ReadAllText(ProximityServicePath);

        StringAssert.Contains("if (!Input.GetKeyDown(_currentCandidate.InteractionKey))", proximityText, "近身 E 键输入必须继续由 Day1 统一 proximity 服务仲裁。");
        StringAssert.Contains("_currentCandidate.Trigger?.Invoke();", proximityText, "被选中的近身焦点应由统一 proximity 服务触发。");
        StringAssert.Contains("InteractionHintOverlay.Instance.ShowPrompt(", proximityText, "统一 proximity 服务仍应负责底部交互提示卡。");
        StringAssert.Contains("ShouldAllowPendingCandidateWhilePageUiIsOpen()", proximityText, "同箱已打开时，统一 proximity 服务也必须允许 toggle 候选继续存活。");
        StringAssert.Contains("AllowWhilePageUiOpen", proximityText, "允许 page UI 打开时继续触发的例外语义，必须被显式声明在候选数据上。");
    }
}
