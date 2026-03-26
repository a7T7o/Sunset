#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NavigationLiveValidationMenu
{
    private const string PendingActionKey = "Sunset.NavigationLiveValidation.PendingAction";
    private enum PendingAction
    {
        None = 0,
        RunAll = 1,
        SetupPlayerAvoidsMovingNpc = 2,
        RunRealInputPush = 3,
        RunRealInputSingleNpcNear = 4,
        RunRealInputCrowd = 5,
        SetupNpcAvoidsPlayer = 6,
        SetupNpcNpcCrossing = 7
    }

    private const string LogPrefix = "[NavValidation]";

    [MenuItem("Tools/Sunset/Navigation/Run Live Validation")]
    private static void RunLiveValidation()
    {
        RunOrQueue(PendingAction.RunAll);
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

    [MenuItem("Tools/Sunset/Navigation/Run Real Input Crowd Validation")]
    private static void RunRealInputCrowdValidation()
    {
        RunOrQueue(PendingAction.RunRealInputCrowd);
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

    private static void ExecuteAction(PendingAction action)
    {
        switch (action)
        {
            case PendingAction.RunAll:
                NavigationLiveValidationRunner.BeginOrRestart();
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
                NavigationLiveValidationRunner.RunRealInputPlayerCrowdPassProbe();
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
