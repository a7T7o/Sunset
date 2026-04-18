using System.Collections.Generic;
using System.Reflection;
using FarmGame.Data;
using FarmGame.Farm;
using FarmGame.UI;
using FarmGame.Data.Core;
using FarmGame.World;
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
    private const string SpringDay1NpcCrowdManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";
    private const string SystemsRootName = "Systems";
    private const string InventoryRootName = "InventorySystem";
    private const string HotbarRootName = "HotbarSelection";
    private const string EquipmentRootName = "EquipmentSystem";
    private const string UiRootName = "UI";
    private const string DialogueCanvasRootName = "DialogueCanvas";
    private const string EventSystemRootName = "EventSystem";
    private const string InteractionHintRootName = "InteractionHintOverlay";
    private const string HealthSystemRootName = "HealthSystem";
    private const string SprintStateManagerRootName = "SprintStateManager";
    private const float BoundaryFocusStartViewportThreshold = 0.25f;
    private const float BoundaryFocusFullViewportThreshold = 0.18f;
    private const float BoundaryFocusMinAlpha = 0.40f;
    private const float BoundaryFocusBlendSpeed = 18f;

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
    private int hotbarInventorySelectionSnapshot;
    private bool hasHotbarSelectionSnapshot;
    private readonly Dictionary<string, List<SceneWorldRuntimeSnapshotEntry>> sceneWorldSnapshotsByScene =
        new Dictionary<string, List<SceneWorldRuntimeSnapshotEntry>>(System.StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> sceneWorldSnapshotCapturedTotalDaysByScene =
        new Dictionary<string, int>(System.StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<NpcResidentRuntimeSnapshot>> nativeResidentSnapshotsByScene =
        new Dictionary<string, List<NpcResidentRuntimeSnapshot>>(System.StringComparer.OrdinalIgnoreCase);
    private List<SpringDay1NpcCrowdDirector.ResidentRuntimeSnapshotEntry> crowdResidentSnapshots =
        new List<SpringDay1NpcCrowdDirector.ResidentRuntimeSnapshotEntry>();
    private HashSet<string> crowdDirectorManagedNpcIds;
    private GameObject persistentSystemsRoot;
    private GameObject persistentInventoryRoot;
    private GameObject persistentHotbarRoot;
    private GameObject persistentEquipmentRoot;
    private GameObject persistentUiRoot;
    private GameObject persistentDialogueCanvasRoot;
    private GameObject persistentEventSystemRoot;
    private GameObject persistentInteractionHintRoot;
    private GameObject persistentHealthSystemRoot;
    private GameObject persistentSprintStateManagerRoot;
    private GameInputManager persistentSceneInputManager;
    private InventoryService persistentInventoryService;
    private HotbarSelectionService persistentHotbarSelectionService;
    private EquipmentService persistentEquipmentService;
    private HealthSystem persistentHealthSystem;
    private SprintStateManager persistentSprintStateManager;
    private CanvasGroup persistentUiCanvasGroup;
    private readonly List<BoundaryFocusUiTarget> boundaryFocusUiTargets = new List<BoundaryFocusUiTarget>();
    private int lastReboundSceneHandle = int.MinValue;
    private Coroutine startupFallbackRebindCoroutine;
    private Coroutine sceneWorldRestoreCoroutine;
    private string suppressedSceneWorldRestoreSceneName = string.Empty;

    [System.Flags]
    private enum BoundaryFocusEdgeMask
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3
    }

    private sealed class BoundaryFocusUiTarget
    {
        public RectTransform RectTransform;
        public CanvasGroup CanvasGroup;
        public SpringDay1PromptOverlay PromptOverlay;
    }

    private sealed class SceneWorldRuntimeSnapshotEntry
    {
        public string Guid;
        public string ObjectType;
        public bool IsActive;
        public WorldObjectSaveData SaveData;
    }

    private sealed class SceneWorldRuntimeBinding
    {
        public MonoBehaviour Component;
        public IPersistentObject PersistentObject;
        public GameObject StateRoot;
        public string ObjectType;
    }

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

    public static bool TryApplyLoadedPlayerState(Vector2 worldPosition, Vector2? facingDirection = null)
    {
        EnsureInstance();
        return s_instance != null && s_instance.TryApplyLoadedPlayerStateInternal(worldPosition, facingDirection);
    }

    public static void ResetPersistentRuntimeForFreshStart()
    {
        if (s_instance == null)
        {
            return;
        }

        s_instance.ResetPersistentRuntimeForFreshStartInternal();
    }

    public static void RefreshActiveSceneRuntimeBindings()
    {
        EnsureInstance();
        if (s_instance == null)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        s_instance.RefreshSceneRuntimeBindingsInternal(activeScene);
    }

    public static bool IsSceneWorldRestoreInProgress()
    {
        if (!Application.isPlaying)
        {
            return false;
        }

        EnsureInstance();
        return s_instance != null && s_instance.sceneWorldRestoreCoroutine != null;
    }

    public static void SyncActiveSceneInventorySnapshot()
    {
        EnsureInstance();
        if (s_instance == null)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        s_instance.SyncSceneInventorySnapshotInternal(activeScene);
    }

    public static void ClearNativeResidentRuntimeSnapshots(params string[] stableKeys)
    {
        if (stableKeys == null || stableKeys.Length <= 0)
        {
            return;
        }

        if (s_instance == null)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            EnsureInstance();
        }

        if (s_instance == null)
        {
            return;
        }

        s_instance.ClearNativeResidentRuntimeSnapshotsInternal(stableKeys);
    }

    public static List<SceneWorldSnapshotSaveData> ExportOffSceneWorldSnapshotsForSave()
    {
        if (s_instance == null)
        {
            return new List<SceneWorldSnapshotSaveData>();
        }

        return s_instance.ExportOffSceneWorldSnapshotsForSaveInternal(SceneManager.GetActiveScene());
    }

    public static void ImportOffSceneWorldSnapshotsFromSave(List<SceneWorldSnapshotSaveData> serializedSnapshots)
    {
        if (s_instance == null)
        {
            return;
        }

        s_instance.ImportOffSceneWorldSnapshotsFromSaveInternal(serializedSnapshots, SceneManager.GetActiveScene());
    }

    public static void SuppressSceneWorldRestoreForScene(string sceneName)
    {
        EnsureInstance();
        if (s_instance == null)
        {
            return;
        }

        s_instance.suppressedSceneWorldRestoreSceneName = string.IsNullOrWhiteSpace(sceneName)
            ? string.Empty
            : sceneName.Trim();
    }

    public static void CancelSuppressedSceneWorldRestore(string sceneName)
    {
        if (s_instance == null
            || string.IsNullOrWhiteSpace(s_instance.suppressedSceneWorldRestoreSceneName)
            || string.IsNullOrWhiteSpace(sceneName))
        {
            return;
        }

        if (string.Equals(
                s_instance.suppressedSceneWorldRestoreSceneName,
                sceneName.Trim(),
                System.StringComparison.OrdinalIgnoreCase))
        {
            s_instance.suppressedSceneWorldRestoreSceneName = string.Empty;
        }
    }

    public static InventoryService GetPreferredRuntimeInventoryService()
    {
        if (!Application.isPlaying)
        {
            return null;
        }

        EnsureInstance();
        if (s_instance == null)
        {
            return null;
        }

        s_instance.RefreshPersistentRuntimeServices();
        return s_instance.persistentInventoryService;
    }

    public static HotbarSelectionService GetPreferredRuntimeHotbarSelectionService()
    {
        if (!Application.isPlaying)
        {
            return null;
        }

        EnsureInstance();
        if (s_instance == null)
        {
            return null;
        }

        s_instance.RefreshPersistentRuntimeServices();
        return s_instance.persistentHotbarSelectionService;
    }

    public static EquipmentService GetPreferredRuntimeEquipmentService()
    {
        if (!Application.isPlaying)
        {
            return null;
        }

        EnsureInstance();
        if (s_instance == null)
        {
            return null;
        }

        s_instance.RefreshPersistentRuntimeServices();
        return s_instance.persistentEquipmentService;
    }

    public static GameInputManager GetPreferredRuntimeInputManager()
    {
        if (!Application.isPlaying)
        {
            return null;
        }

        EnsureInstance();
        if (s_instance == null)
        {
            return null;
        }

        s_instance.RefreshPersistentRuntimeServices();
        return s_instance.persistentSceneInputManager;
    }

    public static GameObject GetPreferredRuntimeUiRoot()
    {
        if (!Application.isPlaying)
        {
            return null;
        }

        EnsureInstance();
        return s_instance != null ? s_instance.persistentUiRoot : null;
    }

    public static PackagePanelTabsUI GetPreferredRuntimePackageTabs()
    {
        if (!Application.isPlaying)
        {
            return null;
        }

        EnsureInstance();
        if (s_instance?.persistentUiRoot == null)
        {
            return null;
        }

        return s_instance.persistentUiRoot.GetComponentInChildren<PackagePanelTabsUI>(true);
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

        if (startupFallbackRebindCoroutine != null)
        {
            StopCoroutine(startupFallbackRebindCoroutine);
            startupFallbackRebindCoroutine = null;
        }

        if (sceneWorldRestoreCoroutine != null)
        {
            StopCoroutine(sceneWorldRestoreCoroutine);
            sceneWorldRestoreCoroutine = null;
        }
    }

    private void Start()
    {
        if (initialSceneProcessed)
        {
            return;
        }

        initialSceneProcessed = true;
        Scene activeScene = SceneManager.GetActiveScene();
        if (WasSceneAlreadyRebound(activeScene))
        {
            return;
        }

        ScheduleStartupFallbackRebind();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // sceneLoaded 是每次真正加载完场景后的正式重绑入口。
        // 这里不能再按 scene.handle 做“已处理”短路，否则往返同一路径时一旦 handle 复用，
        // 整条 runtime rebind 会被跳过，留下 Home 正常 / 回 Primary 又乱的坏相。
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
        RebindAutoPickup(sceneInventory);
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
        RebindScenePlacementManager(scene, sceneInventory, sceneHotbarSelection);
        ReapplyTraversalBindings(scene);
        RebindSceneOcclusionManagers(scene);
        RestoreSceneResidentRuntimeState(scene);
        ScheduleSceneWorldRestore(scene);
        RefreshFallbackCameraBinding(scene);
        SceneTransitionTrigger2D.SuppressPlayerEnter(DefaultTransitionGraceSeconds);

        if (s_pendingSceneEntry.Matches(scene))
        {
            s_pendingSceneEntry.Clear();
        }

        lastReboundSceneHandle = scene.handle;
    }

    private void RefreshSceneRuntimeBindingsInternal(Scene scene)
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

        InventoryService sceneInventory = ResolveRuntimeInventoryService(scene);
        HotbarSelectionService sceneHotbarSelection = ResolveRuntimeHotbarSelectionService(scene);

        RestoreSceneInventoryState(sceneInventory);
        RebindHotbarSelection(sceneHotbarSelection, sceneInventory);
        RebindAutoPickup(sceneInventory);
        RebindPersistentCoreUi(scene, sceneInventory, sceneHotbarSelection);
        RebindSceneInput(scene, sceneInventory, sceneHotbarSelection);
        RebindScenePlacementManager(scene, sceneInventory, sceneHotbarSelection);
        RebindSceneOcclusionManagers(scene);
    }

    private void SyncSceneInventorySnapshotInternal(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        InventoryService sceneInventory = ResolveRuntimeInventoryService(scene);
        if (sceneInventory != null)
        {
            inventorySnapshot = sceneInventory.Save();
            hasInventorySnapshot = inventorySnapshot != null &&
                                 !string.IsNullOrWhiteSpace(inventorySnapshot.genericData);
        }
        else
        {
            inventorySnapshot = null;
            hasInventorySnapshot = false;
        }

        HotbarSelectionService sceneHotbarSelection = ResolveRuntimeHotbarSelectionService(scene);
        if (sceneHotbarSelection != null)
        {
            hotbarSelectionSnapshot = sceneHotbarSelection.selectedIndex;
            hotbarInventorySelectionSnapshot = sceneHotbarSelection.selectedInventoryIndex;
            hasHotbarSelectionSnapshot = true;
        }
        else
        {
            hotbarSelectionSnapshot = 0;
            hotbarInventorySelectionSnapshot = 0;
            hasHotbarSelectionSnapshot = false;
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
        sceneInputManager.ResetPlacementRuntimeState("PersistentPlayerSceneBridge 场景重绑后重置输入运行态");
        persistentSceneInputManager = sceneInputManager;
    }

    private void RebindScenePlacementManager(
        Scene scene,
        InventoryService sceneInventory,
        HotbarSelectionService sceneHotbarSelection)
    {
        PlacementManager scenePlacementManager = FindFirstComponentInScene<PlacementManager>(scene);
        if (scenePlacementManager == null)
        {
            return;
        }

        Camera sceneCamera = ResolveRuntimeCamera(scene);
        PackagePanelTabsUI runtimePackageTabs = persistentUiRoot != null
            ? persistentUiRoot.GetComponentInChildren<PackagePanelTabsUI>(true)
            : null;

        scenePlacementManager.RebindRuntimeSceneReferences(
            persistentPlayerTransform,
            sceneInventory,
            sceneHotbarSelection,
            runtimePackageTabs,
            sceneCamera);
    }

    private void RebindAutoPickup(InventoryService sceneInventory)
    {
        if (persistentPlayerTransform == null)
        {
            return;
        }

        AutoPickupService autoPickupService = persistentPlayerTransform.GetComponentInChildren<AutoPickupService>(true);
        if (autoPickupService == null)
        {
            return;
        }

        autoPickupService.RebindRuntimeReferences(sceneInventory);
    }

    private void ReapplyTraversalBindings(Scene scene)
    {
        NavGrid2D primarySceneNavGrid = FindFirstComponentInScene<NavGrid2D>(scene);
        TraversalBlockManager2D[] managers = FindComponentsInScene<TraversalBlockManager2D>(scene);
        bool appliedAny = false;
        for (int index = 0; index < managers.Length; index++)
        {
            TraversalBlockManager2D manager = managers[index];
            if (manager == null)
            {
                continue;
            }

            manager.BindRuntimeSceneReferences(primarySceneNavGrid, persistentPlayerMovement);
            manager.ApplyConfiguration(rebuildNavGrid: false);
            appliedAny = true;
        }

        if (!appliedAny)
        {
            return;
        }

        NavGrid2D[] sceneNavGrids = FindComponentsInScene<NavGrid2D>(scene);
        for (int index = 0; index < sceneNavGrids.Length; index++)
        {
            NavGrid2D navGrid = sceneNavGrids[index];
            if (navGrid == null)
            {
                continue;
            }

            if (navGrid.LastRebuildFrame >= 0
                && Time.frameCount - navGrid.LastRebuildFrame <= 1)
            {
                continue;
            }

            navGrid.RefreshGrid();
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

    private bool TryApplyLoadedPlayerStateInternal(Vector2 worldPosition, Vector2? facingDirection)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        EnsurePersistentPlayer(activeScene);
        if (persistentPlayerMovement == null || persistentPlayerTransform == null)
        {
            return false;
        }

        BindNavigator(activeScene);
        ApplyPersistentPlayerPosition(
            new Vector3(worldPosition.x, worldPosition.y, persistentPlayerTransform.position.z),
            facingDirection);
        return true;
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

        InventoryService sceneInventory = ResolveRuntimeInventoryService(scene);

        if (sceneInventory != null)
        {
            inventorySnapshot = sceneInventory.Save();
            hasInventorySnapshot = inventorySnapshot != null &&
                                 !string.IsNullOrWhiteSpace(inventorySnapshot.genericData);
        }

        HotbarSelectionService sceneHotbarSelection = ResolveRuntimeHotbarSelectionService(scene);

        if (sceneHotbarSelection != null)
        {
            hotbarSelectionSnapshot = sceneHotbarSelection.selectedIndex;
            hotbarInventorySelectionSnapshot = sceneHotbarSelection.selectedInventoryIndex;
            hasHotbarSelectionSnapshot = true;
        }

        CaptureSceneResidentRuntimeState(scene);
        CaptureSceneWorldRuntimeState(scene);
    }

    private void ResetPersistentRuntimeForFreshStartInternal()
    {
        s_pendingSceneEntry.Clear();
        inventorySnapshot = null;
        hasInventorySnapshot = false;
        hotbarSelectionSnapshot = 0;
        hotbarInventorySelectionSnapshot = 0;
        hasHotbarSelectionSnapshot = false;
        suppressedSceneWorldRestoreSceneName = string.Empty;
        sceneWorldSnapshotsByScene.Clear();
        sceneWorldSnapshotCapturedTotalDaysByScene.Clear();
        nativeResidentSnapshotsByScene.Clear();
        crowdResidentSnapshots.Clear();

        if (sceneWorldRestoreCoroutine != null)
        {
            StopCoroutine(sceneWorldRestoreCoroutine);
            sceneWorldRestoreCoroutine = null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        InventoryService inventoryService = ResolveRuntimeInventoryService(activeScene);
        if (inventoryService != null)
        {
            for (int index = 0; index < inventoryService.Size; index++)
            {
                inventoryService.ClearSlot(index);
            }
        }

        EquipmentService equipmentService = ResolveRuntimeEquipmentService(activeScene);
        if (equipmentService != null)
        {
            for (int index = 0; index < EquipmentService.EquipSlots; index++)
            {
                equipmentService.ClearEquip(index);
            }
        }

        HotbarSelectionService hotbarSelectionService = ResolveRuntimeHotbarSelectionService(activeScene);
        if (hotbarSelectionService != null)
        {
            hotbarSelectionService.RestoreSelectionState(0, 0);
        }
    }

    private void ClearNativeResidentRuntimeSnapshotsInternal(IReadOnlyList<string> stableKeys)
    {
        if (stableKeys == null || stableKeys.Count <= 0 || nativeResidentSnapshotsByScene.Count <= 0)
        {
            return;
        }

        HashSet<string> normalizedStableKeys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
        for (int index = 0; index < stableKeys.Count; index++)
        {
            string normalized = NPCDialogueContentProfile.NormalizeNpcId(stableKeys[index]);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                normalizedStableKeys.Add(normalized.Trim());
            }
        }

        if (normalizedStableKeys.Count <= 0)
        {
            return;
        }

        List<string> sceneKeys = new List<string>(nativeResidentSnapshotsByScene.Keys);
        for (int index = 0; index < sceneKeys.Count; index++)
        {
            string sceneKey = sceneKeys[index];
            if (!nativeResidentSnapshotsByScene.TryGetValue(sceneKey, out List<NpcResidentRuntimeSnapshot> snapshots)
                || snapshots == null)
            {
                nativeResidentSnapshotsByScene.Remove(sceneKey);
                continue;
            }

            snapshots.RemoveAll(snapshot =>
            {
                if (snapshot == null)
                {
                    return true;
                }

                string normalizedSnapshotKey = NPCDialogueContentProfile.NormalizeNpcId(snapshot.stableKey);
                return !string.IsNullOrWhiteSpace(normalizedSnapshotKey)
                    && normalizedStableKeys.Contains(normalizedSnapshotKey.Trim());
            });

            if (snapshots.Count <= 0)
            {
                nativeResidentSnapshotsByScene.Remove(sceneKey);
            }
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
        int restoredHotbarIndex = hasHotbarSelectionSnapshot
            ? hotbarSelectionSnapshot
            : sceneHotbarSelection.selectedIndex;
        int restoredInventoryIndex = hasHotbarSelectionSnapshot
            ? hotbarInventorySelectionSnapshot
            : sceneHotbarSelection.selectedInventoryIndex;
        sceneHotbarSelection.RestoreSelectionState(
            restoredHotbarIndex,
            restoredInventoryIndex);
    }

    private void CaptureSceneResidentRuntimeState(Scene scene)
    {
        CaptureCrowdResidentRuntimeState(scene);
        CaptureNativeResidentRuntimeState(scene);
    }

    private void CaptureCrowdResidentRuntimeState(Scene scene)
    {
        if (!IsCrowdDirectorRuntimeScene(scene))
        {
            return;
        }

        crowdResidentSnapshots = SpringDay1NpcCrowdDirector.CaptureResidentRuntimeSnapshots()
            ?? new List<SpringDay1NpcCrowdDirector.ResidentRuntimeSnapshotEntry>();
    }

    private void CaptureNativeResidentRuntimeState(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        List<NpcResidentRuntimeSnapshot> snapshots = NpcResidentRuntimeContract.CaptureSceneSnapshots(scene);
        if (snapshots == null || snapshots.Count <= 0)
        {
            nativeResidentSnapshotsByScene.Remove(BuildSceneSnapshotKey(scene));
            return;
        }

        HashSet<string> managedIds = GetCrowdDirectorManagedNpcIds();
        snapshots.RemoveAll(snapshot =>
            snapshot == null
            || string.IsNullOrWhiteSpace(snapshot.stableKey)
            || managedIds.Contains(snapshot.stableKey.Trim()));

        if (snapshots.Count <= 0)
        {
            nativeResidentSnapshotsByScene.Remove(BuildSceneSnapshotKey(scene));
            return;
        }

        nativeResidentSnapshotsByScene[BuildSceneSnapshotKey(scene)] = snapshots;
    }

    private void RestoreSceneResidentRuntimeState(Scene scene)
    {
        RestoreCrowdResidentRuntimeState(scene);
        RestoreNativeResidentRuntimeState(scene);
    }

    private void CaptureSceneWorldRuntimeState(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        string sceneKey = BuildSceneSnapshotKey(scene);
        if (string.IsNullOrWhiteSpace(sceneKey))
        {
            return;
        }

        List<SceneWorldRuntimeSnapshotEntry> snapshots = new List<SceneWorldRuntimeSnapshotEntry>();
        HashSet<string> capturedGuids = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<FarmTileManager>(scene), snapshots, capturedGuids);
        CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<CropController>(scene), snapshots, capturedGuids);
        CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<ChestController>(scene), snapshots, capturedGuids);
        CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<WorldItemPickup>(scene), snapshots, capturedGuids);
        CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<TreeController>(scene), snapshots, capturedGuids);
        CaptureSceneWorldRuntimeSnapshots(FindComponentsInScene<StoneController>(scene), snapshots, capturedGuids);

        // Empty snapshots are authoritative too: they mean every tracked world
        // object in that scene was removed, so native scene objects must not revive.
        sceneWorldSnapshotsByScene[sceneKey] = snapshots;
        sceneWorldSnapshotCapturedTotalDaysByScene[sceneKey] = GetCurrentTotalDaysSafe();
    }

    private static void CaptureSceneWorldRuntimeSnapshots<T>(
        T[] components,
        List<SceneWorldRuntimeSnapshotEntry> snapshots,
        HashSet<string> capturedGuids)
        where T : MonoBehaviour, IPersistentObject
    {
        if (components == null || snapshots == null || capturedGuids == null)
        {
            return;
        }

        for (int index = 0; index < components.Length; index++)
        {
            T component = components[index];
            if (component == null)
            {
                continue;
            }

            string guid = component.PersistentId?.Trim();
            if (string.IsNullOrWhiteSpace(guid) || !capturedGuids.Add(guid))
            {
                continue;
            }

            WorldObjectSaveData snapshotData = component.ShouldSave
                ? CloneWorldObjectSaveData(component.Save())
                : CreateInactiveWorldObjectSnapshot(component);
            bool isActive = snapshotData != null && snapshotData.isActive;
            if (snapshotData == null)
            {
                continue;
            }

            snapshots.Add(new SceneWorldRuntimeSnapshotEntry
            {
                Guid = guid,
                ObjectType = component.ObjectType,
                IsActive = isActive,
                SaveData = snapshotData
            });
        }
    }

    private static WorldObjectSaveData CreateInactiveWorldObjectSnapshot(IPersistentObject persistentObject)
    {
        if (!(persistentObject is Component component))
        {
            return null;
        }

        WorldObjectSaveData snapshot = new WorldObjectSaveData
        {
            guid = persistentObject.PersistentId?.Trim(),
            objectType = persistentObject.ObjectType,
            sceneName = component.gameObject.scene.name,
            isActive = false
        };

        GameObject stateRoot = ResolveWorldObjectStateRoot(component);
        if (stateRoot != null)
        {
            snapshot.SetPosition(stateRoot.transform.position);
        }

        return snapshot;
    }

    private void ScheduleSceneWorldRestore(Scene scene)
    {
        if (sceneWorldRestoreCoroutine != null)
        {
            StopCoroutine(sceneWorldRestoreCoroutine);
            sceneWorldRestoreCoroutine = null;
        }

        if (ShouldSuppressSceneWorldRestore(scene))
        {
            return;
        }

        string sceneKey = BuildSceneSnapshotKey(scene);
        if (string.IsNullOrWhiteSpace(sceneKey)
            || !sceneWorldSnapshotsByScene.TryGetValue(sceneKey, out List<SceneWorldRuntimeSnapshotEntry> snapshots)
            || snapshots == null)
        {
            return;
        }

        sceneWorldRestoreCoroutine = StartCoroutine(
            RestoreSceneWorldRuntimeStateDeferred(scene.handle, sceneKey));
    }

    private System.Collections.IEnumerator RestoreSceneWorldRuntimeStateDeferred(int sceneHandle, string sceneKey)
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid()
            || !activeScene.isLoaded
            || activeScene.handle != sceneHandle
            || !string.Equals(BuildSceneSnapshotKey(activeScene), sceneKey, System.StringComparison.OrdinalIgnoreCase))
        {
            sceneWorldRestoreCoroutine = null;
            yield break;
        }

        RestoreSceneWorldRuntimeState(activeScene);
        sceneWorldRestoreCoroutine = null;
    }

    private void RestoreSceneWorldRuntimeState(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        string sceneKey = BuildSceneSnapshotKey(scene);
        if (string.IsNullOrWhiteSpace(sceneKey)
            || !sceneWorldSnapshotsByScene.TryGetValue(sceneKey, out List<SceneWorldRuntimeSnapshotEntry> snapshots)
            || snapshots == null)
        {
            return;
        }

        Dictionary<string, SceneWorldRuntimeBinding> bindings = BuildSceneWorldRuntimeBindings(scene);
        HashSet<string> snapshotGuids = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
        List<SceneWorldRuntimeSnapshotEntry> orderedSnapshots = CloneSceneWorldRuntimeSnapshotEntries(snapshots);
        int currentTotalDays;
        int elapsedDays = ResolveSceneWorldSnapshotElapsedDays(sceneKey, out currentTotalDays);

        for (int index = 0; index < orderedSnapshots.Count; index++)
        {
            SceneWorldRuntimeSnapshotEntry snapshot = orderedSnapshots[index];
            if (snapshot == null || string.IsNullOrWhiteSpace(snapshot.Guid))
            {
                continue;
            }

            string guid = snapshot.Guid.Trim();
            snapshotGuids.Add(guid);

            if (bindings.TryGetValue(guid, out SceneWorldRuntimeBinding binding))
            {
                ApplySceneWorldRuntimeSnapshot(binding, snapshot, elapsedDays, currentTotalDays);
                continue;
            }

            if (!snapshot.IsActive || snapshot.SaveData == null)
            {
                continue;
            }

            TryReconstructSceneWorldObject(snapshot, elapsedDays, currentTotalDays);
        }

        RemoveUnexpectedSceneWorldBindings(bindings, snapshotGuids);
        FinalizeDeferredSceneWorldCatchUp(scene, elapsedDays, currentTotalDays);
    }

    private static Dictionary<string, SceneWorldRuntimeBinding> BuildSceneWorldRuntimeBindings(Scene scene)
    {
        Dictionary<string, SceneWorldRuntimeBinding> bindings =
            new Dictionary<string, SceneWorldRuntimeBinding>(System.StringComparer.OrdinalIgnoreCase);

        CollectSceneWorldRuntimeBindings(FindComponentsInScene<FarmTileManager>(scene), bindings);
        CollectSceneWorldRuntimeBindings(FindComponentsInScene<CropController>(scene), bindings);
        CollectSceneWorldRuntimeBindings(FindComponentsInScene<ChestController>(scene), bindings);
        CollectSceneWorldRuntimeBindings(FindComponentsInScene<WorldItemPickup>(scene), bindings);
        CollectSceneWorldRuntimeBindings(FindComponentsInScene<TreeController>(scene), bindings);
        CollectSceneWorldRuntimeBindings(FindComponentsInScene<StoneController>(scene), bindings);
        return bindings;
    }

    private static void CollectSceneWorldRuntimeBindings<T>(
        T[] components,
        Dictionary<string, SceneWorldRuntimeBinding> bindings)
        where T : MonoBehaviour, IPersistentObject
    {
        if (components == null || bindings == null)
        {
            return;
        }

        for (int index = 0; index < components.Length; index++)
        {
            T component = components[index];
            if (component == null)
            {
                continue;
            }

            string guid = component.PersistentId?.Trim();
            if (string.IsNullOrWhiteSpace(guid) || bindings.ContainsKey(guid))
            {
                continue;
            }

            bindings[guid] = new SceneWorldRuntimeBinding
            {
                Component = component,
                PersistentObject = component,
                StateRoot = ResolveWorldObjectStateRoot(component),
                ObjectType = component.ObjectType
            };
        }
    }

    private static void ApplySceneWorldRuntimeSnapshot(
        SceneWorldRuntimeBinding binding,
        SceneWorldRuntimeSnapshotEntry snapshot,
        int elapsedDays,
        int currentTotalDays)
    {
        if (binding == null || binding.PersistentObject == null)
        {
            return;
        }

        if (!snapshot.IsActive || snapshot.SaveData == null)
        {
            DeactivateSceneWorldBinding(binding);
            return;
        }

        WorldObjectSaveData restoredData = CloneWorldObjectSaveData(snapshot.SaveData);
        if (restoredData == null)
        {
            return;
        }

        ApplySceneWorldObjectPosition(binding.StateRoot, restoredData);
        binding.PersistentObject.Load(restoredData);
        SetSceneWorldBindingActive(binding, true);
        ApplySceneWorldCatchUp(binding.PersistentObject, elapsedDays, currentTotalDays);
    }

    private static void TryReconstructSceneWorldObject(
        SceneWorldRuntimeSnapshotEntry snapshot,
        int elapsedDays,
        int currentTotalDays)
    {
        if (snapshot?.SaveData == null)
        {
            return;
        }

        WorldObjectSaveData restoredData = CloneWorldObjectSaveData(snapshot.SaveData);
        IPersistentObject persistentObject = DynamicObjectFactory.TryReconstruct(restoredData);
        if (persistentObject == null)
        {
            return;
        }

        persistentObject.Load(restoredData);
        if (persistentObject is Component component)
        {
            ApplySceneWorldObjectPosition(ResolveWorldObjectStateRoot(component), restoredData);
            SetSceneWorldBindingActive(
                new SceneWorldRuntimeBinding
                {
                    Component = component as MonoBehaviour,
                    PersistentObject = persistentObject,
                    StateRoot = ResolveWorldObjectStateRoot(component),
                    ObjectType = persistentObject.ObjectType
                },
                true);
        }

        ApplySceneWorldCatchUp(persistentObject, elapsedDays, currentTotalDays);
    }

    private static void RemoveUnexpectedSceneWorldBindings(
        Dictionary<string, SceneWorldRuntimeBinding> bindings,
        HashSet<string> snapshotGuids)
    {
        if (bindings == null || snapshotGuids == null)
        {
            return;
        }

        foreach (KeyValuePair<string, SceneWorldRuntimeBinding> pair in bindings)
        {
            SceneWorldRuntimeBinding binding = pair.Value;
            if (binding == null
                || !IsSceneWorldRemovalAuthoritative(binding.ObjectType)
                || snapshotGuids.Contains(pair.Key))
            {
                continue;
            }

            DeactivateSceneWorldBinding(binding);
        }
    }

    private static bool IsSceneWorldRemovalAuthoritative(string objectType)
    {
        switch (objectType)
        {
            case "Drop":
            case "Tree":
            case "Stone":
            case "Chest":
            case "Crop":
                return true;
            default:
                return false;
        }
    }

    private static void DeactivateSceneWorldBinding(SceneWorldRuntimeBinding binding)
    {
        if (binding?.Component == null)
        {
            return;
        }

        if (binding.Component is WorldItemPickup pickup)
        {
            if (WorldItemPool.Instance != null)
            {
                WorldItemPool.Instance.Despawn(pickup);
            }
            else
            {
                Object.Destroy(pickup.gameObject);
            }

            return;
        }

        SetSceneWorldBindingActive(binding, false);
    }

    private static void SetSceneWorldBindingActive(SceneWorldRuntimeBinding binding, bool isActive)
    {
        if (binding?.StateRoot == null)
        {
            return;
        }

        if (binding.StateRoot.activeSelf == isActive)
        {
            return;
        }

        binding.StateRoot.SetActive(isActive);
    }

    private static void ApplySceneWorldObjectPosition(GameObject stateRoot, WorldObjectSaveData data)
    {
        if (stateRoot == null || data == null)
        {
            return;
        }

        stateRoot.transform.position = data.GetPosition();
    }

    private static GameObject ResolveWorldObjectStateRoot(Component component)
    {
        if (component == null)
        {
            return null;
        }

        Transform parent = component.transform.parent;
        if (parent != null && parent.gameObject.scene == component.gameObject.scene)
        {
            return parent.gameObject;
        }

        return component.gameObject;
    }

    private static WorldObjectSaveData CloneWorldObjectSaveData(WorldObjectSaveData source)
    {
        if (source == null)
        {
            return null;
        }

        return new WorldObjectSaveData
        {
            guid = source.guid,
            objectType = source.objectType,
            prefabId = source.prefabId,
            sceneName = source.sceneName,
            layer = source.layer,
            positionX = source.positionX,
            positionY = source.positionY,
            positionZ = source.positionZ,
            rotationZ = source.rotationZ,
            isActive = source.isActive,
            sortingLayerName = source.sortingLayerName,
            sortingOrder = source.sortingOrder,
            genericData = source.genericData
        };
    }

    private List<SceneWorldSnapshotSaveData> ExportOffSceneWorldSnapshotsForSaveInternal(Scene activeScene)
    {
        string activeSceneKey = BuildSceneSnapshotKey(activeScene);
        List<SceneWorldSnapshotSaveData> serializedSnapshots = new List<SceneWorldSnapshotSaveData>();

        foreach (KeyValuePair<string, List<SceneWorldRuntimeSnapshotEntry>> pair in sceneWorldSnapshotsByScene)
        {
            if (string.IsNullOrWhiteSpace(pair.Key)
                || string.Equals(pair.Key, activeSceneKey, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            List<SceneWorldRuntimeSnapshotEntry> clonedEntries = CloneSceneWorldRuntimeSnapshotEntries(pair.Value);
            bool hasCapturedTotalDays =
                sceneWorldSnapshotCapturedTotalDaysByScene.TryGetValue(pair.Key, out int capturedTotalDays);
            SceneWorldSnapshotSaveData serialized = new SceneWorldSnapshotSaveData
            {
                sceneKey = pair.Key,
                hasCapturedTotalDays = hasCapturedTotalDays,
                capturedTotalDays = hasCapturedTotalDays ? capturedTotalDays : 0,
                worldObjects = new List<WorldObjectSaveData>(clonedEntries.Count)
            };

            for (int index = 0; index < clonedEntries.Count; index++)
            {
                SceneWorldRuntimeSnapshotEntry entry = clonedEntries[index];
                if (entry?.SaveData == null)
                {
                    continue;
                }

                serialized.worldObjects.Add(CloneWorldObjectSaveData(entry.SaveData));
            }

            serializedSnapshots.Add(serialized);
        }

        serializedSnapshots.Sort((left, right) =>
            string.Compare(left?.sceneKey, right?.sceneKey, System.StringComparison.OrdinalIgnoreCase));
        return serializedSnapshots;
    }

    private void ImportOffSceneWorldSnapshotsFromSaveInternal(
        List<SceneWorldSnapshotSaveData> serializedSnapshots,
        Scene activeScene)
    {
        if (sceneWorldRestoreCoroutine != null)
        {
            StopCoroutine(sceneWorldRestoreCoroutine);
            sceneWorldRestoreCoroutine = null;
        }

        string activeSceneKey = BuildSceneSnapshotKey(activeScene);
        sceneWorldSnapshotsByScene.Clear();
        sceneWorldSnapshotCapturedTotalDaysByScene.Clear();

        if (serializedSnapshots == null)
        {
            return;
        }

        for (int index = 0; index < serializedSnapshots.Count; index++)
        {
            SceneWorldSnapshotSaveData serialized = serializedSnapshots[index];
            if (serialized == null
                || string.IsNullOrWhiteSpace(serialized.sceneKey)
                || string.Equals(serialized.sceneKey, activeSceneKey, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            List<SceneWorldRuntimeSnapshotEntry> clonedEntries =
                CreateSceneWorldRuntimeSnapshotsFromSaveData(serialized.worldObjects);

            string sceneKey = serialized.sceneKey.Trim();
            sceneWorldSnapshotsByScene[sceneKey] = clonedEntries;
            if (serialized.hasCapturedTotalDays)
            {
                sceneWorldSnapshotCapturedTotalDaysByScene[sceneKey] = serialized.capturedTotalDays;
            }
        }
    }

    private void RebindSceneOcclusionManagers(Scene scene)
    {
        if (persistentPlayerTransform == null)
        {
            return;
        }

        OcclusionManager[] occlusionManagers = FindComponentsInScene<OcclusionManager>(scene);
        for (int index = 0; index < occlusionManagers.Length; index++)
        {
            OcclusionManager manager = occlusionManagers[index];
            if (manager == null)
            {
                continue;
            }

            manager.RefreshRuntimePlayerBinding(persistentPlayerTransform);
        }
    }

    private int ResolveSceneWorldSnapshotElapsedDays(string sceneKey, out int currentTotalDays)
    {
        currentTotalDays = GetCurrentTotalDaysSafe();
        if (string.IsNullOrWhiteSpace(sceneKey)
            || !sceneWorldSnapshotCapturedTotalDaysByScene.TryGetValue(sceneKey, out int capturedTotalDays))
        {
            return 0;
        }

        return Mathf.Max(0, currentTotalDays - capturedTotalDays);
    }

    private int GetCurrentTotalDaysSafe()
    {
        return TimeManager.Instance != null ? TimeManager.Instance.GetTotalDaysPassed() : 0;
    }

    private static void ApplySceneWorldCatchUp(IPersistentObject persistentObject, int elapsedDays, int currentTotalDays)
    {
        if (elapsedDays <= 0 || persistentObject == null)
        {
            return;
        }

        if (persistentObject is TreeController treeController)
        {
            treeController.ApplyOffSceneElapsedDays(elapsedDays, currentTotalDays);
        }
    }

    private void FinalizeDeferredSceneWorldCatchUp(Scene scene, int elapsedDays, int currentTotalDays)
    {
        if (elapsedDays <= 0 || !scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        CropController[] cropControllers = FindComponentsInScene<CropController>(scene);
        for (int index = 0; index < cropControllers.Length; index++)
        {
            CropController cropController = cropControllers[index];
            if (cropController == null || !cropController.gameObject.activeInHierarchy)
            {
                continue;
            }

            cropController.ApplyOffSceneElapsedDays(elapsedDays, currentTotalDays);
        }

        FarmTileManager[] farmTileManagers = FindComponentsInScene<FarmTileManager>(scene);
        for (int index = 0; index < farmTileManagers.Length; index++)
        {
            FarmTileManager farmTileManager = farmTileManagers[index];
            if (farmTileManager == null || !farmTileManager.gameObject.activeInHierarchy)
            {
                continue;
            }

            farmTileManager.ApplyOffSceneElapsedDays(elapsedDays, currentTotalDays);
        }
    }

    private bool ShouldSuppressSceneWorldRestore(Scene scene)
    {
        if (string.IsNullOrWhiteSpace(suppressedSceneWorldRestoreSceneName))
        {
            return false;
        }

        bool shouldSuppress =
            string.Equals(scene.name, suppressedSceneWorldRestoreSceneName, System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(BuildSceneSnapshotKey(scene), suppressedSceneWorldRestoreSceneName, System.StringComparison.OrdinalIgnoreCase);

        if (shouldSuppress)
        {
            suppressedSceneWorldRestoreSceneName = string.Empty;
        }

        return shouldSuppress;
    }

    private static List<SceneWorldRuntimeSnapshotEntry> CreateSceneWorldRuntimeSnapshotsFromSaveData(
        List<WorldObjectSaveData> worldObjects)
    {
        List<SceneWorldRuntimeSnapshotEntry> snapshots = new List<SceneWorldRuntimeSnapshotEntry>();
        if (worldObjects == null)
        {
            return snapshots;
        }

        for (int index = 0; index < worldObjects.Count; index++)
        {
            WorldObjectSaveData worldObject = CloneWorldObjectSaveData(worldObjects[index]);
            if (worldObject == null || string.IsNullOrWhiteSpace(worldObject.guid))
            {
                continue;
            }

            snapshots.Add(new SceneWorldRuntimeSnapshotEntry
            {
                Guid = worldObject.guid.Trim(),
                ObjectType = worldObject.objectType,
                IsActive = worldObject.isActive,
                SaveData = worldObject
            });
        }

        snapshots.Sort(CompareSceneWorldRuntimeSnapshots);
        return snapshots;
    }

    private static List<SceneWorldRuntimeSnapshotEntry> CloneSceneWorldRuntimeSnapshotEntries(
        List<SceneWorldRuntimeSnapshotEntry> source)
    {
        List<SceneWorldRuntimeSnapshotEntry> cloned = new List<SceneWorldRuntimeSnapshotEntry>();
        if (source == null)
        {
            return cloned;
        }

        for (int index = 0; index < source.Count; index++)
        {
            SceneWorldRuntimeSnapshotEntry entry = source[index];
            if (entry == null)
            {
                continue;
            }

            cloned.Add(new SceneWorldRuntimeSnapshotEntry
            {
                Guid = string.IsNullOrWhiteSpace(entry.Guid) ? string.Empty : entry.Guid.Trim(),
                ObjectType = entry.ObjectType,
                IsActive = entry.IsActive,
                SaveData = CloneWorldObjectSaveData(entry.SaveData)
            });
        }

        cloned.Sort(CompareSceneWorldRuntimeSnapshots);
        return cloned;
    }

    private static int CompareSceneWorldRuntimeSnapshots(
        SceneWorldRuntimeSnapshotEntry left,
        SceneWorldRuntimeSnapshotEntry right)
    {
        int priorityComparison = GetSceneWorldRestorePriority(left?.ObjectType)
            .CompareTo(GetSceneWorldRestorePriority(right?.ObjectType));
        if (priorityComparison != 0)
        {
            return priorityComparison;
        }

        return string.Compare(left?.Guid, right?.Guid, System.StringComparison.OrdinalIgnoreCase);
    }

    private static int GetSceneWorldRestorePriority(string objectType)
    {
        switch (objectType)
        {
            case "FarmTileManager":
                return 0;
            case "Crop":
                return 1;
            case "Chest":
                return 2;
            case "Drop":
                return 10;
            case "Tree":
                return 20;
            case "Stone":
                return 30;
            default:
                return 100;
        }
    }

    private void RestoreCrowdResidentRuntimeState(Scene scene)
    {
        if (!IsCrowdDirectorRuntimeScene(scene) || crowdResidentSnapshots == null || crowdResidentSnapshots.Count <= 0)
        {
            return;
        }

        SpringDay1NpcCrowdDirector.ApplyResidentRuntimeSnapshots(crowdResidentSnapshots);
    }

    private void RestoreNativeResidentRuntimeState(Scene scene)
    {
        if (!scene.IsValid())
        {
            return;
        }

        if (!nativeResidentSnapshotsByScene.TryGetValue(BuildSceneSnapshotKey(scene), out List<NpcResidentRuntimeSnapshot> snapshots)
            || snapshots == null
            || snapshots.Count <= 0)
        {
            return;
        }

        for (int index = 0; index < snapshots.Count; index++)
        {
            NpcResidentRuntimeSnapshot snapshot = snapshots[index];
            if (snapshot == null)
            {
                continue;
            }

            NpcResidentRuntimeContract.TryApplySnapshot(scene, snapshot, resumeResidentLogic: true);
        }
    }

    private HashSet<string> GetCrowdDirectorManagedNpcIds()
    {
        if (crowdDirectorManagedNpcIds != null)
        {
            return crowdDirectorManagedNpcIds;
        }

        crowdDirectorManagedNpcIds = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
        SpringDay1NpcCrowdManifest manifest =
            Resources.Load<SpringDay1NpcCrowdManifest>(SpringDay1NpcCrowdManifestResourcePath);
        if (manifest == null || manifest.Entries == null)
        {
            return crowdDirectorManagedNpcIds;
        }

        SpringDay1NpcCrowdManifest.Entry[] entries = manifest.Entries;
        for (int index = 0; index < entries.Length; index++)
        {
            SpringDay1NpcCrowdManifest.Entry entry = entries[index];
            if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
            {
                continue;
            }

            crowdDirectorManagedNpcIds.Add(entry.npcId.Trim());
        }

        return crowdDirectorManagedNpcIds;
    }

    private static string BuildSceneSnapshotKey(Scene scene)
    {
        if (!scene.IsValid())
        {
            return string.Empty;
        }

        return !string.IsNullOrWhiteSpace(scene.path)
            ? scene.path.Trim()
            : scene.name.Trim();
    }

    private static bool IsCrowdDirectorRuntimeScene(Scene scene)
    {
        return scene.IsValid()
            && (string.Equals(scene.name, "Town", System.StringComparison.Ordinal)
                || string.Equals(scene.name, "Primary", System.StringComparison.Ordinal));
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
        RefreshBoundaryFocusUiTargets();
        if (persistentUiCanvasGroup == null)
        {
            return;
        }

        persistentUiCanvasGroup.alpha = 1f;
        persistentUiCanvasGroup.blocksRaycasts = true;
        persistentUiCanvasGroup.interactable = true;

        if (boundaryFocusUiTargets.Count == 0)
        {
            RefreshBoundaryFocusUiTargets();
        }

        Scene activeScene = SceneManager.GetActiveScene();
        bool keepFullyVisible = IsFixedFallbackCameraScene(activeScene) || ShouldKeepCoreUiFullyVisible();
        Vector3 playerViewport = Vector3.zero;
        if (!keepFullyVisible)
        {
            Camera runtimeCamera = ResolveRuntimeCamera(activeScene);
            if (runtimeCamera == null || persistentPlayerTransform == null)
            {
                keepFullyVisible = true;
            }
            else
            {
                playerViewport = runtimeCamera.WorldToViewportPoint(persistentPlayerTransform.position);
                if (playerViewport.z <= 0f)
                {
                    keepFullyVisible = true;
                }
            }
        }

        for (int index = 0; index < boundaryFocusUiTargets.Count; index++)
        {
            BoundaryFocusUiTarget target = boundaryFocusUiTargets[index];
            if (target == null || target.RectTransform == null || target.CanvasGroup == null)
            {
                continue;
            }

            float targetAlpha = keepFullyVisible
                ? 1f
                : ResolveBoundaryFocusTargetAlpha(playerViewport, target.RectTransform);
            if (target.PromptOverlay != null)
            {
                float nextAlpha = Mathf.MoveTowards(
                    target.PromptOverlay.CurrentBoundaryFocusAlpha,
                    targetAlpha,
                    Time.unscaledDeltaTime * BoundaryFocusBlendSpeed);
                target.PromptOverlay.SetBoundaryFocusAlpha(nextAlpha);
                continue;
            }

            target.CanvasGroup.alpha = Mathf.MoveTowards(
                target.CanvasGroup.alpha,
                targetAlpha,
                Time.unscaledDeltaTime * BoundaryFocusBlendSpeed);

            bool shouldBlockRaycasts = keepFullyVisible || target.CanvasGroup.alpha > 0.95f;
            target.CanvasGroup.blocksRaycasts = shouldBlockRaycasts;
            target.CanvasGroup.interactable = shouldBlockRaycasts;
        }
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
        persistentHealthSystemRoot = CaptureOrPersistSceneRoot(scene, persistentHealthSystemRoot, HealthSystemRootName);
        persistentSprintStateManagerRoot = CaptureOrPersistSceneRoot(scene, persistentSprintStateManagerRoot, SprintStateManagerRootName);

        RefreshPersistentRuntimeServices();
    }

    private void RefreshPersistentRuntimeServices()
    {
        persistentSceneInputManager = ResolvePersistentComponent(
            persistentSceneInputManager,
            persistentSystemsRoot);
        persistentInventoryService = ResolvePersistentComponent(
            persistentInventoryService,
            persistentInventoryRoot,
            persistentSystemsRoot);
        persistentHotbarSelectionService = ResolvePersistentComponent(
            persistentHotbarSelectionService,
            persistentHotbarRoot,
            persistentSystemsRoot);
        persistentEquipmentService = ResolvePersistentComponent(
            persistentEquipmentService,
            persistentEquipmentRoot,
            persistentSystemsRoot);
        persistentHealthSystem = ResolvePersistentComponent(
            persistentHealthSystem,
            persistentHealthSystemRoot);
        persistentSprintStateManager = ResolvePersistentComponent(
            persistentSprintStateManager,
            persistentSprintStateManagerRoot);
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

        if (persistentHealthSystemRoot != null)
        {
            persistentHealthSystemRoot.SetActive(true);
        }

        if (persistentSprintStateManagerRoot != null)
        {
            persistentSprintStateManagerRoot.SetActive(true);
        }

        EnsurePersistentUiCanvasGroup();

        EquipmentService sceneEquipment = ResolveRuntimeEquipmentService(scene);
        ItemDatabase database = sceneInventory != null
            ? sceneInventory.Database
            : persistentInventoryService != null ? persistentInventoryService.Database : null;
        InventorySortService sortService = persistentUiRoot.GetComponentInChildren<InventorySortService>(true);
        if (sortService == null)
        {
            sortService = FindFirstObjectByType<InventorySortService>(FindObjectsInactive.Include);
        }

        if (sortService != null)
        {
            sortService.RebindRuntimeContext(sceneInventory, database);
        }

        CraftingService[] craftingServices = FindObjectsByType<CraftingService>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < craftingServices.Length; index++)
        {
            CraftingService craftingService = craftingServices[index];
            if (craftingService == null)
            {
                continue;
            }

            craftingService.ConfigureRuntimeContext(sceneInventory, database);
        }

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

            panel.ConfigureRuntimeContext(
                sceneInventory,
                sceneEquipment,
                database,
                sceneHotbarSelection);
            panel.EnsureBuilt();
            panel.RefreshAll();
        }

        InventoryInteractionManager interactionManager =
            persistentUiRoot.GetComponentInChildren<InventoryInteractionManager>(true);
        if (interactionManager != null)
        {
            interactionManager.ConfigureRuntimeContext(
                sceneInventory,
                sceneEquipment,
                database,
                sortService);
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

            packagePanel.ConfigureRuntimeContext(
                sceneInventory,
                sceneEquipment,
                database,
                sceneHotbarSelection);
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

        ForceRuntimeUiRefreshAfterSceneRebind(
            sceneInventory,
            sceneEquipment,
            database,
            sceneHotbarSelection);
    }

    private void ForceRuntimeUiRefreshAfterSceneRebind(
        InventoryService sceneInventory,
        EquipmentService sceneEquipment,
        ItemDatabase database,
        HotbarSelectionService sceneHotbarSelection)
    {
        if (persistentUiRoot == null)
        {
            return;
        }

        BoxPanelUI activeBoxPanel = BoxPanelUI.ActiveInstance;
        if (activeBoxPanel != null && activeBoxPanel.IsOpen)
        {
            activeBoxPanel.ConfigureRuntimeContext(
                sceneInventory,
                sceneEquipment,
                database,
                sceneHotbarSelection);
            activeBoxPanel.RefreshUI();
        }

        if (sceneInventory != null)
        {
            sceneInventory.RefreshAll();
        }

        if (sceneHotbarSelection != null)
        {
            sceneHotbarSelection.ReassertCurrentSelection(
                collapseInventorySelectionToHotbar: false,
                invokeEvent: true);
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

    private void RefreshBoundaryFocusUiTargets()
    {
        boundaryFocusUiTargets.Clear();
        if (persistentUiRoot == null)
        {
            return;
        }

        RectTransform uiRootRect = persistentUiRoot.GetComponent<RectTransform>();
        if (uiRootRect == null)
        {
            return;
        }

        for (int index = 0; index < uiRootRect.childCount; index++)
        {
            RectTransform childRect = uiRootRect.GetChild(index) as RectTransform;
            if (childRect == null || !ShouldTrackBoundaryFocusTarget(childRect.gameObject))
            {
                continue;
            }

            CanvasGroup canvasGroup = childRect.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = childRect.gameObject.AddComponent<CanvasGroup>();
            }

            boundaryFocusUiTargets.Add(new BoundaryFocusUiTarget
            {
                RectTransform = childRect,
                CanvasGroup = canvasGroup,
                PromptOverlay = childRect.GetComponent<SpringDay1PromptOverlay>()
            });
        }
    }

    private static bool ShouldTrackBoundaryFocusTarget(GameObject target)
    {
        return ResolveNamedBoundaryFocusEdges(target != null ? target.name : null) != BoundaryFocusEdgeMask.None;
    }

    private float ResolveBoundaryFocusTargetAlpha(Vector3 viewport, RectTransform targetRect)
    {
        if (targetRect == null)
        {
            return 1f;
        }

        BoundaryFocusEdgeMask edges = ResolveNamedBoundaryFocusEdges(targetRect.gameObject.name);
        if (edges == BoundaryFocusEdgeMask.None)
        {
            return 1f;
        }

        float edgePressure = 0f;
        if ((edges & BoundaryFocusEdgeMask.Left) != 0)
        {
            edgePressure = Mathf.Max(edgePressure, ResolveLowerEdgePressure(viewport.x));
        }
        if ((edges & BoundaryFocusEdgeMask.Right) != 0)
        {
            edgePressure = Mathf.Max(edgePressure, ResolveUpperEdgePressure(viewport.x));
        }
        if ((edges & BoundaryFocusEdgeMask.Top) != 0)
        {
            edgePressure = Mathf.Max(edgePressure, ResolveUpperEdgePressure(viewport.y));
        }
        if ((edges & BoundaryFocusEdgeMask.Bottom) != 0)
        {
            edgePressure = Mathf.Max(edgePressure, ResolveLowerEdgePressure(viewport.y));
        }

        edgePressure = Mathf.Clamp01(edgePressure);
        float easedPressure = Mathf.SmoothStep(0f, 1f, edgePressure);
        return Mathf.Lerp(1f, BoundaryFocusMinAlpha, easedPressure);
    }

    private static BoundaryFocusEdgeMask ResolveNamedBoundaryFocusEdges(string targetName)
    {
        switch (targetName)
        {
            case "State":
                return BoundaryFocusEdgeMask.Left | BoundaryFocusEdgeMask.Top;
            case "ToolBar":
                return BoundaryFocusEdgeMask.Bottom;
            case "SpringDay1PromptOverlay":
                return BoundaryFocusEdgeMask.Left | BoundaryFocusEdgeMask.Top;
            case "SpringDay1WorldHintBubble":
                return BoundaryFocusEdgeMask.Right;
            default:
                return BoundaryFocusEdgeMask.None;
        }
    }

    private static float ResolveLowerEdgePressure(float viewportValue)
    {
        return 1f - Mathf.InverseLerp(
            BoundaryFocusFullViewportThreshold,
            BoundaryFocusStartViewportThreshold,
            viewportValue);
    }

    private static float ResolveUpperEdgePressure(float viewportValue)
    {
        return Mathf.InverseLerp(
            1f - BoundaryFocusStartViewportThreshold,
            1f - BoundaryFocusFullViewportThreshold,
            viewportValue);
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

        SpringDay1WorkbenchCraftingOverlay workbenchOverlay =
            FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
        if (workbenchOverlay != null && workbenchOverlay.IsVisible)
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

            if (DoesEntryAnchorNameMatch(candidate.name, s_pendingSceneEntry.EntryAnchorName))
            {
                return candidate;
            }
        }

        return null;
    }

    private static bool DoesEntryAnchorNameMatch(string candidateName, string rawEntryAnchorNames)
    {
        if (string.IsNullOrWhiteSpace(candidateName) || string.IsNullOrWhiteSpace(rawEntryAnchorNames))
        {
            return false;
        }

        string[] aliasNames = rawEntryAnchorNames.Split('|');
        for (int index = 0; index < aliasNames.Length; index++)
        {
            string aliasName = aliasNames[index]?.Trim();
            if (string.IsNullOrWhiteSpace(aliasName))
            {
                continue;
            }

            if (string.Equals(candidateName, aliasName, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
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
        RefreshPersistentRuntimeServices();
        if (persistentInventoryService != null)
        {
            return persistentInventoryService;
        }

        InventoryService sceneInventory = FindFirstComponentInScene<InventoryService>(scene);
        if (sceneInventory != null)
        {
            return sceneInventory;
        }

        return persistentInventoryService;
    }

    private HotbarSelectionService ResolveRuntimeHotbarSelectionService(Scene scene)
    {
        RefreshPersistentRuntimeServices();
        if (persistentHotbarSelectionService != null)
        {
            return persistentHotbarSelectionService;
        }

        HotbarSelectionService sceneHotbar = FindFirstComponentInScene<HotbarSelectionService>(scene);
        if (sceneHotbar != null)
        {
            return sceneHotbar;
        }

        return persistentHotbarSelectionService;
    }

    private EquipmentService ResolveRuntimeEquipmentService(Scene scene)
    {
        RefreshPersistentRuntimeServices();
        if (persistentEquipmentService != null)
        {
            return persistentEquipmentService;
        }

        EquipmentService sceneEquipment = FindFirstComponentInScene<EquipmentService>(scene);
        if (sceneEquipment != null)
        {
            return sceneEquipment;
        }

        return persistentEquipmentService;
    }

    private static T ResolvePersistentComponent<T>(T current, params GameObject[] roots)
        where T : Component
    {
        if (current != null)
        {
            return current;
        }

        for (int index = 0; index < roots.Length; index++)
        {
            GameObject root = roots[index];
            if (root == null)
            {
                continue;
            }

            T direct = root.GetComponent<T>();
            if (direct != null)
            {
                return direct;
            }

            T nested = root.GetComponentInChildren<T>(true);
            if (nested != null)
            {
                return nested;
            }
        }

        return FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }

    private GameInputManager ResolveRuntimeInputManager(Scene scene)
    {
        RefreshPersistentRuntimeServices();
        if (persistentSceneInputManager != null)
        {
            return persistentSceneInputManager;
        }

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

    private bool WasSceneAlreadyRebound(Scene scene)
    {
        return scene.IsValid() && scene.handle == lastReboundSceneHandle;
    }

    private void ScheduleStartupFallbackRebind()
    {
        if (startupFallbackRebindCoroutine != null || !isActiveAndEnabled)
        {
            return;
        }

        startupFallbackRebindCoroutine = StartCoroutine(StartupFallbackRebindRoutine());
    }

    private System.Collections.IEnumerator StartupFallbackRebindRoutine()
    {
        // sceneLoaded 通常会先完成重绑；这里只保留一帧后的兜底，避免首屏同帧重复重活。
        yield return null;
        Scene activeScene = SceneManager.GetActiveScene();
        if (!WasSceneAlreadyRebound(activeScene))
        {
            RebindScene(activeScene);
        }

        // 首屏 Town 仍可能在这一帧后半段才把 persistent player / UI / 遮挡对象全部摆齐，
        // 再补一轮轻量 runtime refresh，避免“第一次进游戏不遮挡，切一次场才恢复”。
        yield return new WaitForEndOfFrame();
        activeScene = SceneManager.GetActiveScene();
        if (activeScene.IsValid() && activeScene.isLoaded)
        {
            RefreshSceneRuntimeBindingsInternal(activeScene);
        }

        startupFallbackRebindCoroutine = null;
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
