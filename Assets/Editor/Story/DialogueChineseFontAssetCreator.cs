using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Sunset.Editor.Story
{
    public static class DialogueChineseFontAssetCreator
    {
        private const string OutputFolderPath = "Assets/TextMesh Pro/Resources/Fonts & Materials";
        private const string AutoCreateSessionKey = "Sunset.Story.DialogueChineseFontAssetCreator.AutoCreateAttempted";

        private sealed class FontAssetProfile
        {
            public FontAssetProfile(
                string displayName,
                string sourceFontPath,
                string outputAssetPath,
                int samplingPointSize,
                int padding,
                int atlasWidth,
                int atlasHeight)
            {
                DisplayName = displayName;
                SourceFontPath = sourceFontPath;
                OutputAssetPath = outputAssetPath;
                SamplingPointSize = samplingPointSize;
                Padding = padding;
                AtlasWidth = atlasWidth;
                AtlasHeight = atlasHeight;
            }

            public string DisplayName { get; }
            public string SourceFontPath { get; }
            public string OutputAssetPath { get; }
            public int SamplingPointSize { get; }
            public int Padding { get; }
            public int AtlasWidth { get; }
            public int AtlasHeight { get; }
            public string AssetName => Path.GetFileNameWithoutExtension(OutputAssetPath);
        }

        private static readonly FontAssetProfile DefaultProfile = new(
            "Dialogue Chinese Base",
            "Assets/111_Data/UI/Fonts/Dialogue/simhei.ttf",
            OutputFolderPath + "/DialogueChinese SDF.asset",
            90,
            9,
            1024,
            1024);

        private static readonly FontAssetProfile CleanV2Profile = new(
            "Dialogue Chinese V2",
            "Assets/111_Data/UI/Fonts/Dialogue/simhei.ttf",
            OutputFolderPath + "/DialogueChinese V2 SDF.asset",
            90,
            9,
            2048,
            2048);

        private static readonly FontAssetProfile PixelProfile = new(
            "Dialogue Chinese Pixel",
            "Assets/111_Data/UI/Fonts/Dialogue/Pixel/fusion-pixel-10px-monospaced-zh_hans.ttf",
            OutputFolderPath + "/DialogueChinese Pixel SDF.asset",
            48,
            4,
            1024,
            1024);

        private static readonly FontAssetProfile SoftPixelProfile = new(
            "Dialogue Chinese SoftPixel",
            "Assets/111_Data/UI/Fonts/Dialogue/PixelAlt/ark-pixel-12px-proportional-zh_cn.ttf",
            OutputFolderPath + "/DialogueChinese SoftPixel SDF.asset",
            56,
            5,
            1024,
            1024);

        private static readonly FontAssetProfile BitmapSongProfile = new(
            "Dialogue Chinese BitmapSong",
            "Assets/111_Data/UI/Fonts/Dialogue/PixelAlt/WenQuanYi Bitmap Song 14px.ttf",
            OutputFolderPath + "/DialogueChinese BitmapSong SDF.asset",
            56,
            5,
            1024,
            1024);

        private static readonly FontAssetProfile[] AutoCreateProfiles =
        {
            DefaultProfile,
            CleanV2Profile,
            PixelProfile,
            SoftPixelProfile,
            BitmapSongProfile
        };

        [InitializeOnLoadMethod]
        private static void AutoCreateDialogueChineseFontAssets()
        {
            if (SessionState.GetBool(AutoCreateSessionKey, false))
            {
                return;
            }

            SessionState.SetBool(AutoCreateSessionKey, true);
            EditorApplication.delayCall += TryAutoCreateMissingAssets;
        }

        [MenuItem("Sunset/Story/Create Dialogue Chinese TMP Font")]
        public static void CreateDialogueChineseFontAsset()
        {
            CreateFontAsset(DefaultProfile, showDialog: true, overwriteExisting: true);
        }

        [MenuItem("Sunset/Story/TMP Fonts/Create Dialogue Chinese Base")]
        public static void CreateDefaultProfile()
        {
            CreateFontAsset(DefaultProfile, showDialog: true, overwriteExisting: true);
        }

        [MenuItem("Sunset/Story/TMP Fonts/Create Dialogue Chinese V2")]
        public static void CreateCleanV2Profile()
        {
            CreateFontAsset(CleanV2Profile, showDialog: true, overwriteExisting: true);
        }

        [MenuItem("Sunset/Story/TMP Fonts/Create Dialogue Chinese Pixel")]
        public static void CreatePixelProfile()
        {
            CreateFontAsset(PixelProfile, showDialog: true, overwriteExisting: true);
        }

        [MenuItem("Sunset/Story/TMP Fonts/Create Dialogue Chinese SoftPixel")]
        public static void CreateSoftPixelProfile()
        {
            CreateFontAsset(SoftPixelProfile, showDialog: true, overwriteExisting: true);
        }

        [MenuItem("Sunset/Story/TMP Fonts/Create Dialogue Chinese BitmapSong")]
        public static void CreateBitmapSongProfile()
        {
            CreateFontAsset(BitmapSongProfile, showDialog: true, overwriteExisting: true);
        }

        [MenuItem("Sunset/Story/TMP Fonts/Create All Dialogue Fonts")]
        public static void CreateAllProfiles()
        {
            foreach (FontAssetProfile profile in AutoCreateProfiles)
            {
                CreateFontAsset(profile, showDialog: false, overwriteExisting: true);
            }

            EditorUtility.DisplayDialog(
                "Done",
                "Created Base, V2 and Pixel dialogue TMP fonts. Please inspect Fonts & Materials.",
                "OK");
        }

        private static void TryAutoCreateMissingAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryAutoCreateMissingAssets;
                return;
            }

            foreach (FontAssetProfile profile in AutoCreateProfiles)
            {
                if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(profile.OutputAssetPath) != null)
                {
                    continue;
                }

                CreateFontAsset(profile, showDialog: false, overwriteExisting: false);
            }
        }

        private static void CreateFontAsset(FontAssetProfile profile, bool showDialog, bool overwriteExisting)
        {
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(profile.SourceFontPath);
            if (sourceFont == null)
            {
                string message = $"Source font not found: {profile.SourceFontPath}";
                Debug.LogError($"[DialogueChineseFontAssetCreator] {message}");

                if (showDialog)
                {
                    EditorUtility.DisplayDialog("Create Failed", message, "OK");
                }

                return;
            }

            if (!AssetDatabase.IsValidFolder(OutputFolderPath))
            {
                Directory.CreateDirectory(OutputFolderPath);
                AssetDatabase.Refresh();
            }

            TMP_FontAsset existingFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(profile.OutputAssetPath);
            if (existingFontAsset != null)
            {
                if (!overwriteExisting)
                {
                    return;
                }

                if (showDialog &&
                    !EditorUtility.DisplayDialog(
                        "Font Exists",
                        $"{profile.DisplayName} already exists. Rebuild it?",
                        "Rebuild",
                        "Cancel"))
                {
                    Selection.activeObject = existingFontAsset;
                    EditorGUIUtility.PingObject(existingFontAsset);
                    return;
                }

                AssetDatabase.DeleteAsset(profile.OutputAssetPath);
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                profile.SamplingPointSize,
                profile.Padding,
                GlyphRenderMode.SDFAA,
                profile.AtlasWidth,
                profile.AtlasHeight,
                AtlasPopulationMode.Dynamic,
                true);

            if (fontAsset == null)
            {
                string message = $"{profile.DisplayName} creation failed.";
                Debug.LogError($"[DialogueChineseFontAssetCreator] {message}");

                if (showDialog)
                {
                    EditorUtility.DisplayDialog("Create Failed", message, "OK");
                }

                return;
            }

            fontAsset.name = profile.AssetName;
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            fontAsset.isMultiAtlasTexturesEnabled = true;

            AssetDatabase.CreateAsset(fontAsset, profile.OutputAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            TMP_FontAsset createdFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(profile.OutputAssetPath);
            Selection.activeObject = createdFontAsset;
            EditorGUIUtility.PingObject(createdFontAsset);

            Debug.Log($"[DialogueChineseFontAssetCreator] Created: {profile.OutputAssetPath}");

            if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    "Created",
                    $"Created {profile.DisplayName}:\n{profile.OutputAssetPath}",
                    "OK");
            }
        }
    }
}
