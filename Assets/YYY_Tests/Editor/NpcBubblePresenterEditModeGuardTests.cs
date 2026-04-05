using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

[TestFixture]
public class NpcBubblePresenterEditModeGuardTests
{
    private GameObject tempObject;

    [TearDown]
    public void TearDown()
    {
        if (tempObject != null)
        {
            UObject.DestroyImmediate(tempObject);
        }
    }

    [Test]
    public void PrefabAssetPresenter_ShouldNotBuildRuntimeBubbleUi()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/222_Prefabs/NPC/101.prefab");
        Assert.That(prefab, Is.Not.Null);

        Component presenter = prefab.GetComponent(presenterType);
        Assert.That(presenter, Is.Not.Null);

        bool canCreate = InvokeInstance<bool>(presenter, "CanCreateBubbleUiInCurrentContext");

        Assert.That(canCreate, Is.False);
    }

    [Test]
    public void TemporaryEditObjectPresenter_ShouldAllowRuntimeBubbleUiForTests()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_Temp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        bool canCreate = InvokeInstance<bool>(presenter, "CanCreateBubbleUiInCurrentContext");

        Assert.That(canCreate, Is.True);
    }

    [Test]
    public void TemporaryEditObjectPresenter_ShowText_ShouldMakeBubbleVisible()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_ShowTextTemp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        bool shown = (bool)presenterType
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "测试气泡", -1f, true });

        Assert.That(shown, Is.True);
        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.True);
        Assert.That(GetPublicProperty<string>(presenter, "CurrentBubbleText"), Does.Contain("测试气泡"));
        Assert.That(GetPublicProperty<string>(presenter, "LastPresentedText"), Does.Contain("测试气泡"));
    }

    [Test]
    public void TemporaryEditObjectPresenter_ShowConversationImmediate_ShouldMakeBubbleVisible()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_ShowConversationTemp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        bool shown = (bool)presenterType
            .GetMethod("ShowConversationImmediate", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "即时对话" });

        Assert.That(shown, Is.True);
        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.True);
        Assert.That(GetPublicProperty<string>(presenter, "CurrentBubbleText"), Does.Contain("即时对话"));
        Assert.That(GetPublicProperty<string>(presenter, "LastPresentedText"), Does.Contain("即时对话"));
    }

    [Test]
    public void TemporaryEditObjectPresenter_ShowText_ShouldClearStalePromptSuppression()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_StaleSuppressionTemp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        SetPrivateField(presenter, "_suppressAmbientWhilePromptFocused", true);

        bool shown = (bool)presenterType
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "残留抑制清理", -1f, true });

        Assert.That(shown, Is.True);
        Assert.That(GetPublicProperty<bool>(presenter, "IsBubbleVisible"), Is.True);
        Assert.That(GetPrivateField<bool>(presenter, "_suppressAmbientWhilePromptFocused"), Is.False);
        Assert.That(GetPublicProperty<string>(presenter, "LastPresentedText"), Does.Contain("残留抑制清理"));
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

    private static T InvokeInstance<T>(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return (T)method.Invoke(target, args);
    }

    private static T GetPublicProperty<T>(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return (T)property.GetValue(target);
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
