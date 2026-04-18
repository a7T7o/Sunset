using System;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class DialogueChineseFontRuntimeBootstrapTests
{
    [Test]
    public void RuntimeBootstrap_ShouldWarmChineseFontAndRedirectTmpDefaultFallback()
    {
        Type tmpSettingsType = ResolveTypeOrFail("TMPro.TMP_Settings");
        PropertyInfo defaultFontProperty = tmpSettingsType.GetProperty("defaultFontAsset", BindingFlags.Static | BindingFlags.Public);
        Assert.That(defaultFontProperty, Is.Not.Null, "应能找到 TMP 默认字体属性。");
        UnityEngine.Object originalDefault = defaultFontProperty.GetValue(null) as UnityEngine.Object;

        try
        {
            Type bootstrapType = ResolveTypeOrFail("Sunset.Story.DialogueChineseFontRuntimeBootstrap");
            UnityEngine.Object fontAsset = bootstrapType
                .GetMethod("EnsureRuntimeFontReady", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                ?.Invoke(null, null) as UnityEngine.Object;

            Assert.That(fontAsset, Is.Not.Null, "应能解析出一个可用的中文运行时字体。");
            Assert.That(fontAsset.name, Does.Contain("DialogueChinese"), "TMP 默认回退应被切到中文字体，而不是继续停在 LiberationSans。");
            Assert.That(defaultFontProperty.GetValue(null), Is.SameAs(fontAsset), "运行时默认回退应改指向当前可用的中文字体。");
            Assert.That(HasUsableAtlas(fontAsset), Is.True, "预热后字体应拥有可用 atlas，而不是继续停在 1x1 占位纹理。");
            MethodInfo hasCharactersMethod = fontAsset.GetType().GetMethod("HasCharacters", new[] { typeof(string) });
            Assert.That(hasCharactersMethod, Is.Not.Null, "应能找到 TMP 字体字符覆盖检查方法。");
            Assert.That((bool)hasCharactersMethod.Invoke(fontAsset, new object[] { "当前任务继续对话工作台摁空格键继续" }), Is.True, "预热后应至少覆盖 Day1 核心中文 UI 探针文字。");
        }
        finally
        {
            defaultFontProperty.SetValue(null, originalDefault);
        }
    }

    [Test]
    public void CanRenderText_ShouldRecoverAfterDynamicAtlasWasCleared()
    {
        TMP_FontAsset sourceFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/DialogueChinese V2 SDF");
        Assert.That(sourceFont, Is.Not.Null, "应能从 Resources 里取到 Day1 主用中文字体。");

        TMP_FontAsset runtimeClone = Object.Instantiate(sourceFont);
        Material runtimeMaterial = null;
        try
        {
            if (sourceFont.material != null)
            {
                runtimeMaterial = Object.Instantiate(sourceFont.material);
                runtimeClone.material = runtimeMaterial;
            }

            runtimeClone.ClearFontAssetData(true);
            Assert.That(HasUsableAtlas(runtimeClone), Is.False, "测试前应先把 atlas 清到 build-like 初始态。");

            Type bootstrapType = ResolveTypeOrFail("Sunset.Story.DialogueChineseFontRuntimeBootstrap");
            MethodInfo canRenderMethod = bootstrapType.GetMethod(
                "CanRenderText",
                BindingFlags.Public | BindingFlags.Static);
            Assert.That(canRenderMethod, Is.Not.Null, "应能找到运行时字体补字入口。");

            bool canRender = (bool)canRenderMethod.Invoke(
                null,
                new object[] { runtimeClone, "晨光照进工坊，继续制作。", "继续制作" });

            Assert.That(canRender, Is.True, "清空 atlas 后也应先尝试动态补字，而不是直接把字体判死。");
            Assert.That(HasUsableAtlas(runtimeClone), Is.True, "动态补字后应重新拿到可用 atlas。");
            Assert.That(runtimeClone.HasCharacters("晨光照进工坊，继续制作。"), Is.True, "当前真实中文探针文字应能被这条字体链覆盖。");
        }
        finally
        {
            if (runtimeMaterial != null)
            {
                Object.DestroyImmediate(runtimeMaterial);
            }

            Object.DestroyImmediate(runtimeClone);
        }
    }

    private static bool HasUsableAtlas(UnityEngine.Object fontAsset)
    {
        if (fontAsset == null)
        {
            return false;
        }

        PropertyInfo atlasProperty = fontAsset.GetType().GetProperty("atlasTextures", BindingFlags.Instance | BindingFlags.Public);
        Assert.That(atlasProperty, Is.Not.Null, "应能找到 TMP atlas 纹理属性。");
        Texture[] atlasTextures = atlasProperty.GetValue(fontAsset) as Texture[];
        if (atlasTextures == null || atlasTextures.Length == 0)
        {
            return false;
        }

        for (int index = 0; index < atlasTextures.Length; index++)
        {
            Texture atlasTexture = atlasTextures[index];
            if (atlasTexture != null && atlasTexture.width > 1 && atlasTexture.height > 1)
            {
                return true;
            }
        }

        return false;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int index = 0; index < assemblies.Length; index++)
        {
            Type resolved = assemblies[index].GetType(fullName, throwOnError: false);
            if (resolved != null)
            {
                return resolved;
            }
        }

        Assert.Fail($"缺少类型：{fullName}");
        return null;
    }
}
