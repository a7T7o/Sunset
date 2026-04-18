using System;
using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NPCInformalChatInteractable : MonoBehaviour, IInteractable
    {
        private const float CoarseInteractionPadding = 1.5f;
        internal const string InformalChatControlOwnerKey = "npc-informal-chat";

        private static PlayerMovement _cachedPlayerMovement;
        private static int _lastPlayerLookupFrame = -1;
        private static int _lastPlayerSamplePointFrame = -1;
        private static Vector2 _cachedPlayerSamplePoint;
        private static global::PlayerNpcChatSessionService _cachedSessionService;

        [Header("Interaction")]
        [SerializeField] private string interactionHint = "闲聊";
        [SerializeField] private float interactionDistance = 1.15f;
        [SerializeField] private float sessionBreakDistance = 1.85f;
        [SerializeField] private int interactionPriority = 29;

        [Header("E 键交互")]
        [SerializeField] private bool enableProximityKeyInteraction = true;
        [SerializeField] private KeyCode proximityInteractionKey = KeyCode.E;
        [SerializeField] private float keyInteractionCooldown = 0.15f;
        [SerializeField] private float bubbleRevealDistance = 1.35f;
        [SerializeField] private string bubbleCaption = "闲聊";
        [SerializeField] private string bubbleDetail = "按 E 开口";

        [Header("NPC 接轨")]
        [SerializeField] private bool freezeRoamDuringChat = true;
        [SerializeField] private bool facePlayerOnInteract = true;
        [SerializeField] private NPCAutoRoamController autoRoamController;
        [SerializeField] private NPCMotionController motionController;
        [SerializeField] private NPCBubblePresenter bubblePresenter;
        [SerializeField] private NPCDialogueInteractable dialogueInteractable;

        private bool _resumeRoamAfterChat;
        private bool _presentationCacheInitialized;
        private InteractionContext _cachedInteractionContext;
        private SpriteRenderer[] _cachedPresentationRenderers = System.Array.Empty<SpriteRenderer>();
        private Collider2D[] _cachedPresentationColliders = System.Array.Empty<Collider2D>();
        private Action _cachedPromptTriggerAction;
        private InteractionContext _pendingPromptContext;
        private string _cachedProximityKeyLabel;

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        public float SessionBreakDistance => Mathf.Max(interactionDistance, sessionBreakDistance);
        public NPCAutoRoamController AutoRoamController => autoRoamController;
        public NPCMotionController MotionController => motionController;
        public NPCBubblePresenter BubblePresenter => bubblePresenter;
        public NPCRoamProfile RoamProfile => autoRoamController != null ? autoRoamController.RoamProfile : null;
        public bool IsResidentScriptedControlActive => autoRoamController != null && autoRoamController.IsResidentScriptedControlActive;

        internal static bool IsInformalChatControlOwner(string ownerKey)
        {
            return string.Equals(ownerKey, InformalChatControlOwnerKey, StringComparison.Ordinal);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            NPCAutoRoamController[] controllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < controllers.Length; index++)
            {
                NPCAutoRoamController controller = controllers[index];
                if (controller == null || controller.GetComponent<NPCInformalChatInteractable>() != null)
                {
                    continue;
                }

                NPCRoamProfile roamProfile = controller.RoamProfile;
                if (roamProfile == null || !roamProfile.HasInformalConversationContent)
                {
                    continue;
                }

                controller.gameObject.AddComponent<NPCInformalChatInteractable>();
            }
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

        private void Awake()
        {
            CacheComponents();
            CacheInteractionLabels();
        }

        private void OnEnable()
        {
            CacheComponents();
            CacheInteractionLabels();
            if (Application.isPlaying)
            {
                NpcAmbientBubblePriorityGuard.EnsureRuntime();
            }
        }

        private void OnDisable()
        {
            HideProximityHints();
        }

        private void OnValidate()
        {
            CacheComponents();
            CacheInteractionLabels();
            interactionDistance = Mathf.Max(0.35f, interactionDistance);
            sessionBreakDistance = Mathf.Max(interactionDistance, sessionBreakDistance);
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

        public bool CanInteract(InteractionContext context)
        {
            InteractionContext effectiveContext = context;
            if (effectiveContext == null && !TryBuildInteractionContext(out effectiveContext))
            {
                return false;
            }

            global::PlayerNpcChatSessionService sessionService = ResolveSessionService(effectiveContext);
            bool isActiveConversationTarget = sessionService != null && sessionService.IsConversationActiveWith(this);
            return CanInteractWithResolvedSession(effectiveContext, sessionService, isActiveConversationTarget);
        }

        private bool CanInteractWithResolvedSession(
            InteractionContext effectiveContext,
            global::PlayerNpcChatSessionService sessionService,
            bool isActiveConversationTarget)
        {
            if (NpcInteractionPriorityPolicy.IsBlockingPageUiOpenCached())
            {
                return false;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return false;
            }

            if (RoamProfile == null || !RoamProfile.HasInformalConversationContent)
            {
                return false;
            }

            if (IsResidentScriptedControlBlockingInformalInteraction())
            {
                return false;
            }

            if (!isActiveConversationTarget &&
                NpcInteractionPriorityPolicy.ShouldSuppressInformalInteractionForCurrentStory(
                    dialogueInteractable,
                    effectiveContext))
            {
                return false;
            }

            return sessionService == null || sessionService.CanRouteInteractable(this);
        }

        public void OnInteract(InteractionContext context)
        {
            TryHandleInteract(context);
        }

        public bool TryHandleInteract(InteractionContext context)
        {
            InteractionContext effectiveContext = context;
            if (effectiveContext == null && !TryBuildInteractionContext(out effectiveContext))
            {
                return false;
            }

            global::PlayerNpcChatSessionService sessionService = ResolveSessionService(effectiveContext);
            bool isActiveConversationTarget = sessionService != null && sessionService.IsConversationActiveWith(this);
            if (!CanInteractWithResolvedSession(effectiveContext, sessionService, isActiveConversationTarget))
            {
                return false;
            }

            if (SpringDay1Director.Instance != null
                && SpringDay1Director.Instance.TryConsumeStoryNpcInteraction(transform, effectiveContext))
            {
                return true;
            }

            return sessionService != null && sessionService.HandleInteract(this, effectiveContext);
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
        }

        public bool ShouldUseResidentPromptTone()
        {
            CacheComponents();
            return dialogueInteractable != null && dialogueInteractable.WillYieldToInformalResident();
        }

        public string ResolveNpcId()
        {
            return RoamProfile != null
                ? RoamProfile.ResolveNpcId(name)
                : NPCDialogueContentProfile.NormalizeNpcId(name);
        }

        public NPCRelationshipStage ResolveRelationshipStage()
        {
            return PlayerNpcRelationshipService.GetStage(ResolveNpcId());
        }

        public NPCDialogueContentProfile.InformalConversationBundle[] ResolveConversationBundles(NPCRelationshipStage relationshipStage)
        {
            return ResolveConversationBundles(relationshipStage, ResolveCurrentStoryPhase());
        }

        public NPCDialogueContentProfile.InformalConversationBundle[] ResolveConversationBundles(
            NPCRelationshipStage relationshipStage,
            StoryPhase storyPhase)
        {
            return RoamProfile != null
                ? RoamProfile.GetInformalConversationBundles(relationshipStage, storyPhase)
                : System.Array.Empty<NPCDialogueContentProfile.InformalConversationBundle>();
        }

        public NPCDialogueContentProfile.InformalChatInterruptReaction ResolveWalkAwayReaction(NPCRelationshipStage relationshipStage)
        {
            return ResolveWalkAwayReaction(relationshipStage, ResolveCurrentStoryPhase());
        }

        public NPCDialogueContentProfile.InformalChatInterruptReaction ResolveWalkAwayReaction(
            NPCRelationshipStage relationshipStage,
            StoryPhase storyPhase)
        {
            return RoamProfile != null
                ? RoamProfile.GetWalkAwayReaction(relationshipStage, storyPhase)
                : null;
        }

        public NPCDialogueContentProfile.InformalChatInterruptReaction ResolveInterruptReaction(
            NPCRelationshipStage relationshipStage,
            NPCInformalChatLeaveCause leaveCause,
            NPCInformalChatLeavePhase leavePhase)
        {
            return ResolveInterruptReaction(relationshipStage, ResolveCurrentStoryPhase(), leaveCause, leavePhase);
        }

        public NPCDialogueContentProfile.InformalChatInterruptReaction ResolveInterruptReaction(
            NPCRelationshipStage relationshipStage,
            StoryPhase storyPhase,
            NPCInformalChatLeaveCause leaveCause,
            NPCInformalChatLeavePhase leavePhase)
        {
            return RoamProfile != null
                ? RoamProfile.GetInterruptReaction(relationshipStage, storyPhase, leaveCause, leavePhase)
                : null;
        }

        public NPCDialogueContentProfile.InformalChatResumeIntro ResolveResumeIntro(
            NPCRelationshipStage relationshipStage,
            NPCInformalChatLeaveCause leaveCause,
            NPCInformalChatLeavePhase leavePhase)
        {
            return ResolveResumeIntro(relationshipStage, ResolveCurrentStoryPhase(), leaveCause, leavePhase);
        }

        public NPCDialogueContentProfile.InformalChatResumeIntro ResolveResumeIntro(
            NPCRelationshipStage relationshipStage,
            StoryPhase storyPhase,
            NPCInformalChatLeaveCause leaveCause,
            NPCInformalChatLeavePhase leavePhase)
        {
            return RoamProfile != null
                ? RoamProfile.GetResumeIntro(relationshipStage, storyPhase, leaveCause, leavePhase)
                : null;
        }

        public void EnterConversationOccupation(InteractionContext context)
        {
            CacheComponents();
            _resumeRoamAfterChat = false;

            if (freezeRoamDuringChat)
            {
                if (autoRoamController != null)
                {
                    _resumeRoamAfterChat = autoRoamController.IsRoaming;
                    autoRoamController.AcquireStoryControl(InformalChatControlOwnerKey, _resumeRoamAfterChat);
                    autoRoamController.HaltStoryControlMotion();
                }
                else if (motionController != null)
                {
                    motionController.StopMotion();
                }
            }

            if (facePlayerOnInteract && context?.PlayerTransform != null)
            {
                FaceToward(context.PlayerTransform.position - transform.position);
            }
        }

        public void ExitConversationOccupation()
        {
            if (autoRoamController != null && autoRoamController.isActiveAndEnabled)
            {
                autoRoamController.ReleaseStoryControl(InformalChatControlOwnerKey, _resumeRoamAfterChat);
            }

            _resumeRoamAfterChat = false;
        }

        public void FaceToward(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            if (autoRoamController != null)
            {
                autoRoamController.ApplyIdleFacing(direction);
            }
            else if (motionController != null)
            {
                motionController.ApplyIdleFacing(direction);
            }
        }

        private void CacheComponents()
        {
            autoRoamController ??= GetComponent<NPCAutoRoamController>();
            motionController ??= GetComponent<NPCMotionController>();
            bubblePresenter ??= GetComponent<NPCBubblePresenter>();
            dialogueInteractable ??= GetComponent<NPCDialogueInteractable>();
            _cachedPresentationRenderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            _cachedPresentationColliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
            _presentationCacheInitialized = true;
        }

        private void ReportProximityInteraction(InteractionContext context)
        {
            if (!enableProximityKeyInteraction)
            {
                HideProximityHints();
                return;
            }

            if (IsResidentScriptedControlBlockingInformalInteraction())
            {
                HideProximityHints();
                return;
            }

            if (NpcInteractionPriorityPolicy.IsBlockingPageUiOpenCached()
                || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive))
            {
                HideProximityHints();
                return;
            }

            if (IsOutsideCoarseInteractionRange(context.PlayerPosition))
            {
                HideProximityHints();
                return;
            }

            global::PlayerNpcChatSessionService sessionService = ResolveSessionService(context);
            bool isActiveConversationTarget = sessionService != null && sessionService.IsConversationActiveWith(this);
            bool canInteractNow = CanInteractWithResolvedSession(context, sessionService, isActiveConversationTarget);
            if (!isActiveConversationTarget && !canInteractNow)
            {
                HideProximityHints();
                return;
            }

            float boundaryDistance = GetBoundaryDistance(context.PlayerPosition);
            float revealDistance = isActiveConversationTarget
                ? Mathf.Max(bubbleRevealDistance, SessionBreakDistance)
                : Mathf.Max(bubbleRevealDistance, interactionDistance);
            if (boundaryDistance > revealDistance)
            {
                HideProximityHints();
                return;
            }

            string caption = sessionService != null ? sessionService.GetPromptCaption(this) : bubbleCaption;
            string detail = sessionService != null ? sessionService.GetPromptDetail(this) : bubbleDetail;

            SpringDay1ProximityInteractionService.ReportCandidate(
                transform,
                proximityInteractionKey,
                _cachedProximityKeyLabel,
                caption,
                detail,
                boundaryDistance,
                interactionPriority,
                keyInteractionCooldown,
                isActiveConversationTarget || (boundaryDistance <= interactionDistance && canInteractNow),
                ResolveCachedPromptTriggerAction(context),
                forceFocus: isActiveConversationTarget,
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

        private Bounds GetInteractionBounds()
        {
            return TryGetCachedPresentationBounds(out Bounds bounds)
                ? bounds
                : new Bounds(transform.position, Vector3.one);
        }

        private static Transform ResolveCachedPlayerTransform()
        {
            if (_cachedPlayerMovement != null)
            {
                return _cachedPlayerMovement.transform;
            }

            if (_lastPlayerLookupFrame == Time.frameCount)
            {
                return null;
            }

            _lastPlayerLookupFrame = Time.frameCount;
            _cachedPlayerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            return _cachedPlayerMovement != null ? _cachedPlayerMovement.transform : null;
        }

        private static Vector2 ResolveCachedPlayerSamplePoint(Transform playerTransform)
        {
            if (_lastPlayerSamplePointFrame != Time.frameCount)
            {
                _cachedPlayerSamplePoint = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform);
                _lastPlayerSamplePointFrame = Time.frameCount;
            }

            return _cachedPlayerSamplePoint;
        }

        private static global::PlayerNpcChatSessionService ResolveSessionService(InteractionContext context)
        {
            if (context?.PlayerTransform == null)
            {
                return global::PlayerNpcChatSessionService.Instance;
            }

            if (_cachedSessionService != null && _cachedSessionService.gameObject == context.PlayerTransform.gameObject)
            {
                return _cachedSessionService;
            }

            _cachedSessionService = global::PlayerNpcChatSessionService.ResolveForPlayer(context.PlayerTransform.gameObject);
            return _cachedSessionService;
        }

        private static StoryPhase ResolveCurrentStoryPhase()
        {
            return NpcInteractionPriorityPolicy.ResolveCurrentStoryPhase();
        }

        private bool IsResidentScriptedControlBlockingInformalInteraction()
        {
            CacheComponents();
            if (!IsResidentScriptedControlActive)
            {
                return false;
            }

            if (autoRoamController != null &&
                IsInformalChatControlOwner(autoRoamController.ResidentScriptedControlOwnerKey))
            {
                return false;
            }

            return !ShouldAllowInformalInteractionDuringFreeTimeReturnHome();
        }

        private bool ShouldAllowInformalInteractionDuringFreeTimeReturnHome()
        {
            StoryPhase currentPhase = ResolveCurrentStoryPhase();
            return autoRoamController != null
                && autoRoamController.IsResidentScriptedControlActive
                && autoRoamController.IsFormalNavigationDriveActive()
                && NpcInteractionPriorityPolicy.AllowsResidentFreeInteractionPhase(currentPhase);
        }

        private void HideProximityHints()
        {
            SpringDay1WorldHintBubble.HideIfExists(transform);
            NpcWorldHintBubble.HideIfExists(transform);
        }

        private bool IsOutsideCoarseInteractionRange(Vector2 playerPosition)
        {
            float coarseDistance = Mathf.Max(bubbleRevealDistance, SessionBreakDistance) + CoarseInteractionPadding;
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

    }
}
