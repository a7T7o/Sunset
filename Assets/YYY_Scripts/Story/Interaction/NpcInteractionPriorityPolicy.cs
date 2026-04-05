using UnityEngine;

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

        public static StoryPhase ResolveCurrentStoryPhase()
        {
#if UNITY_EDITOR
            if (s_validationPhaseOverride.HasValue)
            {
                return s_validationPhaseOverride.Value;
            }
#endif
            StoryManager storyManager = Object.FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            return storyManager != null ? storyManager.CurrentPhase : StoryPhase.None;
        }

        public static bool IsFormalPriorityPhase(StoryPhase phase)
        {
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
            return IsFormalPriorityPhase(phase);
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

            return dialogueInteractable.CanInteract(context);
        }

#if UNITY_EDITOR
        public static void SetEditorValidationPhaseOverride(StoryPhase? phase)
        {
            s_validationPhaseOverride = phase;
        }
#endif
    }
}
