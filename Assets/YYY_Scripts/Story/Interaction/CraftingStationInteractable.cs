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

        private float _lastKeyInteractionAt = -999f;

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

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(interactionHint))
            {
                interactionHint = station == CraftingStation.AnvilForge ? "使用铁砧" : "使用工作台";
            }

            interactionDistance = Mathf.Max(0.1f, interactionDistance);
            overlayAutoCloseDistance = Mathf.Max(interactionDistance, overlayAutoCloseDistance);
        }

        private void Update()
        {
            if (!enableProximityKeyInteraction || !Input.GetKeyDown(proximityInteractionKey))
            {
                return;
            }

            if (Time.unscaledTime - _lastKeyInteractionAt < keyInteractionCooldown)
            {
                return;
            }

            InteractionContext context = BuildProximityInteractionContext();
            if (context?.PlayerTransform == null)
            {
                return;
            }

            float distance = Vector2.Distance(context.PlayerPosition, GetClosestInteractionPoint(context.PlayerPosition));
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

        private Vector2 GetClosestInteractionPoint(Vector2 playerPosition)
        {
            Collider2D collider2D = GetComponent<Collider2D>();
            if (collider2D == null)
            {
                collider2D = GetComponentInChildren<Collider2D>();
            }

            return collider2D != null ? collider2D.ClosestPoint(playerPosition) : (Vector2)transform.position;
        }
    }
}
