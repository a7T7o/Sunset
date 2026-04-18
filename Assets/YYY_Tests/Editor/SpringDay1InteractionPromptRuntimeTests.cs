using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class SpringDay1InteractionPromptRuntimeTests
{
    private readonly List<GameObject> createdObjects = new();

    [SetUp]
    public void SetUp()
    {
        InvokeStaticMethod(GetRuntimeType("Sunset.Story.InteractionHintDisplaySettings"), "SetHintsVisible", true);
        InvokeStaticMethod(GetRuntimeType("Sunset.Story.NpcInteractionPriorityPolicy"), "SetEditorValidationPhaseOverride", null);
    }

    [TearDown]
    public void TearDown()
    {
        for (int index = createdObjects.Count - 1; index >= 0; index--)
        {
            if (createdObjects[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(createdObjects[index]);
            }
        }

        createdObjects.Clear();
        TrySetStaticField(GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService"), "_instance", null);
        TrySetStaticField(GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble"), "_instance", null);
        TrySetStaticField(GetRuntimeType("Sunset.Story.InteractionHintOverlay"), "s_instance", null);
        InvokeStaticMethod(GetRuntimeType("Sunset.Story.NpcInteractionPriorityPolicy"), "SetEditorValidationPhaseOverride", null);
    }

    [UnityTest]
    public System.Collections.IEnumerator ProximityService_PrefersReadyCandidateOverNearerTeaser()
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");

        InvokeStaticMethod(proximityType, "EnsureRuntime");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        InvokeStaticMethod(overlayType, "EnsureRuntime");

        Component service = FindComponentByType(proximityType);
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(service.gameObject);
        Track(worldHint.gameObject);
        Track(overlay.gameObject);

        GameObject nearerTeaser = Track(new GameObject("NearerTeaser"));
        GameObject readyNpc = Track(new GameObject("ReadyNpc"));

        QueueCandidate(
            service,
            nearerTeaser.transform,
            keyLabel: "E",
            caption: "工作台",
            detail: "按 E 打开",
            boundaryDistance: 0.18f,
            priority: 28,
            canTriggerNow: false);
        QueueCandidate(
            service,
            readyNpc.transform,
            keyLabel: "E",
            caption: "对话",
            detail: "按 E 开始对话",
            boundaryDistance: 0.32f,
            priority: 30,
            canTriggerNow: true);

        InvokePrivateMethod(service, "RefreshFocusFromCurrentFrame");
        yield return null;

        string currentFocusSummary = GetStaticPropertyValue<string>(proximityType, "CurrentFocusSummary");
        Assert.That(currentFocusSummary, Is.Not.Null);
        StringAssert.Contains("ReadyNpc", currentFocusSummary, "ready 候选应压过更近但按不动的 teaser，避免唯一 E 被错误目标卡住。");
        Assert.That(GetPropertyValue<Transform>(worldHint, "CurrentAnchorTarget"), Is.EqualTo(readyNpc.transform), "世界提示归属应跟随真正当前可触发的焦点对象。");
        Assert.That(GetPropertyValue<string>(worldHint, "CurrentKeyLabel"), Is.EqualTo("E"), "当前可触发焦点应保留 E 键标识。");
        Assert.That(GetPropertyValue<string>(worldHint, "CurrentCaptionText"), Is.EqualTo("对话"), "世界提示文本应与最终焦点一致。");

        Canvas overlayCanvas = GetPrivateFieldValue<Canvas>(overlay, "overlayCanvas");
        Assert.That(overlayCanvas, Is.Not.Null);
        Assert.That(overlayCanvas.gameObject.activeSelf, Is.True, "当前 ready 焦点应真正显示底部交互提示卡。");
    }

    [UnityTest]
    public System.Collections.IEnumerator ProximityService_HidesECardWhenOnlyTeaserCandidateExists()
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");

        InvokeStaticMethod(proximityType, "EnsureRuntime");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        InvokeStaticMethod(overlayType, "EnsureRuntime");

        Component service = FindComponentByType(proximityType);
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(service.gameObject);
        Track(worldHint.gameObject);
        Track(overlay.gameObject);

        GameObject teaserOnly = Track(new GameObject("TeaserOnly"));

        QueueCandidate(
            service,
            teaserOnly.transform,
            keyLabel: "E",
            caption: "工作台",
            detail: "按 E 打开",
            boundaryDistance: 0.24f,
            priority: 28,
            canTriggerNow: false);

        InvokePrivateMethod(service, "RefreshFocusFromCurrentFrame");
        yield return null;

        string currentFocusSummary = GetStaticPropertyValue<string>(proximityType, "CurrentFocusSummary");
        Assert.That(currentFocusSummary, Is.Not.Null);
        StringAssert.Contains("TeaserOnly", currentFocusSummary, "只有 teaser 时，当前焦点摘要仍应指向最近对象。");
        Assert.That(GetPropertyValue<Transform>(worldHint, "CurrentAnchorTarget"), Is.EqualTo(teaserOnly.transform), "世界提示应仍锚定到最近的目标上。");
        Assert.That(GetPropertyValue<string>(worldHint, "CurrentKeyLabel"), Is.EqualTo(string.Empty), "按不动的 teaser 不应继续冒充可立即按下的 E 提示。");
        Assert.That(GetPropertyValue<string>(worldHint, "CurrentDetailText"), Is.EqualTo("再靠近一些"), "teaser 态应给出靠近提示，而不是继续显示按键动作文案。");
        Assert.That(GetPropertyValue<bool>(worldHint, "CurrentIsActionable"), Is.False, "teaser 态应显式标记为不可立即触发。");

        Canvas overlayCanvas = GetPrivateFieldValue<Canvas>(overlay, "overlayCanvas");
        Assert.That(overlayCanvas, Is.Not.Null);
        Assert.That(overlayCanvas.gameObject.activeSelf, Is.False, "只有 teaser 时，不应继续显示底部 E 卡误导玩家。");
    }

    [UnityTest]
    public System.Collections.IEnumerator ProximityService_WorkbenchTeaser_ShouldStayHiddenUntilInRange()
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");
        Type workbenchType = GetRuntimeType("Sunset.Story.CraftingStationInteractable");

        InvokeStaticMethod(proximityType, "EnsureRuntime");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        InvokeStaticMethod(overlayType, "EnsureRuntime");

        Component service = FindComponentByType(proximityType);
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(service.gameObject);
        Track(worldHint.gameObject);
        Track(overlay.gameObject);

        Component workbench = Track(new GameObject("WorkbenchTeaser")).AddComponent(workbenchType);
        QueueCandidate(
            service,
            workbench.transform,
            keyLabel: "E",
            caption: "工作台",
            detail: "按 E 打开",
            boundaryDistance: 0.46f,
            priority: 28,
            canTriggerNow: false);

        InvokePrivateMethod(service, "RefreshFocusFromCurrentFrame");
        yield return null;

        Assert.That(GetPropertyValue<Transform>(worldHint, "CurrentAnchorTarget"), Is.Null, "工作台超出交互范围时，不应继续冒头顶提示。");

        Canvas overlayCanvas = GetPrivateFieldValue<Canvas>(overlay, "overlayCanvas");
        Assert.That(overlayCanvas, Is.Not.Null);
        Assert.That(overlayCanvas.gameObject.activeSelf, Is.False, "工作台超出交互范围时，左下角提示卡也应直接收起。");
    }

    [UnityTest]
    public System.Collections.IEnumerator ProximityService_WorkbenchReady_ShouldStayOnBottomCardOnly()
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");
        Type workbenchType = GetRuntimeType("Sunset.Story.CraftingStationInteractable");

        InvokeStaticMethod(proximityType, "EnsureRuntime");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        InvokeStaticMethod(overlayType, "EnsureRuntime");

        Component service = FindComponentByType(proximityType);
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(service.gameObject);
        Track(worldHint.gameObject);
        Track(overlay.gameObject);

        Component workbench = Track(new GameObject("WorkbenchReady")).AddComponent(workbenchType);
        QueueCandidate(
            service,
            workbench.transform,
            keyLabel: "E",
            caption: "工作台",
            detail: "按 E 打开工作台，查看配方、材料并开始制作。",
            boundaryDistance: 0.24f,
            priority: 28,
            canTriggerNow: true);

        InvokePrivateMethod(service, "RefreshFocusFromCurrentFrame");
        yield return null;

        Assert.That(GetPropertyValue<Transform>(worldHint, "CurrentAnchorTarget"), Is.Null, "工作台 ready 态也不应再复活头顶提示。");
        Assert.That(GetPropertyValue<string>(overlay, "CurrentKeyLabel"), Is.EqualTo("E"));
        Assert.That(GetPropertyValue<string>(overlay, "CurrentCaptionText"), Is.EqualTo("工作台"));
        Assert.That(GetPropertyValue<string>(overlay, "CurrentDetailText"), Is.EqualTo("按 E 打开工作台，查看配方、材料并开始制作。"));
    }

    [UnityTest]
    public System.Collections.IEnumerator WorldHintBubble_RendersReadableInteractionCardInsteadOfTinyDot()
    {
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Track(worldHint.gameObject);

        GameObject npc = Track(new GameObject("NpcHintTarget"));
        object interactionHintKind = GetEnumValue(worldHintType, "HintVisualKind", "Interaction");
        InvokeInstanceMethod(worldHint, "Show", npc.transform, "E", "交谈", "按 E 开始对话", interactionHintKind);
        yield return null;

        RectTransform bubbleRect = GetPrivateFieldValue<RectTransform>(worldHint, "bubbleRect");
        RectTransform keyPlateRect = GetPrivateFieldValue<RectTransform>(worldHint, "keyPlateRect");
        UnityEngine.UI.Image bubbleBackground = GetPrivateFieldValue<UnityEngine.UI.Image>(worldHint, "bubbleBackground");
        Component captionText = GetPrivateFieldValue<Component>(worldHint, "captionText");
        Component detailText = GetPrivateFieldValue<Component>(worldHint, "detailText");

        Assert.That(bubbleBackground.enabled, Is.True, "普通交互提示应显示可见卡片，不应再退回近乎 gizmo 的小点。");
        Assert.That(bubbleRect.sizeDelta.x, Is.GreaterThanOrEqualTo(160f), "普通交互提示宽度应足以承载可读信息。");
        Assert.That(bubbleRect.sizeDelta.y, Is.GreaterThanOrEqualTo(50f), "普通交互提示高度应明显大于旧的小点指示。");
        Assert.That(keyPlateRect.gameObject.activeSelf, Is.True, "普通交互提示应显式展示当前交互键。");
        Assert.That(GetPropertyValue<string>(captionText, "text"), Is.EqualTo("交谈"));
        Assert.That(((Component)detailText).gameObject.activeSelf, Is.True, "普通交互提示应能显示玩家可读细节文案。");
        Assert.That(GetPropertyValue<bool>(worldHint, "CurrentIsActionable"), Is.True, "ready 态世界提示应显式标记为可立即触发。");
    }

    [UnityTest]
    public System.Collections.IEnumerator InteractionHintOverlay_RendersFormalReadableCard()
    {
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");
        InvokeStaticMethod(overlayType, "EnsureRuntime");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(overlay.gameObject);

        InvokeInstanceMethod(overlay, "ShowPrompt", "E", "对话", "按 E 开始对话");
        yield return null;

        RectTransform cardRect = GetPrivateFieldValue<RectTransform>(overlay, "cardRect");
        RectTransform keyPlateRect = GetPrivateFieldValue<RectTransform>(overlay, "keyPlateRect");
        UnityEngine.UI.Image cardImage = GetPrivateFieldValue<UnityEngine.UI.Image>(overlay, "cardImage");
        UnityEngine.UI.Image accentLineImage = GetPrivateFieldValue<UnityEngine.UI.Image>(overlay, "accentLineImage");

        Assert.That(cardImage.sprite, Is.Not.Null, "底部交互提示卡应使用正式背板，而不是只有纯色测试矩形。");
        Assert.That(cardRect.sizeDelta.x, Is.GreaterThanOrEqualTo(280f), "底部交互提示卡应有足够宽度承载正式提示。");
        Assert.That(cardRect.sizeDelta.y, Is.GreaterThanOrEqualTo(70f), "底部交互提示卡应维持正式卡片高度，而不是扁平测试条。");
        Assert.That(keyPlateRect.gameObject.activeSelf, Is.True, "ready 态底部提示卡应显式展示当前按键。");
        Assert.That(accentLineImage.color.a, Is.GreaterThan(0.8f), "ready 态底部提示卡应保留清晰的强调分隔线。");
    }

    [UnityTest]
    public System.Collections.IEnumerator InteractionHintOverlay_WorkbenchFallbackDetail_ShouldStayFormalAndReadable()
    {
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");
        InvokeStaticMethod(overlayType, "EnsureRuntime");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(overlay.gameObject);

        InvokeInstanceMethod(overlay, "ShowPrompt", "E", "工作台", string.Empty);
        yield return null;

        Assert.That(GetPropertyValue<string>(overlay, "CurrentCaptionText"), Is.EqualTo("工作台"));
        StringAssert.Contains("制作进度", GetPropertyValue<string>(overlay, "CurrentDetailText"), "工作台左下角 fallback detail 应维持正式语义，而不是退回空白或测试态文案。");
    }

    [UnityTest]
    public System.Collections.IEnumerator ProximityService_UsesTaskFirstOverlayCopyWhenDialogueBeatsInformalChatOnSameNpc()
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");
        Type dialogueType = GetRuntimeType("Sunset.Story.NPCDialogueInteractable");
        Type informalType = GetRuntimeType("Sunset.Story.NPCInformalChatInteractable");

        InvokeStaticMethod(proximityType, "EnsureRuntime");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        InvokeStaticMethod(overlayType, "EnsureRuntime");

        Component service = FindComponentByType(proximityType);
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(service.gameObject);
        Track(worldHint.gameObject);
        Track(overlay.gameObject);

        GameObject npc = Track(new GameObject("NpcTaskTarget"));
        npc.AddComponent(dialogueType);
        npc.AddComponent(informalType);

        QueueCandidate(
            service,
            npc.transform,
            keyLabel: "E",
            caption: "闲聊",
            detail: "按 E 开口",
            boundaryDistance: 0.12f,
            priority: 29,
            canTriggerNow: true);
        QueueCandidate(
            service,
            npc.transform,
            keyLabel: "E",
            caption: "剧情对话",
            detail: "按 E 推进当前任务",
            boundaryDistance: 0.28f,
            priority: 30,
            canTriggerNow: true);

        InvokePrivateMethod(service, "RefreshFocusFromCurrentFrame");
        yield return null;

        Assert.That(GetPropertyValue<string>(worldHint, "CurrentCaptionText"), Is.EqualTo("剧情对话"), "这刀只接管左下角内容仲裁，不应顺手把头顶 world hint 文案一起改掉。");
        Assert.That(GetPropertyValue<string>(overlay, "CurrentCaptionText"), Is.EqualTo("进入任务"), "当同一个 NPC 当前更该走正式任务语义时，左下角不应继续显示闲聊/泛化交谈。");
        Assert.That(GetPropertyValue<string>(overlay, "CurrentDetailText"), Is.EqualTo("按 E 开始任务相关对话"), "左下角 detail 应明确压成任务相关对话，而不是继续沿用闲聊/默认对话文案。");
    }

    [UnityTest]
    public System.Collections.IEnumerator ProximityService_NpcCandidate_ShouldStayOffHeadEvenIfWorldIndicatorWasRequested()
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type worldHintType = GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble");
        Type overlayType = GetRuntimeType("Sunset.Story.InteractionHintOverlay");
        Type dialogueType = GetRuntimeType("Sunset.Story.NPCDialogueInteractable");

        InvokeStaticMethod(proximityType, "EnsureRuntime");
        InvokeStaticMethod(worldHintType, "EnsureRuntime");
        InvokeStaticMethod(overlayType, "EnsureRuntime");

        Component service = FindComponentByType(proximityType);
        Component worldHint = GetStaticPropertyValue<Component>(worldHintType, "Instance");
        Component overlay = GetStaticPropertyValue<Component>(overlayType, "Instance");
        Track(service.gameObject);
        Track(worldHint.gameObject);
        Track(overlay.gameObject);

        GameObject npc = Track(new GameObject("NpcNoHeadPrompt"));
        npc.AddComponent(dialogueType);

        QueueCandidate(
            service,
            npc.transform,
            keyLabel: "E",
            caption: "交谈",
            detail: "按 E 开始对话",
            boundaryDistance: 0.24f,
            priority: 30,
            canTriggerNow: true);

        InvokePrivateMethod(service, "RefreshFocusFromCurrentFrame");
        yield return null;

        Assert.That(GetPropertyValue<Transform>(worldHint, "CurrentAnchorTarget"), Is.Null, "NPC 候选即便上报了头顶提示请求，运行时也应统一压回左下角，不再允许头顶交互提示复活。");

        Canvas overlayCanvas = GetPrivateFieldValue<Canvas>(overlay, "overlayCanvas");
        Assert.That(overlayCanvas, Is.Not.Null);
        Assert.That(overlayCanvas.gameObject.activeSelf, Is.True, "NPC 交互语义应继续落在左下角提示卡，而不是被整条链一起隐藏。");
    }

    [UnityTest]
    public System.Collections.IEnumerator CraftingStationInteractable_PrefersNearestVisualEdgeOverFarColliderEnvelope()
    {
        Type workbenchType = GetRuntimeType("Sunset.Story.CraftingStationInteractable");
        Component workbench = Track(new GameObject("WorkbenchBoundary")).AddComponent(workbenchType);

        GameObject visual = Track(new GameObject("Visual"));
        visual.transform.SetParent(workbench.transform, false);
        SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateRuntimeTestSprite();

        GameObject colliderGo = Track(new GameObject("Collider"));
        colliderGo.transform.SetParent(workbench.transform, false);
        colliderGo.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        BoxCollider2D collider = colliderGo.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.6f, 0.6f);

        Vector2 closestPoint = (Vector2)InvokeInstanceMethodResult(workbench, "GetClosestInteractionPoint", new Vector2(0f, -0.9f));
        Assert.That(closestPoint.y, Is.LessThan(0.2f), "不规则工作台应按玩家最近的可见边缘判距，不能继续被偏上的 collider 包络短路。");
        yield break;
    }

    [UnityTest]
    public System.Collections.IEnumerator CraftingStationInteractable_BuildWorkbenchReadyDetail_UsesQueueCopyWhenOverlayHasActiveCraftQueue()
    {
        Type workbenchType = GetRuntimeType("Sunset.Story.CraftingStationInteractable");
        Type overlayType = GetRuntimeType("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        Type recipeType = GetRuntimeType("FarmGame.Data.RecipeData");

        Component workbench = Track(new GameObject("WorkbenchHint")).AddComponent(workbenchType);
        Component overlay = Track(new GameObject("WorkbenchOverlay")).AddComponent(overlayType);
        ScriptableObject recipe = ScriptableObject.CreateInstance(recipeType);
        Coroutine runningRoutine = ((MonoBehaviour)overlay).StartCoroutine(KeepCoroutineAlive());

        SetPrivateFieldValue(overlay, "_craftRoutine", runningRoutine);
        SetPrivateFieldValue(overlay, "_craftingRecipe", recipe);
        SetPrivateFieldValue(overlay, "_craftQueueTotal", 3);
        SetPrivateFieldValue(overlay, "_craftQueueCompleted", 1);
        SetPrivateFieldValue(workbench, "workbenchOverlay", overlay);

        string detail = (string)InvokePrivateMethodResult(workbench, "BuildWorkbenchReadyDetail");
        StringAssert.Contains("单件进度", detail);
        StringAssert.Contains("领取产物", detail);

        ((MonoBehaviour)overlay).StopCoroutine(runningRoutine);
        UnityEngine.Object.DestroyImmediate(recipe);
        yield break;
    }

    [UnityTest]
    public System.Collections.IEnumerator PlayerNpcNearbyFeedbackService_DebugSummary_ReportsPhaseAndLastResidentBubble()
    {
        Type policyType = GetRuntimeType("Sunset.Story.NpcInteractionPriorityPolicy");
        Type phaseType = GetRuntimeType("Sunset.Story.StoryPhase");
        Type serviceType = GetRuntimeType("PlayerNpcNearbyFeedbackService");

        InvokeStaticMethod(policyType, "SetEditorValidationPhaseOverride", Enum.Parse(phaseType, "ReturnAndReminder"));

        Component service = Track(new GameObject("NearbySummaryProbe")).AddComponent(serviceType);
        SetPrivateFieldValue(service, "suppressWhileDialogueActive", true);
        SetPrivateFieldValue(service, "lastNearbyNpcName", "NPC_101");
        SetPrivateFieldValue(service, "lastNearbyBubbleText", "晚上别在外头晃。");

        string summary = GetPropertyValue<string>(service, "DebugSummary");
        StringAssert.Contains("phase=ReturnAndReminder", summary);
        StringAssert.Contains("formalPriority=True", summary);
        StringAssert.Contains("suppressed=True", summary);
        StringAssert.Contains("lastNpc=NPC_101", summary);
        StringAssert.Contains("lastBubble=晚上别在外头晃。", summary);
        yield break;
    }

    [UnityTest]
    public System.Collections.IEnumerator SpringDay1WorkbenchCraftingOverlay_RuntimeRecipeShellSummary_ReportsSelectedRecipeAndFlags()
    {
        Type overlayType = GetRuntimeType("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");
        Type recipeType = GetRuntimeType("FarmGame.Data.RecipeData");

        Component overlay = Track(new GameObject("WorkbenchOverlaySummaryProbe")).AddComponent(overlayType);
        SetPrivateFieldValue(overlay, "_isVisible", true);
        SetPrivateFieldValue(overlay, "_selectedIndex", 0);

        object recipes = GetPrivateFieldValue<object>(overlay, "_recipes");
        ScriptableObject recipe = ScriptableObject.CreateInstance(recipeType);
        recipeType.GetField("recipeName").SetValue(recipe, "夜巡木斧");
        recipeType.GetField("recipeID").SetValue(recipe, 9100);
        recipeType.GetField("resultItemID").SetValue(recipe, 3200);
        recipes.GetType().GetMethod("Add").Invoke(recipes, new object[] { recipe });

        string summary = (string)InvokeInstanceMethodResult(overlay, "GetRuntimeRecipeShellSummary");
        StringAssert.Contains("visible=True", summary);
        StringAssert.Contains("rows=0", summary);
        StringAssert.Contains("generated=False", summary);
        StringAssert.Contains("unreadableRows=False", summary);
        StringAssert.Contains("selected=夜巡木斧", summary);

        UnityEngine.Object.DestroyImmediate(recipe);
        yield break;
    }

    [UnityTest]
    public System.Collections.IEnumerator SpringDay1LiveValidationRunner_BuildSnapshot_ConsumesNpcAndWorkbenchSummaries()
    {
        Type policyType = GetRuntimeType("Sunset.Story.NpcInteractionPriorityPolicy");
        Type phaseType = GetRuntimeType("Sunset.Story.StoryPhase");
        Type runnerType = GetRuntimeType("Sunset.Story.SpringDay1LiveValidationRunner");
        Type informalType = GetRuntimeType("Sunset.Story.NPCInformalChatInteractable");
        Type sessionType = GetRuntimeType("PlayerNpcChatSessionService");
        Type nearbyType = GetRuntimeType("PlayerNpcNearbyFeedbackService");
        Type overlayType = GetRuntimeType("Sunset.Story.SpringDay1WorkbenchCraftingOverlay");

        InvokeStaticMethod(policyType, "SetEditorValidationPhaseOverride", Enum.Parse(phaseType, "ReturnAndReminder"));

        Component runner = Track(new GameObject("LiveValidationRunnerProbe")).AddComponent(runnerType);
        Component informal = Track(new GameObject("NpcPromptProbe")).AddComponent(informalType);
        Track(new GameObject("NpcSessionProbe")).AddComponent(sessionType);
        Component nearby = Track(new GameObject("NearbySummaryProbe")).AddComponent(nearbyType);
        Component overlay = Track(new GameObject("WorkbenchUiProbe")).AddComponent(overlayType);

        SetPrivateFieldValue(nearby, "lastNearbyNpcName", "NPC_201");
        SetPrivateFieldValue(nearby, "lastNearbyBubbleText", "村里今晚风有点紧。");
        SetPrivateFieldValue(overlay, "_isVisible", true);
        SetPrivateFieldValue(overlay, "_selectedIndex", -1);

        string snapshot = (string)InvokeInstanceMethodResult(runner, "BuildSnapshot", "runtime-test");
        StringAssert.Contains("NpcPrompt=", snapshot);
        StringAssert.Contains("NpcPrompt=NpcPromptProbe", snapshot);
        StringAssert.Contains("NpcNearby=phase=ReturnAndReminder", snapshot);
        StringAssert.Contains("lastNpc=NPC_201", snapshot);
        StringAssert.Contains("WorkbenchUi=visible=True", snapshot);
        StringAssert.Contains("OneShot=", snapshot);
        yield break;
    }

    private GameObject Track(GameObject gameObject)
    {
        createdObjects.Add(gameObject);
        return gameObject;
    }

    private static void QueueCandidate(
        Component service,
        Transform anchorTarget,
        string keyLabel,
        string caption,
        string detail,
        float boundaryDistance,
        int priority,
        bool canTriggerNow)
    {
        Type proximityType = GetRuntimeType("Sunset.Story.SpringDay1ProximityInteractionService");
        Type candidateType = proximityType.GetNestedType("Candidate", BindingFlags.NonPublic);
        Assert.That(candidateType, Is.Not.Null, "应能通过反射构造 Day1 近身仲裁候选。");
        object interactionHintKind = GetEnumValue(GetRuntimeType("Sunset.Story.SpringDay1WorldHintBubble"), "HintVisualKind", "Interaction");

        object candidate = Activator.CreateInstance(
            candidateType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: new object[]
            {
                anchorTarget,
                KeyCode.E,
                keyLabel,
                caption,
                detail,
                boundaryDistance,
                priority,
                0.15f,
                canTriggerNow,
                false,
                true,
                interactionHintKind,
                (Action)(() => { })
            },
            culture: null);

        MethodInfo reportMethod = proximityType.GetMethod("ReportCandidateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(reportMethod, Is.Not.Null, "应能调用 Day1 近身仲裁内部上报入口。");
        reportMethod.Invoke(service, new[] { candidate });
    }

    private static T GetPrivateFieldValue<T>(object target, string fieldName) where T : class
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target) as T;
    }

    private static T GetPropertyValue<T>(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return (T)property.GetValue(target);
    }

    private static T GetStaticPropertyValue<T>(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到静态属性: {type.Name}.{propertyName}");
        return (T)property.GetValue(null);
    }

    private static void InvokePrivateMethod(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        method.Invoke(target, null);
    }

    private static void InvokeInstanceMethod(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        method.Invoke(target, args);
    }

    private static object InvokeInstanceMethodResult(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object InvokePrivateMethodResult(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static void InvokeStaticMethod(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {type.Name}.{methodName}");
        method.Invoke(null, args);
    }

    private static Type GetRuntimeType(string fullName)
    {
        Type resolved = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.GetType(fullName, throwOnError: false))
            .FirstOrDefault(type => type != null);
        Assert.That(resolved, Is.Not.Null, $"未找到运行时类型: {fullName}");
        return resolved;
    }

    private static Component FindComponentByType(Type componentType)
    {
        Assert.That(componentType, Is.Not.Null);
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int index = 0; index < objects.Length; index++)
        {
            Component component = objects[index].GetComponent(componentType);
            if (component != null)
            {
                return component;
            }
        }

        Assert.Fail($"未找到组件: {componentType.Name}");
        return null;
    }

    private static object GetEnumValue(Type ownerType, string nestedTypeName, string enumName)
    {
        Type enumType = ownerType.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(enumType, Is.Not.Null, $"未找到枚举类型: {ownerType.Name}.{nestedTypeName}");
        return Enum.Parse(enumType, enumName);
    }

    private static void TrySetStaticField(Type type, string fieldName, object value)
    {
        if (type == null)
        {
            return;
        }

        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(null, value);
        }
    }

    private static void SetPrivateFieldValue(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static System.Collections.IEnumerator KeepCoroutineAlive()
    {
        while (true)
        {
            yield return null;
        }
    }

    private static Sprite CreateRuntimeTestSprite()
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
        texture.filterMode = FilterMode.Point;
        Color[] pixels = { Color.white, Color.white, Color.white, Color.white };
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f), 2f);
    }
}
