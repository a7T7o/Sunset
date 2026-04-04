using System;
using System.Reflection;
using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NPCInformalChatInteractable : MonoBehaviour, IInteractable
    {
        private static Type _playerNpcChatSessionServiceType;

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

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        public float SessionBreakDistance => Mathf.Max(interactionDistance, sessionBreakDistance);
        public NPCAutoRoamController AutoRoamController => autoRoamController;
        public NPCMotionController MotionController => motionController;
        public NPCBubblePresenter BubblePresenter => bubblePresenter;
        public NPCRoamProfile RoamProfile => autoRoamController != null ? autoRoamController.RoamProfile : null;

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

                if (controller.GetComponent<NPCDialogueInteractable>() != null)
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
        }

        private void OnEnable()
        {
            CacheComponents();
        }

        private void OnDisable()
        {
            SpringDay1WorldHintBubble.HideIfExists(transform);
            NpcWorldHintBubble.HideIfExists(transform);
        }

        private void OnValidate()
        {
            CacheComponents();
            interactionDistance = Mathf.Max(0.35f, interactionDistance);
            sessionBreakDistance = Mathf.Max(interactionDistance, sessionBreakDistance);
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

        public bool CanInteract(InteractionContext context)
        {
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
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

            Component sessionService = ResolveSessionService(context);
            if (!InvokeSessionBool(sessionService, "IsConversationActiveWith", this)
                && ShouldYieldToDialogueCandidate(context))
            {
                return false;
            }

            return sessionService == null || InvokeSessionBool(sessionService, "CanRouteInteractable", this);
        }

        public void OnInteract(InteractionContext context)
        {
            TryHandleInteract(context);
        }

        public bool TryHandleInteract(InteractionContext context)
        {
            Component sessionService = ResolveSessionService(context);
            if (sessionService != null)
            {
                return InvokeSessionBool(sessionService, "HandleInteract", this, context);
            }

            return false;
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
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
            return RoamProfile != null
                ? RoamProfile.GetInformalConversationBundles(relationshipStage)
                : System.Array.Empty<NPCDialogueContentProfile.InformalConversationBundle>();
        }

        public NPCDialogueContentProfile.InformalChatInterruptReaction ResolveWalkAwayReaction(NPCRelationshipStage relationshipStage)
        {
            return RoamProfile != null
                ? RoamProfile.GetWalkAwayReaction(relationshipStage)
                : null;
        }

        public NPCDialogueContentProfile.InformalChatInterruptReaction ResolveInterruptReaction(
            NPCRelationshipStage relationshipStage,
            NPCInformalChatLeaveCause leaveCause,
            NPCInformalChatLeavePhase leavePhase)
        {
            return RoamProfile != null
                ? RoamProfile.GetInterruptReaction(relationshipStage, leaveCause, leavePhase)
                : null;
        }

        public NPCDialogueContentProfile.InformalChatResumeIntro ResolveResumeIntro(
            NPCRelationshipStage relationshipStage,
            NPCInformalChatLeaveCause leaveCause,
            NPCInformalChatLeavePhase leavePhase)
        {
            return RoamProfile != null
                ? RoamProfile.GetResumeIntro(relationshipStage, leaveCause, leavePhase)
                : null;
        }

        public void EnterConversationOccupation(InteractionContext context)
        {
            CacheComponents();
            _resumeRoamAfterChat = false;

            if (freezeRoamDuringChat)
            {
                if (autoRoamController != null && autoRoamController.IsRoaming)
                {
                    _resumeRoamAfterChat = true;
                    autoRoamController.StopRoam();
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
            if (_resumeRoamAfterChat && autoRoamController != null && autoRoamController.isActiveAndEnabled)
            {
                autoRoamController.StartRoam();
            }

            _resumeRoamAfterChat = false;
        }

        public void FaceToward(Vector2 direction)
        {
            if (motionController != null && direction.sqrMagnitude > 0.0001f)
            {
                motionController.SetFacingDirection(direction);
            }
        }

        private void CacheComponents()
        {
            autoRoamController ??= GetComponent<NPCAutoRoamController>();
            motionController ??= GetComponent<NPCMotionController>();
            bubblePresenter ??= GetComponent<NPCBubblePresenter>();
            dialogueInteractable ??= GetComponent<NPCDialogueInteractable>();
        }

        private void ReportProximityInteraction(InteractionContext context)
        {
            if (!enableProximityKeyInteraction)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen() || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive))
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            Component sessionService = ResolveSessionService(context);
            bool isActiveConversationTarget = InvokeSessionBool(sessionService, "IsConversationActiveWith", this);
            bool canInteractNow = CanInteract(context);
            if (!isActiveConversationTarget && !canInteractNow)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            float boundaryDistance = GetBoundaryDistance(context.PlayerPosition);
            float revealDistance = isActiveConversationTarget
                ? Mathf.Max(bubbleRevealDistance, SessionBreakDistance)
                : Mathf.Max(bubbleRevealDistance, interactionDistance);
            if (boundaryDistance > revealDistance)
            {
                SpringDay1WorldHintBubble.HideIfExists(transform);
                return;
            }

            string caption = InvokeSessionString(sessionService, "GetPromptCaption", bubbleCaption, this);
            string detail = InvokeSessionString(sessionService, "GetPromptDetail", bubbleDetail, this);

            SpringDay1ProximityInteractionService.ReportCandidate(
                transform,
                proximityInteractionKey,
                proximityInteractionKey.ToString(),
                caption,
                detail,
                boundaryDistance,
                interactionPriority,
                keyInteractionCooldown,
                isActiveConversationTarget || (boundaryDistance <= interactionDistance && canInteractNow),
                () => OnInteract(context),
                forceFocus: isActiveConversationTarget,
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

        private bool ShouldYieldToDialogueCandidate(InteractionContext context)
        {
            if (context == null || dialogueInteractable == null || !dialogueInteractable.isActiveAndEnabled)
            {
                return false;
            }

            if (!dialogueInteractable.CanInteract(context))
            {
                return false;
            }

            float dialogueDistance = dialogueInteractable.GetBoundaryDistance(context.PlayerPosition);
            float revealDistance = Mathf.Max(bubbleRevealDistance, dialogueInteractable.InteractionDistance);
            return dialogueDistance <= revealDistance;
        }

        private static Component ResolveSessionService(InteractionContext context)
        {
            Type serviceType = FindPlayerNpcChatSessionServiceType();
            if (serviceType == null)
            {
                return null;
            }

            if (context?.PlayerTransform != null)
            {
                MethodInfo resolveForPlayer = serviceType.GetMethod(
                    "ResolveForPlayer",
                    BindingFlags.Public | BindingFlags.Static);
                if (resolveForPlayer != null)
                {
                    return resolveForPlayer.Invoke(null, new object[] { context.PlayerTransform.gameObject }) as Component;
                }
            }

            PropertyInfo instanceProperty = serviceType.GetProperty(
                "Instance",
                BindingFlags.Public | BindingFlags.Static);
            return instanceProperty?.GetValue(null) as Component;
        }

        private static Type FindPlayerNpcChatSessionServiceType()
        {
            if (_playerNpcChatSessionServiceType != null)
            {
                return _playerNpcChatSessionServiceType;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int index = 0; index < assemblies.Length; index++)
            {
                Type serviceType = assemblies[index].GetType("PlayerNpcChatSessionService");
                if (serviceType != null)
                {
                    _playerNpcChatSessionServiceType = serviceType;
                    return _playerNpcChatSessionServiceType;
                }
            }

            return null;
        }

        private static bool InvokeSessionBool(Component sessionService, string methodName, params object[] args)
        {
            if (sessionService == null)
            {
                return false;
            }

            MethodInfo method = sessionService.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                return false;
            }

            object result = method.Invoke(sessionService, args);
            return result is bool boolResult && boolResult;
        }

        private static string InvokeSessionString(Component sessionService, string methodName, string fallback, params object[] args)
        {
            if (sessionService == null)
            {
                return fallback;
            }

            MethodInfo method = sessionService.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                return fallback;
            }

            object result = method.Invoke(sessionService, args);
            return result as string ?? fallback;
        }
    }
}
