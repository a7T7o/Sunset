using System.Reflection;
using FarmGame.Data;
using FarmGame.UI;
using FarmGame.Data.Core;
using Sunset.Story;
using Unity.Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
[DisallowMultipleComponent]
public sealed class PersistentPlayerSceneBridge : MonoBehaviour
{
    private const string BridgeObjectName = "_PersistentPlayerSceneBridge";
    private const float DefaultTransitionGraceSeconds = 0.35f;
    private const float FallbackCameraSmoothTime = 0.08f;
    private const string HomeScenePath = "Assets/000_Scenes/Home.unity";
    private const string SystemsRootName = "Systems";
    private const string InventoryRootName = "InventorySystem";
    private const string HotbarRootName = "HotbarSelection";
    private const string EquipmentRootName = "EquipmentSystem";
    private const string UiRootName = "UI";
    private const string DialogueCanvasRootName = "DialogueCanvas";
    private const string EventSystemRootName = "EventSystem";
    private const string InteractionHintRootName = "InteractionHintOverlay";
    private const float BoundaryFocusBottomViewportThreshold = 0.24f;
    private const float BoundaryFocusSideViewportThreshold = 0.12f;
    private const float BoundaryFocusMinAlpha = 0.2f;
    private const float BoundaryFocusBlendSpeed = 6f;

    private static readonly BindingFlags InstanceBindingFlags =
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    private static PersistentPlayerSceneBridge s_instance;
    private static PendingSceneEntry s_pendingSceneEntry;

    private PlayerMovement persistentPlayerMovement;
    private PlayerAutoNavigator persistentAutoNavigator;
    private PlayerInteraction persistentPlayerInteraction;
    private PlayerToolController persistentPlayerToolController;
    private Transform persistentPlayerTransform;
    private Rigidbody2D persistentPlayerRigidbody;
    private bool initialSceneProcessed;
    private Camera fallbackSceneCamera;
    private Vector3 fallbackCameraVelocity;
    private WorldObjectSaveData inventorySnapshot;
    private bool hasInventorySnapshot;
    private int hotbarSelectionSnapshot;
    private bool hasHotbarSelectionSnapshot;
    private GameObject persistentSystemsRoot;
    private GameObject persistentInventoryRoot;
    private GameObject persistentHotbarRoot;
    private GameObject persistentEquipmentRoot;
    private GameObject persistentUiRoot;
    private GameObject persistentDialogueCanvasRoot;
    private GameObject persistentEventSystemRoot;
    private GameObject persistentInteractionHintRoot;
    private GameInputManager persistentSceneInputManager;
    private InventoryService persistentInventoryService;
    private HotbarSelectionService persistentHotbarSelectionService;
    private EquipmentService persistentEquipmentService;
    private CanvasGroup persistentUiCanvasGroup;

    private struct PendingSceneEntry
    {
        public string SceneName;
        public string ScenePath;
        public string EntryAnchorName;
        public Vector2 FacingDirection;
        public bool HasFacingDirection;

        public bool IsSet =>
            !string.IsNullOrWhiteSpace(SceneName) ||
            !string.IsNullOrWhiteSpace(ScenePath) ||
            !string.IsNullOrWhiteSpace(EntryAnchorName) ||
            HasFacingDirection;

        public bool Matches(Scene scene)
        {
            if (!scene.IsValid())
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(ScenePath) &&
                string.Equals(scene.path, ScenePath, System.StringComparison.Ordinal))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(SceneName) &&
                   string.Equals(scene.name, SceneName, System.StringComparison.Ordinal);
        }

        public void Clear()
        {
            SceneName = string.Empty;
            ScenePath = string.Empty;
            EntryAnchorName = string.Empty;
            FacingDirection = Vector2.zero;
            HasFacingDirection = false;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    public static void QueueSceneEntry(string sceneName, string scenePath, string entryAnchorName)
    {
        EnsureInstance();
        s_instance.CaptureSceneRuntimeState(SceneManager.GetActiveScene());
        Vector2 facingDirection = s_instance.ResolvePersistentFacingDirection();
        s_pendingSceneEntry.SceneName = string.IsNullOrWhiteSpace(sceneName) ? string.Empty : sceneName.Trim();
        s_pendingSceneEntry.ScenePath = string.IsNullOrWhiteSpace(scenePath) ? string.Empty : scenePath.Trim();
        s_pendingSceneEntry.EntryAnchorName = string.IsNullOrWhiteSpace(entryAnchorName) ? string.Empty : entryAnchorName.Trim();
        s_pendingSceneEntry.FacingDirection = facingDirection;
        s_pendingSceneEntry.HasFacingDirection = facingDirection.sqrMagnitude > 0.01f;
    }

    private static void EnsureInstance()
    {
        if (s_instance != null)
        {
            return;
        }

        GameObject bridgeObject = new GameObject(BridgeObjectName);
        DontDestroyOnLoad(bridgeObject);
        s_instance = bridgeObject.AddComponent<PersistentPlayerSceneBridge>();
    }

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (initialSceneProcessed)
        {
            return;
        }

        initialSceneProcessed = true;
        RebindScene(SceneManager.GetActiveScene());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindScene(scene);
    }

    private void LateUpdate()
    {
        UpdateFallbackInput();
        UpdateFallbackCamera();
        UpdatePersistentUiBoundaryFocus();
    }

    private void RebindScene(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        EnsurePersistentPlayer(scene);
        if (persistentPlayerMovement == null || persistentPlayerTransform == null)
        {
            return;
        }

        PlayerMovement[] scenePlayers = FindPlayersInScene(scene);
        Vector3? spawnPosition = ResolveSpawnPosition(scene, scenePlayers);
        PromoteSceneRuntimeRoots(scene);

        InventoryService sceneInventory = ResolveRuntimeInventoryService(scene);
        HotbarSelectionService sceneHotbarSelection = ResolveRuntimeHotbarSelectionService(scene);

        DestroySceneDuplicatePlayers(scenePlayers);
        BindNavigator(scene);
        RestoreSceneInventoryState(sceneInventory);
        RebindHotbarSelection(sceneHotbarSelection, sceneInventory);
        RebindPersistentCoreUi(scene, sceneInventory, sceneHotbarSelection);

        if (spawnPosition.HasValue)
        {
            ApplyPersistentPlayerPosition(
                spawnPosition.Value,
                s_pendingSceneEntry.HasFacingDirection
                    ? s_pendingSceneEntry.FacingDirection
                    : (Vector2?)null);
        }

        RebindSceneInput(scene, sceneInventory, sceneHotbarSelection);
        ReapplyTraversalBindings(scene);
        RefreshFallbackCameraBinding(scene);
        SceneTransitionTrigger2D.SuppressPlayerEnter(DefaultTransitionGraceSeconds);

        if (s_pendingSceneEntry.Matches(scene))
        {
            s_pendingSceneEntry.Clear();
        }
    }

    private void EnsurePersistentPlayer(Scene preferredScene)
    {
        if (persistentPlayerMovement != null && persistentPlayerTransform != null)
        {
            return;
        }

        PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        PlayerMovement bestCandidate = null;
        int bestScore = int.MinValue;
        for (int index = 0; index < players.Length; index++)
        {
            PlayerMovement candidate = players[index];
            if (candidate == null)
            {
                continue;
            }

            int score = ScorePlayerCandidate(candidate, preferredScene);
            if (score > bestScore)
            {
                bestScore = score;
                bestCandidate = candidate;
            }
        }

        if (bestCandidate == null)
        {
            return;
        }

        persistentPlayerMovement = bestCandidate;
        persistentPlayerTransform = bestCandidate.transform;
        persistentPlayerRigidbody = bestCandidate.GetComponent<Rigidbody2D>();
        persistentAutoNavigator = bestCandidate.GetComponent<PlayerAutoNavigator>();
        persistentPlayerInteraction = bestCandidate.GetComponent<PlayerInteraction>();
        persistentPlayerToolController = bestCandidate.GetComponent<PlayerToolController>();

        if (bestCandidate.gameObject.scene.IsValid())
        {
            DontDestroyOnLoad(bestCandidate.gameObject);
        }
    }

    private void BindNavigator(Scene scene)
    {
        if (persistentAutoNavigator == null)
        {
            persistentAutoNavigator = persistentPlayerMovement != null
                ? persistentPlayerMovement.GetComponent<PlayerAutoNavigator>()
                : null;
        }

        if (persistentAutoNavigator == null)
        {
            return;
        }

        NavGrid2D navGrid = FindFirstComponentInScene<NavGrid2D>(scene);
        persistentAutoNavigator.BindRuntimeSceneReferences(
            persistentPlayerMovement,
            persistentPlayerTransform,
            navGrid);
        persistentAutoNavigator.ForceCancel();
    }

    private void RebindSceneInput(
        Scene scene,
        InventoryService sceneInventory,
        HotbarSelectionService sceneHotbarSelection)
    {
        GameInputManager sceneInputManager = ResolveRuntimeInputManager(scene);
        if (sceneInputManager == null)
        {
            return;
        }

        Camera sceneCamera = ResolveRuntimeCamera(scene);
        TrySetField(sceneInputManager, "playerMovement", persistentPlayerMovement);
        TrySetField(sceneInputManager, "playerInteraction", persistentPlayerInteraction);
        TrySetField(sceneInputManager, "playerToolController", persistentPlayerToolController);
        TrySetField(sceneInputManager, "autoNavigator", persistentAutoNavigator);
        TrySetField(sceneInputManager, "inventory", sceneInventory);
        TrySetField(sceneInputManager, "hotbarSelection", sceneHotbarSelection);
        TrySetField(sceneInputManager, "database", sceneInventory != null ? sceneInventory.Database : null);
        TrySetField(sceneInputManager, "worldCamera", sceneCamera);
        TrySetField(sceneInputManager, "packageTabs", persistentUiRoot != null ? persistentUiRoot.GetComponentInChildren<PackagePanelTabsUI>(true) : null);
        TrySetBooleanField(sceneInputManager, "packageTabsInitialized", false);
    }

    private void ReapplyTraversalBindings(Scene scene)
    {
        TraversalBlockManager2D[] managers = FindComponentsInScene<TraversalBlockManager2D>(scene);
        for (int index = 0; index < managers.Length; index++)
        {
            TraversalBlockManager2D manager = managers[index];
            if (manager == null)
            {
                continue;
            }

            manager.ApplyConfiguration(rebuildNavGrid: true);
        }
    }

    private void ApplyPersistentPlayerPosition(Vector3 worldPosition, Vector2? facingDirection = null)
    {
        if (persistentPlayerTransform == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(
            worldPosition.x,
            worldPosition.y,
            persistentPlayerTransform.position.z);

        if (persistentAutoNavigator != null)
        {
            persistentAutoNavigator.ForceCancel();
        }

        if (persistentPlayerMovement != null)
        {
            persistentPlayerMovement.SetMovementInput(Vector2.zero, false, facingDirection);
        }

        if (persistentPlayerRigidbody != null)
        {
            persistentPlayerRigidbody.position = targetPosition;
            persistentPlayerRigidbody.linearVelocity = Vector2.zero;
        }

        persistentPlayerTransform.position = targetPosition;
    }

    private Vector2 ResolvePersistentFacingDirection()
    {
        if (persistentPlayerMovement == null)
        {
            return Vector2.down;
        }

        switch (persistentPlayerMovement.GetCurrentFacingDirection())
        {
            case PlayerAnimController.AnimDirection.Up:
                return Vector2.up;
            case PlayerAnimController.AnimDirection.Left:
                return Vector2.left;
            case PlayerAnimController.AnimDirection.Right:
                return Vector2.right;
            default:
                return Vector2.down;
        }
    }

    private void CaptureSceneRuntimeState(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        InventoryService sceneInventory = FindFirstComponentInScene<InventoryService>(scene);
        if (sceneInventory == null)
        {
            sceneInventory = persistentInventoryService;
        }

        if (sceneInventory != null)
        {
            inventorySnapshot = sceneInventory.Save();
            hasInventorySnapshot = inventorySnapshot != null &&
                                 !string.IsNullOrWhiteSpace(inventorySnapshot.genericData);
        }

        HotbarSelectionService sceneHotbarSelection = FindFirstComponentInScene<HotbarSelectionService>(scene);
        if (sceneHotbarSelection == null)
        {
            sceneHotbarSelection = persistentHotbarSelectionService;
        }

        if (sceneHotbarSelection != null)
        {
            hotbarSelectionSnapshot = sceneHotbarSelection.selectedIndex;
            hasHotbarSelectionSnapshot = true;
        }
    }

    private void RestoreSceneInventoryState(InventoryService sceneInventory)
    {
        if (!hasInventorySnapshot || sceneInventory == null || inventorySnapshot == null)
        {
            return;
        }

        sceneInventory.Load(inventorySnapshot);
    }

    private void RebindHotbarSelection(
        HotbarSelectionService sceneHotbarSelection,
        InventoryService sceneInventory)
    {
        if (sceneHotbarSelection == null)
        {
            return;
        }

        sceneHotbarSelection.RebindRuntimeReferences(persistentPlayerToolController, sceneInventory);
        sceneHotbarSelection.RestoreSelection(
            hasHotbarSelectionSnapshot ? hotbarSelectionSnapshot : sceneHotbarSelection.selectedIndex);
    }

    private void RefreshFallbackCameraBinding(Scene scene)
    {
        if (FindFirstComponentInScene<CinemachineCamera>(scene) != null)
        {
            fallbackSceneCamera = null;
            fallbackCameraVelocity = Vector3.zero;
            return;
        }

        fallbackSceneCamera = FindMainCameraInScene(scene);
        fallbackCameraVelocity = Vector3.zero;
        if (IsFixedFallbackCameraScene(scene))
        {
            return;
        }

        SnapFallbackCameraToPlayer();
    }

    private void UpdateFallbackInput()
    {
        if (persistentPlayerMovement == null)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (ResolveRuntimeInputManager(activeScene) != null)
        {
            return;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        persistentPlayerMovement.SetMovementInput(input, sprint);
    }

    private void UpdateFallbackCamera()
    {
        if (fallbackSceneCamera == null || persistentPlayerTransform == null)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (FindFirstComponentInScene<CinemachineCamera>(activeScene) != null)
        {
            return;
        }

        if (IsFixedFallbackCameraScene(activeScene))
        {
            fallbackCameraVelocity = Vector3.zero;
            return;
        }

        Vector3 current = fallbackSceneCamera.transform.position;
        Vector3 target = new Vector3(
            persistentPlayerTransform.position.x,
            persistentPlayerTransform.position.y,
            current.z);

        fallbackSceneCamera.transform.position = Vector3.SmoothDamp(
            current,
            target,
            ref fallbackCameraVelocity,
            FallbackCameraSmoothTime);
    }

    private void UpdatePersistentUiBoundaryFocus()
    {
        if (persistentUiRoot == null)
        {
            return;
        }

        EnsurePersistentUiCanvasGroup();
        if (persistentUiCanvasGroup == null)
        {
            return;
        }

        float targetAlpha = ResolveBoundaryFocusTargetAlpha();
        persistentUiCanvasGroup.alpha = Mathf.MoveTowards(
            persistentUiCanvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * BoundaryFocusBlendSpeed);

        bool shouldBlockRaycasts = persistentUiCanvasGroup.alpha > 0.95f || ShouldKeepCoreUiFullyVisible();
        persistentUiCanvasGroup.blocksRaycasts = shouldBlockRaycasts;
        persistentUiCanvasGroup.interactable = shouldBlockRaycasts;
    }

    private void SnapFallbackCameraToPlayer()
    {
        if (fallbackSceneCamera == null || persistentPlayerTransform == null)
        {
            return;
        }

        Vector3 current = fallbackSceneCamera.transform.position;
        fallbackSceneCamera.transform.position = new Vector3(
            persistentPlayerTransform.position.x,
            persistentPlayerTransform.position.y,
            current.z);
    }

    private void PromoteSceneRuntimeRoots(Scene scene)
    {
        persistentSystemsRoot = CaptureOrPersistSceneRoot(scene, persistentSystemsRoot, SystemsRootName);
        persistentInventoryRoot = CaptureOrPersistSceneRoot(scene, persistentInventoryRoot, InventoryRootName);
        persistentHotbarRoot = CaptureOrPersistSceneRoot(scene, persistentHotbarRoot, HotbarRootName);
        persistentEquipmentRoot = CaptureOrPersistSceneRoot(scene, persistentEquipmentRoot, EquipmentRootName);
        persistentUiRoot = CaptureOrPersistSceneRoot(scene, persistentUiRoot, UiRootName);
        persistentDialogueCanvasRoot = CaptureOrPersistSceneRoot(scene, persistentDialogueCanvasRoot, DialogueCanvasRootName);
        persistentEventSystemRoot = CaptureOrPersistSceneRoot(scene, persistentEventSystemRoot, EventSystemRootName);
        persistentInteractionHintRoot = CaptureOrPersistSceneRoot(scene, persistentInteractionHintRoot, InteractionHintRootName);

        persistentSceneInputManager = persistentSystemsRoot != null
            ? persistentSystemsRoot.GetComponent<GameInputManager>()
            : persistentSceneInputManager;
        persistentInventoryService = persistentInventoryRoot != null
            ? persistentInventoryRoot.GetComponent<InventoryService>()
            : persistentInventoryService;
        persistentHotbarSelectionService = persistentHotbarRoot != null
            ? persistentHotbarRoot.GetComponent<HotbarSelectionService>()
            : persistentHotbarSelectionService;
        persistentEquipmentService = persistentEquipmentRoot != null
            ? persistentEquipmentRoot.GetComponent<EquipmentService>()
            : persistentEquipmentService;
    }

    private void RebindPersistentCoreUi(
        Scene scene,
        InventoryService sceneInventory,
        HotbarSelectionService sceneHotbarSelection)
    {
        if (persistentUiRoot == null)
        {
            return;
        }

        persistentUiRoot.SetActive(true);
        if (persistentDialogueCanvasRoot != null)
        {
            persistentDialogueCanvasRoot.SetActive(true);
        }

        if (persistentEventSystemRoot != null)
        {
            persistentEventSystemRoot.SetActive(true);
        }

        if (persistentInteractionHintRoot != null)
        {
            persistentInteractionHintRoot.SetActive(true);
        }

        EnsurePersistentUiCanvasGroup();

        EquipmentService sceneEquipment = ResolveRuntimeEquipmentService(scene);
        ItemDatabase database = sceneInventory != null ? sceneInventory.Database : null;
        InventorySortService sortService = FindFirstObjectByType<InventorySortService>(FindObjectsInactive.Include);

        ToolbarUI[] toolbarUis = persistentUiRoot.GetComponentsInChildren<ToolbarUI>(true);
        for (int index = 0; index < toolbarUis.Length; index++)
        {
            ToolbarUI toolbarUi = toolbarUis[index];
            if (toolbarUi == null)
            {
                continue;
            }

            TrySetField(toolbarUi, "inventory", sceneInventory);
            TrySetField(toolbarUi, "database", database);
            TrySetField(toolbarUi, "selection", sceneHotbarSelection);
            toolbarUi.Build();
            toolbarUi.ForceRefresh();
        }

        InventoryPanelUI[] inventoryPanels = persistentUiRoot.GetComponentsInChildren<InventoryPanelUI>(true);
        for (int index = 0; index < inventoryPanels.Length; index++)
        {
            InventoryPanelUI panel = inventoryPanels[index];
            if (panel == null)
            {
                continue;
            }

            TrySetField(panel, "inventory", sceneInventory);
            TrySetField(panel, "equipment", sceneEquipment);
            TrySetField(panel, "database", database);
            TrySetField(panel, "selection", sceneHotbarSelection);
            panel.EnsureBuilt();
        }

        InventoryInteractionManager interactionManager =
            persistentUiRoot.GetComponentInChildren<InventoryInteractionManager>(true);
        if (interactionManager != null)
        {
            TrySetField(interactionManager, "inventory", sceneInventory);
            TrySetField(interactionManager, "equipment", sceneEquipment);
            TrySetField(interactionManager, "database", database);
            TrySetField(interactionManager, "sortService", sortService);
            interactionManager.ClearHeldState();
            interactionManager.HideHeldIcon();
        }

        PackagePanelTabsUI[] packagePanels = persistentUiRoot.GetComponentsInChildren<PackagePanelTabsUI>(true);
        for (int index = 0; index < packagePanels.Length; index++)
        {
            PackagePanelTabsUI packagePanel = packagePanels[index];
            if (packagePanel == null)
            {
                continue;
            }

            packagePanel.EnsureReady();
            packagePanel.ClosePanelForExternalAction();
        }

        ItemTooltip[] tooltips = persistentUiRoot.GetComponentsInChildren<ItemTooltip>(true);
        for (int index = 0; index < tooltips.Length; index++)
        {
            ItemTooltip tooltip = tooltips[index];
            if (tooltip == null)
            {
                continue;
            }

            tooltip.SetDatabase(database);
        }

        Canvas.ForceUpdateCanvases();
    }

    private void EnsurePersistentUiCanvasGroup()
    {
        if (persistentUiRoot == null)
        {
            return;
        }

        if (persistentUiCanvasGroup == null)
        {
            persistentUiCanvasGroup = persistentUiRoot.GetComponent<CanvasGroup>();
            if (persistentUiCanvasGroup == null)
            {
                persistentUiCanvasGroup = persistentUiRoot.AddComponent<CanvasGroup>();
            }
        }
    }

    private float ResolveBoundaryFocusTargetAlpha()
    {
        if (ShouldKeepCoreUiFullyVisible())
        {
            return 1f;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        Camera runtimeCamera = ResolveRuntimeCamera(activeScene);
        if (runtimeCamera == null || persistentPlayerTransform == null)
        {
            return 1f;
        }

        Vector3 viewport = runtimeCamera.WorldToViewportPoint(persistentPlayerTransform.position);
        if (viewport.z <= 0f)
        {
            return 1f;
        }

        float bottomPressure = Mathf.Clamp01((BoundaryFocusBottomViewportThreshold - viewport.y) / BoundaryFocusBottomViewportThreshold);
        float leftPressure = Mathf.Clamp01((BoundaryFocusSideViewportThreshold - viewport.x) / BoundaryFocusSideViewportThreshold);
        float rightPressure = Mathf.Clamp01((viewport.x - (1f - BoundaryFocusSideViewportThreshold)) / BoundaryFocusSideViewportThreshold);
        float edgePressure = Mathf.Max(bottomPressure, leftPressure * 0.65f, rightPressure * 0.65f);

        return Mathf.Lerp(1f, BoundaryFocusMinAlpha, edgePressure);
    }

    private bool ShouldKeepCoreUiFullyVisible()
    {
        if (persistentUiRoot == null || !persistentUiRoot.activeInHierarchy)
        {
            return true;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return true;
        }

        PackagePanelTabsUI packageTabs = persistentUiRoot.GetComponentInChildren<PackagePanelTabsUI>(true);
        if (packageTabs != null && packageTabs.IsPanelOpen())
        {
            return true;
        }

        return BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen;
    }

    private Vector3? ResolveSpawnPosition(Scene scene, PlayerMovement[] scenePlayers)
    {
        Transform entryAnchor = ResolveEntryAnchor(scene);
        if (entryAnchor != null)
        {
            return entryAnchor.position;
        }

        for (int index = 0; index < scenePlayers.Length; index++)
        {
            PlayerMovement scenePlayer = scenePlayers[index];
            if (scenePlayer == null || scenePlayer == persistentPlayerMovement)
            {
                continue;
            }

            return scenePlayer.transform.position;
        }

        return null;
    }

    private Transform ResolveEntryAnchor(Scene scene)
    {
        if (!s_pendingSceneEntry.Matches(scene) ||
            string.IsNullOrWhiteSpace(s_pendingSceneEntry.EntryAnchorName))
        {
            return null;
        }

        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < transforms.Length; index++)
        {
            Transform candidate = transforms[index];
            if (candidate == null || candidate.gameObject.scene != scene)
            {
                continue;
            }

            if (string.Equals(candidate.name, s_pendingSceneEntry.EntryAnchorName, System.StringComparison.Ordinal))
            {
                return candidate;
            }
        }

        return null;
    }

    private void DestroySceneDuplicatePlayers(PlayerMovement[] scenePlayers)
    {
        for (int index = 0; index < scenePlayers.Length; index++)
        {
            PlayerMovement scenePlayer = scenePlayers[index];
            if (scenePlayer == null || scenePlayer == persistentPlayerMovement)
            {
                continue;
            }

            Destroy(scenePlayer.gameObject);
        }
    }

    private static PlayerMovement[] FindPlayersInScene(Scene scene)
    {
        PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        System.Collections.Generic.List<PlayerMovement> results = new System.Collections.Generic.List<PlayerMovement>();
        for (int index = 0; index < players.Length; index++)
        {
            PlayerMovement candidate = players[index];
            if (candidate == null || candidate.gameObject.scene != scene)
            {
                continue;
            }

            results.Add(candidate);
        }

        return results.ToArray();
    }

    private static int ScorePlayerCandidate(PlayerMovement candidate, Scene preferredScene)
    {
        int score = 0;
        GameObject gameObject = candidate.gameObject;
        if (gameObject.scene == preferredScene)
        {
            score += 200;
        }

        if (gameObject.activeInHierarchy)
        {
            score += 80;
        }

        if (gameObject.CompareTag("Player"))
        {
            score += 40;
        }

        if (string.Equals(gameObject.name, "Player", System.StringComparison.Ordinal))
        {
            score += 20;
        }

        return score;
    }

    private static GameObject CaptureOrPersistSceneRoot(Scene scene, GameObject persistentRoot, string rootName)
    {
        if (persistentRoot != null)
        {
            GameObject duplicateRoot = FindRootGameObjectInScene(scene, rootName);
            if (duplicateRoot != null && duplicateRoot != persistentRoot)
            {
                Destroy(duplicateRoot);
            }

            return persistentRoot;
        }

        GameObject sceneRoot = FindRootGameObjectInScene(scene, rootName);
        if (sceneRoot == null)
        {
            return null;
        }

        DontDestroyOnLoad(sceneRoot);
        return sceneRoot;
    }

    private static GameObject FindRootGameObjectInScene(Scene scene, string rootName)
    {
        if (!scene.IsValid() || !scene.isLoaded || string.IsNullOrWhiteSpace(rootName))
        {
            return null;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int index = 0; index < roots.Length; index++)
        {
            GameObject candidate = roots[index];
            if (candidate != null &&
                string.Equals(candidate.name, rootName, System.StringComparison.Ordinal))
            {
                return candidate;
            }
        }

        return null;
    }

    private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
    {
        T[] components = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < components.Length; index++)
        {
            T candidate = components[index];
            if (candidate != null && candidate.gameObject.scene == scene)
            {
                return candidate;
            }
        }

        return null;
    }

    private static T[] FindComponentsInScene<T>(Scene scene) where T : Component
    {
        T[] components = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        System.Collections.Generic.List<T> results = new System.Collections.Generic.List<T>();
        for (int index = 0; index < components.Length; index++)
        {
            T candidate = components[index];
            if (candidate != null && candidate.gameObject.scene == scene)
            {
                results.Add(candidate);
            }
        }

        return results.ToArray();
    }

    private InventoryService ResolveRuntimeInventoryService(Scene scene)
    {
        InventoryService sceneInventory = FindFirstComponentInScene<InventoryService>(scene);
        if (sceneInventory != null)
        {
            return sceneInventory;
        }

        return persistentInventoryService;
    }

    private HotbarSelectionService ResolveRuntimeHotbarSelectionService(Scene scene)
    {
        HotbarSelectionService sceneHotbar = FindFirstComponentInScene<HotbarSelectionService>(scene);
        if (sceneHotbar != null)
        {
            return sceneHotbar;
        }

        return persistentHotbarSelectionService;
    }

    private EquipmentService ResolveRuntimeEquipmentService(Scene scene)
    {
        EquipmentService sceneEquipment = FindFirstComponentInScene<EquipmentService>(scene);
        if (sceneEquipment != null)
        {
            return sceneEquipment;
        }

        return persistentEquipmentService;
    }

    private GameInputManager ResolveRuntimeInputManager(Scene scene)
    {
        GameInputManager sceneInput = FindFirstComponentInScene<GameInputManager>(scene);
        if (sceneInput != null)
        {
            return sceneInput;
        }

        return persistentSceneInputManager;
    }

    private static Camera FindMainCameraInScene(Scene scene)
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Camera best = null;
        int bestScore = int.MinValue;
        for (int index = 0; index < cameras.Length; index++)
        {
            Camera candidate = cameras[index];
            if (candidate == null || candidate.gameObject.scene != scene)
            {
                continue;
            }

            int score = 0;
            if (candidate.CompareTag("MainCamera"))
            {
                score += 100;
            }

            if (string.Equals(candidate.name, "Main Camera", System.StringComparison.Ordinal))
            {
                score += 20;
            }

            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private Camera ResolveRuntimeCamera(Scene scene)
    {
        Camera sceneCamera = FindMainCameraInScene(scene);
        if (sceneCamera != null)
        {
            return sceneCamera;
        }

        if (Camera.main != null)
        {
            return Camera.main;
        }

        return fallbackSceneCamera;
    }

    private static bool IsFixedFallbackCameraScene(Scene scene)
    {
        if (!scene.IsValid())
        {
            return false;
        }

        return string.Equals(scene.path, HomeScenePath, System.StringComparison.Ordinal);
    }

    private static void TrySetField(Component target, string fieldName, Object value)
    {
        if (target == null)
        {
            return;
        }

        FieldInfo field = target.GetType().GetField(fieldName, InstanceBindingFlags);
        if (field == null || !field.FieldType.IsAssignableFrom(value != null ? value.GetType() : typeof(Object)))
        {
            if (field != null && value == null && field.FieldType.IsClass)
            {
                field.SetValue(target, null);
            }

            return;
        }

        field.SetValue(target, value);
    }

    private static void TrySetBooleanField(Component target, string fieldName, bool value)
    {
        if (target == null)
        {
            return;
        }

        FieldInfo field = target.GetType().GetField(fieldName, InstanceBindingFlags);
        if (field == null || field.FieldType != typeof(bool))
        {
            return;
        }

        field.SetValue(target, value);
    }
}
