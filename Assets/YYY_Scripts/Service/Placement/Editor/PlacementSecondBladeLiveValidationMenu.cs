#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PlacementSecondBladeLiveValidationMenu
{
    private const string PendingRunScopeKey = "Sunset.PlacementSecondBlade.PendingRunScope";

    private enum PendingRunScope
    {
        None,
        Full,
        SaplingOnly
    }

    private static PendingRunScope pendingRunScope;

    static PlacementSecondBladeLiveValidationMenu()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem("Tools/Sunset/Placement/Run Second Blade Live Validation")]
    private static void RunSecondBladeLiveValidation()
    {
        if (EditorApplication.isPlaying)
        {
            PlacementSecondBladeLiveValidationRunner.BeginOrRestart();
            return;
        }

        SetPendingRunScope(PendingRunScope.Full);

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("[PlacementSecondBlade] 检测到当前仍在 Edit Mode，已申请进入 Play Mode 并将在进入后自动启动 live validation。");
            EditorApplication.EnterPlaymode();
            return;
        }

        Debug.Log("[PlacementSecondBlade] Play Mode 切换中，进入后将自动启动 second-blade live validation。");
    }

    [MenuItem("Tools/Sunset/Placement/Run Sapling Ghost Validation")]
    private static void RunSaplingGhostValidation()
    {
        if (EditorApplication.isPlaying)
        {
            PlacementSecondBladeLiveValidationRunner.BeginSaplingOnly();
            return;
        }

        SetPendingRunScope(PendingRunScope.SaplingOnly);

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("[PlacementSecondBlade] 检测到当前仍在 Edit Mode，已申请进入 Play Mode 并将在进入后自动启动 sapling-only validation。");
            EditorApplication.EnterPlaymode();
            return;
        }

        Debug.Log("[PlacementSecondBlade] Play Mode 切换中，进入后将自动启动 sapling-only validation。");
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        PendingRunScope scope = GetPendingRunScope();
        if (scope == PendingRunScope.None)
        {
            return;
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorApplication.delayCall += StartDeferredRun;
            return;
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            SetPendingRunScope(PendingRunScope.None);
        }
    }

    private static void StartDeferredRun()
    {
        PendingRunScope scope = GetPendingRunScope();
        if (scope == PendingRunScope.None || !EditorApplication.isPlaying)
        {
            return;
        }

        SetPendingRunScope(PendingRunScope.None);

        if (scope == PendingRunScope.SaplingOnly)
        {
            PlacementSecondBladeLiveValidationRunner.BeginSaplingOnly();
            return;
        }

        PlacementSecondBladeLiveValidationRunner.BeginOrRestart();
    }

    private static PendingRunScope GetPendingRunScope()
    {
        pendingRunScope = (PendingRunScope)SessionState.GetInt(PendingRunScopeKey, (int)pendingRunScope);
        return pendingRunScope;
    }

    private static void SetPendingRunScope(PendingRunScope scope)
    {
        pendingRunScope = scope;
        SessionState.SetInt(PendingRunScopeKey, (int)scope);
    }
}
#endif
