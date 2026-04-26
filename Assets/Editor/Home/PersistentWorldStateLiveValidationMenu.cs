#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

internal static class PersistentWorldStateLiveValidationMenu
{
    private const string RunMenuPath = "Tools/Sunset/Runtime/Run World-State Continuity Validation";

    [MenuItem(RunMenuPath)]
    private static void Run()
    {
        if (!EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            Debug.LogWarning("[WorldStateLive] 请先在 Primary 进入 Play Mode，再执行 world-state continuity live 验证。");
            return;
        }

        PersistentWorldStateLiveValidationRunner.BeginOrRestart();
    }

    [MenuItem(RunMenuPath, true)]
    private static bool ValidateRun()
    {
        return EditorApplication.isPlaying && !EditorApplication.isPaused;
    }
}
#endif
