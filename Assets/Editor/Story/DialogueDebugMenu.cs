using System.Collections;
using System.Reflection;
using Sunset.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DialogueDebugMenu
{
    private const string NpcInformalChatValidationLockKey = "Sunset.NpcInformalChatValidation.Active";
    private const string PlayMenuPath = "Sunset/Story/Debug/Play Spring Day1 Dialogue";
    private const string AdvanceMenuPath = "Sunset/Story/Debug/Advance Active Dialogue";
    private const string ForceAdvanceMenuPath = "Sunset/Story/Debug/Force Complete Or Advance Dialogue";
    private const string InteractNpcMenuPath = "Sunset/Story/Debug/Trigger DialogueTestNPC";
    private const string ClickButtonMenuPath = "Sunset/Story/Debug/Invoke Continue Button";
    private const string LogStateMenuPath = "Sunset/Story/Debug/Log Dialogue State";
    private const string BootstrapValidationMenuPath = "Sunset/Story/Debug/Bootstrap Spring Day1 Validation";
    private const string LogValidationSnapshotMenuPath = "Sunset/Story/Debug/Log Spring Day1 Validation Snapshot";
    private const string StepValidationMenuPath = "Sunset/Story/Debug/Step Spring Day1 Validation";
    private const string SequencePath = "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset";

    private static bool IsSuppressedByNpcValidation(string source)
    {
        if (!EditorPrefs.GetBool(NpcInformalChatValidationLockKey, false))
        {
            return false;
        }

        Debug.Log($"[DialogueDebugMenu] 已让出 {source}，当前由 NPC 非正式聊天 trace 独占验证窗口。");
        return true;
    }

    [MenuItem(PlayMenuPath)]
    private static void PlaySpringDay1Dialogue()
    {
        if (IsSuppressedByNpcValidation(nameof(PlaySpringDay1Dialogue)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再触发对话。");
            return;
        }

        DialogueManager dialogueManager = Object.FindFirstObjectByType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("[DialogueDebugMenu] 未找到 DialogueManager。");
            return;
        }

        DialogueSequenceSO sequence = AssetDatabase.LoadAssetAtPath<DialogueSequenceSO>(SequencePath);
        if (sequence == null)
        {
            Debug.LogError($"[DialogueDebugMenu] 无法加载对话资源: {SequencePath}");
            return;
        }

        dialogueManager.PlayDialogue(sequence);
    }

    [MenuItem(AdvanceMenuPath)]
    private static void AdvanceActiveDialogue()
    {
        if (IsSuppressedByNpcValidation(nameof(AdvanceActiveDialogue)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再推进对话。");
            return;
        }

        DialogueManager dialogueManager = Object.FindFirstObjectByType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("[DialogueDebugMenu] 未找到 DialogueManager。");
            return;
        }

        dialogueManager.AdvanceDialogue();
    }

    [MenuItem(ForceAdvanceMenuPath)]
    private static void ForceCompleteOrAdvanceDialogue()
    {
        if (IsSuppressedByNpcValidation(nameof(ForceCompleteOrAdvanceDialogue)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再强制推进对话。");
            return;
        }

        DialogueManager dialogueManager = Object.FindFirstObjectByType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("[DialogueDebugMenu] 未找到 DialogueManager。");
            return;
        }

        dialogueManager.ForceCompleteOrAdvance();
    }

    [MenuItem(InteractNpcMenuPath)]
    private static void TriggerDialogueTestNpc()
    {
        if (IsSuppressedByNpcValidation(nameof(TriggerDialogueTestNpc)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再触发 NPC 对话。");
            return;
        }

        NPCDialogueInteractable interactable = Object.FindFirstObjectByType<NPCDialogueInteractable>();
        if (interactable == null)
        {
            Debug.LogError("[DialogueDebugMenu] 未找到 NPCDialogueInteractable。");
            return;
        }

        interactable.OnInteract(null);
    }

    [MenuItem(ClickButtonMenuPath)]
    private static void InvokeContinueButton()
    {
        if (IsSuppressedByNpcValidation(nameof(InvokeContinueButton)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再触发继续按钮。");
            return;
        }

        DialogueUI dialogueUI = Object.FindFirstObjectByType<DialogueUI>(FindObjectsInactive.Include);
        if (dialogueUI == null)
        {
            Debug.LogError("[DialogueDebugMenu] 未找到 DialogueUI。");
            return;
        }

        Transform buttonTransform = dialogueUI.transform.Find("ContinueButton");
        Button continueButton = buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
        if (continueButton == null)
        {
            Debug.LogError("[DialogueDebugMenu] 未找到 ContinueButton。");
            return;
        }

        continueButton.onClick.Invoke();
    }

    [MenuItem(LogStateMenuPath)]
    private static void LogDialogueState()
    {
        DialogueManager dialogueManager = Object.FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
        DialogueUI dialogueUI = Object.FindFirstObjectByType<DialogueUI>(FindObjectsInactive.Include);
        StoryManager storyManager = Object.FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
        SpringDay1Director day1Director = Object.FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
        HealthSystem healthSystem = Object.FindFirstObjectByType<HealthSystem>(FindObjectsInactive.Include);
        EnergySystem energySystem = Object.FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
        CanvasGroup canvasGroup = dialogueUI != null ? dialogueUI.GetComponent<CanvasGroup>() : null;
        TimeManager timeManager = Object.FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
        GameInputManager gameInputManager = Object.FindFirstObjectByType<GameInputManager>(FindObjectsInactive.Include);
        int continueListenerCount = GetContinueButtonListenerCount(dialogueUI);

        string state =
            $"[DialogueDebugMenu] " +
            $"IsDialogueActive={(dialogueManager != null && dialogueManager.IsDialogueActive)}, " +
            $"IsNodeTyping={(dialogueManager != null && dialogueManager.IsNodeTyping)}, " +
            $"CurrentText='{(dialogueManager != null ? dialogueManager.CurrentTypedText : string.Empty)}', " +
            $"SpeakerVisible={(dialogueUI != null && dialogueUI.IsSpeakerVisible)}, " +
            $"SpeakerText='{(dialogueUI != null ? dialogueUI.CurrentSpeakerName : string.Empty)}', " +
            $"PortraitVisible={(dialogueUI != null && dialogueUI.IsPortraitVisible)}, " +
            $"PortraitSprite='{(dialogueUI != null ? dialogueUI.CurrentPortraitSpriteName : string.Empty)}', " +
            $"PortraitColor={(dialogueUI != null ? dialogueUI.CurrentPortraitColor.ToString() : "n/a")}, " +
            $"DialogueFont='{(dialogueUI != null ? dialogueUI.CurrentDialogueFontName : string.Empty)}', " +
            $"SpeakerFont='{(dialogueUI != null ? dialogueUI.CurrentSpeakerFontName : string.Empty)}', " +
            $"DialogueFontStyle={(dialogueUI != null ? dialogueUI.CurrentDialogueFontStyle.ToString() : "n/a")}, " +
            $"CanvasAlpha={(dialogueUI != null ? dialogueUI.CurrentCanvasAlpha.ToString("F2") : (canvasGroup != null ? canvasGroup.alpha.ToString("F2") : "n/a"))}, " +
            $"CanvasInteractable={(dialogueUI != null && dialogueUI.IsCanvasInteractable)}, " +
            $"CanvasBlocksRaycasts={(dialogueUI != null && dialogueUI.IsCanvasBlockingRaycasts)}, " +
            $"ButtonInteractable={(dialogueUI != null && dialogueUI.IsContinueButtonInteractable)}, " +
            $"ContinueListeners={continueListenerCount}, " +
            $"StoryPhase={(storyManager != null ? storyManager.CurrentPhase.ToString() : "n/a")}, " +
            $"LanguageDecoded={(storyManager != null && storyManager.IsLanguageDecoded)}, " +
            $"Day1Director='{(day1Director != null ? day1Director.GetDebugSummary() : "n/a")}', " +
            $"HP={(healthSystem != null ? $"{healthSystem.CurrentHealth}/{healthSystem.MaxHealth}" : "n/a")}, " +
            $"EP={(energySystem != null ? $"{energySystem.CurrentEnergy}/{energySystem.MaxEnergy}" : "n/a")}, " +
            $"TimePaused={(timeManager != null && timeManager.IsTimePaused())}, " +
            $"PauseDepth={(timeManager != null ? timeManager.GetPauseStackDepth().ToString() : "n/a")}, " +
            $"InputEnabled={(gameInputManager != null && gameInputManager.IsInputEnabledForDebug)}";

        Debug.Log(state);
    }

    [MenuItem(BootstrapValidationMenuPath)]
    private static void BootstrapSpringDay1Validation()
    {
        if (IsSuppressedByNpcValidation(nameof(BootstrapSpringDay1Validation)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再初始化 spring-day1 验收入口。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        Debug.Log($"[DialogueDebugMenu] {runner.BootstrapRuntime()}");
        runner.LogSnapshot("bootstrap");
    }

    [MenuItem(LogValidationSnapshotMenuPath)]
    private static void LogSpringDay1ValidationSnapshot()
    {
        if (IsSuppressedByNpcValidation(nameof(LogSpringDay1ValidationSnapshot)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再记录 spring-day1 验收快照。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        runner.LogSnapshot("manual");
    }

    [MenuItem(StepValidationMenuPath)]
    private static void StepSpringDay1Validation()
    {
        if (IsSuppressedByNpcValidation(nameof(StepSpringDay1Validation)))
        {
            return;
        }

        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueDebugMenu] 请先进入 PlayMode 再执行 spring-day1 验收步骤。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        Debug.Log($"[DialogueDebugMenu] {runner.TriggerRecommendedAction()}");
        runner.LogSnapshot("after-step");
    }

    private static int GetContinueButtonListenerCount(DialogueUI dialogueUI)
    {
        if (dialogueUI == null)
        {
            return 0;
        }

        Transform buttonTransform = dialogueUI.transform.Find("ContinueButton");
        Button continueButton = buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
        if (continueButton == null)
        {
            return 0;
        }

        MethodInfo prepareInvoke = typeof(UnityEngine.Events.UnityEventBase)
            .GetMethod("PrepareInvoke", BindingFlags.Instance | BindingFlags.NonPublic);

        if (prepareInvoke == null)
        {
            return continueButton.onClick.GetPersistentEventCount();
        }

        IList listeners = prepareInvoke.Invoke(continueButton.onClick, null) as IList;
        return listeners != null ? listeners.Count : continueButton.onClick.GetPersistentEventCount();
    }

    [MenuItem(PlayMenuPath, true)]
    private static bool ValidatePlaySpringDay1Dialogue()
    {
        return true;
    }

    [MenuItem(AdvanceMenuPath, true)]
    private static bool ValidateAdvanceActiveDialogue()
    {
        return true;
    }

    [MenuItem(ForceAdvanceMenuPath, true)]
    private static bool ValidateForceCompleteOrAdvanceDialogue()
    {
        return true;
    }

    [MenuItem(InteractNpcMenuPath, true)]
    private static bool ValidateTriggerDialogueTestNpc()
    {
        return true;
    }

    [MenuItem(ClickButtonMenuPath, true)]
    private static bool ValidateInvokeContinueButton()
    {
        return true;
    }

    [MenuItem(LogStateMenuPath, true)]
    private static bool ValidateLogDialogueState()
    {
        return true;
    }

    [MenuItem(BootstrapValidationMenuPath, true)]
    private static bool ValidateBootstrapSpringDay1Validation()
    {
        return true;
    }

    [MenuItem(LogValidationSnapshotMenuPath, true)]
    private static bool ValidateLogSpringDay1ValidationSnapshot()
    {
        return true;
    }

    [MenuItem(StepValidationMenuPath, true)]
    private static bool ValidateStepSpringDay1Validation()
    {
        return true;
    }
}
