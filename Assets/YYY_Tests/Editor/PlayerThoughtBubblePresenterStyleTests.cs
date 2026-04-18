using System;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UObject = UnityEngine.Object;

[TestFixture]
public class PlayerThoughtBubblePresenterStyleTests
{
    private GameObject playerObject;
    private GameObject npcObject;

    [TearDown]
    public void TearDown()
    {
        Type npcType = ResolveTypeOrFail("NPCBubblePresenter");
        SetPrivateStaticField(npcType, "s_conversationChannelOwner", null);

        if (playerObject != null)
        {
            UObject.DestroyImmediate(playerObject);
        }

        if (npcObject != null)
        {
            UObject.DestroyImmediate(npcObject);
        }
    }

    [Test]
    public void PlayerPreset_ShouldKeepReadableLayoutWhileStayingDistinctFromNpcBubble()
    {
        Component player = CreatePresenter("PlayerThoughtBubblePresenter", ref playerObject, "PlayerBubblePresenter");
        Component npc = CreatePresenter("NPCBubblePresenter", ref npcObject, "NpcBubblePresenter");

        InvokeInstance(player, "ApplyPlayerBubbleStylePreset");
        InvokeInstance(npc, "ApplyCurrentStylePreset");

        Vector3 playerOffset = GetPrivateField<Vector3>(player, "bubbleLocalOffset");
        Vector3 npcOffset = GetPrivateField<Vector3>(npc, "bubbleLocalOffset");
        Assert.That(playerOffset.x, Is.LessThanOrEqualTo(0f));
        Assert.That(npcOffset.x, Is.GreaterThanOrEqualTo(0f));
        Assert.That(Mathf.Abs(playerOffset.y - npcOffset.y), Is.LessThanOrEqualTo(0.12f));

        Assert.That(
            GetPrivateField<float>(player, "fontSize"),
            Is.EqualTo(GetPrivateField<float>(npc, "fontSize")).Within(0.001f));
        Assert.That(
            GetPrivateField<int>(player, "preferredCharactersPerLine"),
            Is.EqualTo(GetPrivateField<int>(npc, "preferredCharactersPerLine")));

        Color playerFill = GetPrivateField<Color>(player, "bubbleFillColor");
        Color npcFill = GetPrivateField<Color>(npc, "bubbleColor");
        Assert.That(playerFill.a, Is.EqualTo(1f).Within(0.001f), "玩家气泡背景不应再保留半透明。");
        Assert.That(npcFill.a, Is.EqualTo(1f).Within(0.001f), "NPC 气泡背景不应再保留半透明。");
        Assert.That(AreColorsClose(playerFill, npcFill), Is.False, "玩家气泡和 NPC 气泡应该保持明显区分。");
    }

    [Test]
    public void PlayerTextFormatting_ShouldMatchNpcBubbleWrappingRhythm()
    {
        Component player = CreatePresenter("PlayerThoughtBubblePresenter", ref playerObject, "PlayerBubblePresenter");
        Component npc = CreatePresenter("NPCBubblePresenter", ref npcObject, "NpcBubblePresenter");

        InvokeInstance(player, "ApplyPlayerBubbleStylePreset");
        InvokeInstance(npc, "ApplyCurrentStylePreset");

        const string sourceText = "12345678901";
        string playerFormatted = (string)InvokeInstance(player, "FormatBubbleText", sourceText);
        string npcFormatted = (string)InvokeInstance(npc, "FormatBubbleText", sourceText);

        Assert.That(playerFormatted, Is.EqualTo(npcFormatted));
        Assert.That(playerFormatted, Is.EqualTo("1234567890\n1"));
    }

    [Test]
    public void BubbleForegroundSorting_ShouldStayInDedicatedTopBand()
    {
        Type playerType = ResolveTypeOrFail("PlayerThoughtBubblePresenter");
        Type npcType = ResolveTypeOrFail("NPCBubblePresenter");
        Type sessionType = ResolveTypeOrFail("PlayerNpcChatSessionService");

        int playerBase = GetPrivateStaticField<int>(playerType, "BubbleForegroundSortingBase");
        int npcBase = GetPrivateStaticField<int>(npcType, "BubbleForegroundSortingBase");
        int playerSpeakerBoost = GetPrivateStaticField<int>(playerType, "SpeakerForegroundSortBoost");
        int npcSpeakerBoost = GetPrivateStaticField<int>(npcType, "SpeakerForegroundSortBoost");
        int conversationBoost = GetPrivateStaticField<int>(sessionType, "ConversationForegroundSortBoost");

        Assert.That(playerBase, Is.GreaterThanOrEqualTo(20000), "玩家气泡应固定在足够高的前景排序带，避免再被场景树木压住。");
        Assert.That(npcBase, Is.GreaterThanOrEqualTo(20000), "NPC 气泡应固定在足够高的前景排序带，避免再被场景树木压住。");
        Assert.That(playerSpeakerBoost, Is.GreaterThanOrEqualTo(300), "玩家说话者置顶 boost 不能再小到被稳定 bias 吃掉。");
        Assert.That(npcSpeakerBoost, Is.GreaterThanOrEqualTo(300), "NPC 说话者置顶 boost 不能再小到被稳定 bias 吃掉。");
        Assert.That(conversationBoost, Is.GreaterThanOrEqualTo(200), "对话态前景 boost 应足够大，确保谁说话谁稳定在上面。");
    }

    [Test]
    public void PlayerSpeakerFocus_ShouldRaiseAndClearForegroundBoost()
    {
        Component player = CreatePresenter("PlayerThoughtBubblePresenter", ref playerObject, "PlayerBubblePresenter");

        InvokeInstance(player, "SetSpeakerForegroundFocus");
        Assert.That(GetPrivateField<int>(player, "speakerForegroundSortBoost"), Is.GreaterThanOrEqualTo(300));

        InvokeInstance(player, "ClearSpeakerForegroundFocus");
        Assert.That(GetPrivateField<int>(player, "speakerForegroundSortBoost"), Is.EqualTo(0));
    }

    [Test]
    public void ConversationLayout_ShouldStayCloseToSpeakerHeads_WhileKeepingReadableSeparation()
    {
        Component service = CreateConversationService(out Component playerPresenter, out Component npcPresenter);

        SetPrivateField(service, "_state", ParseNestedEnum(service, "SessionState", "PlayerTyping"));
        InvokeInstance(service, "UpdateConversationBubbleLayout");

        Vector3 playerShift = GetPrivateField<Vector3>(playerPresenter, "conversationLayoutShift");
        Vector3 npcShift = GetPrivateField<Vector3>(npcPresenter, "_conversationLayoutShift");
        int playerSortBoost = GetPrivateField<int>(playerPresenter, "conversationSortBoost");
        int npcSortBoost = GetPrivateField<int>(npcPresenter, "_conversationSortBoost");

        Assert.That(Mathf.Abs(playerShift.x), Is.LessThanOrEqualTo(0.14f));
        Assert.That(Mathf.Abs(npcShift.x), Is.LessThanOrEqualTo(0.14f));
        Assert.That(playerShift.y - npcShift.y, Is.GreaterThan(0f));
        Assert.That(Mathf.Abs(playerShift.y - npcShift.y), Is.LessThanOrEqualTo(0.12f));
        Assert.That(playerSortBoost, Is.GreaterThan(npcSortBoost));

        SetPrivateField(service, "_conversationBubbleFocus", ParseNestedEnum(service, "ConversationBubbleFocus", "None"));
        SetPrivateField(service, "_state", ParseNestedEnum(service, "SessionState", "NpcTyping"));
        InvokeInstance(service, "UpdateConversationBubbleLayout");

        playerShift = GetPrivateField<Vector3>(playerPresenter, "conversationLayoutShift");
        npcShift = GetPrivateField<Vector3>(npcPresenter, "_conversationLayoutShift");
        playerSortBoost = GetPrivateField<int>(playerPresenter, "conversationSortBoost");
        npcSortBoost = GetPrivateField<int>(npcPresenter, "_conversationSortBoost");

        Assert.That(Mathf.Abs(playerShift.x), Is.LessThanOrEqualTo(0.14f));
        Assert.That(Mathf.Abs(npcShift.x), Is.LessThanOrEqualTo(0.14f));
        Assert.That(npcShift.y - playerShift.y, Is.GreaterThan(0f));
        Assert.That(Mathf.Abs(npcShift.y - playerShift.y), Is.LessThanOrEqualTo(0.12f));
        Assert.That(npcSortBoost, Is.GreaterThan(playerSortBoost));
    }

    [Test]
    public void ReadableHoldSeconds_ShouldFollowFixedPacingFormula()
    {
        Component service = CreateConversationService(out _, out _);

        float tenChars = (float)InvokeInstance(service, "CalculateReadableHoldSeconds", "1234567890", 0f, -1f);
        float twentyChars = (float)InvokeInstance(service, "CalculateReadableHoldSeconds", "12345678901234567890", 0f, -1f);
        float minimumClamp = (float)InvokeInstance(service, "CalculateReadableHoldSeconds", "123", 1.8f, -1f);

        Assert.That(tenChars, Is.EqualTo(1.8f).Within(0.001f));
        Assert.That(twentyChars, Is.EqualTo(2.6f).Within(0.001f));
        Assert.That(minimumClamp, Is.EqualTo(1.8f).Within(0.001f));
    }

    [Test]
    public void AmbientBubble_ShouldIgnoreHiddenStaleConversationOwner()
    {
        Component owner = CreatePresenter("NPCBubblePresenter", ref playerObject, "OwnerBubblePresenter");
        Component candidate = CreatePresenter("NPCBubblePresenter", ref npcObject, "CandidateBubblePresenter");
        Type npcType = ResolveTypeOrFail("NPCBubblePresenter");
        object ambientPriority = ParseNestedEnum(npcType, "BubbleChannelPriority", "Ambient");

        SetPrivateStaticField(npcType, "s_conversationChannelOwner", owner);

        bool canShowAmbient = (bool)InvokeInstance(candidate, "CanShow", ambientPriority);

        Assert.That(canShowAmbient, Is.True);
        Assert.That(GetPrivateStaticFieldValue(npcType, "s_conversationChannelOwner"), Is.Null);
    }

    [Test]
    public void PlayerBubble_ShouldUseStableFontMaterialAndLighterTypography()
    {
        Component player = CreatePresenter("PlayerThoughtBubblePresenter", ref playerObject, "PlayerBubblePresenter");

        player.GetType()
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(player, new object[] { "玩家气泡字体修复验证", -1f, true });

        TextMeshProUGUI bubbleText = GetPrivateField<TextMeshProUGUI>(player, "bubbleText");
        Assert.That(bubbleText, Is.Not.Null);
        Assert.That(bubbleText.font, Is.Not.Null);
        Assert.That(bubbleText.font.name, Does.Contain("DialogueChinese"));
        Assert.That(bubbleText.font.name, Does.Not.Contain("Pixel"));
        Assert.That(bubbleText.fontSharedMaterial, Is.SameAs(bubbleText.font.material));
        Assert.That(bubbleText.characterSpacing, Is.EqualTo(0f).Within(0.001f));
        Assert.That(bubbleText.lineSpacing, Is.EqualTo(-0.4f).Within(0.001f));
        Assert.That(bubbleText.outlineWidth, Is.EqualTo(0.08f).Within(0.001f));
    }


    private static Component CreatePresenter(string typeName, ref GameObject owner, string objectName)
    {
        owner = new GameObject(objectName);
        Type componentType = ResolveTypeOrFail(typeName);
        return owner.AddComponent(componentType);
    }

    private Component CreateConversationService(out Component playerPresenter, out Component npcPresenter)
    {
        playerObject = new GameObject("PlayerConversationRig");
        npcObject = new GameObject("NpcConversationRig");
        npcObject.transform.position = new Vector3(0.12f, 0f, 0f);

        Component service = playerObject.AddComponent(ResolveTypeOrFail("PlayerNpcChatSessionService"));
        Component playerMovement = playerObject.AddComponent(ResolveTypeOrFail("PlayerMovement"));
        SpriteRenderer playerRenderer = playerObject.AddComponent<SpriteRenderer>();
        playerPresenter = playerObject.AddComponent(ResolveTypeOrFail("PlayerThoughtBubblePresenter"));

        SpriteRenderer npcRenderer = npcObject.AddComponent<SpriteRenderer>();
        npcPresenter = npcObject.AddComponent(ResolveTypeOrFail("NPCBubblePresenter"));
        Component npcInteractable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable"));

        SetPrivateField(playerPresenter, "targetRenderer", playerRenderer);
        SetPrivateField(npcPresenter, "targetRenderer", npcRenderer);
        SetPrivateField(npcInteractable, "bubblePresenter", npcPresenter);
        SetPrivateField(service, "_playerMovement", playerMovement);
        SetPrivateField(service, "playerBubblePresenter", playerPresenter);
        SetPrivateField(service, "_activeInteractable", npcInteractable);

        return service;
    }

    private static object ParseNestedEnum(object target, string nestedTypeName, string value)
    {
        return ParseNestedEnum(target.GetType(), nestedTypeName, value);
    }

    private static object ParseNestedEnum(Type targetType, string nestedTypeName, string value)
    {
        Type nestedType = targetType.GetNestedType(nestedTypeName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {targetType.Name}.{nestedTypeName}");
        return Enum.Parse(nestedType, value);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static object GetPrivateStaticFieldValue(Type targetType, string fieldName)
    {
        FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {targetType.Name}.{fieldName}");
        return field.GetValue(null);
    }

    private static void SetPrivateStaticField(Type targetType, string fieldName, object value)
    {
        FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {targetType.Name}.{fieldName}");
        field.SetValue(null, value);
    }


    private static Type ResolveTypeOrFail(string typeName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int index = 0; index < assemblies.Length; index++)
        {
            Type resolvedType = assemblies[index].GetType(typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static T GetPrivateStaticField<T>(Type targetType, string fieldName)
    {
        FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {targetType.Name}.{fieldName}");
        return (T)field.GetValue(null);
    }

    private static bool AreColorsClose(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) <= 0.001f
            && Mathf.Abs(a.g - b.g) <= 0.001f
            && Mathf.Abs(a.b - b.b) <= 0.001f
            && Mathf.Abs(a.a - b.a) <= 0.001f;
    }
}
