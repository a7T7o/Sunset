#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NavigationLiveValidationMenu
{
    private const string PendingActionKey = "Sunset.NavigationLiveValidation.PendingAction";
    private const string NpcInformalChatExclusiveValidationLockKey = "Sunset.NpcInformalChatValidation.Active";
    private enum PendingAction
    {
        None = 0,
        RunAll = 1,
        RunFinalAcceptancePack = 17,
        SetupPlayerAvoidsMovingNpc = 2,
        RunRealInputPush = 3,
        RunRealInputSingleNpcNear = 4,
        RunRealInputCrowd = 5,
        SetupNpcAvoidsPlayer = 6,
        SetupNpcNpcCrossing = 7,
        RunRawRealInputPush = 8,
        RunRawRealInputSingleNpcNear = 9,
        RunRawRealInputCrowd = 10,
        RunRealInputGroundPointMatrix = 11,
        RunRawRealInputGroundPointMatrix = 12,
        RunRealInputEndpointNpcOccupied = 13,
        RunRawRealInputEndpointNpcOccupied = 14,
        RunRealInputPassableCorridor = 15,
        RunRealInputStaticNpcWall = 16
    }

    private const string LogPrefix = "[NavValidation]";

    static NavigationLiveValidationMenu()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        EditorApplication.delayCall += ResumePendingActionAfterReload;
    }

    [MenuItem("Tools/Sunset/Navigation/Run Live Validation")]
    private static void RunLiveValidation()
    {
        RunOrQueue(PendingAction.RunAll);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Final Player Navigation Acceptance Pack")]
    private static void RunFinalPlayerNavigationAcceptancePack()
    {
        RunOrQueue(PendingAction.RunFinalAcceptancePack);
    }

    [MenuItem("Tools/Sunset/Navigation/Probe Setup/Player Avoids Moving NPC")]
    private static void SetupPlayerAvoidsMovingNpcProbe()
    {
        RunOrQueue(PendingAction.SetupPlayerAvoidsMovingNpc);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Push Validation")]
    private static void RunRealInputPushValidation()
    {
        RunOrQueue(PendingAction.RunRealInputPush);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Single NPC Near Validation")]
    private static void RunRealInputSingleNpcNearValidation()
    {
        RunOrQueue(PendingAction.RunRealInputSingleNpcNear);
    }

    [MenuItem("Tools/Sunset/Navigation/Legacy/Run Real Input Crowd Blocked-Wall Stress")]
    private static void RunRealInputCrowdValidation()
    {
        RunOrQueue(PendingAction.RunRealInputCrowd);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Raw Real Input Push Validation")]
    private static void RunRawRealInputPushValidation()
    {
        RunOrQueue(PendingAction.RunRawRealInputPush);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Raw Real Input Single NPC Near Validation")]
    private static void RunRawRealInputSingleNpcNearValidation()
    {
        RunOrQueue(PendingAction.RunRawRealInputSingleNpcNear);
    }

    [MenuItem("Tools/Sunset/Navigation/Legacy/Run Raw Real Input Crowd Blocked-Wall Stress")]
    private static void RunRawRealInputCrowdValidation()
    {
        RunOrQueue(PendingAction.RunRawRealInputCrowd);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Ground Point Matrix")]
    private static void RunRealInputGroundPointMatrixValidation()
    {
        RunOrQueue(PendingAction.RunRealInputGroundPointMatrix);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Raw Real Input Ground Point Matrix")]
    private static void RunRawRealInputGroundPointMatrixValidation()
    {
        RunOrQueue(PendingAction.RunRawRealInputGroundPointMatrix);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Endpoint NPC Occupied Validation")]
    private static void RunRealInputEndpointNpcOccupiedValidation()
    {
        RunOrQueue(PendingAction.RunRealInputEndpointNpcOccupied);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Raw Real Input Endpoint NPC Occupied Validation")]
    private static void RunRawRealInputEndpointNpcOccupiedValidation()
    {
        RunOrQueue(PendingAction.RunRawRealInputEndpointNpcOccupied);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Passable Corridor Validation")]
    private static void RunRealInputPassableCorridorValidation()
    {
        RunOrQueue(PendingAction.RunRealInputPassableCorridor);
    }

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Static NPC Wall Validation")]
    private static void RunRealInputStaticNpcWallValidation()
    {
        RunOrQueue(PendingAction.RunRealInputStaticNpcWall);
    }

    [MenuItem("Tools/Sunset/Navigation/Probe Setup/NPC Avoids Player")]
    private static void SetupNpcAvoidsPlayerProbe()
    {
        RunOrQueue(PendingAction.SetupNpcAvoidsPlayer);
    }

    [MenuItem("Tools/Sunset/Navigation/Probe Setup/NPC NPC Crossing")]
    private static void SetupNpcNpcCrossingProbe()
    {
        RunOrQueue(PendingAction.SetupNpcNpcCrossing);
    }

    private static void RunOrQueue(PendingAction action)
    {
        if (EditorPrefs.GetBool(NpcInformalChatExclusiveValidationLockKey, false))
        {
            if (EditorPrefs.HasKey(PendingActionKey))
            {
                EditorPrefs.DeleteKey(PendingActionKey);
            }

            Debug.Log($"{LogPrefix} suppressed_by_npc_validation action={action}");
            return;
        }

        if (EditorApplication.isPlaying)
        {
            ExecuteAction(action);
            return;
        }

        EditorPrefs.SetString(PendingActionKey, action.ToString());

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log($"{LogPrefix} queued_action={action} entering_play_mode");
            EditorApplication.isPlaying = true;
            return;
        }

        Debug.Log($"{LogPrefix} queued_action={action} waiting_for_play_mode");
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode)
        {
            return;
        }

        EditorApplication.delayCall += DispatchPendingActionAfterPlayModeEntered;
    }

    private static void ResumePendingActionAfterReload()
    {
        if (EditorPrefs.GetBool(NpcInformalChatExclusiveValidationLockKey, false))
        {
            if (EditorPrefs.HasKey(PendingActionKey))
            {
                EditorPrefs.DeleteKey(PendingActionKey);
            }

            Debug.Log($"{LogPrefix} pending_action_suppressed_by_npc_validation");
            return;
        }

        if (!EditorPrefs.HasKey(PendingActionKey))
        {
            return;
        }

        if (EditorApplication.isPlaying)
        {
            DispatchPendingAction("delay_call_already_playing");
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

    private static void DispatchPendingActionAfterPlayModeEntered()
    {
        DispatchPendingAction("entered_play_mode");
    }

    private static void DispatchPendingAction(string source)
    {
        if (EditorPrefs.GetBool(NpcInformalChatExclusiveValidationLockKey, false))
        {
            if (EditorPrefs.HasKey(PendingActionKey))
            {
                EditorPrefs.DeleteKey(PendingActionKey);
            }

            Debug.Log($"{LogPrefix} dispatch_suppressed_by_npc_validation source={source}");
            return;
        }

        if (!EditorApplication.isPlaying || !EditorPrefs.HasKey(PendingActionKey))
        {
            return;
        }

        string actionText = EditorPrefs.GetString(PendingActionKey, string.Empty);
        EditorPrefs.DeleteKey(PendingActionKey);
        if (string.IsNullOrEmpty(actionText))
        {
            return;
        }

        if (!System.Enum.TryParse(actionText, out PendingAction action))
        {
            Debug.LogWarning($"{LogPrefix} unknown_editor_pending_action={actionText} source={source}");
            return;
        }

        Debug.Log($"{LogPrefix} editor_dispatch_pending_action={action} source={source}");
        ExecuteAction(action);
    }

    private static void ExecuteAction(PendingAction action)
    {
        switch (action)
        {
            case PendingAction.RunAll:
                NavigationLiveValidationRunner.BeginOrRestart();
                break;

            case PendingAction.RunFinalAcceptancePack:
                NavigationLiveValidationRunner.BeginFinalAcceptancePack();
                break;

            case PendingAction.SetupPlayerAvoidsMovingNpc:
                NavigationLiveValidationRunner.SetupPlayerAvoidsMovingNpcProbe();
                break;

            case PendingAction.RunRealInputPush:
                NavigationLiveValidationRunner.RunRealInputPlayerAvoidsMovingNpcProbe();
                break;

            case PendingAction.RunRealInputSingleNpcNear:
                NavigationLiveValidationRunner.RunRealInputPlayerSingleNpcNearProbe();
                break;

            case PendingAction.RunRealInputCrowd:
                NavigationLiveValidationRunner.RunLegacyRealInputPlayerCrowdBlockedWallStressProbe();
                break;

            case PendingAction.RunRawRealInputPush:
                NavigationLiveValidationRunner.RunRawRealInputPlayerAvoidsMovingNpcProbe();
                break;

            case PendingAction.RunRawRealInputSingleNpcNear:
                NavigationLiveValidationRunner.RunRawRealInputPlayerSingleNpcNearProbe();
                break;

            case PendingAction.RunRawRealInputCrowd:
                NavigationLiveValidationRunner.RunLegacyRawRealInputPlayerCrowdBlockedWallStressProbe();
                break;

            case PendingAction.RunRealInputGroundPointMatrix:
                NavigationLiveValidationRunner.RunRealInputGroundPointMatrixProbe();
                break;

            case PendingAction.RunRawRealInputGroundPointMatrix:
                NavigationLiveValidationRunner.RunRawRealInputGroundPointMatrixProbe();
                break;

            case PendingAction.RunRealInputEndpointNpcOccupied:
                NavigationLiveValidationRunner.RunRealInputPlayerEndpointNpcOccupiedProbe();
                break;

            case PendingAction.RunRawRealInputEndpointNpcOccupied:
                NavigationLiveValidationRunner.RunRawRealInputPlayerEndpointNpcOccupiedProbe();
                break;

            case PendingAction.RunRealInputPassableCorridor:
                NavigationLiveValidationRunner.RunRealInputPlayerPassableCorridorProbe();
                break;

            case PendingAction.RunRealInputStaticNpcWall:
                NavigationLiveValidationRunner.RunRealInputPlayerStaticNpcWallProbe();
                break;

            case PendingAction.SetupNpcAvoidsPlayer:
                NavigationLiveValidationRunner.SetupNpcAvoidsPlayerProbe();
                break;

            case PendingAction.SetupNpcNpcCrossing:
                NavigationLiveValidationRunner.SetupNpcNpcCrossingProbe();
                break;
        }
    }
}
#endif
