using System.Collections;
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;

[TestFixture]
public class NpcAmbientBubblePriorityGuardTests
{
    private GameObject storyObject;
    private GameObject playerObject;
    private GameObject npcObject;
    private GameObject partnerObject;
    private GameObject dialogueObject;
    private GameObject proximityServiceObject;

    [TearDown]
    public void TearDown()
    {
        Type npcBubbleType = ResolveTypeOrFail("NPCBubblePresenter");
        SetPrivateStaticField(npcBubbleType, "s_conversationChannelOwner", null);
        SetPrivateStaticField(ResolveTypeOrFail("Sunset.Story.StoryManager"), "_instance", null);
        SetStaticProperty(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "Instance", null);
        SetPrivateStaticField(ResolveTypeOrFail("Sunset.Story.SpringDay1ProximityInteractionService"), "_instance", null);

        if (npcObject != null)
        {
            UObject.DestroyImmediate(npcObject);
        }

        if (partnerObject != null)
        {
            UObject.DestroyImmediate(partnerObject);
        }

        if (storyObject != null)
        {
            UObject.DestroyImmediate(storyObject);
        }

        if (playerObject != null)
        {
            UObject.DestroyImmediate(playerObject);
        }

        if (dialogueObject != null)
        {
            UObject.DestroyImmediate(dialogueObject);
        }

        if (proximityServiceObject != null)
        {
            UObject.DestroyImmediate(proximityServiceObject);
        }
    }

    [Test]
    public void FormalPriorityPhase_ShouldHideVisibleAmbientBubble_WhenDialogueIsActive()
    {
        object crashAndMeetPhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "CrashAndMeet");
        CreateStoryManager(crashAndMeetPhase);
        CreateDialogueManager(activeDialogue: true);
        Component presenter = CreatePresenter();
        Type npcBubbleType = presenter.GetType();

        object displayMode = ParseNestedEnum(npcBubbleType, "BubbleDisplayMode", "Default");
        object ambientPriority = ParseNestedEnum(npcBubbleType, "BubbleChannelPriority", "Ambient");

        bool shown = InvokeInstance<bool>(
            presenter,
            "ShowTextInternal",
            "这是一条环境气泡。",
            -1f,
            false,
            true,
            displayMode,
            ambientPriority);

        Assert.That(shown, Is.True);
        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.True);

        Assert.That(
            InvokeStatic<bool>(ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy"), "ShouldSuppressAmbientBubbleForCurrentStory"),
            Is.True);
        InvokeStatic(ResolveTypeOrFail("Sunset.Story.NpcAmbientBubblePriorityGuard"), "SuppressVisibleAmbientBubbles");

        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.False);
    }

    [Test]
    public void FormalPriorityPhase_ShouldAllowAmbientBubble_WhenNoFormalTakeoverIsActive()
    {
        object crashAndMeetPhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "CrashAndMeet");
        CreateStoryManager(crashAndMeetPhase);
        CreatePlayerContext();
        Component presenter = CreatePresenter();
        Type npcBubbleType = presenter.GetType();
        object displayMode = ParseNestedEnum(npcBubbleType, "BubbleDisplayMode", "Default");
        object ambientPriority = ParseNestedEnum(npcBubbleType, "BubbleChannelPriority", "Ambient");

        bool shown = InvokeInstance<bool>(
            presenter,
            "ShowTextInternal",
            "这条环境气泡应该还能保留。",
            -1f,
            false,
            true,
            displayMode,
            ambientPriority);

        Assert.That(shown, Is.True);
        Assert.That(
            InvokeStatic<bool>(ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy"), "ShouldSuppressAmbientBubbleForCurrentStory"),
            Is.False);
        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.True);
    }

    [Test]
    public void FormalPriorityPhase_ShouldHideVisibleAmbientBubble_WhenFormalPromptOwnsCurrentFocus()
    {
        object crashAndMeetPhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "CrashAndMeet");
        CreateStoryManager(crashAndMeetPhase);
        CreatePlayerContext();
        CreateDialogueManager(activeDialogue: false);
        Component presenter = CreatePresenter(withFormalDialogueCandidate: true);
        Component dialogueInteractable = GetRequiredComponent(npcObject, ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
        SetCurrentFormalPromptFocus(npcObject.transform);

        Assert.That(InvokeInstance<bool>(dialogueInteractable, "CanInteract", CreateInteractionContext()), Is.True);

        Type npcBubbleType = presenter.GetType();
        object displayMode = ParseNestedEnum(npcBubbleType, "BubbleDisplayMode", "Default");
        object ambientPriority = ParseNestedEnum(npcBubbleType, "BubbleChannelPriority", "Ambient");

        bool shown = InvokeInstance<bool>(
            presenter,
            "ShowTextInternal",
            "这条环境气泡不该压住 formal 提示。",
            -1f,
            false,
            true,
            displayMode,
            ambientPriority);

        Assert.That(shown, Is.True);
        Assert.That(
            InvokeStatic<bool>(ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy"), "ShouldSuppressAmbientBubbleForCurrentStory"),
            Is.True);
        InvokeStatic(ResolveTypeOrFail("Sunset.Story.NpcAmbientBubblePriorityGuard"), "SuppressVisibleAmbientBubbles");
        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.False);
    }

    [Test]
    public void FormalPriorityPhase_ShouldPreventDelayedAmbientBubbleFromAppearing_WhenFormalTakeoverIsActive()
    {
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        object crashAndMeetPhase = ParseEnum(storyPhaseType, "CrashAndMeet");
        CreateStoryManager(crashAndMeetPhase);
        CreatePlayerContext();
        CreateDialogueManager(activeDialogue: true);

        Component controller = CreateRoamController(ref npcObject, "AmbientInitiator");
        Component partnerController = CreateRoamController(ref partnerObject, "AmbientPartner");
        SetPrivateField(controller, "chatPartner", partnerController);

        bool canShow = InvokeInstance<bool>(
            controller,
            "CanShowAmbientBubbleNow",
            new[] { "这条环境气泡不该在 formal 期冒出来。" },
            crashAndMeetPhase);

        Assert.That(canShow, Is.False);
    }

    private Component CreateStoryManager(object phase)
    {
        storyObject = new GameObject("StoryManagerTestHost");
        Component storyManager = storyObject.AddComponent(ResolveTypeOrFail("Sunset.Story.StoryManager"));
        InvokeInstance<object>(storyManager, "ResetState", phase, false);
        return storyManager;
    }

    private void CreatePlayerContext()
    {
        playerObject = new GameObject("PlayerContext");
        playerObject.AddComponent(ResolveTypeOrFail("PlayerMovement"));
    }

    private Component CreateDialogueManager(bool activeDialogue)
    {
        dialogueObject = new GameObject("DialogueManagerTestHost");
        Component dialogueManager = dialogueObject.AddComponent(ResolveTypeOrFail("Sunset.Story.DialogueManager"));
        SetStaticProperty(dialogueManager.GetType(), "Instance", dialogueManager);

        if (activeDialogue)
        {
            SetInstanceProperty(dialogueManager, "IsDialogueActive", true);
        }

        return dialogueManager;
    }

    private Component CreatePresenter(bool withFormalDialogueCandidate = false)
    {
        npcObject = new GameObject("AmbientBubbleNpc");
        npcObject.AddComponent<SpriteRenderer>();
        Component presenter = npcObject.AddComponent(ResolveTypeOrFail("NPCBubblePresenter"));

        if (withFormalDialogueCandidate)
        {
            Component dialogueInteractable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
            SetPrivateField(dialogueInteractable, "initialSequence", CreateDialogueSequence());
        }

        return presenter;
    }

    private static Component CreateRoamController(ref GameObject host, string objectName)
    {
        host = new GameObject(objectName);
        host.AddComponent<SpriteRenderer>();
        host.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component presenter = host.AddComponent(ResolveTypeOrFail("NPCBubblePresenter"));
        Component controller = host.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        SetPrivateField(controller, "bubblePresenter", presenter);
        return controller;
    }

    private object CreateInteractionContext()
    {
        object context = Activator.CreateInstance(ResolveAssemblyCSharpTypeOrFail("InteractionContext"));
        SetPublicProperty(context, "PlayerTransform", playerObject.transform);
        SetPublicProperty(context, "PlayerPosition", (Vector2)playerObject.transform.position);
        return context;
    }

    private void SetCurrentFormalPromptFocus(Transform anchorTarget)
    {
        proximityServiceObject = new GameObject("PromptService");
        Component service = proximityServiceObject.AddComponent(ResolveTypeOrFail("Sunset.Story.SpringDay1ProximityInteractionService"));
        SetPrivateStaticField(service.GetType(), "_instance", service);

        Type serviceType = service.GetType();
        Type candidateType = ResolveNestedTypeOrFail(serviceType, "Candidate");
        Type visualKindType = ResolveTypeOrFail("Sunset.Story.SpringDay1WorldHintBubble+HintVisualKind");
        object visualKind = Enum.Parse(visualKindType, "Interaction");
        ConstructorInfo constructor = candidateType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .First(info => info.GetParameters().Length == 13);

        object candidate = constructor.Invoke(new object[]
        {
            anchorTarget,
            KeyCode.E,
            "E",
            "交谈",
            "按 E 开始对话",
            0.1f,
            30,
            0f,
            true,
            true,
            false,
            visualKind,
            new Action(() => { })
        });

        SetPrivateField(service, "_currentCandidate", candidate);
        SetPrivateField(service, "_hasCurrentCandidate", true);
    }

    private static ScriptableObject CreateDialogueSequence()
    {
        Type sequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");
        Type nodeType = ResolveTypeOrFail("Sunset.Story.DialogueNode");

        ScriptableObject sequence = ScriptableObject.CreateInstance(sequenceType);
        SetPublicField(sequence, "sequenceId", "ambient-priority-guard-test");

        object node = Activator.CreateInstance(nodeType);
        SetPublicField(node, "speakerName", "村长");
        SetPublicField(node, "text", "正式对白");

        IList nodeList = GetPublicField<IList>(sequence, "nodes");
        nodeList.Add(node);
        return sequence;
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolvedType = TryResolveType(assembly, typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static Type ResolveAssemblyCSharpTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!string.Equals(assembly.GetName().Name, "Assembly-CSharp", StringComparison.Ordinal))
            {
                continue;
            }

            Type resolvedType = assembly.GetType(typeName, false);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未在 Assembly-CSharp 中找到类型: {typeName}");
        return null;
    }

    private static Type TryResolveType(Assembly assembly, string typeName)
    {
        Type direct = assembly.GetType(typeName, false);
        if (direct != null)
        {
            return direct;
        }

        try
        {
            Type fullNameMatch = assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal));
            if (fullNameMatch != null)
            {
                return fullNameMatch;
            }

            return assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            Type fullNameMatch = ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal));
            if (fullNameMatch != null)
            {
                return fullNameMatch;
            }

            return ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }

    private static object ParseEnum(Type enumType, string value)
    {
        Assert.That(enumType, Is.Not.Null, $"未找到枚举类型: {value}");
        return Enum.Parse(enumType, value);
    }

    private static object ParseNestedEnum(Type targetType, string nestedTypeName, string value)
    {
        Type nestedType = targetType.GetNestedType(nestedTypeName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {targetType.Name}.{nestedTypeName}");
        return Enum.Parse(nestedType, value);
    }

    private static T InvokeInstance<T>(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return (T)method.Invoke(target, args);
    }

    private static void InvokeStatic(Type targetType, string methodName)
    {
        MethodInfo method = targetType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        method.Invoke(null, null);
    }

    private static T InvokeStatic<T>(Type targetType, string methodName)
    {
        MethodInfo method = targetType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        return (T)method.Invoke(null, null);
    }

    private static T GetPublicProperty<T>(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return (T)property.GetValue(target);
    }

    private static void SetPrivateStaticField(Type targetType, string fieldName, object value)
    {
        FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {targetType.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void SetStaticProperty(Type targetType, string propertyName, object value)
    {
        PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"未找到静态属性: {targetType.Name}.{propertyName}");
        property.SetValue(null, value);
    }

    private static void SetInstanceProperty(object target, string propertyName, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"未找到实例属性: {target.GetType().Name}.{propertyName}");
        property.SetValue(target, value);
    }

    private static void SetPublicProperty(object target, string propertyName, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        property.SetValue(target, value);
    }

    private static void SetPublicField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static T GetPublicField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static Type ResolveNestedTypeOrFail(Type targetType, string nestedTypeName)
    {
        Type nestedType = targetType.GetNestedType(nestedTypeName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {targetType.Name}.{nestedTypeName}");
        return nestedType;
    }

    private static Component GetRequiredComponent(GameObject host, Type componentType)
    {
        Component component = host.GetComponent(componentType);
        Assert.That(component, Is.Not.Null, $"未找到组件: {componentType.Name}");
        return component;
    }
}
