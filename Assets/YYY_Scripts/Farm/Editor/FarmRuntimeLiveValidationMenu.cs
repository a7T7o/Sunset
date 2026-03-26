#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class FarmRuntimeLiveValidationMenu
{
    [MenuItem("Tools/Sunset/Farm/Run Runtime Feedback Live Validation")]
    private static void RunRuntimeFeedbackLiveValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[FarmRuntimeLive] 请先进入 Play Mode，再执行运行时与反馈链 live 验收。");
            return;
        }

        FarmRuntimeLiveValidationRunner.BeginOrRestart();
    }

    [MenuItem("Tools/Sunset/Farm/Run Hover Occlusion Live Validation")]
    private static void RunHoverOcclusionLiveValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[FarmRuntimeLive] 请先进入 Play Mode，再执行 hover 遮挡 live 验收。");
            return;
        }

        FarmRuntimeLiveValidationRunner.BeginHoverOnly();
    }
}
#endif
