using UnityEngine;
using FarmGame.UI;

namespace Sunset.Story
{
    /// <summary>
    /// NPC own 的交互优先级底座：
    /// 正式剧情阶段优先于闲聊，闲聊优先于环境气泡。
    /// </summary>
    public static class NpcInteractionPriorityPolicy
    {
#if UNITY_EDITOR
        private static StoryPhase? s_validationPhaseOverride;
#endif
        private static StoryManager s_cachedStoryManager;
        private static int s_lastStoryManagerLookupFrame = -1;
        private static PackagePanelTabsUI s_cachedPackagePanelTabs;
        private static int s_lastPackagePanelLookupFrame = -1;
        private static int s_lastBlockingUiEvalFrame = -1;
        private static bool s_cachedBlockingUiOpen;

        public static StoryPhase ResolveCurrentStoryPhase()
        {
#if UNITY_EDITOR
            if (s_validationPhaseOverride.HasValue)
            {
                return s_validationPhaseOverride.Value;
            }
#endif
            StoryManager storyManager = ResolveCachedStoryManager();
            return storyManager != null ? storyManager.CurrentPhase : StoryPhase.None;
        }

        public static bool IsBlockingPageUiOpenCached()
        {
            if (s_lastBlockingUiEvalFrame == Time.frameCount)
            {
                return s_cachedBlockingUiOpen;
            }

            PackagePanelTabsUI packageTabs = ResolveCachedPackagePanelTabs();
            bool packageOpen = packageTabs != null && packageTabs.IsPanelOpen();
            bool boxOpen = FarmGame.UI.BoxPanelUI.ActiveInstance != null && FarmGame.UI.BoxPanelUI.ActiveInstance.IsOpen;

            s_cachedBlockingUiOpen = packageOpen || boxOpen;
            s_lastBlockingUiEvalFrame = Time.frameCount;
            return s_cachedBlockingUiOpen;
        }

        public static bool AllowsResidentFreeInteractionPhase(StoryPhase phase)
        {
            if (phase == StoryPhase.FreeTime)
            {
                return true;
            }

            return phase == StoryPhase.FarmingTutorial
                && SpringDay1Director.Instance != null
                && SpringDay1Director.Instance.IsPostTutorialExploreWindowActive();
        }

        public static bool IsFormalPriorityPhase(StoryPhase phase)
        {
            if (AllowsResidentFreeInteractionPhase(phase))
            {
                return false;
            }

            return phase is StoryPhase.CrashAndMeet
                or StoryPhase.EnterVillage
                or StoryPhase.HealingAndHP
                or StoryPhase.WorkbenchFlashback
                or StoryPhase.FarmingTutorial
                or StoryPhase.DinnerConflict
                or StoryPhase.ReturnAndReminder;
        }

        public static bool ShouldSuppressInformalInteraction(StoryPhase phase)
        {
            return IsFormalPriorityPhase(phase);
        }

        public static bool ShouldSuppressInformalInteraction(
            StoryPhase phase,
            NPCDialogueInteractable dialogueInteractable,
            InteractionContext context)
        {
            return IsFormalPriorityPhase(phase) &&
                   HasFormalDialogueTakeover(dialogueInteractable, context);
        }

        public static bool ShouldSuppressAmbientBubble(StoryPhase phase)
        {
            if (!IsFormalPriorityPhase(phase))
            {
                return false;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return true;
            }

            return HasFocusedFormalDialogueTakeover();
        }

        public static bool ShouldSuppressInformalInteractionForCurrentStory()
        {
            return ShouldSuppressInformalInteraction(ResolveCurrentStoryPhase());
        }

        public static bool ShouldSuppressInformalInteractionForCurrentStory(
            NPCDialogueInteractable dialogueInteractable,
            InteractionContext context)
        {
            return ShouldSuppressInformalInteraction(
                ResolveCurrentStoryPhase(),
                dialogueInteractable,
                context);
        }

        public static bool ShouldSuppressAmbientBubbleForCurrentStory()
        {
            return ShouldSuppressAmbientBubble(ResolveCurrentStoryPhase());
        }

        public static bool HasFormalDialogueTakeover(
            NPCDialogueInteractable dialogueInteractable,
            InteractionContext context)
        {
            if (dialogueInteractable == null || !dialogueInteractable.isActiveAndEnabled)
            {
                return false;
            }

            return dialogueInteractable.HasFormalDialoguePriorityAvailable();
        }

        private static bool HasFocusedFormalDialogueTakeover()
        {
            Transform currentAnchorTarget = SpringDay1ProximityInteractionService.CurrentAnchorTarget;
            if (currentAnchorTarget == null || !SpringDay1ProximityInteractionService.CurrentCanTriggerNow)
            {
                return false;
            }

            NPCDialogueInteractable dialogueInteractable = ResolveFocusedDialogueInteractable(currentAnchorTarget);
            if (dialogueInteractable == null)
            {
                return false;
            }

            return HasFormalDialogueTakeover(dialogueInteractable, null);
        }

        private static NPCDialogueInteractable ResolveFocusedDialogueInteractable(Transform anchorTarget)
        {
            if (anchorTarget == null)
            {
                return null;
            }

            return anchorTarget.GetComponent<NPCDialogueInteractable>()
                   ?? anchorTarget.GetComponentInParent<NPCDialogueInteractable>()
                   ?? anchorTarget.GetComponentInChildren<NPCDialogueInteractable>(true);
        }

        private static StoryManager ResolveCachedStoryManager()
        {
            if (s_cachedStoryManager != null)
            {
                return s_cachedStoryManager;
            }

            StoryManager resolved = Object.FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            if (resolved != null)
            {
                s_cachedStoryManager = resolved;
                s_lastStoryManagerLookupFrame = Time.frameCount;
                return s_cachedStoryManager;
            }

            if (s_lastStoryManagerLookupFrame == Time.frameCount)
            {
                return null;
            }

            s_lastStoryManagerLookupFrame = Time.frameCount;
            return s_cachedStoryManager;
        }

        private static PackagePanelTabsUI ResolveCachedPackagePanelTabs()
        {
            if (s_cachedPackagePanelTabs != null)
            {
                return s_cachedPackagePanelTabs;
            }

            PackagePanelTabsUI resolved = Object.FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
            if (resolved != null)
            {
                s_cachedPackagePanelTabs = resolved;
                s_lastPackagePanelLookupFrame = Time.frameCount;
                return s_cachedPackagePanelTabs;
            }

            if (s_lastPackagePanelLookupFrame == Time.frameCount)
            {
                return null;
            }

            s_lastPackagePanelLookupFrame = Time.frameCount;
            return s_cachedPackagePanelTabs;
        }

#if UNITY_EDITOR
        public static void SetEditorValidationPhaseOverride(StoryPhase? phase)
        {
            s_validationPhaseOverride = phase;
        }
#endif
    }
}
