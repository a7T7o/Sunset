using Sunset.Events;
using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace Sunset.Story
{
    public enum NPCFormalDialogueState
    {
        None = 0,
        Available = 1,
        Consumed = 2
    }

    [DisallowMultipleComponent]
    public class NPCDialogueInteractable : MonoBehaviour, IInteractable
    {
        private const float CoarseInteractionPadding = 1.5f;
        internal const string FormalDialogueControlOwnerKey = "npc-formal-dialogue";

        private static PlayerMovement s_cachedPlayerMovement;
        private static int s_lastPlayerLookupFrame = -1;
        private static int s_lastPlayerSamplePointFrame = -1;
        private static Vector2 s_cachedPlayerSamplePoint;

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
        private bool _presentationCacheInitialized;
        private InteractionContext _cachedInteractionContext;
        private SpriteRenderer[] _cachedPresentationRenderers = System.Array.Empty<SpriteRenderer>();
        private Collider2D[] _cachedPresentationColliders = System.Array.Empty<Collider2D>();
        private Action _cachedPromptTriggerAction;
        private InteractionContext _pendingPromptContext;
        private string _cachedProximityKeyLabel;
        #endregion

        #region Public Properties
        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        public bool HasFormalDialogueConfigured => HasPlayableNodes(initialSequence);
        #endregion

        internal static bool IsFormalDialogueControlOwner(string ownerKey)
        {
            return string.Equals(ownerKey, FormalDialogueControlOwnerKey, StringComparison.Ordinal);
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

        #region Unity Lifecycle
        private void Awake()
        {
            CacheComponents();
            CacheInteractionLabels();
        }

        private void OnEnable()
        {
            CacheComponents();
            CacheInteractionLabels();
            EventBus.Subscribe<DialogueEndEvent>(HandleDialogueEnded, owner: this);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            HideProximityHints();
            _ownsActiveDialogue = false;
            _resumeRoamAfterDialogue = false;
        }

        private void OnValidate()
        {
            CacheComponents();
            CacheInteractionLabels();
            interactionDistance = Mathf.Max(0.25f, interactionDistance);
            bubbleRevealDistance = Mathf.Max(interactionDistance, bubbleRevealDistance);
        }

        private void Update()
        {
            if (!TryBuildInteractionContext(out InteractionContext context))
            {
                return;
            }

            ReportProximityInteraction(context);
        }
        #endregion

        #region Public Methods
        public bool CanInteract(InteractionContext context)
        {
            if (NpcInteractionPriorityPolicy.IsBlockingPageUiOpenCached())
            {
                return false;
            }

            DialogueManager manager = DialogueManager.Instance;
            if (SpringDay1Director.Instance != null
                && SpringDay1Director.Instance.CanConsumeStoryNpcInteraction(transform, context)
                && (manager == null || !manager.IsDialogueActive))
            {
                return true;
            }

            if (IsResidentScriptedControlBlockingFormalInteraction())
            {
                return false;
            }

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

        public bool HasFormalDialoguePriorityAvailable()
        {
            DialogueManager manager = DialogueManager.Instance;
            if (manager == null || manager.IsDialogueActive)
            {
                return false;
            }

            if (TryBuildInteractionContext(out InteractionContext context)
                && SpringDay1Director.Instance != null
                && SpringDay1Director.Instance.CanConsumeStoryNpcInteraction(transform, context))
            {
                return true;
            }

            if (IsResidentScriptedControlBlockingFormalInteraction())
            {
                return false;
            }

            if (playOnlyOnce && _hasPlayed)
            {
                return false;
            }

            return ResolveDialogueSequence(manager) != null;
        }

        public void OnInteract(InteractionContext context)
        {
            DialogueManager manager = DialogueManager.Instance;
            if (SpringDay1Director.Instance != null
                && SpringDay1Director.Instance.TryConsumeStoryNpcInteraction(transform, context))
            {
                return;
            }

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

        public NPCFormalDialogueState GetFormalDialogueStateForCurrentStory()
        {
            if (!HasFormalDialogueConfigured)
            {
                return NPCFormalDialogueState.None;
            }

            return HasConsumedFormalDialogue(DialogueManager.Instance)
                ? NPCFormalDialogueState.Consumed
                : NPCFormalDialogueState.Available;
        }

        public bool HasConsumedFormalDialogueForCurrentStory()
        {
            return GetFormalDialogueStateForCurrentStory() == NPCFormalDialogueState.Consumed;
        }

        public bool WillYieldToInformalResident()
        {
            return HasFormalDialogueConfigured && HasConsumedFormalDialogueForCurrentStory();
        }
        #endregion

        private bool IsResidentScriptedControlBlockingFormalInteraction()
        {
            CacheComponents();
            if (autoRoamController == null || !autoRoamController.IsResidentScriptedControlActive)
            {
                return false;
            }

            if (IsFormalDialogueControlOwner(autoRoamController.ResidentScriptedControlOwnerKey))
            {
                return false;
            }

            return !ShouldAllowFormalInteractionDuringFreeTimeReturnHome();
        }

        private bool ShouldAllowFormalInteractionDuringFreeTimeReturnHome()
        {
            StoryPhase currentPhase = NpcInteractionPriorityPolicy.ResolveCurrentStoryPhase();
            return autoRoamController != null
                && autoRoamController.IsResidentScriptedControlActive
                && autoRoamController.IsFormalNavigationDriveActive()
                && NpcInteractionPriorityPolicy.AllowsResidentFreeInteractionPhase(currentPhase);
        }

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

            _cachedPresentationRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            _cachedPresentationColliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
            _presentationCacheInitialized = true;
        }

        private void EnterDialogueOccupation(InteractionContext context)
        {
            CacheComponents();

            _ownsActiveDialogue = true;
            _resumeRoamAfterDialogue = false;

            if (freezeRoamDuringDialogue)
            {
                if (autoRoamController != null)
                {
                    _resumeRoamAfterDialogue = autoRoamController.IsRoaming;
                    autoRoamController.AcquireStoryControl(FormalDialogueControlOwnerKey, _resumeRoamAfterDialogue);
                    autoRoamController.HaltStoryControlMotion();
                }
                else if (motionController != null)
                {
                    motionController.StopMotion();
                }
            }

            if (facePlayerOnInteract && motionController != null && context?.PlayerTransform != null)
            {
                Vector2 facingDirection = context.PlayerTransform.position - transform.position;
                if (autoRoamController != null)
                {
                    autoRoamController.ApplyIdleFacing(facingDirection);
                }
                else
                {
                    motionController.ApplyIdleFacing(facingDirection);
                }
            }
        }

        private void HandleDialogueEnded(DialogueEndEvent _)
        {
            if (!_ownsActiveDialogue || ShouldIgnoreDialogueEndEvent())
            {
                return;
            }

            if (autoRoamController != null && autoRoamController.isActiveAndEnabled)
            {
                autoRoamController.ReleaseStoryControl(FormalDialogueControlOwnerKey, _resumeRoamAfterDialogue);
            }

            _ownsActiveDialogue = false;
            _resumeRoamAfterDialogue = false;
        }

        private void ReportProximityInteraction(InteractionContext context)
        {
            if (!enableProximityKeyInteraction)
            {
                HideProximityHints();
                return;
            }

            if (NpcInteractionPriorityPolicy.IsBlockingPageUiOpenCached())
            {
                HideProximityHints();
                return;
            }

            if (IsOutsideCoarseInteractionRange(context.PlayerPosition))
            {
                HideProximityHints();
                return;
            }

            float boundaryDistance = GetBoundaryDistance(context.PlayerPosition);
            if (boundaryDistance > Mathf.Max(bubbleRevealDistance, interactionDistance))
            {
                HideProximityHints();
                return;
            }

            if (!CanInteract(context))
            {
                HideProximityHints();
                return;
            }

            SpringDay1ProximityInteractionService.ReportCandidate(
                transform,
                proximityInteractionKey,
                _cachedProximityKeyLabel,
                bubbleCaption,
                "按 E 开始对话",
                boundaryDistance,
                interactionPriority,
                keyInteractionCooldown,
                boundaryDistance <= interactionDistance,
                ResolveCachedPromptTriggerAction(context),
                showWorldIndicator: false);
        }

        private bool TryBuildInteractionContext(out InteractionContext context)
        {
            Transform playerTransform = ResolveCachedPlayerTransform();
            if (playerTransform == null)
            {
                context = null;
                return false;
            }

            _cachedInteractionContext ??= new InteractionContext();
            _cachedInteractionContext.PlayerTransform = playerTransform;
            _cachedInteractionContext.PlayerPosition = ResolveCachedPlayerSamplePoint(playerTransform);
            context = _cachedInteractionContext;
            return true;
        }

        private static Transform ResolveCachedPlayerTransform()
        {
            if (s_cachedPlayerMovement != null)
            {
                return s_cachedPlayerMovement.transform;
            }

            if (s_lastPlayerLookupFrame == Time.frameCount)
            {
                return null;
            }

            s_lastPlayerLookupFrame = Time.frameCount;
            s_cachedPlayerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            return s_cachedPlayerMovement != null ? s_cachedPlayerMovement.transform : null;
        }

        private static Vector2 ResolveCachedPlayerSamplePoint(Transform playerTransform)
        {
            if (s_lastPlayerSamplePointFrame != Time.frameCount)
            {
                s_cachedPlayerSamplePoint = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform);
                s_lastPlayerSamplePointFrame = Time.frameCount;
            }

            return s_cachedPlayerSamplePoint;
        }

        private Bounds GetInteractionBounds()
        {
            return TryGetCachedPresentationBounds(out Bounds bounds)
                ? bounds
                : new Bounds(transform.position, Vector3.one);
        }

        private DialogueSequenceSO ResolveDialogueSequence(DialogueManager manager)
        {
            if (!HasPlayableNodes(initialSequence))
            {
                return null;
            }

            if (HasConsumedFormalDialogue(manager))
            {
                // Formal follow-up sequences are auto-consumed inside the same one-shot chain by DialogueManager.
                // Re-interaction after completion must yield back to informal / resident content instead of replaying formal nodes.
                return null;
            }

            return initialSequence;
        }

        private bool HasConsumedFormalDialogue(DialogueManager manager)
        {
            StoryManager storyManager = StoryManager.Instance;
            bool languageAlreadyDecoded = initialSequence.markLanguageDecodedOnComplete && storyManager != null && storyManager.IsLanguageDecoded;
            bool phaseAlreadyAdvanced = initialSequence.advanceStoryPhaseOnComplete
                && initialSequence.nextStoryPhase != StoryPhase.None
                && storyManager != null
                && storyManager.HasReachedPhase(initialSequence.nextStoryPhase);
            bool sequenceAlreadyCompleted = manager != null && manager.HasCompletedSequence(initialSequence.sequenceId);

            return languageAlreadyDecoded || phaseAlreadyAdvanced || sequenceAlreadyCompleted;
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

        private Action ResolveCachedPromptTriggerAction(InteractionContext context)
        {
            _pendingPromptContext = context;
            _cachedPromptTriggerAction ??= TriggerPendingPromptInteraction;
            return _cachedPromptTriggerAction;
        }

        private void CacheInteractionLabels()
        {
            _cachedProximityKeyLabel = proximityInteractionKey.ToString();
        }

        private void TriggerPendingPromptInteraction()
        {
            if (_pendingPromptContext != null)
            {
                OnInteract(_pendingPromptContext);
            }
        }

        private void HideProximityHints()
        {
            SpringDay1WorldHintBubble.HideIfExists(transform);
            NpcWorldHintBubble.HideIfExists(transform);
        }

        private bool IsOutsideCoarseInteractionRange(Vector2 playerPosition)
        {
            float coarseDistance = Mathf.Max(bubbleRevealDistance, interactionDistance) + CoarseInteractionPadding;
            return ((Vector2)transform.position - playerPosition).sqrMagnitude > coarseDistance * coarseDistance;
        }

        private bool TryGetCachedPresentationBounds(out Bounds bounds)
        {
            bounds = default;
            if (!_presentationCacheInitialized)
            {
                CacheComponents();
            }

            bool hasRenderer = false;
            for (int index = 0; index < _cachedPresentationRenderers.Length; index++)
            {
                SpriteRenderer renderer = _cachedPresentationRenderers[index];
                if (renderer == null || !renderer.enabled || renderer.sprite == null)
                {
                    continue;
                }

                if (!hasRenderer)
                {
                    bounds = renderer.bounds;
                    hasRenderer = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (hasRenderer)
            {
                return true;
            }

            bool hasCollider = false;
            for (int index = 0; index < _cachedPresentationColliders.Length; index++)
            {
                Collider2D collider2D = _cachedPresentationColliders[index];
                if (collider2D == null || !collider2D.enabled)
                {
                    continue;
                }

                if (!hasCollider)
                {
                    bounds = collider2D.bounds;
                    hasCollider = true;
                }
                else
                {
                    bounds.Encapsulate(collider2D.bounds);
                }
            }

            if (hasCollider)
            {
                return true;
            }

            bounds = new Bounds(transform.position, Vector3.one);
            return true;
        }
        #endregion
    }
}
