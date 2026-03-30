using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1BedInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionHint = "睡觉";
        [SerializeField] private float interactionDistance = 1.6f;
        [SerializeField] private int interactionPriority = 24;
        [SerializeField] private bool requireFreeTime = true;

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;

        public bool CanInteract(InteractionContext context)
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return false;
            }

            if (!requireFreeTime)
            {
                return true;
            }

            return StoryManager.Instance.CurrentPhase == StoryPhase.FreeTime;
        }

        public void OnInteract(InteractionContext context)
        {
            if (!CanInteract(context))
            {
                return;
            }

            SpringDay1Director.EnsureRuntime();
            if (SpringDay1Director.Instance != null && SpringDay1Director.Instance.TryTriggerSleepFromBed())
            {
                return;
            }

            TimeManager.Instance.Sleep();
        }

        public string GetInteractionHint(InteractionContext context)
        {
            if (!CanInteract(context))
            {
                return string.Empty;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            if (director == null)
            {
                return interactionHint;
            }

            return director.GetRestInteractionHint(interactionHint);
        }

        public void ConfigureRuntimeDefaults(string hint, float distance, int priority)
        {
            interactionHint = hint;
            interactionDistance = distance;
            interactionPriority = priority;
        }
    }
}
