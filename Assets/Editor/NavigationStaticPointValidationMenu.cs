#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NavigationStaticPointValidationMenu
{
    private const string PendingActionKey = "Sunset.NavigationStaticValidation.PendingAction";
    private const string PendingActionValue = "RunStaticPointValidation";
    private const string LiveValidationPendingActionKey = "Sunset.NavigationLiveValidation.PendingAction";
    private const string LogPrefix = "[NavStaticValidation]";

    static NavigationStaticPointValidationMenu()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        EditorApplication.delayCall += ResumePendingActionAfterReload;
    }

    [MenuItem("Tools/Sunset/Navigation/Run Static Point Accuracy Validation")]
    private static void RunStaticPointAccuracyValidation()
    {
        if (HasConflictingLiveValidationPendingAction())
        {
            return;
        }

        if (EditorApplication.isPlaying)
        {
            NavigationStaticPointValidationRunner.BeginOrRestart();
            return;
        }

        EditorPrefs.SetString(PendingActionKey, PendingActionValue);
        NavigationStaticPointValidationRunner.WritePendingLaunchMarker();
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log($"{LogPrefix} queued_action={PendingActionValue} entering_play_mode");
            EditorApplication.isPlaying = true;
            return;
        }

        Debug.Log($"{LogPrefix} queued_action={PendingActionValue} waiting_for_play_mode");
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode)
        {
            return;
        }

        EditorApplication.delayCall += DispatchPendingAction;
    }

    private static void ResumePendingActionAfterReload()
    {
        if (!EditorPrefs.HasKey(PendingActionKey))
        {
            return;
        }

        if (HasConflictingLiveValidationPendingAction())
        {
            EditorPrefs.DeleteKey(PendingActionKey);
            Debug.LogWarning($"{LogPrefix} pending_action_cancelled_by_live_validation_residue");
            return;
        }

        if (EditorApplication.isPlaying)
        {
            DispatchPendingAction();
            return;
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log($"{LogPrefix} pending_action_waiting_for_play_mode");
            return;
        }

        Debug.Log($"{LogPrefix} pending_action_reenter_play_mode");
        EditorApplication.isPlaying = true;
    }

    private static void DispatchPendingAction()
    {
        if (!EditorPrefs.HasKey(PendingActionKey))
        {
            return;
        }

        if (HasConflictingLiveValidationPendingAction())
        {
            EditorPrefs.DeleteKey(PendingActionKey);
            Debug.LogWarning($"{LogPrefix} dispatch_cancelled_by_live_validation_residue");
            return;
        }

        string action = EditorPrefs.GetString(PendingActionKey, string.Empty);
        EditorPrefs.DeleteKey(PendingActionKey);
        if (action != PendingActionValue)
        {
            return;
        }

        Debug.Log($"{LogPrefix} editor_dispatch_pending_action={action}");
        NavigationStaticPointValidationRunner.BeginOrRestart();
    }

    private static bool HasConflictingLiveValidationPendingAction()
    {
        if (!EditorPrefs.HasKey(LiveValidationPendingActionKey))
        {
            return false;
        }

        string liveAction = EditorPrefs.GetString(LiveValidationPendingActionKey, string.Empty);
        Debug.LogWarning($"{LogPrefix} blocked_by_live_validation_pending_action action={liveAction}");
        return true;
    }
}
#endif
