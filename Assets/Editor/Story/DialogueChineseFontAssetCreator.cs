using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Sunset.Editor.Story
{
    public static class DialogueChineseFontAssetCreator
    {
        private const string SourceFontPath = "Assets/111_Data/UI/Fonts/Dialogue/simhei.ttf";
        private const string OutputFolderPath = "Assets/TextMesh Pro/Resources/Fonts & Materials";
        private const string OutputAssetPath = OutputFolderPath + "/DialogueChinese SDF.asset";
        private const string AutoCreateSessionKey = "Sunset.Story.DialogueChineseFontAssetCreator.AutoCreateAttempted";

        [InitializeOnLoadMethod]
        private static void AutoCreateDialogueChineseFontAsset()
        {
            Debug.Log("[DialogueChineseFontAssetCreator] AutoCreateDialogueChineseFontAsset 已触发。");
            if (SessionState.GetBool(AutoCreateSessionKey, false))
            {
                Debug.Log("[DialogueChineseFontAssetCreator] 本次会话已尝试过自动创建，跳过。");
                return;
            }

            SessionState.SetBool(AutoCreateSessionKey, true);
            EditorApplication.delayCall += TryAutoCreateDialogueChineseFontAsset;
        }

        [MenuItem("Sunset/Story/生成对话专用中文TMP字体")]
        public static void CreateDialogueChineseFontAsset()
        {
            CreateDialogueChineseFontAssetInternal(showDialog: true, overwriteExisting: true);
        }

        private static void TryAutoCreateDialogueChineseFontAsset()
        {
            Debug.Log("[DialogueChineseFontAssetCreator] TryAutoCreateDialogueChineseFontAsset 开始执行。");
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.Log("[DialogueChineseFontAssetCreator] 编辑器仍在编译或更新，延后重试。");
                EditorApplication.delayCall += TryAutoCreateDialogueChineseFontAsset;
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(OutputAssetPath) != null)
            {
                Debug.Log("[DialogueChineseFontAssetCreator] 目标字体资产已存在，跳过自动创建。");
                return;
            }

            CreateDialogueChineseFontAssetInternal(showDialog: false, overwriteExisting: false);
        }

        private static void CreateDialogueChineseFontAssetInternal(bool showDialog, bool overwriteExisting)
        {
            Debug.Log($"[DialogueChineseFontAssetCreator] 开始创建字体资产，源字体：{SourceFontPath}");
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(SourceFontPath);
            if (sourceFont == null)
            {
                if (showDialog)
                {
                    EditorUtility.DisplayDialog("生成失败", $"未找到源字体：{SourceFontPath}", "确定");
                }

                Debug.LogError($"[DialogueChineseFontAssetCreator] 未找到源字体：{SourceFontPath}");
                return;
            }

            if (!AssetDatabase.IsValidFolder(OutputFolderPath))
            {
                Directory.CreateDirectory(OutputFolderPath);
                AssetDatabase.Refresh();
            }

            TMP_FontAsset existingFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(OutputAssetPath);
            if (existingFontAsset != null)
            {
                if (!overwriteExisting)
                {
                    return;
                }

                if (showDialog && !EditorUtility.DisplayDialog("字体已存在", "对话专用中文TMP字体已存在，是否覆盖重建？", "覆盖", "取消"))
                {
                    Selection.activeObject = existingFontAsset;
                    EditorGUIUtility.PingObject(existingFontAsset);
                    return;
                }

                AssetDatabase.DeleteAsset(OutputAssetPath);
            }

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic,
                true);

            if (fontAsset == null)
            {
                if (showDialog)
                {
                    EditorUtility.DisplayDialog("生成失败", "TMP 字体资产创建失败。", "确定");
                }

                Debug.LogError("[DialogueChineseFontAssetCreator] TMP 字体资产创建失败。");
                return;
            }

            fontAsset.name = "DialogueChinese SDF";
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            fontAsset.isMultiAtlasTexturesEnabled = true;
            AssetDatabase.CreateAsset(fontAsset, OutputAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            TMP_FontAsset createdFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(OutputAssetPath);
            Selection.activeObject = createdFontAsset;
            EditorGUIUtility.PingObject(createdFontAsset);

            Debug.Log($"[DialogueChineseFontAssetCreator] 已生成对话专用中文TMP字体：{OutputAssetPath}");

            if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    "生成完成",
                    $"已生成对话专用中文TMP字体：\n{OutputAssetPath}\n\n它会和 LiberationSans SDF 一样出现在 TMP Fonts & Materials 里，但不会自动影响全局；后续只需手动拖到对话UI的 Font Asset 字段即可。",
                    "确定");
            }
        }
    }
}
