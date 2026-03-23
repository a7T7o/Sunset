#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class PlacementSecondBladeLiveValidationMenu
{
    [MenuItem("Tools/Sunset/Placement/Run Second Blade Live Validation")]
    private static void RunSecondBladeLiveValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[PlacementSecondBlade] 请先进入 Play Mode，再执行 second-blade live validation。");
            return;
        }

        PlacementSecondBladeLiveValidationRunner.BeginOrRestart();
    }
}
#endif
