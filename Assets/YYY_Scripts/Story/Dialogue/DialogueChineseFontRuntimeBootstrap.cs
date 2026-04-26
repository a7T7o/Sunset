using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Sunset.Story
{
    public static class DialogueChineseFontRuntimeBootstrap
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese BitmapSong SDF"
        };

        private const string WarmupSeedText =
            "这里是落日村先离开矿洞口那边有怪物不适合久留你好谢谢可以不可以继续说几句慢慢说" +
            "选择配方后即可开始制作已完成制作工作台使用工作台剩余材料数量制作中返回休息明天继续" +
            "村长艾拉卡尔马库斯围观闲置小屋疗伤花菜种子开垦浇水木头木剑小木箱子正式对话提示字幕详情标题" +
            "摁空格键继续";

        private static bool _initialized;
        private static TMP_FontAsset _runtimeDefaultFontAsset;
        private static readonly Dictionary<int, TMP_FontAsset> RuntimeFontClones = new();
        private static readonly Dictionary<int, Material> RuntimeMaterialClones = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BootstrapBeforeSceneLoad()
        {
            EnsureRuntimeFontReadyForBootstrap();
        }

        public static TMP_FontAsset EnsureRuntimeFontReady()
        {
            return EnsureRuntimeFontReadyInternal(eagerWarmup: true);
        }

        private static TMP_FontAsset EnsureRuntimeFontReadyForBootstrap()
        {
            // 打包首屏优先“快速给默认字体”，避免 BeforeSceneLoad 做大段动态补字导致卡顿。
            return EnsureRuntimeFontReadyInternal(eagerWarmup: false);
        }

        private static TMP_FontAsset EnsureRuntimeFontReadyInternal(bool eagerWarmup)
        {
            if (_initialized)
            {
                return _runtimeDefaultFontAsset != null ? _runtimeDefaultFontAsset : TMP_Settings.defaultFontAsset;
            }

            _initialized = true;

            TMP_FontAsset preferredFont = eagerWarmup
                ? ResolveWarmPreferredFont()
                : ResolveFastPreferredFont();

            if (preferredFont == null)
            {
                preferredFont = GetRuntimeSafeFontClone(TMP_Settings.defaultFontAsset);
            }

            if (preferredFont != null)
            {
                _runtimeDefaultFontAsset = preferredFont;
                TMP_Settings.defaultFontAsset = preferredFont;
            }

            return preferredFont;
        }

        private static TMP_FontAsset ResolveFastPreferredFont()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                candidate = GetRuntimeSafeFontClone(candidate);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return null;
        }

        public static TMP_FontAsset ResolveBestFontForText(
            string actualText,
            TMP_FontAsset preferredFont = null,
            string fallbackProbeText = null)
        {
            EnsureRuntimeFontReady();

            if (TryResolveUsableFont(preferredFont, actualText, fallbackProbeText, out TMP_FontAsset resolvedPreferred))
            {
                return resolvedPreferred;
            }

            if (TryResolveUsableFont(_runtimeDefaultFontAsset, actualText, fallbackProbeText, out TMP_FontAsset resolvedRuntimeDefault))
            {
                return resolvedRuntimeDefault;
            }

            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (TryResolveUsableFont(candidate, actualText, fallbackProbeText, out TMP_FontAsset resolvedCandidate))
                {
                    return resolvedCandidate;
                }
            }

            if (TryResolveUsableFont(TMP_Settings.defaultFontAsset, actualText, fallbackProbeText, out TMP_FontAsset resolvedDefault))
            {
                return resolvedDefault;
            }

            return _runtimeDefaultFontAsset != null ? _runtimeDefaultFontAsset : TMP_Settings.defaultFontAsset;
        }

        public static bool CanRenderText(TMP_FontAsset fontAsset, string actualText, string fallbackProbeText = null)
        {
            TMP_FontAsset runtimeSafeFont = GetRuntimeSafeFontClone(fontAsset);
            return TryPrepareCharacters(runtimeSafeFont, actualText, fallbackProbeText);
        }

        public static TMP_FontAsset ResolveAndAssign(TextMeshProUGUI target, string fallbackProbeText = null)
        {
            if (target == null)
            {
                return null;
            }

            TMP_FontAsset resolvedFont = ResolveBestFontForText(target.text, target.font, fallbackProbeText);
            if (resolvedFont != null)
            {
                target.font = resolvedFont;
                if (resolvedFont.material != null)
                {
                    target.fontSharedMaterial = resolvedFont.material;
                }
            }

            return resolvedFont;
        }

        private static TMP_FontAsset ResolveWarmPreferredFont()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                candidate = GetRuntimeSafeFontClone(candidate);
                if (WarmAndValidate(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool WarmAndValidate(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null || fontAsset.material == null)
            {
                return false;
            }

            return TryPrepareCharacters(fontAsset, WarmupSeedText, WarmupSeedText);
        }

        private static bool TryResolveUsableFont(
            TMP_FontAsset candidate,
            string actualText,
            string fallbackProbeText,
            out TMP_FontAsset resolved)
        {
            resolved = GetRuntimeSafeFontClone(candidate);
            if (resolved == null)
            {
                return false;
            }

            return TryPrepareCharacters(resolved, actualText, fallbackProbeText);
        }

        private static bool TryPrepareCharacters(TMP_FontAsset fontAsset, string actualText, string fallbackProbeText)
        {
            if (fontAsset == null)
            {
                return false;
            }

            string probeText = BuildProbeText(actualText, fallbackProbeText);
            if (!HasUsableAtlas(fontAsset))
            {
                // 打包版动态 TMP 字体会先清掉 atlas，必须先给它一次补字机会，不能在这里直接判死。
                TryPrimeCharacters(fontAsset);
            }

            TryAddCharactersSafe(fontAsset, probeText, "动态补字");

            if (!HasUsableAtlas(fontAsset))
            {
                return false;
            }

            return fontAsset.HasCharacters(probeText);
        }

        private static string BuildProbeText(string actualText, string fallbackProbeText)
        {
            string sanitizedText = StripTmpTags(actualText);
            if (!string.IsNullOrWhiteSpace(sanitizedText))
            {
                return sanitizedText;
            }

            string normalizedFallback = StripTmpTags(fallbackProbeText);
            if (!string.IsNullOrWhiteSpace(normalizedFallback))
            {
                return normalizedFallback;
            }

            return WarmupSeedText;
        }

        private static string StripTmpTags(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(text.Length);
            bool insideTag = false;
            for (int index = 0; index < text.Length; index++)
            {
                char current = text[index];
                if (current == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (current == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (!char.IsControl(current))
                {
                    builder.Append(current);
                }
            }

            return builder.ToString().Trim();
        }

        private static void TryPrimeCharacters(TMP_FontAsset fontAsset)
        {
            TryAddCharactersSafe(fontAsset, WarmupSeedText, "预热字体");
        }

        private static void TryAddCharactersSafe(TMP_FontAsset fontAsset, string text, string actionLabel)
        {
            if (fontAsset == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                fontAsset.TryAddCharacters(text, out _);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[DialogueChineseFontRuntimeBootstrap] {actionLabel}失败：{fontAsset.name} | {ex.Message}");
            }
        }

        private static TMP_FontAsset GetRuntimeSafeFontClone(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null)
            {
                return null;
            }

            if (!Application.isPlaying)
            {
                return fontAsset;
            }

            if ((fontAsset.hideFlags & HideFlags.DontSaveInBuild) != 0 &&
                fontAsset.name.EndsWith(" (Runtime)", System.StringComparison.Ordinal))
            {
                return fontAsset;
            }

            int sourceId = fontAsset.GetInstanceID();
            if (RuntimeFontClones.TryGetValue(sourceId, out TMP_FontAsset existingClone) && existingClone != null)
            {
                return existingClone;
            }

            TMP_FontAsset runtimeClone = Object.Instantiate(fontAsset);
            runtimeClone.name = $"{fontAsset.name} (Runtime)";
            runtimeClone.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

            Material sourceMaterial = fontAsset.material;
            if (sourceMaterial != null)
            {
                Material runtimeMaterial = Object.Instantiate(sourceMaterial);
                runtimeMaterial.name = $"{sourceMaterial.name} (Runtime)";
                runtimeMaterial.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                runtimeClone.material = runtimeMaterial;
                RuntimeMaterialClones[sourceId] = runtimeMaterial;
            }

            RuntimeFontClones[sourceId] = runtimeClone;
            return runtimeClone;
        }

        private static bool HasUsableAtlas(TMP_FontAsset fontAsset)
        {
            Texture[] atlasTextures = fontAsset.atlasTextures;
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
    }
}
