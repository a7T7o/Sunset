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
        [Header("E 键交互")]
        [SerializeField] private bool enableProximityKeyInteraction = true;
        [SerializeField] private KeyCode proximityInteractionKey = KeyCode.E;
        [SerializeField] private float keyInteractionCooldown = 0.15f;
        [SerializeField] private float bubbleRevealDistance = 1.9f;
        [SerializeField] private string bubbleCaption = "休息";
        [SerializeField] private string bubbleDetail = "按 E 回屋休息";

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;

        public bool CanInteract(InteractionContext context)
        {
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                return false;
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return false;
            }

            if (!requireFreeTime)
            {
                return true;
            }

            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null && storyManager.CurrentPhase == StoryPhase.FreeTime;
        }

        public void OnInteract(InteractionContext context)
        {
            SpringDay1WorldHintBubble.HideIfExists(transform);
            if (!CanInteract(context))
            {
                return;
            }

            SpringDay1Director.EnsureRuntime();
            if (SpringDay1Director.Instance != null && SpringDay1Director.Instance.TryTriggerSleepFromBed())
            {
                return;
            }

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.Sleep();
            }
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

        public Vector2 GetClosestInteractionPoint(Vector2 playerPosition)
        {
            Bounds bounds = GetInteractionBounds();
            return bounds.ClosestPoint(playerPosition);
        }

        public float GetBoundaryDistance(Vector2 playerPosition)
        {
            return Vector2.Distance(playerPosition, GetClosestInteractionPoint(playerPosition));
        }

        private void OnDisable()
        {
            SpringDay1WorldHintBubble.HideIfExists(transform);
        }

        private void OnValidate()
        {
            interactionDistance = Mathf.Max(0.35f, interactionDistance);
            bubbleRevealDistance = Mathf.Max(interactionDistance, bubbleRevealDistance);
        }

        private void Update()
        {
            InteractionContext context = BuildInteractionContext();
            if (context?.PlayerTransform == null)
            {
                return;
            }

            ReportRestProximityInteraction(context);
        }

        private void ReportRestProximityInteraction(InteractionContext context)
        {
            if (!enableProximityKeyInteraction)
            {
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            float distance = GetBoundaryDistance(context.PlayerPosition);
            if (distance > Mathf.Max(bubbleRevealDistance, interactionDistance))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            if (!CanInteract(context))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            string caption = director != null ? director.GetRestInteractionHint(bubbleCaption) : bubbleCaption;
            string detail = ResolveRestInteractionDetail(director, bubbleDetail);

            SpringDay1ProximityInteractionService.ReportCandidate(
                transform,
                proximityInteractionKey,
                proximityInteractionKey.ToString(),
                caption,
                detail,
                distance,
                interactionPriority,
                keyInteractionCooldown,
                distance <= interactionDistance,
                () => OnInteract(context));
        }

        private InteractionContext BuildInteractionContext()
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            Transform playerTransform = playerMovement != null ? playerMovement.transform : null;
            if (playerTransform == null)
            {
                return null;
            }

            return new InteractionContext
            {
                PlayerTransform = playerTransform,
                PlayerPosition = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform)
            };
        }

        private Bounds GetInteractionBounds()
        {
            return SpringDay1UiLayerUtility.TryGetPresentationBounds(transform, out Bounds bounds)
                ? bounds
                : new Bounds(transform.position, Vector3.one);
        }

        private static string ResolveRestInteractionDetail(SpringDay1Director director, string fallbackDetail)
        {
            if (director == null)
            {
                return fallbackDetail;
            }

            System.Reflection.MethodInfo method = typeof(SpringDay1Director).GetMethod(
                "GetRestInteractionDetail",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (method == null)
            {
                return fallbackDetail;
            }

            object resolved = method.Invoke(director, new object[] { fallbackDetail });
            return resolved as string ?? fallbackDetail;
        }
    }
}
