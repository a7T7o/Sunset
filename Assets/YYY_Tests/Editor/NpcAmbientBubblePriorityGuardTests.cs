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
    private GameObject npcObject;
    private GameObject partnerObject;

    [TearDown]
    public void TearDown()
    {
        Type npcBubbleType = ResolveTypeOrFail("NPCBubblePresenter");
        SetPrivateStaticField(npcBubbleType, "s_conversationChannelOwner", null);

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
    }

    [Test]
    public void FormalPriorityPhase_ShouldHideVisibleAmbientBubble()
    {
        object crashAndMeetPhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "CrashAndMeet");
        Component storyManager = CreateStoryManager(crashAndMeetPhase);
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

        InvokeInstance<object>(storyManager, "ResetState", crashAndMeetPhase, false);
        InvokeStatic(ResolveTypeOrFail("Sunset.Story.NpcAmbientBubblePriorityGuard"), "SuppressVisibleAmbientBubbles");

        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.False);
    }

    [Test]
    public void FormalPriorityPhase_ShouldPreventDelayedAmbientBubbleFromAppearing()
    {
        Type storyPhaseType = ResolveTypeOrFail("Sunset.Story.StoryPhase");
        object crashAndMeetPhase = ParseEnum(storyPhaseType, "CrashAndMeet");
        CreateStoryManager(crashAndMeetPhase);

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

    private Component CreatePresenter()
    {
        npcObject = new GameObject("AmbientBubbleNpc");
        npcObject.AddComponent<SpriteRenderer>();
        return npcObject.AddComponent(ResolveTypeOrFail("NPCBubblePresenter"));
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

    private static Type TryResolveType(Assembly assembly, string typeName)
    {
        Type direct = assembly.GetType(typeName, false);
        if (direct != null)
        {
            return direct;
        }

        try
        {
            return assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
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

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }
}
