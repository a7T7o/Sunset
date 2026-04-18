using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
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

    [Test]
    public void TemporaryEditObjectPresenter_ShowText_ShouldRefreshLegacyBubbleSprites()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_LegacyBubbleTemp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        CreateLegacyBubbleUi(tempObject);

        bool shown = (bool)presenterType
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "旧壳补口验证", -1f, true });

        Assert.That(shown, Is.True);

        Transform canvasTransform = tempObject.transform.Find("NPCBubbleCanvas");
        Assert.That(canvasTransform, Is.Not.Null);
        Assert.That(tempObject.transform.Cast<Transform>().Count(child => child.name == "NPCBubbleCanvas"), Is.EqualTo(1));

        Image shadowBody = canvasTransform.Find("BubbleRoot/ShadowBody").GetComponent<Image>();
        Image borderBody = canvasTransform.Find("BubbleRoot/BorderBody").GetComponent<Image>();
        Image fillBody = canvasTransform.Find("BubbleRoot/FillBody").GetComponent<Image>();
        Image shadowTail = canvasTransform.Find("BubbleRoot/ShadowTail").GetComponent<Image>();
        Image borderTail = canvasTransform.Find("BubbleRoot/BorderTail").GetComponent<Image>();
        Image fillTail = canvasTransform.Find("BubbleRoot/FillTail").GetComponent<Image>();
        TextMeshProUGUI bubbleText = canvasTransform.Find("BubbleRoot/FillBody/BubbleText").GetComponent<TextMeshProUGUI>();

        Assert.That(shadowBody.sprite, Is.Not.Null);
        Assert.That(borderBody.sprite, Is.SameAs(shadowBody.sprite));
        Assert.That(fillBody.sprite, Is.SameAs(shadowBody.sprite));
        Assert.That(shadowBody.type, Is.EqualTo(Image.Type.Sliced));
        Assert.That(borderBody.type, Is.EqualTo(Image.Type.Sliced));
        Assert.That(fillBody.type, Is.EqualTo(Image.Type.Sliced));

        Assert.That(shadowTail.sprite, Is.Not.Null);
        Assert.That(borderTail.sprite, Is.SameAs(shadowTail.sprite));
        Assert.That(fillTail.sprite, Is.SameAs(shadowTail.sprite));
        Assert.That(shadowTail.sprite, Is.Not.SameAs(shadowBody.sprite));
        Assert.That(shadowTail.type, Is.EqualTo(Image.Type.Simple));
        Assert.That(borderTail.type, Is.EqualTo(Image.Type.Simple));
        Assert.That(fillTail.type, Is.EqualTo(Image.Type.Simple));

        Assert.That(bubbleText.font, Is.Not.Null);
        Assert.That(bubbleText.text, Does.Contain("旧壳补口验证"));
    }

    [Test]
    public void TemporaryEditObjectPresenter_ShowText_ShouldRecoverCachedBubbleSpritesAfterTheyGoNull()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_CachedBubbleRefreshTemp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        bool initialShown = (bool)presenterType
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "第一次显示", -1f, true });

        Assert.That(initialShown, Is.True);

        Transform canvasTransform = tempObject.transform.Find("NPCBubbleCanvas");
        Assert.That(canvasTransform, Is.Not.Null);

        Image[] images =
        {
            canvasTransform.Find("BubbleRoot/ShadowBody").GetComponent<Image>(),
            canvasTransform.Find("BubbleRoot/ShadowTail").GetComponent<Image>(),
            canvasTransform.Find("BubbleRoot/BorderBody").GetComponent<Image>(),
            canvasTransform.Find("BubbleRoot/BorderTail").GetComponent<Image>(),
            canvasTransform.Find("BubbleRoot/FillBody").GetComponent<Image>(),
            canvasTransform.Find("BubbleRoot/FillTail").GetComponent<Image>()
        };

        for (int index = 0; index < images.Length; index++)
        {
            Assert.That(images[index], Is.Not.Null);
            images[index].sprite = null;
            images[index].type = Image.Type.Simple;
        }

        bool recoveredShown = (bool)presenterType
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "第二次显示", -1f, true });

        Assert.That(recoveredShown, Is.True);

        Image shadowBody = canvasTransform.Find("BubbleRoot/ShadowBody").GetComponent<Image>();
        Image borderBody = canvasTransform.Find("BubbleRoot/BorderBody").GetComponent<Image>();
        Image fillBody = canvasTransform.Find("BubbleRoot/FillBody").GetComponent<Image>();
        Image shadowTail = canvasTransform.Find("BubbleRoot/ShadowTail").GetComponent<Image>();
        Image borderTail = canvasTransform.Find("BubbleRoot/BorderTail").GetComponent<Image>();
        Image fillTail = canvasTransform.Find("BubbleRoot/FillTail").GetComponent<Image>();

        Assert.That(shadowBody.sprite, Is.Not.Null);
        Assert.That(borderBody.sprite, Is.SameAs(shadowBody.sprite));
        Assert.That(fillBody.sprite, Is.SameAs(shadowBody.sprite));
        Assert.That(shadowBody.type, Is.EqualTo(Image.Type.Sliced));
        Assert.That(borderBody.type, Is.EqualTo(Image.Type.Sliced));
        Assert.That(fillBody.type, Is.EqualTo(Image.Type.Sliced));

        Assert.That(shadowTail.sprite, Is.Not.Null);
        Assert.That(borderTail.sprite, Is.SameAs(shadowTail.sprite));
        Assert.That(fillTail.sprite, Is.SameAs(shadowTail.sprite));
        Assert.That(shadowTail.type, Is.EqualTo(Image.Type.Simple));
        Assert.That(borderTail.type, Is.EqualTo(Image.Type.Simple));
        Assert.That(fillTail.type, Is.EqualTo(Image.Type.Simple));
    }

    [Test]
    public void TemporaryEditObjectPresenter_ShowText_ShouldUseStableFontMaterialAndLighterTypography()
    {
        Type presenterType = ResolveTypeOrFail("NPCBubblePresenter");
        tempObject = new GameObject("NpcBubblePresenter_FontFixTemp");
        tempObject.AddComponent<SpriteRenderer>();
        Component presenter = tempObject.AddComponent(presenterType);

        bool shown = (bool)presenterType
            .GetMethod("ShowText", BindingFlags.Instance | BindingFlags.Public)
            .Invoke(presenter, new object[] { "字体修复验证", -1f, true });

        Assert.That(shown, Is.True);

        TextMeshProUGUI bubbleText = GetPrivateField<TextMeshProUGUI>(presenter, "_bubbleText");
        Assert.That(bubbleText, Is.Not.Null);
        Assert.That(bubbleText.font, Is.Not.Null);
        Assert.That(bubbleText.font.name, Does.Contain("DialogueChinese"));
        Assert.That(bubbleText.font.name, Does.Not.Contain("Pixel"));
        Assert.That(bubbleText.fontSharedMaterial, Is.SameAs(bubbleText.font.material));
        Assert.That(bubbleText.characterSpacing, Is.EqualTo(0f).Within(0.001f));
        Assert.That(bubbleText.lineSpacing, Is.EqualTo(-0.8f).Within(0.001f));
        Assert.That(bubbleText.outlineWidth, Is.EqualTo(0.08f).Within(0.001f));
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

    private static void CreateLegacyBubbleUi(GameObject host)
    {
        GameObject canvasObject = new GameObject(
            "NPCBubbleCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasGroup));
        canvasObject.transform.SetParent(host.transform, false);

        GameObject bubbleRoot = new GameObject("BubbleRoot", typeof(RectTransform));
        bubbleRoot.transform.SetParent(canvasObject.transform, false);

        CreateLegacyImageChild(bubbleRoot.transform, "ShadowBody");
        CreateLegacyImageChild(bubbleRoot.transform, "ShadowTail");
        CreateLegacyImageChild(bubbleRoot.transform, "BorderBody");
        CreateLegacyImageChild(bubbleRoot.transform, "BorderTail");
        GameObject fillBody = CreateLegacyImageChild(bubbleRoot.transform, "FillBody");
        CreateLegacyImageChild(bubbleRoot.transform, "FillTail");

        GameObject bubbleText = new GameObject("BubbleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        bubbleText.transform.SetParent(fillBody.transform, false);
    }

    private static GameObject CreateLegacyImageChild(Transform parent, string name)
    {
        GameObject child = new GameObject(name, typeof(RectTransform), typeof(Image));
        child.transform.SetParent(parent, false);
        return child;
    }
}
