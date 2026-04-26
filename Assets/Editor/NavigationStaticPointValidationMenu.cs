#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NavigationStaticPointValidationMenu
{
    private const string PendingActionKey = "Sunset.NavigationStaticValidation.PendingAction";
    private const string PendingActionValue = "RunStaticPointValidation";
    private const string LiveValidationPendingActionKey = "Sunset.NavigationLiveValidation.PendingAction";
    private const string ConsoleErrorPauseSnapshotKey = "Sunset.NavigationStaticValidation.ConsoleErrorPauseSnapshot";
    private const string ConsoleErrorPauseOverrideKey = "Sunset.NavigationStaticValidation.ConsoleErrorPauseOverrideActive";
    private const string LogPrefix = "[NavStaticValidation]";
    private static readonly Type ConsoleWindowType = Type.GetType("UnityEditor.ConsoleWindow, UnityEditor");
    private static readonly MethodInfo GetConsoleErrorPauseMethod =
        ConsoleWindowType?.GetMethod("GetConsoleErrorPause", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    private static readonly MethodInfo SetConsoleErrorPauseMethod =
        ConsoleWindowType?.GetMethod("SetConsoleErrorPause", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

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

        ApplyConsoleValidationGuard();

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
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            RestoreConsoleValidationGuard();
        }

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
            RestoreConsoleValidationGuard();
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
            RestoreConsoleValidationGuard();
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

    private static void ApplyConsoleValidationGuard()
    {
        if (EditorPrefs.GetBool(ConsoleErrorPauseOverrideKey, false))
        {
            return;
        }

        if (!TryGetConsoleErrorPause(out bool previousValue))
        {
            Debug.LogWarning($"{LogPrefix} console_error_pause_guard_unavailable");
            return;
        }

        EditorPrefs.SetBool(ConsoleErrorPauseSnapshotKey, previousValue);
        EditorPrefs.SetBool(ConsoleErrorPauseOverrideKey, true);
        TrySetConsoleErrorPause(false);
        Debug.Log($"{LogPrefix} console_error_pause_disabled_for_validation previous={previousValue}");
    }

    private static void RestoreConsoleValidationGuard()
    {
        if (!EditorPrefs.GetBool(ConsoleErrorPauseOverrideKey, false))
        {
            return;
        }

        bool previousValue = EditorPrefs.GetBool(ConsoleErrorPauseSnapshotKey, false);
        if (TrySetConsoleErrorPause(previousValue))
        {
            Debug.Log($"{LogPrefix} console_error_pause_restored value={previousValue}");
        }
        else
        {
            Debug.LogWarning($"{LogPrefix} console_error_pause_restore_failed value={previousValue}");
        }

        EditorPrefs.DeleteKey(ConsoleErrorPauseOverrideKey);
        EditorPrefs.DeleteKey(ConsoleErrorPauseSnapshotKey);
    }

    private static bool TryGetConsoleErrorPause(out bool isEnabled)
    {
        isEnabled = false;
        if (GetConsoleErrorPauseMethod == null)
        {
            return false;
        }

        try
        {
            object value = GetConsoleErrorPauseMethod.Invoke(null, null);
            if (value is bool boolValue)
            {
                isEnabled = boolValue;
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"{LogPrefix} get_console_error_pause_failed error={ex.GetType().Name}:{ex.Message}");
        }

        return false;
    }

    private static bool TrySetConsoleErrorPause(bool isEnabled)
    {
        if (SetConsoleErrorPauseMethod == null)
        {
            return false;
        }

        try
        {
            SetConsoleErrorPauseMethod.Invoke(null, new object[] { isEnabled });
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"{LogPrefix} set_console_error_pause_failed error={ex.GetType().Name}:{ex.Message} value={isEnabled}");
            return false;
        }
    }
}
#endif
