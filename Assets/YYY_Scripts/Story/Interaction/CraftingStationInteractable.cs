using FarmGame.Data;
using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class CraftingStationInteractable : MonoBehaviour, IInteractable
    {
        [Header("Crafting Station")]
        [SerializeField] private CraftingStation station = CraftingStation.Workbench;
        [SerializeField] private string interactionHint = "使用工作台";
        [SerializeField] private float interactionDistance = 0.5f;
        [SerializeField] private int interactionPriority = 28;
        [SerializeField] private bool createCraftingServiceIfMissing = true;
        [SerializeField] private bool notifySpringDay1Director = true;
        [SerializeField] private CraftingPanel craftingPanel;
        [SerializeField] private bool preferStoryWorkbenchOverlay = true;
        [SerializeField] private SpringDay1WorkbenchCraftingOverlay workbenchOverlay;
        [SerializeField] private float overlayAutoCloseDistance = 1.5f;

        [Header("Test Interaction")]
        [SerializeField] private bool enableProximityKeyInteraction = true;
        [SerializeField] private KeyCode proximityInteractionKey = KeyCode.E;
        [SerializeField] private float keyInteractionCooldown = 0.15f;

        [Header("Workbench Hint Bubble")]
        [SerializeField] private bool showFirstUseBubble = true;
        [SerializeField] private float bubbleRevealDistance = 1.15f;
        [SerializeField] private string bubbleCaption = "交互";

        private float _lastKeyInteractionAt = -999f;
        private Collider2D[] _cachedColliders;
        private bool _bubbleAlreadyAppeared;

        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;

        public bool CanInteract(InteractionContext context)
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return false;
            }

            return ResolveCraftingPanel() != null
                || FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include) != null
                || notifySpringDay1Director;
        }

        public void OnInteract(InteractionContext context)
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return;
            }

            ConsumeBubbleIfNeeded();

            CraftingService craftingService = ResolveCraftingService(createCraftingServiceIfMissing);
            CraftingPanel panel = ResolveCraftingPanel();

            if (craftingService == null && panel == null && !notifySpringDay1Director)
            {
                Debug.LogWarning($"[CraftingStationInteractable] {name} 找不到可用的制作服务或制作面板。");
                return;
            }

            if (craftingService != null)
            {
                craftingService.SetStation(station);
            }

            bool openedRealCraftingUi = false;
            bool handledByWorkbenchOverlay = false;
            if (station == CraftingStation.Workbench && preferStoryWorkbenchOverlay)
            {
                SpringDay1WorkbenchCraftingOverlay overlay = ResolveWorkbenchOverlay();
                if (overlay != null)
                {
                    bool wasVisible = overlay.IsVisible;
                    bool isVisible = overlay.Toggle(transform, context?.PlayerTransform, craftingService, station, overlayAutoCloseDistance);
                    if (wasVisible && !isVisible)
                    {
                        return;
                    }

                    handledByWorkbenchOverlay = isVisible;
                }
            }

            if (!handledByWorkbenchOverlay && panel != null)
            {
                panel.Open(station);
                openedRealCraftingUi = true;
            }

            if (notifySpringDay1Director)
            {
                SpringDay1Director.EnsureRuntime();
                SpringDay1Director.Instance?.NotifyCraftingStationOpened(station);

                if (!openedRealCraftingUi && !handledByWorkbenchOverlay)
                {
                    string fallbackMessage = SpringDay1Director.Instance?.TryHandleWorkbenchTestInteraction(station);
                    if (!string.IsNullOrWhiteSpace(fallbackMessage))
                    {
                        SpringDay1PromptOverlay.EnsureRuntime();
                        SpringDay1PromptOverlay.Instance?.Show(fallbackMessage);
                    }
                }
            }
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
        }

        public void ConfigureRuntimeDefaults(CraftingStation targetStation, string hint, float distance, int priority)
        {
            station = targetStation;
            interactionHint = hint;
            interactionDistance = distance;
            interactionPriority = priority;
        }

        public Bounds GetCombinedBounds()
        {
            Collider2D[] colliders = GetRelevantColliders();
            if (colliders.Length == 0)
            {
                return new Bounds(transform.position, Vector3.one);
            }

            Bounds combinedBounds = colliders[0].bounds;
            for (int index = 1; index < colliders.Length; index++)
            {
                combinedBounds.Encapsulate(colliders[index].bounds);
            }

            return combinedBounds;
        }

        public Vector2 GetClosestInteractionPoint(Vector2 playerPosition)
        {
            Collider2D[] colliders = GetRelevantColliders();
            if (colliders.Length == 0)
            {
                return transform.position;
            }

            float bestDistance = float.MaxValue;
            Vector2 bestPoint = transform.position;
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null)
                {
                    continue;
                }

                Vector2 candidate = collider2D.ClosestPoint(playerPosition);
                float distance = (candidate - playerPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = candidate;
                }
            }

            return bestPoint;
        }

        public float GetBoundaryDistance(Vector2 playerPosition)
        {
            return Vector2.Distance(playerPosition, GetClosestInteractionPoint(playerPosition));
        }

        public bool ShouldDisplayOverlayBelow(Vector2 playerPosition)
        {
            Vector2 closestPoint = GetClosestInteractionPoint(playerPosition);
            float verticalDelta = playerPosition.y - closestPoint.y;
            if (Mathf.Abs(verticalDelta) > 0.04f)
            {
                return verticalDelta > 0f;
            }

            return playerPosition.y > GetCombinedBounds().center.y;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(interactionHint))
            {
                interactionHint = station == CraftingStation.AnvilForge ? "使用铁砧" : "使用工作台";
            }

            interactionDistance = Mathf.Max(0.1f, interactionDistance);
            overlayAutoCloseDistance = Mathf.Max(interactionDistance, overlayAutoCloseDistance);
            bubbleRevealDistance = Mathf.Max(interactionDistance, bubbleRevealDistance);
            CacheColliders();
        }

        private void Awake()
        {
            CacheColliders();
        }

        private void Update()
        {
            InteractionContext context = BuildProximityInteractionContext();
            if (context?.PlayerTransform == null)
            {
                return;
            }

            UpdateWorkbenchHintBubble(context);

            if (!enableProximityKeyInteraction || !Input.GetKeyDown(proximityInteractionKey))
            {
                return;
            }

            if (Time.unscaledTime - _lastKeyInteractionAt < keyInteractionCooldown)
            {
                return;
            }

            float distance = GetBoundaryDistance(context.PlayerPosition);
            if (distance > interactionDistance)
            {
                return;
            }

            if (!CanInteract(context))
            {
                return;
            }

            _lastKeyInteractionAt = Time.unscaledTime;
            OnInteract(context);
        }

        private void UpdateWorkbenchHintBubble(InteractionContext context)
        {
            if (station != CraftingStation.Workbench || !showFirstUseBubble || _bubbleAlreadyAppeared)
            {
                return;
            }

            if (!ShouldShowWorkbenchHint())
            {
                SpringDay1WorldHintBubble.Instance?.Hide();
                return;
            }

            SpringDay1WorkbenchCraftingOverlay overlay = workbenchOverlay != null
                ? workbenchOverlay
                : FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            if (overlay != null && overlay.IsVisible)
            {
                SpringDay1WorldHintBubble.Instance?.Hide();
                return;
            }

            float distance = GetBoundaryDistance(context.PlayerPosition);
            if (distance > bubbleRevealDistance)
            {
                SpringDay1WorldHintBubble.Instance?.Hide();
                return;
            }

            SpringDay1WorldHintBubble.EnsureRuntime();
            SpringDay1WorldHintBubble.Instance.Show(transform, proximityInteractionKey.ToString(), bubbleCaption);
            _bubbleAlreadyAppeared = true;
        }

        private bool ShouldShowWorkbenchHint()
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            return director != null && director.ShouldShowWorkbenchEntryHint();
        }

        private void ConsumeBubbleIfNeeded()
        {
            if (station != CraftingStation.Workbench)
            {
                return;
            }

            SpringDay1WorldHintBubble.Instance?.Hide();
            _bubbleAlreadyAppeared = true;
        }

        private void CacheColliders()
        {
            _cachedColliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
            if (_cachedColliders == null || _cachedColliders.Length == 0)
            {
                Collider2D collider2D = GetComponent<Collider2D>();
                _cachedColliders = collider2D != null ? new[] { collider2D } : System.Array.Empty<Collider2D>();
            }
        }

        private Collider2D[] GetRelevantColliders()
        {
            if (_cachedColliders == null || _cachedColliders.Length == 0)
            {
                CacheColliders();
            }

            return _cachedColliders ?? System.Array.Empty<Collider2D>();
        }

        private CraftingPanel ResolveCraftingPanel()
        {
            if (craftingPanel == null)
            {
                craftingPanel = FindFirstObjectByType<CraftingPanel>(FindObjectsInactive.Include);
            }

            return craftingPanel;
        }

        private SpringDay1WorkbenchCraftingOverlay ResolveWorkbenchOverlay()
        {
            if (workbenchOverlay == null)
            {
                SpringDay1WorkbenchCraftingOverlay.EnsureRuntime();
                workbenchOverlay = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            }

            return workbenchOverlay;
        }

        private static CraftingService ResolveCraftingService(bool createIfMissing)
        {
            CraftingService craftingService = FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
            if (craftingService != null || !createIfMissing)
            {
                return craftingService;
            }

            GameObject runtimeObject = new GameObject(nameof(CraftingService));
            return runtimeObject.AddComponent<CraftingService>();
        }

        private InteractionContext BuildProximityInteractionContext()
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
                PlayerPosition = playerTransform.position
            };
        }
    }
}
