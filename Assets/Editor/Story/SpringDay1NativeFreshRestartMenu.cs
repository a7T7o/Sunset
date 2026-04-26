#if UNITY_EDITOR
using FarmGame.Data.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Sunset.Editor.Story
{
    internal static class SpringDay1NativeFreshRestartMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Restart Spring Day1 Native Fresh";
        private const string TownScenePath = "Assets/000_Scenes/Town.unity";

        [MenuItem(MenuPath)]
        private static void RestartNativeFresh()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.OpenScene(TownScenePath, OpenSceneMode.Single);
                Debug.Log("[SpringDay1NativeFreshRestart] 已在编辑态对齐到 Town 场景；下一次 Play 将直接从 Town 原生开局进入。");
                return;
            }

            bool started = SaveManager.Instance != null && SaveManager.Instance.RestartToFreshGame();
            if (!started)
            {
                Debug.LogWarning("[SpringDay1NativeFreshRestart] 未能启动原生重开；可能当前已在重开流程中。");
                return;
            }

            Debug.Log("[SpringDay1NativeFreshRestart] 已请求重开到 Town 原生开局。");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRestartNativeFresh()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }
    }
}
#endif
