using Sunset.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NPCDialogueInteractable : MonoBehaviour, IInteractable
    {
        #region Serialized Fields
        [Header("Dialogue")]
        [FormerlySerializedAs("dialogueSequence")]
        [SerializeField] private DialogueSequenceSO initialSequence;
        [SerializeField] private DialogueSequenceSO followupSequence;
        [SerializeField] private string interactionHint = "对话";
        [SerializeField] private float interactionDistance = 0.95f;
        [SerializeField] private int interactionPriority = 30;
        [SerializeField] private bool playOnlyOnce = false;

        [Header("NPC 对话接轨")]
        [SerializeField] private bool freezeRoamDuringDialogue = true;
        [SerializeField] private bool facePlayerOnInteract = true;
        [SerializeField] private NPCAutoRoamController autoRoamController;
        [SerializeField] private NPCMotionController motionController;

        [Header("E 键交互")]
        [SerializeField] private bool enableProximityKeyInteraction = true;
        [SerializeField] private KeyCode proximityInteractionKey = KeyCode.E;
        [SerializeField] private float keyInteractionCooldown = 0.15f;
        [SerializeField] private float bubbleRevealDistance = 1.2f;
        [SerializeField] private string bubbleCaption = "交谈";
        #endregion

        #region Private Fields
        private bool _hasPlayed;
        private bool _ownsActiveDialogue;
        private bool _resumeRoamAfterDialogue;
        #endregion

        #region Public Properties
        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        #endregion

        public Vector2 GetClosestInteractionPoint(Vector2 playerPosition)
        {
            Bounds bounds = GetInteractionBounds();
            return bounds.ClosestPoint(playerPosition);
        }

        public float GetBoundaryDistance(Vector2 playerPosition)
        {
            return Vector2.Distance(playerPosition, GetClosestInteractionPoint(playerPosition));
        }

        #region Unity Lifecycle
        private void Awake()
        {
            CacheComponents();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueEndEvent>(HandleDialogueEnded, owner: this);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            SpringDay1WorldHintBubble.HideIfExists(transform);
            NpcWorldHintBubble.HideIfExists(transform);
            _ownsActiveDialogue = false;
            _resumeRoamAfterDialogue = false;
        }

        private void OnValidate()
        {
            CacheComponents();
            interactionDistance = Mathf.Max(0.25f, interactionDistance);
            bubbleRevealDistance = Mathf.Max(interactionDistance, bubbleRevealDistance);
        }

        private void Update()
        {
            InteractionContext context = BuildInteractionContext();
            if (context?.PlayerTransform == null)
            {
                return;
            }

            ReportProximityInteraction(context);
        }
        #endregion

        #region Public Methods
        public bool CanInteract(InteractionContext context)
        {
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                return false;
            }

            DialogueManager manager = DialogueManager.Instance;
            DialogueSequenceSO sequenceToPlay = ResolveDialogueSequence(manager);

            if (sequenceToPlay == null)
            {
                return false;
            }

            if (playOnlyOnce && _hasPlayed)
            {
                return false;
            }

            return manager != null && !manager.IsDialogueActive;
        }

        public void OnInteract(InteractionContext context)
        {
            DialogueManager manager = DialogueManager.Instance;
            DialogueSequenceSO sequenceToPlay = ResolveDialogueSequence(manager);

            if (sequenceToPlay == null)
            {
                Debug.LogWarning($"[NPCDialogueInteractable] {name} 未配置可播放的对话序列。");
                return;
            }

            if (manager == null)
            {
                Debug.LogError($"[NPCDialogueInteractable] {name} 找不到 DialogueManager.Instance。");
                return;
            }

            if (manager.IsDialogueActive)
            {
                return;
            }

            SpringDay1WorldHintBubble.HideIfExists(transform);
            NpcWorldHintBubble.HideIfExists(transform);
            EnterDialogueOccupation(context);

            _hasPlayed = true;
            manager.PlayDialogue(sequenceToPlay);
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
        }
        #endregion

        #region Private Methods
        private void CacheComponents()
        {
            if (autoRoamController == null)
            {
                autoRoamController = GetComponent<NPCAutoRoamController>();
            }

            if (motionController == null)
            {
                motionController = GetComponent<NPCMotionController>();
            }
        }

        private void EnterDialogueOccupation(InteractionContext context)
        {
            CacheComponents();

            _ownsActiveDialogue = true;
            _resumeRoamAfterDialogue = false;

            if (freezeRoamDuringDialogue)
            {
                if (autoRoamController != null && autoRoamController.IsRoaming)
                {
                    _resumeRoamAfterDialogue = true;
                    autoRoamController.StopRoam();
                }
                else if (motionController != null)
                {
                    motionController.StopMotion();
                }
            }

            if (facePlayerOnInteract && motionController != null && context?.PlayerTransform != null)
            {
                Vector2 facingDirection = context.PlayerTransform.position - transform.position;
                motionController.SetFacingDirection(facingDirection);
            }
        }

        private void HandleDialogueEnded(DialogueEndEvent _)
        {
            if (!_ownsActiveDialogue || ShouldIgnoreDialogueEndEvent())
            {
                return;
            }

            if (_resumeRoamAfterDialogue && autoRoamController != null && autoRoamController.isActiveAndEnabled)
            {
                autoRoamController.StartRoam();
            }

            _ownsActiveDialogue = false;
            _resumeRoamAfterDialogue = false;
        }

        private void ReportProximityInteraction(InteractionContext context)
        {
            if (!enableProximityKeyInteraction)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                NpcWorldHintBubble.HideIfExists(transform);
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                NpcWorldHintBubble.HideIfExists(transform);
                return;
            }

            float boundaryDistance = GetBoundaryDistance(context.PlayerPosition);
            if (boundaryDistance > Mathf.Max(bubbleRevealDistance, interactionDistance))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                NpcWorldHintBubble.HideIfExists(transform);
                return;
            }

            if (!CanInteract(context))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                NpcWorldHintBubble.HideIfExists(transform);
                return;
            }

            SpringDay1ProximityInteractionService.ReportCandidate(
                transform,
                proximityInteractionKey,
                proximityInteractionKey.ToString(),
                bubbleCaption,
                "按 E 开始对话",
                boundaryDistance,
                interactionPriority,
                keyInteractionCooldown,
                boundaryDistance <= interactionDistance,
                () => OnInteract(context),
                showWorldIndicator: false);
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

        private DialogueSequenceSO ResolveDialogueSequence(DialogueManager manager)
        {
            if (!HasPlayableNodes(initialSequence))
            {
                return null;
            }

            DialogueSequenceSO resolvedFollowup = HasPlayableNodes(followupSequence)
                ? followupSequence
                : initialSequence.followupSequence;

            if (!HasPlayableNodes(resolvedFollowup))
            {
                return initialSequence;
            }

            StoryManager storyManager = StoryManager.Instance;
            bool languageAlreadyDecoded = initialSequence.markLanguageDecodedOnComplete && storyManager.IsLanguageDecoded;
            bool phaseAlreadyAdvanced = initialSequence.advanceStoryPhaseOnComplete
                && initialSequence.nextStoryPhase != StoryPhase.None
                && storyManager.HasReachedPhase(initialSequence.nextStoryPhase);
            bool sequenceAlreadyCompleted = manager != null && manager.HasCompletedSequence(initialSequence.sequenceId);

            if (languageAlreadyDecoded || phaseAlreadyAdvanced || sequenceAlreadyCompleted)
            {
                return resolvedFollowup;
            }

            return initialSequence;
        }

        private static bool HasPlayableNodes(DialogueSequenceSO sequence)
        {
            return sequence != null && sequence.nodes != null && sequence.nodes.Count > 0;
        }

        private static bool ShouldIgnoreDialogueEndEvent()
        {
            DialogueManager manager = DialogueManager.Instance;
            return manager != null && manager.IsDialogueActive;
        }
        #endregion
    }
}
