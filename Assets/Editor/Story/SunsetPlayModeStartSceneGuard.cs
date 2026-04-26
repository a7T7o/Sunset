#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story.Editor
{
    [InitializeOnLoad]
    internal static class SunsetPlayModeStartSceneGuard
    {
        private const string SceneRootPrefix = "Assets/000_Scenes/";

        private static SceneAsset s_previousPlayModeStartScene;
        private static bool s_overrodePlayModeStartScene;

        static SunsetPlayModeStartSceneGuard()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingEditMode:
                    AlignPlayModeStartSceneToActiveScene();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    RestorePlayModeStartScene();
                    break;
            }
        }

        private static void AlignPlayModeStartSceneToActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid()
                || string.IsNullOrWhiteSpace(activeScene.path)
                || !activeScene.path.StartsWith(SceneRootPrefix, System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            SceneAsset desiredScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            if (desiredScene == null)
            {
                return;
            }

            if (EditorSceneManager.playModeStartScene == desiredScene)
            {
                return;
            }

            s_previousPlayModeStartScene = EditorSceneManager.playModeStartScene;
            s_overrodePlayModeStartScene = true;
            EditorSceneManager.playModeStartScene = desiredScene;
            Debug.Log($"[SunsetPlayModeStartSceneGuard] 本轮 Play 已对齐当前场景：{activeScene.path}");
        }

        private static void RestorePlayModeStartScene()
        {
            if (!s_overrodePlayModeStartScene)
            {
                return;
            }

            EditorSceneManager.playModeStartScene = s_previousPlayModeStartScene;
            s_previousPlayModeStartScene = null;
            s_overrodePlayModeStartScene = false;
        }
    }
}
#endif
