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
        [SerializeField] private float interactionDistance = 1.8f;
        [SerializeField] private int interactionPriority = 28;
        [SerializeField] private bool createCraftingServiceIfMissing = true;
        [SerializeField] private bool notifySpringDay1Director = true;
        [SerializeField] private CraftingPanel craftingPanel;

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

            if (panel != null)
            {
                panel.Open(station);
            }

            if (notifySpringDay1Director)
            {
                SpringDay1Director.EnsureRuntime();
                SpringDay1Director.Instance?.NotifyCraftingStationOpened(station);
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
        }

        private CraftingPanel ResolveCraftingPanel()
        {
            if (craftingPanel == null)
            {
                craftingPanel = FindFirstObjectByType<CraftingPanel>(FindObjectsInactive.Include);
            }

            return craftingPanel;
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
    }
}
