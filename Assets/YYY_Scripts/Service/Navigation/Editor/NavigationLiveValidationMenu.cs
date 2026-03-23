#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class NavigationLiveValidationMenu
{
    [MenuItem("Tools/Sunset/Navigation/Run Live Validation")]
    private static void RunLiveValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[NavValidation] 请先进入 Play Mode，再执行 live validation。");
            return;
        }

        NavigationLiveValidationRunner.BeginOrRestart();
    }

    [MenuItem("Tools/Sunset/Navigation/Probe Setup/Player Avoids Moving NPC")]
    private static void SetupPlayerAvoidsMovingNpcProbe()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[NavValidation] 请先进入 Play Mode，再执行 probe setup。");
            return;
        }

        NavigationLiveValidationRunner.SetupPlayerAvoidsMovingNpcProbe();
    }

    [MenuItem("Tools/Sunset/Navigation/Probe Setup/NPC Avoids Player")]
    private static void SetupNpcAvoidsPlayerProbe()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[NavValidation] 请先进入 Play Mode，再执行 probe setup。");
            return;
        }

        NavigationLiveValidationRunner.SetupNpcAvoidsPlayerProbe();
    }

    [MenuItem("Tools/Sunset/Navigation/Probe Setup/NPC NPC Crossing")]
    private static void SetupNpcNpcCrossingProbe()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[NavValidation] 请先进入 Play Mode，再执行 probe setup。");
            return;
        }

        NavigationLiveValidationRunner.SetupNpcNpcCrossingProbe();
    }
}
#endif
