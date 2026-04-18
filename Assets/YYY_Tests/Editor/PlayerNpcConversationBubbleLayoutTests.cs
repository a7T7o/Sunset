using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;

[TestFixture]
public class PlayerNpcConversationBubbleLayoutTests
{
    private GameObject playerObject;
    private GameObject npcObject;

    [TearDown]
    public void TearDown()
    {
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
    public void SpeakingBubble_ShouldGainHigherLiftAndSortingBoost()
    {
        ConversationRig rig = CreateConversationRig();

        SetPrivateField(rig.Service, "_state", ParseNestedEnum(rig.Service, "SessionState", "PlayerTyping"));
        InvokeInstance(rig.Service, "UpdateConversationBubbleLayout");

        Vector3 playerShift = GetPrivateField<Vector3>(rig.PlayerPresenter, "conversationLayoutShift");
        Vector3 npcShift = GetPrivateField<Vector3>(rig.NpcPresenter, "_conversationLayoutShift");
        int playerSortBoost = GetPrivateField<int>(rig.PlayerPresenter, "conversationSortBoost");
        int npcSortBoost = GetPrivateField<int>(rig.NpcPresenter, "_conversationSortBoost");

        Assert.That(playerShift.x, Is.LessThan(0f));
        Assert.That(npcShift.x, Is.GreaterThan(0f));
        Assert.That(playerShift.y, Is.GreaterThan(npcShift.y));
        Assert.That(playerSortBoost, Is.GreaterThan(npcSortBoost));

        SetPrivateField(rig.Service, "_conversationBubbleFocus", ParseNestedEnum(rig.Service, "ConversationBubbleFocus", "None"));
        SetPrivateField(rig.Service, "_state", ParseNestedEnum(rig.Service, "SessionState", "NpcTyping"));
        InvokeInstance(rig.Service, "UpdateConversationBubbleLayout");

        playerShift = GetPrivateField<Vector3>(rig.PlayerPresenter, "conversationLayoutShift");
        npcShift = GetPrivateField<Vector3>(rig.NpcPresenter, "_conversationLayoutShift");
        playerSortBoost = GetPrivateField<int>(rig.PlayerPresenter, "conversationSortBoost");
        npcSortBoost = GetPrivateField<int>(rig.NpcPresenter, "_conversationSortBoost");

        Assert.That(npcShift.y, Is.GreaterThan(playerShift.y));
        Assert.That(npcSortBoost, Is.GreaterThan(playerSortBoost));
    }

    [Test]
    public void ResetConversationBubbleVisuals_ShouldClearShiftAndSortBoost()
    {
        ConversationRig rig = CreateConversationRig();

        InvokeInstance(rig.PlayerPresenter, "SetConversationLayoutShift", new Vector3(-0.22f, 0.28f, 0f));
        InvokeInstance(rig.PlayerPresenter, "SetConversationSortBoost", 8);
        InvokeInstance(rig.NpcPresenter, "SetConversationLayoutShift", new Vector3(0.22f, 0.14f, 0f));
        InvokeInstance(rig.NpcPresenter, "SetConversationSortBoost", 3);
        SetPrivateField(rig.Service, "_conversationBubbleFocus", ParseNestedEnum(rig.Service, "ConversationBubbleFocus", "Player"));

        InvokeInstance(rig.Service, "ResetConversationBubbleVisualsImmediate");

        Assert.That(GetPrivateField<Vector3>(rig.PlayerPresenter, "conversationLayoutShift"), Is.EqualTo(Vector3.zero));
        Assert.That(GetPrivateField<int>(rig.PlayerPresenter, "conversationSortBoost"), Is.Zero);
        Assert.That(GetPrivateField<Vector3>(rig.NpcPresenter, "_conversationLayoutShift"), Is.EqualTo(Vector3.zero));
        Assert.That(GetPrivateField<int>(rig.NpcPresenter, "_conversationSortBoost"), Is.Zero);
        Assert.That(
            GetPrivateField<object>(rig.Service, "_conversationBubbleFocus").ToString(),
            Is.EqualTo("None"));
    }

    private ConversationRig CreateConversationRig()
    {
        playerObject = new GameObject("PlayerConversationRig");
        npcObject = new GameObject("NpcConversationRig");
        npcObject.transform.position = new Vector3(0.12f, 0f, 0f);

        Component service = playerObject.AddComponent(ResolveTypeOrFail("PlayerNpcChatSessionService"));
        Component playerMovement = playerObject.AddComponent(ResolveTypeOrFail("PlayerMovement"));
        SpriteRenderer playerRenderer = playerObject.AddComponent<SpriteRenderer>();
        Component playerPresenter = playerObject.AddComponent(ResolveTypeOrFail("PlayerThoughtBubblePresenter"));

        SpriteRenderer npcRenderer = npcObject.AddComponent<SpriteRenderer>();
        Component npcPresenter = npcObject.AddComponent(ResolveTypeOrFail("NPCBubblePresenter"));
        Component npcInteractable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable"));

        SetPrivateField(playerPresenter, "targetRenderer", playerRenderer);
        SetPrivateField(npcPresenter, "targetRenderer", npcRenderer);
        SetPrivateField(npcInteractable, "bubblePresenter", npcPresenter);
        SetPrivateField(service, "_playerMovement", playerMovement);
        SetPrivateField(service, "playerBubblePresenter", playerPresenter);
        SetPrivateField(service, "_activeInteractable", npcInteractable);

        return new ConversationRig
        {
            Service = service,
            PlayerPresenter = playerPresenter,
            NpcPresenter = npcPresenter
        };
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

    private static object ParseNestedEnum(object target, string nestedTypeName, string value)
    {
        Type nestedType = target.GetType().GetNestedType(nestedTypeName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {target.GetType().Name}.{nestedTypeName}");
        return Enum.Parse(nestedType, value);
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

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private sealed class ConversationRig
    {
        public Component Service;
        public Component PlayerPresenter;
        public Component NpcPresenter;
    }
}
