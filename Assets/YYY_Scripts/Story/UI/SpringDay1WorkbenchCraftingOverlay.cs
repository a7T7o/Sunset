using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.UI;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1WorkbenchCraftingOverlay : MonoBehaviour
    {
        private const string PrefabAssetPath = "Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab";

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private const string FontCoverageProbeText = "工作台配方材料进度制作";

        private static readonly FieldInfo BlockNavOverUiField =
            typeof(GameInputManager).GetField("blockNavOverUI", BindingFlags.Instance | BindingFlags.NonPublic);

        private const string RecipeResourceFolder = "Story/SpringDay1Workbench";
        private const int MaxFloatingProgressCards = 6;
        private const int OverlaySortingOrder = 168;
        private const float CraftExitAutoHideDistance = 1.6f;
        private const float QueueOverflowToastFadeDuration = 0.25f;
        private const float QueueOverflowToastHoldDuration = 0.5f;

        private static SpringDay1WorkbenchCraftingOverlay _instance;
        private static Sprite _runtimeProgressFillSprite;
        private static readonly Color ActiveProgressFillColor = new(0.43f, 0.73f, 0.56f, 0.96f);
        private static readonly Color ActiveProgressIdleColor = new(0.43f, 0.73f, 0.56f, 0.42f);
        private static readonly Color ActiveProgressEmptyColor = new(0.43f, 0.73f, 0.56f, 0.18f);
        private static readonly Color CompletedProgressFillColor = new(0.71f, 0.52f, 0.12f, 0.98f);
        private static readonly Color CancelProgressFillColor = new(0.81f, 0.29f, 0.24f, 0.96f);
        private static readonly Color ProgressLabelReadableColor = new(1f, 1f, 1f, 0.98f);

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform pointerRect;
        [SerializeField] private RectTransform queueToastRect;
        [SerializeField] private CanvasGroup queueToastGroup;
        [SerializeField] private TextMeshProUGUI queueToastText;
        [SerializeField] private RectTransform recipeViewportRect;
        [SerializeField] private RectTransform recipeContentRect;
        [SerializeField] private Scrollbar recipeScrollbar;
        [SerializeField] private RectTransform materialsViewportRect;
        [SerializeField] private RectTransform materialsContentRect;
        [SerializeField] private Image selectedIcon;
        [SerializeField] private TextMeshProUGUI selectedNameText;
        [SerializeField] private TextMeshProUGUI selectedDescriptionText;
        [SerializeField] private TextMeshProUGUI selectedMaterialsText;
        [SerializeField] private TextMeshProUGUI stageHintText;
        [SerializeField] private TextMeshProUGUI progressLabelText;
        [SerializeField] private RectTransform progressRoot;
        [SerializeField] private Button progressButton;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private Image progressBackgroundImage;
        [SerializeField] private Slider quantitySlider;
        [SerializeField] private TextMeshProUGUI quantityValueText;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button craftButton;
        [SerializeField] private Image craftButtonBackground;
        [SerializeField] private TextMeshProUGUI craftButtonLabel;
        [SerializeField] private RectTransform floatingProgressRoot;
        [SerializeField] private Image floatingProgressIcon;
        [SerializeField] private Image floatingProgressFillImage;
        [SerializeField] private TextMeshProUGUI floatingProgressLabel;

        [Header("Layout")]
        [SerializeField] private float panelWidth = 428f;
        [SerializeField] private float panelHeight = 236f;
        [SerializeField] private float leftWidth = 172f;
        [SerializeField] private float rowHeight = 48f;
        [SerializeField] private float rowSpacing = 6f;
        [SerializeField] private Vector2 aboveOffset = new(0f, 38f);
        [SerializeField] private Vector2 belowOffset = new(0f, -30f);
        [SerializeField] private Vector2 screenMargin = new(18f, 18f);
        [SerializeField] private float defaultCraftDuration = 1.15f;
        [SerializeField] private float panelFollowSharpness = 14f;
        [SerializeField] private float displayDirectionHysteresis = 0.16f;

        private readonly List<RecipeData> _recipes = new();
        private readonly List<RowRefs> _rows = new();
        private readonly List<FloatingProgressCardRefs> _floatingCards = new();
        private readonly List<FloatingProgressDisplayState> _floatingStateBuffer = new();
        private readonly List<FloatingProgressDisplayState> _floatingStatePool = new();
        private static readonly string[] WorkbenchAnimatorBoolNames = { "IsWorking", "Working", "IsCrafting", "Crafting" };

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private Transform _playerTransform;
        private CraftingService _craftingService;
        private InventoryService _inventoryService;
        private ItemDatabase _fallbackItemDatabase;
        private GameInputManager _gameInputManager;
        private ScrollRect _recipeScrollRect;
        private ScrollRect _materialsScrollRect;
        private int _selectedIndex = -1;
        private int _selectedQuantity;
        private float _autoHideDistance = 1.5f;
        private bool _displayBelow;
        private bool _isVisible;
        private bool _hasDisplayDirectionDecision;
        private bool _animateDisplayDirectionTransition;
        private bool _navBlockWasEnabled;
        private bool _navBlockOverrideApplied;
        private Coroutine _craftRoutine;
        private Coroutine _queueToastRoutine;
        private float _craftProgress;
        private int _craftQueueTotal;
        private int _craftQueueCompleted;
        private int _lastCompletedQueueTotal;
        private int _lastCompletedRecipeId = -1;
        private float _panelVerticalVelocity;
        private RecipeData _craftingRecipe;
        private bool _craftButtonHovered;
        private bool _progressBarHovered;
        private Animator _workbenchAnimator;
        private string _workbenchAnimatorBoolName;
        private readonly List<WorkbenchQueueEntry> _queueEntries = new();
        private WorkbenchQueueEntry _activeQueueEntry;
        private bool _hasReservedActiveCraft;
        private string _boundScenePath;
        private string _boundSceneName;
        private string _boundWorkbenchStationKey;
        private RectTransform _detailColumnRect;
        private RectTransform _materialsTitleRect;
        private RectTransform _quantityTitleRect;
        private RectTransform _quantityControlsRect;
        private RectTransform _iconCardRect;
        private RectBaseline _nameBaseline;
        private RectBaseline _descriptionBaseline;
        private RectBaseline _materialsTitleBaseline;
        private RectBaseline _materialsViewportBaseline;
        private RectBaseline _quantityTitleBaseline;
        private RectBaseline _quantityControlsBaseline;
        private RectBaseline _stageHintBaseline;
        private RectBaseline _progressLabelBaseline;
        private RectBaseline _progressBaseline;
        private RectBaseline _craftButtonBaseline;
        private RectBaseline _iconCardBaseline;
        private float _materialsTextBaselineHeight;
        private static readonly Color RecipeViewportVisibleColor = new(0f, 0f, 0f, 1f);

        public static SpringDay1WorkbenchCraftingOverlay Instance
        {
            get
            {
                if (_instance == null)
                {
                    EnsureRuntime();
                }

                return _instance;
            }
        }

        public bool IsVisible => _isVisible;
        public bool HasActiveCraftQueue => IsStateVisibleInActiveScene() && HasRawActiveCraftQueue;
        public bool HasReadyWorkbenchOutputs => IsStateVisibleInActiveScene() && HasRawReadyWorkbenchOutputs;
        public bool HasWorkbenchFloatingState => IsStateVisibleInActiveScene() && HasRawWorkbenchState;

        private bool HasRawActiveCraftQueue => _craftRoutine != null && _activeQueueEntry != null && _activeQueueEntry.recipe != null && HasPendingCrafts(_activeQueueEntry);
        private bool HasRawReadyWorkbenchOutputs => HasReadyOutputs();
        private bool HasRawWorkbenchState => HasTrackedQueueEntries || HasRawActiveCraftQueue || HasRawReadyWorkbenchOutputs;

        public string GetRuntimeRecipeShellSummary()
        {
            int visibleRowCount = 0;
            for (int index = 0; index < _rows.Count; index++)
            {
                if (_rows[index]?.rect != null && _rows[index].rect.gameObject.activeSelf)
                {
                    visibleRowCount++;
                }
            }

            string selectedRecipe = "none";
            if (_selectedIndex >= 0 && _selectedIndex < _recipes.Count)
            {
                RecipeData recipe = _recipes[_selectedIndex];
                ItemData item = recipe != null ? ResolveItem(recipe.resultItemID) : null;
                selectedRecipe = recipe != null ? GetRecipeDisplayName(recipe, item) : "none";
            }

            return
                $"visible={_isVisible}" +
                $"|rows={_rows.Count}" +
                $"|visibleRows={visibleRowCount}" +
                $"|generated={HasGeneratedRecipeRowChain()}" +
                $"|prefabRecipeShell={UsesPrefabRecipeShell()}" +
                $"|prefabDetailShell={UsesPrefabDetailShell()}" +
                $"|unreadableRows={HasUnreadableVisibleRecipeRows()}" +
                $"|hardRecovery={NeedsRecipeRowHardRecovery()}" +
                $"|selected={selectedRecipe}";
        }

        public static void FlushRuntimeStateToPersistence()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.FlushCurrentRuntimeStateToPersistence();
        }

        public static void NotifyPersistentStateReplaced()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.ReloadRuntimeStateFromPersistence();
        }

        public static void EnsureRuntime()
        {
            if (_instance != null && !CanReuseRuntimeInstance(_instance, requireScreenOverlay: true))
            {
                RetireRuntimeInstance(_instance);
                _instance = null;
            }

            RetireIncompatibleRuntimeInstances();

            if (TryUseExistingRuntimeInstance(requireScreenOverlay: true))
            {
                RetireOtherRuntimeInstances(_instance);
                return;
            }

            SpringDay1WorkbenchCraftingOverlay prefabInstance = InstantiateRuntimePrefab();
            if (prefabInstance != null)
            {
                _instance = prefabInstance;
                _instance.EnsureBuilt();
                RetireOtherRuntimeInstances(_instance);
                _instance.HideImmediate();
                return;
            }

            Transform parent = ResolveParent();
            GameObject root = new GameObject(nameof(SpringDay1WorkbenchCraftingOverlay), typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup));
            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            _instance = root.AddComponent<SpringDay1WorkbenchCraftingOverlay>();
            _instance.BuildUi();
            RetireOtherRuntimeInstances(_instance);
            _instance.HideImmediate();
        }

        private static bool TryUseExistingRuntimeInstance(bool requireScreenOverlay)
        {
            SpringDay1WorkbenchCraftingOverlay existing = FindReusableRuntimeInstance(requireScreenOverlay);
            if (existing == null)
            {
                return false;
            }

            _instance = existing;
            _instance.EnsureBuilt();
            _instance.HideImmediate();
            return true;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueStartEvent>(OnDialogueStart, owner: this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SuspendCraftRoutineForPersistence();
            FlushCurrentRuntimeStateToPersistence();
            DetachScenePresentation();
            CleanupTransientState(resetSession: false);
        }

        private void LateUpdate()
        {
            if (_anchorTarget == null)
            {
                return;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen() || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive))
            {
                Hide();
                return;
            }

            if (_isVisible &&
                _playerTransform != null &&
                GetOverlayAutoHideDistance(_playerTransform) > GetCurrentAutoHideDistance())
            {
                Hide();
            }

            if (_isVisible)
            {
                if (_playerTransform != null)
                {
                    ApplyDisplayDirection(ShouldDisplayBelow(GetDisplayDecisionSamplePoint(_playerTransform)));
                }

                Reposition();
            }

            UpdateFloatingProgressVisibility();
        }

        public bool Toggle(Transform anchorTarget, Transform playerTransform, CraftingService craftingService, CraftingStation station, float autoHideDistance)
        {
            if (station != CraftingStation.Workbench)
            {
                Hide();
                return false;
            }

            if (_isVisible && _anchorTarget == anchorTarget)
            {
                Hide();
                return false;
            }

            Open(anchorTarget, playerTransform, craftingService, autoHideDistance);
            return true;
        }

        public void Open(Transform anchorTarget, Transform playerTransform, CraftingService craftingService, float autoHideDistance)
        {
            EnsureBuilt();
            FlushCurrentRuntimeStateToPersistence();
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                return;
            }

            if (craftingService == null || !EnsureRecipesLoaded())
            {
                Hide();
                return;
            }

            _anchorTarget = anchorTarget;
            _boundWorkbenchStationKey = BuildWorkbenchStationKey(_anchorTarget);
            LoadRuntimeStateFromPersistence(clearIfMissing: true);
            _playerTransform = playerTransform != null ? playerTransform : ResolvePlayerTransform();
            _craftingService = craftingService;
            _craftingService.RefreshRuntimeContextFromScene();
            _craftingService.SetStation(CraftingStation.Workbench);
            _autoHideDistance = Mathf.Max(0.6f, autoHideDistance);
            BindStateToScene(_anchorTarget != null ? _anchorTarget.gameObject.scene : SceneManager.GetActiveScene());
            ApplyDisplayDirection(_playerTransform != null && ShouldDisplayBelow(GetDisplayDecisionSamplePoint(_playerTransform)));
            BindInventory(_craftingService.Inventory);
            ApplyNavigationBlock(true);

            bool selectedIndexValid = _selectedIndex >= 0 && _selectedIndex < _recipes.Count;
            if (!selectedIndexValid)
            {
                if (HasActiveCraftQueue && _activeQueueEntry?.recipe != null)
                {
                    int craftingRecipeIndex = FindRecipeIndex(_activeQueueEntry.recipe);
                    if (craftingRecipeIndex >= 0)
                    {
                        _selectedIndex = craftingRecipeIndex;
                        selectedIndexValid = true;
                    }
                }

                if (!selectedIndexValid)
                {
                    WorkbenchQueueEntry firstReadyEntry = _queueEntries.FirstOrDefault(entry => entry != null && entry.readyCount > 0);
                    if (firstReadyEntry != null)
                    {
                        int completedRecipeIndex = FindRecipeIndex(firstReadyEntry.recipe);
                        if (completedRecipeIndex >= 0)
                        {
                            _selectedIndex = completedRecipeIndex;
                            selectedIndexValid = true;
                        }
                    }
                }
            }

            if (!selectedIndexValid)
            {
                _selectedIndex = 0;
            }

            _selectedQuantity = 0;
            _isVisible = true;
            if (floatingProgressRoot != null)
            {
                floatingProgressRoot.gameObject.SetActive(false);
            }

            RefreshAll(validateRecipeRows: true, rebuildRecipeGeometry: true, rebuildDetailGeometry: true);
            UpdateFloatingProgressVisibility();
            SetWorkbenchPanelVisible(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            Reposition(true);
        }

        public void Hide()
        {
            _isVisible = false;
            ApplyNavigationBlock(false);
            HideImmediate();
            UpdateFloatingProgressVisibility();
            if (_craftRoutine == null)
            {
                CleanupTransientState(resetSession: !HasRawWorkbenchState);
            }
        }

        private void CleanupTransientState(bool resetSession)
        {
            ApplyNavigationBlock(false);
            if (_craftRoutine == null)
            {
                UnbindInventory();
            }

            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = 0f;
            }

            if (floatingProgressFillImage != null)
            {
                floatingProgressFillImage.fillAmount = 0f;
            }

            StopQueueOverflowToastRoutine();
            HideQueueOverflowToastImmediate();

            if (progressLabelText != null)
            {
                progressLabelText.text = string.Empty;
            }

            if (floatingProgressLabel != null)
            {
                floatingProgressLabel.text = string.Empty;
            }

            if (resetSession)
            {
                _anchorTarget = null;
                _playerTransform = null;
                _craftingService = null;
                _craftingRecipe = null;
                _activeQueueEntry = null;
                _hasReservedActiveCraft = false;
                _craftQueueTotal = 0;
                _craftQueueCompleted = 0;
                _lastCompletedQueueTotal = 0;
                _lastCompletedRecipeId = -1;
                _queueEntries.Clear();
                _boundWorkbenchStationKey = string.Empty;
                _hasDisplayDirectionDecision = false;
                ClearSceneBinding();
            }

            UpdateFloatingProgressVisibility();
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            Hide();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (!IsStateBoundToScene(scene))
            {
                DetachScenePresentation();
                return;
            }

            RefreshBoundSceneRuntimeState();
        }

        private void OnActiveSceneChanged(Scene _, Scene nextScene)
        {
            if (!IsStateBoundToScene(nextScene))
            {
                DetachScenePresentation();
                return;
            }

            RefreshBoundSceneRuntimeState();
        }

        private void BindStateToScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                ClearSceneBinding();
                return;
            }

            _boundScenePath = scene.path ?? string.Empty;
            _boundSceneName = scene.name ?? string.Empty;
        }

        private void ClearSceneBinding()
        {
            _boundScenePath = string.Empty;
            _boundSceneName = string.Empty;
        }

        private void FlushCurrentRuntimeStateToPersistence()
        {
            string stationKey = ResolveBoundWorkbenchStationKey();
            if (string.IsNullOrWhiteSpace(stationKey))
            {
                return;
            }

            if (!HasTrackedQueueEntries)
            {
                StoryProgressPersistenceService.RemoveWorkbenchRuntimeState(stationKey);
                return;
            }

            StoryProgressPersistenceService.StoreWorkbenchRuntimeState(BuildWorkbenchRuntimeSaveData(stationKey));
        }

        private void ReloadRuntimeStateFromPersistence()
        {
            SuspendCraftRoutineForPersistence();
            string stationKey = ResolveBoundWorkbenchStationKey();
            ClearLocalWorkbenchRuntimeState(clearBinding: false);
            if (!string.IsNullOrWhiteSpace(stationKey))
            {
                _boundWorkbenchStationKey = stationKey;
                LoadRuntimeStateFromPersistence(clearIfMissing: true);
            }

            if (_craftingService != null && _anchorTarget != null)
            {
                _craftingService.RefreshRuntimeContextFromScene();
                _craftingService.SetStation(CraftingStation.Workbench);
                BindInventory(_craftingService.Inventory);
                ResumeCraftRoutineIfNeeded();
                if (_isVisible)
                {
                    RefreshAll(validateRecipeRows: true, rebuildRecipeGeometry: true, rebuildDetailGeometry: true);
                }
            }

            UpdateFloatingProgressVisibility();
            PushDirectorCraftProgress();
        }

        private void LoadRuntimeStateFromPersistence(bool clearIfMissing)
        {
            string stationKey = ResolveBoundWorkbenchStationKey();
            if (string.IsNullOrWhiteSpace(stationKey))
            {
                if (clearIfMissing)
                {
                    ClearLocalWorkbenchRuntimeState(clearBinding: true);
                }

                return;
            }

            if (!StoryProgressPersistenceService.TryGetWorkbenchRuntimeState(stationKey, out WorkbenchRuntimeSaveData state)
                || state == null)
            {
                if (clearIfMissing)
                {
                    ClearLocalWorkbenchRuntimeState(clearBinding: false);
                }

                return;
            }

            ApplyPersistedWorkbenchRuntimeState(state);
        }

        private void ApplyPersistedWorkbenchRuntimeState(WorkbenchRuntimeSaveData state)
        {
            if (state == null)
            {
                return;
            }

            EnsureRecipesLoaded();
            ClearLocalWorkbenchRuntimeState(clearBinding: false);
            _boundWorkbenchStationKey = state.stationKey?.Trim() ?? string.Empty;
            _boundScenePath = state.scenePath ?? string.Empty;
            _boundSceneName = state.sceneName ?? string.Empty;

            if (state.queueEntries != null)
            {
                for (int index = 0; index < state.queueEntries.Count; index++)
                {
                    WorkbenchQueueEntrySaveData savedEntry = state.queueEntries[index];
                    RecipeData recipe = ResolveRecipeById(savedEntry?.recipeId ?? -1);
                    if (savedEntry == null || recipe == null)
                    {
                        continue;
                    }

                    WorkbenchQueueEntry restored = new WorkbenchQueueEntry
                    {
                        recipe = recipe,
                        recipeId = recipe.recipeID,
                        resultItemId = savedEntry.resultItemId > 0 ? savedEntry.resultItemId : recipe.resultItemID,
                        resultAmountPerCraft = Mathf.Max(1, savedEntry.resultAmountPerCraft),
                        totalCount = Mathf.Max(0, savedEntry.totalCount),
                        readyCount = Mathf.Clamp(savedEntry.readyCount, 0, Mathf.Max(0, savedEntry.totalCount)),
                        currentUnitProgress = Mathf.Clamp01(savedEntry.currentUnitProgress),
                        hasReservedCurrentCraft = savedEntry.hasReservedCurrentCraft
                    };
                    _queueEntries.Add(restored);
                    if (savedEntry.isActiveEntry && _activeQueueEntry == null && HasPendingCrafts(restored))
                    {
                        _activeQueueEntry = restored;
                    }
                }
            }

            _activeQueueEntry ??= FindNextPendingQueueEntry();
            _craftingRecipe = _activeQueueEntry != null ? _activeQueueEntry.recipe : null;
            _hasReservedActiveCraft = _activeQueueEntry != null && _activeQueueEntry.hasReservedCurrentCraft;
            _craftProgress = _activeQueueEntry != null ? GetEntryUnitProgress(_activeQueueEntry) : 0f;
            SyncCountsFromActiveEntry();
        }

        private void ClearLocalWorkbenchRuntimeState(bool clearBinding)
        {
            _craftRoutine = null;
            _craftProgress = 0f;
            _craftQueueTotal = 0;
            _craftQueueCompleted = 0;
            _lastCompletedQueueTotal = 0;
            _lastCompletedRecipeId = -1;
            _craftingRecipe = null;
            _activeQueueEntry = null;
            _hasReservedActiveCraft = false;
            _selectedQuantity = 0;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            _queueEntries.Clear();
            if (clearBinding)
            {
                _boundWorkbenchStationKey = string.Empty;
                ClearSceneBinding();
            }
        }

        private void SuspendCraftRoutineForPersistence()
        {
            if (_craftRoutine != null)
            {
                StopCoroutine(_craftRoutine);
            }

            _craftRoutine = null;
            _craftingRecipe = _activeQueueEntry != null ? _activeQueueEntry.recipe : null;
            _craftProgress = _activeQueueEntry != null ? GetEntryUnitProgress(_activeQueueEntry) : 0f;
            _hasReservedActiveCraft = _activeQueueEntry != null && _activeQueueEntry.hasReservedCurrentCraft;
            SetWorkbenchAnimating(false);
        }

        private void ResumeCraftRoutineIfNeeded()
        {
            if (_craftRoutine != null
                || _craftingService == null
                || _activeQueueEntry == null
                || _activeQueueEntry.recipe == null
                || !HasPendingCrafts(_activeQueueEntry))
            {
                return;
            }

            _craftingRecipe = _activeQueueEntry.recipe;
            _craftRoutine = StartCoroutine(CraftRoutine(_activeQueueEntry.recipe, 0, resumeExistingEntry: true));
        }

        private string ResolveBoundWorkbenchStationKey()
        {
            if (string.IsNullOrWhiteSpace(_boundWorkbenchStationKey) && _anchorTarget != null)
            {
                _boundWorkbenchStationKey = BuildWorkbenchStationKey(_anchorTarget);
            }

            return _boundWorkbenchStationKey;
        }

        private WorkbenchRuntimeSaveData BuildWorkbenchRuntimeSaveData(string stationKey)
        {
            WorkbenchRuntimeSaveData saveData = new WorkbenchRuntimeSaveData
            {
                stationKey = stationKey,
                sceneName = _boundSceneName ?? string.Empty,
                scenePath = _boundScenePath ?? string.Empty,
                queueEntries = new List<WorkbenchQueueEntrySaveData>()
            };

            for (int index = 0; index < _queueEntries.Count; index++)
            {
                WorkbenchQueueEntry entry = _queueEntries[index];
                if (entry == null || (entry.totalCount <= 0 && entry.readyCount <= 0))
                {
                    continue;
                }

                saveData.queueEntries.Add(new WorkbenchQueueEntrySaveData
                {
                    recipeId = entry.recipeId,
                    resultItemId = entry.resultItemId,
                    resultAmountPerCraft = Mathf.Max(1, entry.resultAmountPerCraft),
                    totalCount = Mathf.Max(0, entry.totalCount),
                    readyCount = Mathf.Clamp(entry.readyCount, 0, Mathf.Max(0, entry.totalCount)),
                    currentUnitProgress = Mathf.Clamp01(entry.currentUnitProgress),
                    hasReservedCurrentCraft = entry.hasReservedCurrentCraft,
                    isActiveEntry = entry == _activeQueueEntry
                });
            }

            return saveData;
        }

        private static string BuildWorkbenchStationKey(Transform anchorTarget)
        {
            if (anchorTarget == null)
            {
                return string.Empty;
            }

            Stack<string> hierarchy = new Stack<string>();
            Transform current = anchorTarget;
            while (current != null)
            {
                hierarchy.Push(current.name);
                current = current.parent;
            }

            Scene scene = anchorTarget.gameObject.scene;
            string sceneKey = !string.IsNullOrWhiteSpace(scene.path) ? scene.path : scene.name;
            Vector3 position = anchorTarget.position;
            return $"{sceneKey}|{string.Join("/", hierarchy)}|{position.x:F3}|{position.y:F3}|{position.z:F3}";
        }

        private bool IsStateVisibleInActiveScene()
        {
            return IsStateBoundToScene(SceneManager.GetActiveScene());
        }

        private bool IsStateBoundToScene(Scene scene)
        {
            if (!HasRawWorkbenchState)
            {
                return true;
            }

            if (string.IsNullOrEmpty(_boundScenePath) && string.IsNullOrEmpty(_boundSceneName))
            {
                return true;
            }

            if (!scene.IsValid())
            {
                return false;
            }

            if (!string.IsNullOrEmpty(_boundScenePath) && !string.IsNullOrEmpty(scene.path))
            {
                return string.Equals(_boundScenePath, scene.path, System.StringComparison.Ordinal);
            }

            return string.Equals(_boundSceneName, scene.name, System.StringComparison.Ordinal);
        }

        private void DetachScenePresentation()
        {
            if (!_isVisible && _anchorTarget == null)
            {
                UpdateFloatingProgressVisibility();
                return;
            }

            _isVisible = false;
            _anchorTarget = null;
            _playerTransform = null;
            _workbenchAnimator = null;
            _workbenchAnimatorBoolName = null;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            ApplyNavigationBlock(false);
            HideImmediate();
            SetWorkbenchPanelVisible(false);
            UpdateFloatingProgressVisibility();
        }

        private void RefreshBoundSceneRuntimeState()
        {
            if (!HasRawWorkbenchState)
            {
                LoadRuntimeStateFromPersistence(clearIfMissing: false);
            }

            if (!HasRawWorkbenchState || _craftingService == null)
            {
                return;
            }

            _craftingService.RefreshRuntimeContextFromScene();
            BindInventory(_craftingService.Inventory);
            HandleInventoryChanged();
            ResumeCraftRoutineIfNeeded();
            PushDirectorCraftProgress();
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            ApplyRuntimeCanvasDefaults();

            panelRect = CreateRect(transform, "PanelRoot");
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
            Image panelImage = panelRect.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0.32f, 0.22f, 0.12f, 0.94f);
            panelImage.raycastTarget = true;
            ApplyOutline(panelRect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));
            ApplyShadow(panelRect.gameObject.AddComponent<Shadow>(), new Color(0f, 0f, 0f, 0.28f), new Vector2(0f, -6f));

            pointerRect = CreateRect(panelRect, "Pointer");
            pointerRect.sizeDelta = new Vector2(14f, 14f);
            pointerRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Image pointerImage = pointerRect.gameObject.AddComponent<Image>();
            pointerImage.color = new Color(0.32f, 0.22f, 0.12f, 0.94f);
            pointerImage.raycastTarget = false;

            RectTransform surface = CreateRect(panelRect, "Surface");
            Stretch(surface, new Vector2(4f, 4f), new Vector2(-4f, -4f));
            Image surfaceImage = surface.gameObject.AddComponent<Image>();
            surfaceImage.color = new Color(0.07f, 0.09f, 0.13f, 0.94f);
            surfaceImage.raycastTarget = true;
            EnsureQueueOverflowToastCompatibility(surface);

            RectTransform leftPanel = CreateSection(surface, "RecipeColumn");
            leftPanel.anchorMin = new Vector2(0f, 0f);
            leftPanel.anchorMax = new Vector2(0f, 1f);
            leftPanel.pivot = new Vector2(0f, 0.5f);
            leftPanel.anchoredPosition = new Vector2(12f, 0f);
            leftPanel.sizeDelta = new Vector2(leftWidth, -24f);

            RectTransform rightPanel = CreateSection(surface, "DetailColumn");
            rightPanel.anchorMin = new Vector2(1f, 0f);
            rightPanel.anchorMax = new Vector2(1f, 1f);
            rightPanel.pivot = new Vector2(1f, 0.5f);
            rightPanel.anchoredPosition = new Vector2(-12f, 0f);
            rightPanel.sizeDelta = new Vector2(panelWidth - leftWidth - 38f, -24f);

            RectTransform leftTag = CreateRect(leftPanel, "LeftTag");
            leftTag.anchorMin = new Vector2(0f, 1f);
            leftTag.anchorMax = new Vector2(0f, 1f);
            leftTag.pivot = new Vector2(0f, 1f);
            leftTag.anchoredPosition = new Vector2(10f, -10f);
            leftTag.sizeDelta = new Vector2(90f, 18f);
            leftTag.gameObject.AddComponent<Image>().color = new Color(0.98f, 0.81f, 0.44f, 0.15f);
            CreateText(leftTag, "Label", "工具配方", 10f, new Color(0.97f, 0.8f, 0.42f, 1f), TextAlignmentOptions.Center, false, true);

            recipeViewportRect = CreateRect(leftPanel, "Viewport");
            recipeViewportRect.anchorMin = new Vector2(0f, 0f);
            recipeViewportRect.anchorMax = new Vector2(1f, 1f);
            recipeViewportRect.offsetMin = new Vector2(8f, 10f);
            recipeViewportRect.offsetMax = new Vector2(-18f, -38f);
            Image viewportImage = recipeViewportRect.gameObject.AddComponent<Image>();
            viewportImage.color = RecipeViewportVisibleColor;
            viewportImage.raycastTarget = true;
            recipeViewportRect.gameObject.AddComponent<Mask>().showMaskGraphic = true;
            _recipeScrollRect = recipeViewportRect.gameObject.AddComponent<ScrollRect>();
            _recipeScrollRect.horizontal = false;
            _recipeScrollRect.vertical = true;
            _recipeScrollRect.inertia = false;
            _recipeScrollRect.movementType = ScrollRect.MovementType.Clamped;
            _recipeScrollRect.scrollSensitivity = 18f;

            recipeContentRect = CreateRect(recipeViewportRect, "Content");
            recipeContentRect.anchorMin = new Vector2(0f, 1f);
            recipeContentRect.anchorMax = new Vector2(1f, 1f);
            recipeContentRect.pivot = new Vector2(0.5f, 1f);
            recipeContentRect.anchoredPosition = Vector2.zero;
            VerticalLayoutGroup recipeLayout = recipeContentRect.gameObject.AddComponent<VerticalLayoutGroup>();
            recipeLayout.padding = new RectOffset(0, 0, 0, 0);
            recipeLayout.spacing = rowSpacing;
            recipeLayout.childAlignment = TextAnchor.UpperLeft;
            recipeLayout.childControlWidth = true;
            recipeLayout.childControlHeight = false;
            recipeLayout.childForceExpandWidth = true;
            recipeLayout.childForceExpandHeight = false;
            ContentSizeFitter recipeContentFitter = recipeContentRect.gameObject.AddComponent<ContentSizeFitter>();
            recipeContentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            recipeContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            recipeScrollbar = CreateRecipeScrollbar(leftPanel);
            _recipeScrollRect.viewport = recipeViewportRect;
            _recipeScrollRect.content = recipeContentRect;
            _recipeScrollRect.verticalScrollbar = recipeScrollbar;
            _recipeScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            _recipeScrollRect.verticalScrollbarSpacing = 2f;

            RectTransform detailLayout = CreateRect(rightPanel, "DetailLayout");
            Stretch(detailLayout, new Vector2(10f, 10f), new Vector2(-10f, -10f));
            VerticalLayoutGroup detailRootLayout = detailLayout.gameObject.AddComponent<VerticalLayoutGroup>();
            detailRootLayout.padding = new RectOffset(0, 0, 0, 0);
            detailRootLayout.spacing = 8f;
            detailRootLayout.childAlignment = TextAnchor.UpperLeft;
            detailRootLayout.childControlWidth = true;
            detailRootLayout.childControlHeight = false;
            detailRootLayout.childForceExpandWidth = true;
            detailRootLayout.childForceExpandHeight = false;

            RectTransform headerRow = CreateRect(detailLayout, "HeaderRow");
            LayoutElement headerLayoutElement = headerRow.gameObject.AddComponent<LayoutElement>();
            headerLayoutElement.preferredHeight = 58f;
            HorizontalLayoutGroup headerLayout = headerRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 10f;
            headerLayout.childAlignment = TextAnchor.UpperLeft;
            headerLayout.childControlWidth = false;
            headerLayout.childControlHeight = false;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = false;

            RectTransform iconCard = CreateRect(headerRow, "IconCard");
            LayoutElement iconCardLayout = iconCard.gameObject.AddComponent<LayoutElement>();
            iconCardLayout.preferredWidth = 48f;
            iconCardLayout.preferredHeight = 48f;
            iconCard.gameObject.AddComponent<Image>().color = new Color(0.23f, 0.29f, 0.39f, 0.95f);
            ApplyOutline(iconCard.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));
            selectedIcon = CreateIcon(iconCard, "SelectedIcon", 28f);
            Center(selectedIcon.rectTransform);

            RectTransform metaColumn = CreateRect(headerRow, "MetaColumn");
            LayoutElement metaLayoutElement = metaColumn.gameObject.AddComponent<LayoutElement>();
            metaLayoutElement.flexibleWidth = 1f;
            VerticalLayoutGroup metaLayout = metaColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            metaLayout.spacing = 4f;
            metaLayout.childAlignment = TextAnchor.UpperLeft;
            metaLayout.childControlWidth = true;
            metaLayout.childControlHeight = false;
            metaLayout.childForceExpandWidth = true;
            metaLayout.childForceExpandHeight = false;
            ContentSizeFitter metaFitter = metaColumn.gameObject.AddComponent<ContentSizeFitter>();
            metaFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            metaFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            selectedNameText = CreateText(metaColumn, "SelectedName", string.Empty, 16f, Color.white, TextAlignmentOptions.TopLeft, true);
            selectedNameText.gameObject.AddComponent<LayoutElement>().minHeight = 18f;

            RectTransform descriptionRoot = CreateRect(metaColumn, "DescriptionRoot");
            ContentSizeFitter descriptionFitter = descriptionRoot.gameObject.AddComponent<ContentSizeFitter>();
            descriptionFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            descriptionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            selectedDescriptionText = CreateText(descriptionRoot, "SelectedDescription", string.Empty, 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.TopLeft, true);
            Stretch(selectedDescriptionText.rectTransform, Vector2.zero, Vector2.zero);
            descriptionRoot.gameObject.AddComponent<LayoutElement>().minHeight = 26f;

            TextMeshProUGUI materialsTitle = CreateText(detailLayout, "MaterialsTitle", "所需材料", 10f, new Color(0.97f, 0.8f, 0.42f, 1f), TextAlignmentOptions.Left);
            materialsTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 14f;

            materialsViewportRect = CreateRect(detailLayout, "MaterialsViewport");
            LayoutElement materialsViewportLayout = materialsViewportRect.gameObject.AddComponent<LayoutElement>();
            materialsViewportLayout.minHeight = 56f;
            materialsViewportLayout.flexibleHeight = 1f;
            Image materialsBg = materialsViewportRect.gameObject.AddComponent<Image>();
            materialsBg.color = new Color(0.05f, 0.07f, 0.1f, 0.45f);
            materialsBg.raycastTarget = true;
            materialsViewportRect.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            _materialsScrollRect = materialsViewportRect.gameObject.AddComponent<ScrollRect>();
            _materialsScrollRect.horizontal = false;
            _materialsScrollRect.vertical = true;
            _materialsScrollRect.inertia = false;
            _materialsScrollRect.movementType = ScrollRect.MovementType.Clamped;
            _materialsScrollRect.scrollSensitivity = 18f;

            materialsContentRect = CreateRect(materialsViewportRect, "Content");
            materialsContentRect.anchorMin = new Vector2(0f, 1f);
            materialsContentRect.anchorMax = new Vector2(1f, 1f);
            materialsContentRect.pivot = new Vector2(0.5f, 1f);
            materialsContentRect.anchoredPosition = Vector2.zero;
            VerticalLayoutGroup materialsLayout = materialsContentRect.gameObject.AddComponent<VerticalLayoutGroup>();
            materialsLayout.padding = new RectOffset(6, 6, 6, 6);
            materialsLayout.spacing = 0f;
            materialsLayout.childAlignment = TextAnchor.UpperLeft;
            materialsLayout.childControlWidth = true;
            materialsLayout.childControlHeight = false;
            materialsLayout.childForceExpandWidth = true;
            materialsLayout.childForceExpandHeight = false;
            ContentSizeFitter materialsContentFitter = materialsContentRect.gameObject.AddComponent<ContentSizeFitter>();
            materialsContentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            materialsContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            selectedMaterialsText = CreateText(materialsContentRect, "SelectedMaterials", string.Empty, 11f, Color.white, TextAlignmentOptions.TopLeft, true);
            selectedMaterialsText.enableAutoSizing = false;
            selectedMaterialsText.gameObject.AddComponent<LayoutElement>().minHeight = 20f;
            _materialsScrollRect.viewport = materialsViewportRect;
            _materialsScrollRect.content = materialsContentRect;

            TextMeshProUGUI quantityTitle = CreateText(detailLayout, "QuantityTitle", "数量", 10f, new Color(0.97f, 0.8f, 0.42f, 1f), TextAlignmentOptions.Left);
            quantityTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 14f;

            RectTransform quantityControls = CreateRect(detailLayout, "QuantityControls");
            quantityControls.gameObject.AddComponent<LayoutElement>().preferredHeight = 38f;

            decreaseButton = CreateButton(quantityControls, "Decrease", "-", new Vector2(20f, 18f), 10f);
            AnchorTopLeft(decreaseButton.GetComponent<RectTransform>(), 10f, 14f);
            quantitySlider = CreateSlider(quantityControls);
            RectTransform sliderRect = quantitySlider.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0f, 1f);
            sliderRect.anchorMax = new Vector2(1f, 1f);
            sliderRect.offsetMin = new Vector2(34f, -38f);
            sliderRect.offsetMax = new Vector2(-34f, -20f);
            increaseButton = CreateButton(quantityControls, "Increase", "+", new Vector2(20f, 18f), 10f);
            RectTransform incRect = increaseButton.GetComponent<RectTransform>();
            incRect.anchorMin = new Vector2(1f, 1f);
            incRect.anchorMax = new Vector2(1f, 1f);
            incRect.pivot = new Vector2(1f, 1f);
            incRect.anchoredPosition = new Vector2(-10f, -14f);
            quantityValueText = CreateText(quantityControls, "QuantityValue", "x0", 11f, Color.white, TextAlignmentOptions.Center);
            quantityValueText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            quantityValueText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            quantityValueText.rectTransform.pivot = new Vector2(0.5f, 1f);
            quantityValueText.rectTransform.anchoredPosition = new Vector2(0f, -14f);
            quantityValueText.rectTransform.sizeDelta = new Vector2(44f, 16f);

            stageHintText = CreateText(detailLayout, "StageHint", string.Empty, 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.TopLeft, true);
            stageHintText.gameObject.AddComponent<LayoutElement>().minHeight = 18f;

            RectTransform craftArea = CreateRect(detailLayout, "CraftArea");
            craftArea.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
            progressRoot = CreateRect(craftArea, "ProgressBackground");
            Stretch(progressRoot, Vector2.zero, Vector2.zero);
            progressBackgroundImage = progressRoot.gameObject.AddComponent<Image>();
            progressBackgroundImage.color = new Color(0.04f, 0.05f, 0.08f, 0f);
            progressBackgroundImage.raycastTarget = true;
            progressButton = progressRoot.gameObject.AddComponent<Button>();
            progressButton.targetGraphic = progressBackgroundImage;
            RectTransform progressFill = CreateRect(progressRoot, "ProgressFill");
            Stretch(progressFill, new Vector2(1f, 1f), new Vector2(-1f, -1f));
            progressFillImage = progressFill.gameObject.AddComponent<Image>();
            EnsureProgressFillGraphic(progressFillImage);

            progressLabelText = CreateText(progressRoot, "ProgressLabel", "进度  0/0", 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.Center, true, true);
            EnsureProgressLabelForeground(progressLabelText);
            AddProgressHoverRelay(progressRoot.gameObject);

            craftButton = CreateButton(craftArea, "CraftButton", "开始制作", new Vector2(0f, 30f), 13f);
            RectTransform craftRect = craftButton.GetComponent<RectTransform>();
            craftRect.anchorMin = new Vector2(0f, 0f);
            craftRect.anchorMax = new Vector2(1f, 0f);
            craftRect.offsetMin = Vector2.zero;
            craftRect.offsetMax = new Vector2(0f, 40f);
            craftButtonBackground = craftButton.targetGraphic as Image;
            craftButtonLabel = craftButton.GetComponentInChildren<TextMeshProUGUI>();
            AddHoverRelay(craftButton.gameObject);

            floatingProgressRoot = CreateRect(transform, "FloatingProgressRoot");
            floatingProgressRoot.anchorMin = new Vector2(0.5f, 0.5f);
            floatingProgressRoot.anchorMax = new Vector2(0.5f, 0.5f);
            floatingProgressRoot.pivot = new Vector2(0.5f, 0f);
            floatingProgressRoot.sizeDelta = new Vector2(54f, 46f);
            floatingProgressRoot.gameObject.SetActive(false);
            Image floatingCard = floatingProgressRoot.gameObject.AddComponent<Image>();
            floatingCard.color = new Color(0.07f, 0.09f, 0.13f, 0.92f);
            floatingCard.raycastTarget = false;
            ApplyOutline(floatingProgressRoot.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));
            ApplyShadow(floatingProgressRoot.gameObject.AddComponent<Shadow>(), new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, -3f));
            RectTransform floatingIconCard = CreateRect(floatingProgressRoot, "IconCard");
            floatingIconCard.anchorMin = new Vector2(0.5f, 1f);
            floatingIconCard.anchorMax = new Vector2(0.5f, 1f);
            floatingIconCard.pivot = new Vector2(0.5f, 1f);
            floatingIconCard.anchoredPosition = new Vector2(0f, -5f);
            floatingIconCard.sizeDelta = new Vector2(30f, 30f);
            floatingIconCard.gameObject.AddComponent<Image>().color = new Color(0.18f, 0.22f, 0.31f, 0.96f);
            floatingProgressIcon = CreateIcon(floatingIconCard, "Icon", 18f);
            Center(floatingProgressIcon.rectTransform);
            RectTransform floatingFillBackground = CreateRect(floatingProgressRoot, "ProgressBackground");
            floatingFillBackground.anchorMin = new Vector2(0f, 0f);
            floatingFillBackground.anchorMax = new Vector2(1f, 0f);
            floatingFillBackground.offsetMin = new Vector2(4f, 6f);
            floatingFillBackground.offsetMax = new Vector2(-4f, 14f);
            floatingFillBackground.gameObject.AddComponent<Image>().color = new Color(0.04f, 0.05f, 0.08f, 0.94f);
            RectTransform floatingFill = CreateRect(floatingFillBackground, "ProgressFill");
            Stretch(floatingFill, new Vector2(1f, 1f), new Vector2(-1f, -1f));
            floatingProgressFillImage = floatingFill.gameObject.AddComponent<Image>();
            floatingProgressFillImage.type = Image.Type.Filled;
            floatingProgressFillImage.fillMethod = Image.FillMethod.Horizontal;
            floatingProgressFillImage.fillAmount = 0f;
            floatingProgressFillImage.color = new Color(0.43f, 0.73f, 0.56f, 0.96f);
            floatingProgressLabel = CreateText(floatingProgressRoot, "Label", string.Empty, 8f, Color.white, TextAlignmentOptions.Center);
            floatingProgressLabel.rectTransform.anchorMin = new Vector2(0f, 0f);
            floatingProgressLabel.rectTransform.anchorMax = new Vector2(1f, 0f);
            floatingProgressLabel.rectTransform.offsetMin = new Vector2(2f, 16f);
            floatingProgressLabel.rectTransform.offsetMax = new Vector2(-2f, 28f);
            floatingProgressLabel.raycastTarget = false;
            EnsureProgressLabelForeground(floatingProgressLabel);
            _floatingCards.Clear();
            _floatingCards.Add(new FloatingProgressCardRefs
            {
                root = floatingProgressRoot,
                icon = floatingProgressIcon,
                fill = floatingProgressFillImage,
                label = floatingProgressLabel
            });

            decreaseButton.onClick.AddListener(() => ChangeQuantity(-1));
            increaseButton.onClick.AddListener(() => ChangeQuantity(1));
            quantitySlider.onValueChanged.AddListener(v => SetQuantity(Mathf.RoundToInt(v), false));
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }

        private void EnsureBuilt()
        {
            if (TryBindRuntimeShell())
            {
                return;
            }

            RebuildShellFromScratch();
        }

        private void RebuildShellFromScratch()
        {
            rootRect = transform as RectTransform;
            ClearRuntimeShellForRebuild();
            BuildUi();
        }

        private void ClearRuntimeShellForRebuild()
        {
            StopQueueOverflowToastRoutine();

            if (rootRect != null)
            {
                for (int index = rootRect.childCount - 1; index >= 0; index--)
                {
                    if (rootRect.GetChild(index) is RectTransform child)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(child.gameObject);
                        }
                        else
                        {
                            DestroyImmediate(child.gameObject);
                        }
                    }
                }
            }

            panelRect = null;
            pointerRect = null;
            queueToastRect = null;
            queueToastGroup = null;
            queueToastText = null;
            recipeViewportRect = null;
            recipeContentRect = null;
            recipeScrollbar = null;
            selectedIcon = null;
            selectedNameText = null;
            selectedDescriptionText = null;
            selectedMaterialsText = null;
            materialsViewportRect = null;
            materialsContentRect = null;
            stageHintText = null;
            progressLabelText = null;
            progressRoot = null;
            progressButton = null;
            progressBackgroundImage = null;
            progressFillImage = null;
            quantitySlider = null;
            quantityValueText = null;
            decreaseButton = null;
            increaseButton = null;
            craftButton = null;
            craftButtonBackground = null;
            craftButtonLabel = null;
            floatingProgressRoot = null;
            floatingProgressIcon = null;
            floatingProgressFillImage = null;
            floatingProgressLabel = null;
            _floatingCards.Clear();
            _recipeScrollRect = null;
            _materialsScrollRect = null;
            _detailColumnRect = null;
            _materialsTitleRect = null;
            _quantityTitleRect = null;
            _quantityControlsRect = null;
            _iconCardRect = null;
            _rows.Clear();
            _nameBaseline = default;
            _descriptionBaseline = default;
            _materialsTitleBaseline = default;
            _materialsViewportBaseline = default;
            _quantityTitleBaseline = default;
            _quantityControlsBaseline = default;
            _stageHintBaseline = default;
            _progressLabelBaseline = default;
            _progressBaseline = default;
            _craftButtonBaseline = default;
            _iconCardBaseline = default;
        }

        private static void DestroyDirectChildIfExists(RectTransform parent, string childName)
        {
            RectTransform child = FindDirectChildRect(parent, childName);
            if (child == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private bool TryBindRuntimeShell()
        {
            if (rootRect == null)
            {
                rootRect = transform as RectTransform;
            }

            if (overlayCanvas == null)
            {
                overlayCanvas = GetComponent<Canvas>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_fontAsset == null)
            {
                _fontAsset = ResolveFont();
            }

            if (rootRect == null || overlayCanvas == null || canvasGroup == null)
            {
                return false;
            }

            ApplyRuntimeCanvasDefaults();

            if (panelRect == null)
            {
                panelRect = FindDirectChildRect(rootRect, "PanelRoot");
            }

            if (pointerRect == null && panelRect != null)
            {
                pointerRect = FindDirectChildRect(panelRect, "Pointer");
            }

            EnsureQueueOverflowToastCompatibility();

            if (recipeViewportRect == null)
            {
                recipeViewportRect = FindDescendantRect(panelRect, "Viewport");
            }

            if (recipeContentRect == null && recipeViewportRect != null)
            {
                recipeContentRect = FindDirectChildRect(recipeViewportRect, "Content");
            }

            if (recipeScrollbar == null && recipeViewportRect != null)
            {
                recipeScrollbar = FindDescendantComponent<Scrollbar>(recipeViewportRect.parent as RectTransform, "RecipeScrollbar");
            }

            if (selectedIcon == null)
            {
                selectedIcon = FindDescendantComponent<Image>(panelRect, "SelectedIcon");
            }

            if (selectedNameText == null)
            {
                selectedNameText = FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedName");
            }

            if (selectedDescriptionText == null)
            {
                selectedDescriptionText = FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedDescription");
            }

            if (selectedMaterialsText == null)
            {
                selectedMaterialsText = FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedMaterials");
            }

            if (stageHintText == null)
            {
                stageHintText = FindDescendantComponent<TextMeshProUGUI>(panelRect, "StageHint");
            }

            if (progressLabelText == null)
            {
                progressLabelText = FindDescendantComponent<TextMeshProUGUI>(panelRect, "ProgressLabel");
            }

            if (progressRoot == null)
            {
                progressRoot = FindDescendantRect(panelRect, "ProgressBackground");
            }

            if (progressBackgroundImage == null && progressRoot != null)
            {
                progressBackgroundImage = progressRoot.GetComponent<Image>();
            }

            if (progressButton == null && progressRoot != null)
            {
                progressButton = progressRoot.GetComponent<Button>();
                if (progressButton == null)
                {
                    progressButton = SpringDay1UiLayerUtility.EnsureComponent<Button>(progressRoot.gameObject);
                }
            }

            if (progressButton != null && progressBackgroundImage != null && progressButton.targetGraphic == null)
            {
                progressButton.targetGraphic = progressBackgroundImage;
            }

            if (progressFillImage == null)
            {
                progressFillImage = FindDescendantComponent<Image>(progressRoot, "ProgressFill");
            }
            EnsureProgressFillGraphic(progressFillImage);
            EnsureProgressLabelBinding();

            if (quantitySlider == null)
            {
                quantitySlider = FindDescendantComponent<Slider>(panelRect, "Slider");
            }

            if (quantityValueText == null)
            {
                quantityValueText = FindDescendantComponent<TextMeshProUGUI>(panelRect, "QuantityValue");
            }

            if (decreaseButton == null)
            {
                decreaseButton = FindDescendantComponent<Button>(panelRect, "Decrease");
            }

            if (increaseButton == null)
            {
                increaseButton = FindDescendantComponent<Button>(panelRect, "Increase");
            }

            if (craftButton == null)
            {
                craftButton = FindDescendantComponent<Button>(panelRect, "CraftButton");
            }

            if (craftButtonBackground == null && craftButton != null)
            {
                craftButtonBackground = craftButton.targetGraphic as Image;
                if (craftButtonBackground == null)
                {
                    craftButtonBackground = craftButton.GetComponent<Image>();
                }
            }

            if (craftButtonLabel == null && craftButton != null)
            {
                craftButtonLabel = craftButton.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (selectedMaterialsText != null)
            {
                if (materialsContentRect == null)
                {
                    materialsContentRect = selectedMaterialsText.rectTransform.parent as RectTransform;
                }

                if (materialsViewportRect == null)
                {
                    materialsViewportRect = materialsContentRect;
                }
            }

            if (_recipeScrollRect == null && recipeViewportRect != null)
            {
                _recipeScrollRect = recipeViewportRect.GetComponent<ScrollRect>();
            }

            if (_materialsScrollRect == null && materialsViewportRect != null)
            {
                _materialsScrollRect = materialsViewportRect.GetComponent<ScrollRect>();
            }

            if (_detailColumnRect == null)
            {
                _detailColumnRect = FindDescendantRect(panelRect, "DetailColumn");
            }

            if (_materialsTitleRect == null)
            {
                _materialsTitleRect = FindDescendantRect(_detailColumnRect, "MaterialsTitle");
            }

            if (_quantityTitleRect == null)
            {
                _quantityTitleRect = FindDescendantRect(_detailColumnRect, "QuantityTitle");
            }

            if (_quantityControlsRect == null)
            {
                _quantityControlsRect = FindDescendantRect(_detailColumnRect, "QuantityControls");
            }

            if (_iconCardRect == null && selectedIcon != null)
            {
                _iconCardRect = selectedIcon.rectTransform.parent as RectTransform;
            }

            bool usesPrefabDetailShell = UsesPrefabDetailShell();

            ApplyResolvedFontToShellTexts();
            EnsureRecipeViewportCompatibility();
            EnsureMaterialsViewportCompatibility();
            EnsureFloatingProgressCompatibility();
            if (usesPrefabDetailShell)
            {
                EnsurePrefabDetailTextChain();
            }
            else
            {
                EnsureDetailColumnCompatibility();
            }

            CaptureDetailLayoutBaselines();

            if (panelRect == null || pointerRect == null || recipeContentRect == null || selectedIcon == null || selectedNameText == null
                || selectedDescriptionText == null || selectedMaterialsText == null || stageHintText == null || progressLabelText == null
                || progressButton == null || progressFillImage == null || quantitySlider == null || quantityValueText == null || decreaseButton == null
                || increaseButton == null || craftButton == null || craftButtonBackground == null || craftButtonLabel == null)
            {
                return false;
            }

            panelWidth = panelRect.rect.width;
            panelHeight = panelRect.rect.height;

            bool shouldForceGeneratedRecipeRows = recipeContentRect != null
                && EnsureRecipesLoaded()
                && !HasGeneratedRecipeRowChain();
            if (shouldForceGeneratedRecipeRows)
            {
                RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true);
                EnsureRecipeViewportCompatibility();
            }

            BindExistingRows();

            decreaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.RemoveAllListeners();
            quantitySlider.onValueChanged.RemoveAllListeners();
            progressButton.onClick.RemoveAllListeners();
            craftButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(() => ChangeQuantity(-1));
            increaseButton.onClick.AddListener(() => ChangeQuantity(1));
            quantitySlider.onValueChanged.AddListener(v => SetQuantity(Mathf.RoundToInt(v), false));
            progressButton.onClick.AddListener(OnProgressBarClicked);
            craftButton.onClick.AddListener(OnCraftButtonClicked);
            AddProgressHoverRelay(progressRoot.gameObject);
            AddHoverRelay(craftButton.gameObject);
            RefreshCompatibilityLayout();
            if (!HasStableWorkbenchShellBindings())
            {
                return false;
            }

            return true;
        }

        private bool HasStableWorkbenchShellBindings()
        {
            return panelRect != null
                && recipeViewportRect != null
                && recipeContentRect != null
                && _detailColumnRect != null
                && quantitySlider != null
                && quantitySlider.transform.IsChildOf(panelRect)
                && quantityValueText != null
                && quantityValueText.transform.IsChildOf(panelRect)
                && progressRoot != null
                && progressRoot.IsChildOf(panelRect)
                && progressLabelText != null
                && progressLabelText.transform.IsChildOf(panelRect)
                && craftButton != null
                && craftButton.transform.IsChildOf(panelRect)
                && (UsesPrefabDetailShell() || !UsesLegacyDetailManualLayout());
        }

        private void ApplyRuntimeCanvasDefaults()
        {
            if (overlayCanvas != null)
            {
                overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                overlayCanvas.worldCamera = null;
                overlayCanvas.planeDistance = 100f;
                overlayCanvas.overrideSorting = true;
                overlayCanvas.sortingOrder = OverlaySortingOrder;
                overlayCanvas.pixelPerfect = true;
            }

            if (rootRect != null)
            {
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;
                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;
                rootRect.localScale = Vector3.one;
                rootRect.localRotation = Quaternion.identity;
            }
        }

        private void EnsureDetailColumnCompatibility()
        {
            if (_detailColumnRect == null)
            {
                return;
            }

            RectTransform detailLayout = FindDirectChildRect(_detailColumnRect, "DetailLayout");
            if (detailLayout != null)
            {
                Stretch(detailLayout, new Vector2(10f, 10f), new Vector2(-10f, -10f));
                VerticalLayoutGroup detailRootLayout = SpringDay1UiLayerUtility.EnsureComponent<VerticalLayoutGroup>(detailLayout.gameObject);
                detailRootLayout.padding = new RectOffset(0, 0, 0, 0);
                detailRootLayout.spacing = 8f;
                detailRootLayout.childAlignment = TextAnchor.UpperLeft;
                detailRootLayout.childControlWidth = true;
                detailRootLayout.childControlHeight = false;
                detailRootLayout.childForceExpandWidth = true;
                detailRootLayout.childForceExpandHeight = false;

                RectTransform headerRow = FindDirectChildRect(detailLayout, "HeaderRow");
                if (headerRow != null)
                {
                    SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(headerRow.gameObject).preferredHeight = 58f;
                    HorizontalLayoutGroup headerLayout = SpringDay1UiLayerUtility.EnsureComponent<HorizontalLayoutGroup>(headerRow.gameObject);
                    headerLayout.spacing = 10f;
                    headerLayout.childAlignment = TextAnchor.UpperLeft;
                    headerLayout.childControlWidth = false;
                    headerLayout.childControlHeight = false;
                    headerLayout.childForceExpandWidth = false;
                    headerLayout.childForceExpandHeight = false;

                    RectTransform metaColumn = FindDirectChildRect(headerRow, "MetaColumn");
                    if (metaColumn != null)
                    {
                        SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(metaColumn.gameObject).flexibleWidth = 1f;
                        VerticalLayoutGroup metaLayout = SpringDay1UiLayerUtility.EnsureComponent<VerticalLayoutGroup>(metaColumn.gameObject);
                        metaLayout.spacing = 4f;
                        metaLayout.childAlignment = TextAnchor.UpperLeft;
                        metaLayout.childControlWidth = true;
                        metaLayout.childControlHeight = false;
                        metaLayout.childForceExpandWidth = true;
                        metaLayout.childForceExpandHeight = false;
                        ContentSizeFitter metaFitter = SpringDay1UiLayerUtility.EnsureComponent<ContentSizeFitter>(metaColumn.gameObject);
                        metaFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                        metaFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                        RectTransform descriptionRoot = FindDirectChildRect(metaColumn, "DescriptionRoot");
                        if (descriptionRoot != null)
                        {
                            SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(descriptionRoot.gameObject).minHeight = 26f;
                            ContentSizeFitter descriptionFitter = SpringDay1UiLayerUtility.EnsureComponent<ContentSizeFitter>(descriptionRoot.gameObject);
                            descriptionFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                            descriptionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        }
                    }
                }

                if (_materialsTitleRect != null && _materialsTitleRect.parent == detailLayout)
                {
                    SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(_materialsTitleRect.gameObject).preferredHeight = 14f;
                }

                if (materialsViewportRect != null && materialsViewportRect.parent == detailLayout)
                {
                    LayoutElement viewportLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(materialsViewportRect.gameObject);
                    viewportLayout.minHeight = 56f;
                    viewportLayout.flexibleHeight = 1f;
                }

                if (_quantityTitleRect != null && _quantityTitleRect.parent == detailLayout)
                {
                    SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(_quantityTitleRect.gameObject).preferredHeight = 14f;
                }

                if (_quantityControlsRect != null && _quantityControlsRect.parent == detailLayout)
                {
                    SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(_quantityControlsRect.gameObject).preferredHeight = 38f;
                }

                if (stageHintText != null && stageHintText.rectTransform.parent == detailLayout)
                {
                    SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(stageHintText.gameObject).minHeight = 18f;
                }

                RectTransform craftArea = FindDirectChildRect(detailLayout, "CraftArea");
                if (craftArea != null)
                {
                    SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(craftArea.gameObject).preferredHeight = 40f;
                    if (progressRoot != null && progressRoot.parent == craftArea)
                    {
                        Stretch(progressRoot, Vector2.zero, Vector2.zero);
                    }

                    if (craftButton != null && craftButton.transform.parent == craftArea)
                    {
                        RectTransform craftRect = craftButton.transform as RectTransform;
                        craftRect.anchorMin = new Vector2(0f, 0f);
                        craftRect.anchorMax = new Vector2(1f, 0f);
                        craftRect.pivot = new Vector2(0.5f, 0.5f);
                        craftRect.offsetMin = Vector2.zero;
                        craftRect.offsetMax = new Vector2(0f, 40f);
                    }
                }
            }

            if (selectedNameText != null)
            {
                SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(selectedNameText.gameObject).minHeight = 18f;
            }
        }

        private void EnsurePrefabDetailTextChain()
        {
            EnsureProgressLabelBinding();
            EnsureWorkbenchTextReady(selectedNameText, forceActive: true, minAlpha: 0.98f);
            EnsureWorkbenchTextReady(selectedDescriptionText, forceActive: true, minAlpha: 0.94f);
            EnsureWorkbenchTextReady(selectedMaterialsText, forceActive: true, minAlpha: 0.92f);
            EnsureWorkbenchTextReady(stageHintText, forceActive: true, minAlpha: 0.9f);
            EnsureWorkbenchTextReady(progressLabelText, forceActive: true, minAlpha: 0.94f);
            ApplyDetailColumnFontTuning();

            NormalizePrefabDetailShellGeometry();

            if (selectedDescriptionText != null)
            {
                selectedDescriptionText.enableAutoSizing = false;
                selectedDescriptionText.textWrappingMode = TextWrappingModes.Normal;
                selectedDescriptionText.overflowMode = TextOverflowModes.Overflow;
            }

            if (selectedMaterialsText != null)
            {
                selectedMaterialsText.enableAutoSizing = false;
                selectedMaterialsText.textWrappingMode = TextWrappingModes.Normal;
                selectedMaterialsText.overflowMode = TextOverflowModes.Overflow;
            }
        }

        private void NormalizePrefabDetailShellGeometry()
        {
            if (_detailColumnRect == null)
            {
                return;
            }

            float headerLeftInset = _iconCardRect != null
                ? Mathf.Max(54f, GetCurrentWidth(_iconCardRect, 40f) + 14f)
                : 54f;
            const float headerRightInset = 10f;
            const float headerTop = 10f;

            if (selectedNameText != null)
            {
                RectTransform nameRect = selectedNameText.rectTransform;
                NormalizeTopStretchRect(nameRect);
                float titleWidth = Mathf.Max(96f, _detailColumnRect.rect.width - headerLeftInset - headerRightInset);
                float titleHeight = Mathf.Max(20f, MeasureTextHeight(selectedNameText, titleWidth, 20f));
                SetTopStretchRect(nameRect, headerLeftInset, headerRightInset, headerTop, titleHeight);
            }

            if (selectedDescriptionText != null)
            {
                RectTransform descriptionRect = selectedDescriptionText.rectTransform;
                NormalizeTopStretchRect(descriptionRect);
                float descriptionWidth = Mathf.Max(96f, _detailColumnRect.rect.width - headerLeftInset - headerRightInset);
                float descriptionHeight = Mathf.Clamp(
                    MeasureTextHeight(selectedDescriptionText, descriptionWidth, 24f),
                    24f,
                    38f);
                float descriptionTop = (selectedNameText != null
                    ? headerTop + Mathf.Max(20f, GetCurrentHeight(selectedNameText.rectTransform, 20f)) + 4f
                    : headerTop + 24f);
                SetTopStretchRect(descriptionRect, headerLeftInset, headerRightInset, descriptionTop, descriptionHeight);
            }
        }

        private void ApplyResolvedFontToShellTexts()
        {
            RectTransform scanRoot = rootRect != null ? rootRect : panelRect;
            if (scanRoot == null)
            {
                return;
            }

            TextMeshProUGUI[] texts = scanRoot.GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int index = 0; index < texts.Length; index++)
            {
                TextMeshProUGUI text = texts[index];
                if (text == null)
                {
                    continue;
                }

                TMP_FontAsset resolvedFont = ResolveFont(text.text);
                if (resolvedFont == null)
                {
                    continue;
                }

                ApplyResolvedFontToText(text, resolvedFont);
            }
        }

        private void BindExistingRows()
        {
            _rows.Clear();
            if (recipeContentRect == null)
            {
                return;
            }

            List<RowRefs> boundRows = new();
            for (int index = 0; index < recipeContentRect.childCount; index++)
            {
                RectTransform rowRect = recipeContentRect.GetChild(index) as RectTransform;
                if (rowRect == null || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                RowRefs row = BindRecipeRow(rowRect, ParseTrailingIndex(rowRect.name));
                if (row != null)
                {
                    boundRows.Add(row);
                }
            }

            boundRows.Sort((left, right) => ParseTrailingIndex(left.rect.name).CompareTo(ParseTrailingIndex(right.rect.name)));
            _rows.AddRange(boundRows);
        }

        private RowRefs BindRecipeRow(RectTransform rowRect, int rowIndex)
        {
            if (rowRect == null)
            {
                return null;
            }

            Image background = rowRect.GetComponent<Image>();
            Image accent = FindDescendantComponent<Image>(rowRect, "Accent");
            Image icon = FindDescendantComponent<Image>(rowRect, "Icon");
            TextMeshProUGUI name = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Name");
            TextMeshProUGUI summary = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Summary");
            Button button = rowRect.GetComponent<Button>();
            if (background == null || accent == null || icon == null || name == null || summary == null || button == null)
            {
                return null;
            }

            int capturedIndex = rowIndex;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectRecipe(capturedIndex));
            EnsureWorkbenchTextReady(name, forceActive: true, minAlpha: 0.98f);
            EnsureWorkbenchTextReady(summary, forceActive: true, minAlpha: 0.9f);
            EnsureRecipeRowCompatibility(rowRect);
            return new RowRefs
            {
                rect = rowRect,
                background = background,
                accent = accent,
                icon = icon,
                name = name,
                summary = summary,
                button = button,
                preferredHeight = GetCurrentHeight(rowRect, rowHeight)
            };
        }

        private void EnsureRecipeViewportCompatibility()
        {
            if (recipeViewportRect == null || recipeContentRect == null)
            {
                return;
            }

            Image viewportImage = SpringDay1UiLayerUtility.EnsureComponent<Image>(recipeViewportRect.gameObject);
            viewportImage.color = RecipeViewportVisibleColor;
            viewportImage.raycastTarget = true;

            Mask mask = SpringDay1UiLayerUtility.EnsureComponent<Mask>(recipeViewportRect.gameObject);
            mask.showMaskGraphic = true;

            _recipeScrollRect = SpringDay1UiLayerUtility.EnsureComponent<ScrollRect>(recipeViewportRect.gameObject);
            _recipeScrollRect.horizontal = false;
            _recipeScrollRect.vertical = true;
            _recipeScrollRect.inertia = false;
            _recipeScrollRect.movementType = ScrollRect.MovementType.Clamped;
            _recipeScrollRect.scrollSensitivity = 18f;
            _recipeScrollRect.viewport = recipeViewportRect;
            _recipeScrollRect.content = recipeContentRect;
            recipeScrollbar = EnsureRecipeScrollbar();
            _recipeScrollRect.verticalScrollbar = recipeScrollbar;
            _recipeScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            _recipeScrollRect.verticalScrollbarSpacing = 2f;

            if (UsesManualRecipeShell())
            {
                if (recipeContentRect.TryGetComponent(out VerticalLayoutGroup manualLayout))
                {
                    manualLayout.enabled = false;
                }

                if (recipeContentRect.TryGetComponent(out ContentSizeFitter manualFitter))
                {
                    manualFitter.enabled = false;
                }

                for (int index = 0; index < recipeContentRect.childCount; index++)
                {
                    if (recipeContentRect.GetChild(index) is RectTransform rowRect && rowRect.name.StartsWith("RecipeRow_"))
                    {
                        EnsureRecipeRowCompatibility(rowRect);
                    }
                }

                return;
            }

            VerticalLayoutGroup recipeLayout = SpringDay1UiLayerUtility.EnsureComponent<VerticalLayoutGroup>(recipeContentRect.gameObject);
            recipeLayout.enabled = true;
            recipeLayout.padding = new RectOffset(0, 0, 0, 0);
            recipeLayout.spacing = rowSpacing;
            recipeLayout.childAlignment = TextAnchor.UpperLeft;
            recipeLayout.childControlWidth = true;
            recipeLayout.childControlHeight = false;
            recipeLayout.childForceExpandWidth = true;
            recipeLayout.childForceExpandHeight = false;

            ContentSizeFitter recipeFitter = SpringDay1UiLayerUtility.EnsureComponent<ContentSizeFitter>(recipeContentRect.gameObject);
            recipeFitter.enabled = true;
            recipeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            recipeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            for (int index = 0; index < recipeContentRect.childCount; index++)
            {
                if (recipeContentRect.GetChild(index) is RectTransform rowRect && rowRect.name.StartsWith("RecipeRow_"))
                {
                    EnsureRecipeRowCompatibility(rowRect);
                }
            }
        }

        private void EnsureRecipeRowCompatibility(RectTransform rowRect)
        {
            if (rowRect == null)
            {
                return;
            }

            bool usesGeneratedLayout = rowRect.GetComponent<HorizontalLayoutGroup>() != null;
            LayoutElement layoutElement = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(rowRect.gameObject);
            layoutElement.preferredHeight = rowHeight;
            layoutElement.minHeight = rowHeight;
            layoutElement.flexibleHeight = 0f;

            Image background = rowRect.GetComponent<Image>();
            RectTransform accentRect = FindDescendantRect(rowRect, "Accent");
            RectTransform iconCardRect = FindDescendantRect(rowRect, "IconCard");
            RectTransform textColumnRect = FindDescendantRect(rowRect, "TextColumn");
            Image accentImage = accentRect != null ? accentRect.GetComponent<Image>() : null;
            Image iconCardImage = iconCardRect != null ? iconCardRect.GetComponent<Image>() : null;
            Image iconImage = FindDescendantComponent<Image>(rowRect, "Icon");

            SanitizeRecipeRowImage(background, preserveAspect: false);
            SanitizeRecipeRowImage(accentImage, preserveAspect: false);
            SanitizeRecipeRowImage(iconCardImage, preserveAspect: false);
            SanitizeRecipeRowImage(iconImage, preserveAspect: true);

            if (usesGeneratedLayout)
            {
                HorizontalLayoutGroup generatedLayout = rowRect.GetComponent<HorizontalLayoutGroup>();
                if (generatedLayout != null)
                {
                    generatedLayout.childAlignment = TextAnchor.UpperLeft;
                    generatedLayout.childControlWidth = true;
                    generatedLayout.childControlHeight = true;
                    generatedLayout.childForceExpandWidth = false;
                    generatedLayout.childForceExpandHeight = false;
                }

                rowRect.anchorMin = new Vector2(0f, 1f);
                rowRect.anchorMax = new Vector2(1f, 1f);
                rowRect.pivot = new Vector2(0.5f, 1f);
                rowRect.sizeDelta = new Vector2(0f, rowHeight);
                NormalizeGeneratedRecipeRowGeometry(rowRect, accentRect, iconCardRect, textColumnRect);
            }
            else if (rowRect.sizeDelta.y <= 0.01f)
            {
                rowRect.sizeDelta = new Vector2(rowRect.sizeDelta.x, rowHeight);
            }

            TextMeshProUGUI name = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Name");
            if (name != null)
            {
                EnsureWorkbenchTextReady(name, forceActive: true, minAlpha: 0.98f);
                RectTransform nameRect = name.rectTransform;
                name.enableAutoSizing = false;
                name.textWrappingMode = TextWrappingModes.Normal;
                name.overflowMode = TextOverflowModes.Overflow;
                name.raycastTarget = false;
                if (usesGeneratedLayout && nameRect.sizeDelta.y <= 0.01f)
                {
                    nameRect.sizeDelta = new Vector2(nameRect.sizeDelta.x, 18f);
                }
            }

            TextMeshProUGUI summary = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Summary");
            if (summary != null)
            {
                EnsureWorkbenchTextReady(summary, forceActive: true, minAlpha: 0.9f);
                RectTransform summaryRect = summary.rectTransform;
                summary.enableAutoSizing = false;
                summary.textWrappingMode = TextWrappingModes.Normal;
                summary.overflowMode = TextOverflowModes.Overflow;
                summary.raycastTarget = false;
                if (usesGeneratedLayout && summaryRect.sizeDelta.y <= 0.01f)
                {
                    summaryRect.sizeDelta = new Vector2(summaryRect.sizeDelta.x, 12f);
                }
            }

            if (!usesGeneratedLayout && name != null && summary != null)
            {
                RectTransform nameRect = name.rectTransform;
                RectTransform summaryRect = summary.rectTransform;
                NormalizeTopStretchRect(nameRect);
                NormalizeTopStretchRect(summaryRect);
                float textWidth = Mathf.Max(72f, GetCurrentWidth(summaryRect, rowRect.rect.width - 66f));
                float nameHeight = Mathf.Max(18f, MeasureTextHeight(name, textWidth, 18f));
                float summaryHeight = Mathf.Max(12f, MeasureTextHeight(summary, textWidth, 12f));
                float nameTop = 8f;
                float summaryTop = nameTop + nameHeight + 2f;
                float dynamicRowHeight = Mathf.Max(rowHeight, summaryTop + summaryHeight + 8f);

                SetTopKeepingHorizontal(nameRect, nameTop, nameHeight);
                SetTopKeepingHorizontal(summaryRect, summaryTop, summaryHeight);

                rowRect.anchorMin = new Vector2(0f, 1f);
                rowRect.anchorMax = new Vector2(1f, 1f);
                rowRect.pivot = new Vector2(0.5f, 1f);
                rowRect.sizeDelta = new Vector2(0f, dynamicRowHeight);
                layoutElement.preferredHeight = dynamicRowHeight;
                layoutElement.minHeight = dynamicRowHeight;

                EnsureManualRecipeRowGeometry(rowRect, accentRect, iconCardRect, textColumnRect, nameRect, summaryRect, dynamicRowHeight);
            }
        }

        private static void SanitizeRecipeRowImage(Image image, bool preserveAspect)
        {
            if (image == null)
            {
                return;
            }

            image.material = null;
            image.type = Image.Type.Simple;
            image.preserveAspect = preserveAspect;
        }

        private static bool HasInvalidRecipeRowGraphicMaterial(Image image)
        {
            if (image == null || image.material == null)
            {
                return false;
            }

            string materialName = image.material.name ?? string.Empty;
            string shaderName = image.material.shader != null ? image.material.shader.name ?? string.Empty : string.Empty;
            string signature = (materialName + "|" + shaderName).ToLowerInvariant();
            return signature.Contains("font material")
                || signature.Contains("textmeshpro")
                || signature.Contains("tmp");
        }

        private static void NormalizeGeneratedRecipeRowGeometry(
            RectTransform rowRect,
            RectTransform accentRect,
            RectTransform iconCardRect,
            RectTransform textColumnRect)
        {
            if (rowRect == null)
            {
                return;
            }

            if (accentRect != null)
            {
                accentRect.anchorMin = new Vector2(0.5f, 0.5f);
                accentRect.anchorMax = new Vector2(0.5f, 0.5f);
                accentRect.pivot = new Vector2(0.5f, 0.5f);
                accentRect.anchoredPosition = Vector2.zero;
                accentRect.sizeDelta = new Vector2(4f, Mathf.Max(32f, rowRect.rect.height - 12f));
                SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(accentRect.gameObject).preferredWidth = 4f;
                SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(accentRect.gameObject).minWidth = 4f;
            }

            if (iconCardRect != null)
            {
                iconCardRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconCardRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconCardRect.pivot = new Vector2(0.5f, 0.5f);
                iconCardRect.anchoredPosition = Vector2.zero;
                iconCardRect.sizeDelta = new Vector2(36f, 36f);
                LayoutElement iconCardLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(iconCardRect.gameObject);
                iconCardLayout.preferredWidth = 36f;
                iconCardLayout.minWidth = 36f;
                iconCardLayout.preferredHeight = 36f;
                iconCardLayout.minHeight = 36f;
            }

            if (textColumnRect != null)
            {
                textColumnRect.anchorMin = new Vector2(0.5f, 0.5f);
                textColumnRect.anchorMax = new Vector2(0.5f, 0.5f);
                textColumnRect.pivot = new Vector2(0.5f, 0.5f);
                textColumnRect.anchoredPosition = Vector2.zero;
                textColumnRect.localScale = Vector3.one;
                textColumnRect.sizeDelta = Vector2.zero;

                LayoutElement textColumnLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(textColumnRect.gameObject);
                textColumnLayout.flexibleWidth = 1f;
                textColumnLayout.minWidth = 78f;

                RectTransform nameRect = FindDescendantRect(textColumnRect, "Name");
                if (nameRect != null)
                {
                    nameRect.anchorMin = new Vector2(0f, 1f);
                    nameRect.anchorMax = new Vector2(1f, 1f);
                    nameRect.pivot = new Vector2(0f, 1f);
                    nameRect.anchoredPosition = Vector2.zero;
                    nameRect.localScale = Vector3.one;
                    nameRect.sizeDelta = new Vector2(0f, 18f);
                }

                RectTransform summaryRect = FindDescendantRect(textColumnRect, "Summary");
                if (summaryRect != null)
                {
                    summaryRect.anchorMin = new Vector2(0f, 1f);
                    summaryRect.anchorMax = new Vector2(1f, 1f);
                    summaryRect.pivot = new Vector2(0f, 1f);
                    summaryRect.anchoredPosition = Vector2.zero;
                    summaryRect.localScale = Vector3.one;
                    summaryRect.sizeDelta = new Vector2(0f, 12f);
                }
            }
        }

        private static void EnsureManualRecipeRowGeometry(
            RectTransform rowRect,
            RectTransform accentRect,
            RectTransform iconCardRect,
            RectTransform textColumnRect,
            RectTransform nameRect,
            RectTransform summaryRect,
            float rowHeightValue)
        {
            if (rowRect == null)
            {
                return;
            }

            if (accentRect != null)
            {
                accentRect.anchorMin = new Vector2(0f, 1f);
                accentRect.anchorMax = new Vector2(0f, 1f);
                accentRect.pivot = new Vector2(0f, 1f);
                accentRect.anchoredPosition = new Vector2(10f, -6f);
                accentRect.sizeDelta = new Vector2(4f, Mathf.Max(32f, rowHeightValue - 12f));
            }

            if (iconCardRect != null)
            {
                iconCardRect.anchorMin = new Vector2(0f, 1f);
                iconCardRect.anchorMax = new Vector2(0f, 1f);
                iconCardRect.pivot = new Vector2(0f, 1f);
                iconCardRect.anchoredPosition = new Vector2(22f, -6f);
                iconCardRect.sizeDelta = new Vector2(36f, 36f);
            }

            float textLeft = 68f;
            float textRight = 10f;
            float contentTop = 8f;
            float contentBottom = 8f;

            if (textColumnRect != null)
            {
                textColumnRect.anchorMin = new Vector2(0f, 1f);
                textColumnRect.anchorMax = new Vector2(1f, 1f);
                textColumnRect.pivot = new Vector2(0f, 1f);
                textColumnRect.offsetMin = new Vector2(textLeft, -rowHeightValue + contentBottom);
                textColumnRect.offsetMax = new Vector2(-textRight, -contentTop);
            }

            if (nameRect != null)
            {
                nameRect.anchorMin = new Vector2(0f, 1f);
                nameRect.anchorMax = new Vector2(1f, 1f);
                nameRect.pivot = new Vector2(0f, 1f);
            }

            if (summaryRect != null)
            {
                summaryRect.anchorMin = new Vector2(0f, 1f);
                summaryRect.anchorMax = new Vector2(1f, 1f);
                summaryRect.pivot = new Vector2(0f, 1f);
            }
        }

        private static void NormalizeTopStretchRect(RectTransform rect)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
        }

        private static void SetTopStretchRect(RectTransform rect, float left, float right, float top, float height)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.offsetMin = new Vector2(left, -top - height);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private void EnsureMaterialsViewportCompatibility()
        {
            if (_detailColumnRect == null || selectedMaterialsText == null)
            {
                return;
            }

            if (UsesPrefabDetailShell())
            {
                RectTransform prefabMaterialsTextRect = selectedMaterialsText.rectTransform;
                RectTransform prefabCurrentParent = prefabMaterialsTextRect.parent as RectTransform;
                bool prefabTextWasDirectChild = prefabCurrentParent == _detailColumnRect;
                int prefabTextSiblingIndex = prefabMaterialsTextRect.GetSiblingIndex();
                _materialsTextBaselineHeight = Mathf.Max(20f, _materialsTextBaselineHeight);

                bool prefabNeedsViewport = materialsViewportRect == null
                    || materialsViewportRect == prefabMaterialsTextRect
                    || materialsViewportRect == _detailColumnRect
                    || materialsViewportRect.parent != _detailColumnRect;
                if (prefabNeedsViewport)
                {
                    materialsViewportRect = FindDirectChildRect(_detailColumnRect, "MaterialsViewport");
                    if (materialsViewportRect == null)
                    {
                        materialsViewportRect = CreateRect(_detailColumnRect, "MaterialsViewport");
                        if (prefabTextWasDirectChild)
                        {
                            CopyRectGeometry(prefabMaterialsTextRect, materialsViewportRect);
                            materialsViewportRect.SetSiblingIndex(prefabTextSiblingIndex);
                        }
                    }
                }

                Image prefabViewportImage = SpringDay1UiLayerUtility.EnsureComponent<Image>(materialsViewportRect.gameObject);
                prefabViewportImage.color = new Color(0f, 0f, 0f, 0.001f);
                prefabViewportImage.raycastTarget = true;

                Mask prefabViewportMask = SpringDay1UiLayerUtility.EnsureComponent<Mask>(materialsViewportRect.gameObject);
                prefabViewportMask.showMaskGraphic = false;

                _materialsScrollRect = SpringDay1UiLayerUtility.EnsureComponent<ScrollRect>(materialsViewportRect.gameObject);
                _materialsScrollRect.horizontal = false;
                _materialsScrollRect.vertical = true;
                _materialsScrollRect.inertia = false;
                _materialsScrollRect.movementType = ScrollRect.MovementType.Clamped;
                _materialsScrollRect.scrollSensitivity = 18f;

                bool prefabNeedsContent = materialsContentRect == null
                    || materialsContentRect == prefabMaterialsTextRect
                    || materialsContentRect == _detailColumnRect
                    || materialsContentRect == materialsViewportRect
                    || materialsContentRect.parent != materialsViewportRect;
                if (prefabNeedsContent)
                {
                    materialsContentRect = FindDirectChildRect(materialsViewportRect, "Content");
                    if (materialsContentRect == null)
                    {
                        materialsContentRect = CreateRect(materialsViewportRect, "Content");
                    }
                }

                materialsContentRect.anchorMin = new Vector2(0f, 1f);
                materialsContentRect.anchorMax = new Vector2(1f, 1f);
                materialsContentRect.pivot = new Vector2(0.5f, 1f);
                materialsContentRect.anchoredPosition = Vector2.zero;
                materialsContentRect.sizeDelta = new Vector2(0f, Mathf.Max(20f, GetCurrentHeight(materialsContentRect, 20f)));

                if (prefabMaterialsTextRect.parent != materialsContentRect)
                {
                    prefabMaterialsTextRect.SetParent(materialsContentRect, false);
                }

                prefabMaterialsTextRect.anchorMin = new Vector2(0f, 1f);
                prefabMaterialsTextRect.anchorMax = new Vector2(1f, 1f);
                prefabMaterialsTextRect.pivot = new Vector2(0f, 1f);
                prefabMaterialsTextRect.anchoredPosition = Vector2.zero;
                prefabMaterialsTextRect.sizeDelta = new Vector2(0f, 20f);
                selectedMaterialsText.textWrappingMode = TextWrappingModes.Normal;
                selectedMaterialsText.overflowMode = TextOverflowModes.Overflow;

                LayoutElement prefabMaterialsLabelLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(selectedMaterialsText.gameObject);
                prefabMaterialsLabelLayout.minHeight = 20f;
                prefabMaterialsLabelLayout.preferredHeight = 20f;
                prefabMaterialsLabelLayout.flexibleHeight = 0f;

                _materialsScrollRect.viewport = materialsViewportRect;
                _materialsScrollRect.content = materialsContentRect;
                RefreshMaterialsContentGeometry();
                EnsurePrefabDetailTextChain();
                return;
            }

            RectTransform materialsTextRect = selectedMaterialsText.rectTransform;
            RectTransform currentParent = materialsTextRect.parent as RectTransform;
            bool textWasDirectChild = currentParent == _detailColumnRect;
            int textSiblingIndex = materialsTextRect.GetSiblingIndex();
            _materialsTextBaselineHeight = Mathf.Max(20f, _materialsTextBaselineHeight);

            bool needsViewport = materialsViewportRect == null
                || materialsViewportRect == _detailColumnRect
                || materialsViewportRect == currentParent;

            if (needsViewport)
            {
                materialsViewportRect = FindDirectChildRect(_detailColumnRect, "MaterialsViewport");
                if (materialsViewportRect == null)
                {
                    materialsViewportRect = CreateRect(_detailColumnRect, "MaterialsViewport");
                    if (textWasDirectChild)
                    {
                        CopyRectGeometry(materialsTextRect, materialsViewportRect);
                        materialsViewportRect.SetSiblingIndex(textSiblingIndex);
                    }
                }
            }

            Image viewportImage = SpringDay1UiLayerUtility.EnsureComponent<Image>(materialsViewportRect.gameObject);
            viewportImage.color = new Color(0f, 0f, 0f, 0.001f);
            viewportImage.raycastTarget = true;

            Mask viewportMask = SpringDay1UiLayerUtility.EnsureComponent<Mask>(materialsViewportRect.gameObject);
            viewportMask.showMaskGraphic = false;

            _materialsScrollRect = SpringDay1UiLayerUtility.EnsureComponent<ScrollRect>(materialsViewportRect.gameObject);
            _materialsScrollRect.horizontal = false;
            _materialsScrollRect.vertical = true;
            _materialsScrollRect.inertia = false;
            _materialsScrollRect.movementType = ScrollRect.MovementType.Clamped;
            _materialsScrollRect.scrollSensitivity = 18f;

            bool needsContent = materialsContentRect == null
                || materialsContentRect == _detailColumnRect
                || materialsContentRect == materialsViewportRect
                || materialsContentRect == currentParent;

            if (needsContent)
            {
                materialsContentRect = FindDirectChildRect(materialsViewportRect, "Content");
                if (materialsContentRect == null)
                {
                    materialsContentRect = CreateRect(materialsViewportRect, "Content");
                }
            }

            materialsContentRect.anchorMin = new Vector2(0f, 1f);
            materialsContentRect.anchorMax = new Vector2(1f, 1f);
            materialsContentRect.pivot = new Vector2(0.5f, 1f);
            materialsContentRect.anchoredPosition = Vector2.zero;
            materialsContentRect.sizeDelta = new Vector2(0f, Mathf.Max(20f, GetCurrentHeight(materialsContentRect, 20f)));

            if (materialsContentRect.TryGetComponent(out VerticalLayoutGroup materialsLayout))
            {
                materialsLayout.enabled = false;
            }

            if (materialsContentRect.TryGetComponent(out ContentSizeFitter materialsFitter))
            {
                materialsFitter.enabled = false;
            }

            if (materialsTextRect.parent != materialsContentRect)
            {
                materialsTextRect.SetParent(materialsContentRect, false);
            }

            materialsTextRect.anchorMin = new Vector2(0f, 1f);
            materialsTextRect.anchorMax = new Vector2(1f, 1f);
            materialsTextRect.pivot = new Vector2(0f, 1f);
            materialsTextRect.anchoredPosition = Vector2.zero;
            materialsTextRect.sizeDelta = new Vector2(0f, 20f);
            selectedMaterialsText.textWrappingMode = TextWrappingModes.Normal;
            selectedMaterialsText.overflowMode = TextOverflowModes.Overflow;
            LayoutElement materialsLabelLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(selectedMaterialsText.gameObject);
            materialsLabelLayout.minHeight = 20f;
            materialsLabelLayout.preferredHeight = 20f;
            materialsLabelLayout.flexibleHeight = 0f;

            _materialsScrollRect.viewport = materialsViewportRect;
            _materialsScrollRect.content = materialsContentRect;
            AdjustLegacyDetailLayoutToFitCurrentContent();
            RefreshMaterialsContentGeometry();
        }

        private void AdjustLegacyDetailLayoutToFitCurrentContent()
        {
            if (_detailColumnRect == null || selectedNameText == null || selectedDescriptionText == null || materialsViewportRect == null)
            {
                return;
            }

            RectTransform nameRect = selectedNameText.rectTransform;
            RectTransform descriptionRect = selectedDescriptionText.rectTransform;
            RectTransform craftButtonRect = craftButton != null ? craftButton.transform as RectTransform : null;
            float contentWidth = Mathf.Max(120f, GetCurrentWidth(descriptionRect, _detailColumnRect.rect.width - 8f));

            float nameTop = _nameBaseline.TopOr(GetTopInParent(nameRect));
            float nameHeight = MeasureTextHeight(selectedNameText, contentWidth, _nameBaseline.HeightOr(18f));
            SetTopFlowRect(nameRect, nameTop, nameHeight);

            float descriptionTop = nameTop + nameHeight + 6f;
            float reservedBottomTop = GetDetailContentFloorTop();
            float materialsTitleHeight = _materialsTitleBaseline.HeightOr(GetCurrentHeight(_materialsTitleRect, 14f));
            float minimumViewportHeight = Mathf.Max(56f, _materialsViewportBaseline.HeightOr(56f));
            float availableForDescription = reservedBottomTop - descriptionTop - 8f - materialsTitleHeight - 6f - minimumViewportHeight - 8f;
            float measuredDescriptionHeight = MeasureTextHeight(selectedDescriptionText, contentWidth, _descriptionBaseline.HeightOr(22f));
            float descriptionHeight = Mathf.Max(22f, Mathf.Min(measuredDescriptionHeight, availableForDescription));
            SetTopFlowRect(descriptionRect, descriptionTop, descriptionHeight);

            float materialsTitleTop = descriptionTop + descriptionHeight + 8f;
            SetTopFlowRect(_materialsTitleRect, materialsTitleTop, materialsTitleHeight);

            float viewportTop = materialsTitleTop + materialsTitleHeight + 6f;
            float quantityTop = _quantityControlsBaseline.TopOr(GetTopInParent(_quantityControlsRect));
            float viewportFloor = Mathf.Min(reservedBottomTop - 8f, quantityTop - 8f);
            float viewportHeight = Mathf.Max(minimumViewportHeight, viewportFloor - viewportTop);
            SetTopFlowRect(materialsViewportRect, viewportTop, viewportHeight);

            if (_quantityControlsRect != null)
            {
                float quantityHeight = _quantityControlsBaseline.HeightOr(GetCurrentHeight(_quantityControlsRect, 38f));
                SetTopFlowRect(_quantityControlsRect, viewportTop + viewportHeight + 8f, quantityHeight);
            }

            if (stageHintText != null && stageHintText.gameObject.activeSelf)
            {
                float stageHintTop = GetTopInParent(_quantityControlsRect) + GetCurrentHeight(_quantityControlsRect, 38f) + 4f;
                float stageHintHeight = MeasureTextHeight(stageHintText, contentWidth, _stageHintBaseline.HeightOr(18f));
                if (craftButtonRect != null)
                {
                    stageHintHeight = Mathf.Min(stageHintHeight, Mathf.Max(18f, GetTopInParent(craftButtonRect) - stageHintTop - 6f));
                }

                SetTopFlowRect(stageHintText.rectTransform, stageHintTop, stageHintHeight);
            }
        }

        private void EnsureFloatingProgressCompatibility()
        {
            if (floatingProgressRoot != null && floatingProgressIcon != null && floatingProgressFillImage != null && floatingProgressLabel != null)
            {
                ApplyFloatingProgressVisualBaseline();
                EnsureFloatingCardRegistry();
                return;
            }

            if (floatingProgressRoot == null)
            {
                floatingProgressRoot = FindDirectChildRect(rootRect, "FloatingProgressRoot");
            }

            if (floatingProgressRoot == null)
            {
                floatingProgressRoot = CreateRect(transform, "FloatingProgressRoot");
                floatingProgressRoot.anchorMin = new Vector2(0.5f, 0.5f);
                floatingProgressRoot.anchorMax = new Vector2(0.5f, 0.5f);
                floatingProgressRoot.pivot = new Vector2(0.5f, 0f);
                floatingProgressRoot.sizeDelta = new Vector2(54f, 46f);
                floatingProgressRoot.gameObject.SetActive(false);
                Image floatingCard = floatingProgressRoot.gameObject.AddComponent<Image>();
                floatingCard.color = new Color(0.07f, 0.09f, 0.13f, 0.92f);
                floatingCard.raycastTarget = false;
                ApplyOutline(floatingProgressRoot.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));
                ApplyShadow(floatingProgressRoot.gameObject.AddComponent<Shadow>(), new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, -3f));

                RectTransform floatingIconCard = CreateRect(floatingProgressRoot, "IconCard");
                floatingIconCard.anchorMin = new Vector2(0.5f, 1f);
                floatingIconCard.anchorMax = new Vector2(0.5f, 1f);
                floatingIconCard.pivot = new Vector2(0.5f, 1f);
                floatingIconCard.anchoredPosition = new Vector2(0f, -5f);
                floatingIconCard.sizeDelta = new Vector2(30f, 30f);
                floatingIconCard.gameObject.AddComponent<Image>().color = new Color(0.18f, 0.22f, 0.31f, 0.96f);
                floatingProgressIcon = CreateIcon(floatingIconCard, "Icon", 18f);
                Center(floatingProgressIcon.rectTransform);

                RectTransform fillBackground = CreateRect(floatingProgressRoot, "ProgressBackground");
                fillBackground.anchorMin = new Vector2(0f, 0f);
                fillBackground.anchorMax = new Vector2(1f, 0f);
                fillBackground.offsetMin = new Vector2(4f, 6f);
                fillBackground.offsetMax = new Vector2(-4f, 14f);
                fillBackground.gameObject.AddComponent<Image>().color = new Color(0.04f, 0.05f, 0.08f, 0.94f);

                RectTransform fill = CreateRect(fillBackground, "ProgressFill");
                Stretch(fill, new Vector2(1f, 1f), new Vector2(-1f, -1f));
                floatingProgressFillImage = fill.gameObject.AddComponent<Image>();
                EnsureProgressFillGraphic(floatingProgressFillImage);

                floatingProgressLabel = CreateText(floatingProgressRoot, "Label", string.Empty, 8f, Color.white, TextAlignmentOptions.Center);
                floatingProgressLabel.rectTransform.anchorMin = new Vector2(0f, 0f);
                floatingProgressLabel.rectTransform.anchorMax = new Vector2(1f, 0f);
                floatingProgressLabel.rectTransform.offsetMin = new Vector2(2f, 16f);
                floatingProgressLabel.rectTransform.offsetMax = new Vector2(-2f, 28f);
                floatingProgressLabel.raycastTarget = false;
                ApplyFloatingProgressVisualBaseline();
                EnsureFloatingCardRegistry();
                return;
            }

            if (floatingProgressIcon == null)
            {
                floatingProgressIcon = FindDescendantComponent<Image>(floatingProgressRoot, "Icon");
            }

            if (floatingProgressFillImage == null)
            {
                floatingProgressFillImage = FindDescendantComponent<Image>(floatingProgressRoot, "ProgressFill");
            }
            EnsureProgressFillGraphic(floatingProgressFillImage);

            if (floatingProgressLabel == null)
            {
                floatingProgressLabel = FindDescendantComponent<TextMeshProUGUI>(floatingProgressRoot, "Label");
            }
            EnsureProgressLabelForeground(floatingProgressLabel);

            ApplyFloatingProgressVisualBaseline();
            EnsureFloatingCardRegistry();
        }

        private void EnsureQueueOverflowToastCompatibility(RectTransform preferredParent = null)
        {
            RectTransform parent = preferredParent;
            if (parent == null && panelRect != null)
            {
                parent = FindDirectChildRect(panelRect, "Surface") ?? panelRect;
            }

            if (panelRect == null || parent == null)
            {
                return;
            }

            if (queueToastRect == null)
            {
                queueToastRect = FindDescendantRect(panelRect, "QueueOverflowToast");
            }

            if (queueToastRect == null)
            {
                queueToastRect = CreateRect(parent, "QueueOverflowToast");
            }

            if (queueToastRect.parent != parent)
            {
                queueToastRect.SetParent(parent, false);
            }

            queueToastRect.anchorMin = new Vector2(0.5f, 1f);
            queueToastRect.anchorMax = new Vector2(0.5f, 1f);
            queueToastRect.pivot = new Vector2(0.5f, 1f);
            queueToastRect.anchoredPosition = new Vector2(0f, -8f);
            float toastWidth = panelRect.rect.width > 0.01f
                ? Mathf.Clamp(panelRect.rect.width - 46f, 216f, 284f)
                : 252f;
            queueToastRect.sizeDelta = new Vector2(toastWidth, 24f);

            Image toastBackground = SpringDay1UiLayerUtility.EnsureComponent<Image>(queueToastRect.gameObject);
            toastBackground.color = new Color(0.92f, 0.76f, 0.34f, 0.96f);
            toastBackground.raycastTarget = false;
            ApplyOutline(SpringDay1UiLayerUtility.EnsureComponent<Outline>(queueToastRect.gameObject), new Color(0f, 0f, 0f, 0.22f), new Vector2(1f, -1f));

            queueToastGroup = SpringDay1UiLayerUtility.EnsureComponent<CanvasGroup>(queueToastRect.gameObject);
            queueToastGroup.interactable = false;
            queueToastGroup.blocksRaycasts = false;

            if (queueToastText == null)
            {
                queueToastText = FindDescendantComponent<TextMeshProUGUI>(queueToastRect, "Label");
            }

            if (queueToastText == null)
            {
                queueToastText = CreateText(queueToastRect, "Label", string.Empty, 10f, new Color(0.16f, 0.11f, 0.05f, 1f), TextAlignmentOptions.Center, false, true);
            }

            queueToastText.rectTransform.anchorMin = Vector2.zero;
            queueToastText.rectTransform.anchorMax = Vector2.one;
            queueToastText.rectTransform.offsetMin = new Vector2(10f, 1f);
            queueToastText.rectTransform.offsetMax = new Vector2(-10f, -1f);
            queueToastText.alignment = TextAlignmentOptions.Center;
            queueToastText.fontSize = 10f;
            queueToastText.color = new Color(0.16f, 0.11f, 0.05f, 1f);
            queueToastText.textWrappingMode = TextWrappingModes.NoWrap;
            queueToastText.overflowMode = TextOverflowModes.Ellipsis;
            queueToastText.raycastTarget = false;
            EnsureWorkbenchTextContent(queueToastText, queueToastText.text, 0.98f);

            queueToastRect.SetAsLastSibling();
            HideQueueOverflowToastImmediate();
        }

        private void TryShowQueueOverflowToast()
        {
            if (!_isVisible || GetFloatingProgressSourceCount() <= MaxFloatingProgressCards)
            {
                return;
            }

            ShowQueueOverflowToast(GetQueueOverflowToastMessage());
        }

        private void ShowQueueOverflowToast(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            EnsureQueueOverflowToastCompatibility();
            if (queueToastRect == null || queueToastGroup == null || queueToastText == null)
            {
                return;
            }

            StopQueueOverflowToastRoutine();
            EnsureWorkbenchTextContent(queueToastText, message, 0.98f);
            _queueToastRoutine = StartCoroutine(PlayQueueOverflowToastRoutine());
        }

        private IEnumerator PlayQueueOverflowToastRoutine()
        {
            SetQueueOverflowToastAlpha(0f, true);
            yield return FadeQueueOverflowToast(0f, 1f, QueueOverflowToastFadeDuration);
            yield return new WaitForSecondsRealtime(QueueOverflowToastHoldDuration);
            yield return FadeQueueOverflowToast(1f, 0f, QueueOverflowToastFadeDuration);
            HideQueueOverflowToastImmediate();
            _queueToastRoutine = null;
        }

        private IEnumerator FadeQueueOverflowToast(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                SetQueueOverflowToastAlpha(to, to > 0.001f);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetQueueOverflowToastAlpha(Mathf.Lerp(from, to, t), true);
                yield return null;
            }

            SetQueueOverflowToastAlpha(to, to > 0.001f);
        }

        private void SetQueueOverflowToastAlpha(float alpha, bool visible)
        {
            if (queueToastRect != null)
            {
                queueToastRect.gameObject.SetActive(visible || alpha > 0.001f);
            }

            if (queueToastGroup != null)
            {
                queueToastGroup.alpha = Mathf.Clamp01(alpha);
                queueToastGroup.interactable = false;
                queueToastGroup.blocksRaycasts = false;
            }
        }

        private void HideQueueOverflowToastImmediate()
        {
            if (queueToastText != null)
            {
                queueToastText.text = string.Empty;
            }

            SetQueueOverflowToastAlpha(0f, false);
        }

        private void StopQueueOverflowToastRoutine()
        {
            if (_queueToastRoutine == null)
            {
                return;
            }

            StopCoroutine(_queueToastRoutine);
            _queueToastRoutine = null;
        }

        private int GetFloatingProgressSourceCount()
        {
            int count = 0;
            for (int index = 0; index < _queueEntries.Count; index++)
            {
                WorkbenchQueueEntry entry = _queueEntries[index];
                if (entry != null && (entry.totalCount > 0 || entry.readyCount > 0))
                {
                    count++;
                }
            }

            return count;
        }

        private static string GetQueueOverflowToastMessage()
        {
            return $"队列超过 {MaxFloatingProgressCards} 项，其余制作仍会继续排队";
        }

        private void ApplyFloatingProgressVisualBaseline()
        {
            EnsureFloatingCardRegistry();
            if (_floatingCards.Count > 0)
            {
                ApplyFloatingProgressVisualBaseline(_floatingCards[0]);
            }
        }

        private void ApplyFloatingProgressVisualBaseline(FloatingProgressCardRefs card)
        {
            if (card?.root == null)
            {
                return;
            }

            card.root.sizeDelta = new Vector2(72f, 78f);

            if (card.icon != null)
            {
                RectTransform iconCardRect = card.icon.rectTransform.parent as RectTransform;
                if (iconCardRect != null)
                {
                    iconCardRect.anchorMin = new Vector2(0f, 0f);
                    iconCardRect.anchorMax = new Vector2(1f, 1f);
                    iconCardRect.pivot = new Vector2(0.5f, 0.5f);
                    iconCardRect.offsetMin = new Vector2(6f, 20f);
                    iconCardRect.offsetMax = new Vector2(-6f, -8f);
                }

                card.icon.rectTransform.sizeDelta = new Vector2(46f, 46f);
                card.icon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            }

            RectTransform floatingProgressBackground = FindDescendantRect(card.root, "ProgressBackground");
            if (floatingProgressBackground != null)
            {
                floatingProgressBackground.anchorMin = new Vector2(0f, 0f);
                floatingProgressBackground.anchorMax = new Vector2(1f, 0f);
                floatingProgressBackground.pivot = new Vector2(0.5f, 0f);
                floatingProgressBackground.offsetMin = new Vector2(7f, 6f);
                floatingProgressBackground.offsetMax = new Vector2(-7f, 20f);
            }

            if (card.label != null)
            {
                card.label.alignment = TextAlignmentOptions.Center;
                card.label.rectTransform.anchorMin = new Vector2(0f, 0f);
                card.label.rectTransform.anchorMax = new Vector2(1f, 0f);
                card.label.rectTransform.pivot = new Vector2(0.5f, 0f);
                card.label.rectTransform.offsetMin = new Vector2(8f, 4f);
                card.label.rectTransform.offsetMax = new Vector2(-8f, 22f);
            }
        }

        private void EnsureFloatingCardRegistry()
        {
            if (_floatingCards.Count == 0 && floatingProgressRoot != null && floatingProgressIcon != null && floatingProgressFillImage != null && floatingProgressLabel != null)
            {
                _floatingCards.Add(new FloatingProgressCardRefs
                {
                    root = floatingProgressRoot,
                    icon = floatingProgressIcon,
                    fill = floatingProgressFillImage,
                    label = floatingProgressLabel
                });
            }
        }

        private FloatingProgressCardRefs CreateFloatingProgressCard(int index)
        {
            RectTransform root = CreateRect(transform, $"FloatingProgressCard_{index}");
            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0f);
            root.gameObject.SetActive(false);
            Image floatingCard = root.gameObject.AddComponent<Image>();
            floatingCard.color = new Color(0.07f, 0.09f, 0.13f, 0.92f);
            floatingCard.raycastTarget = false;
            ApplyOutline(root.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));
            ApplyShadow(root.gameObject.AddComponent<Shadow>(), new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, -3f));

            RectTransform iconCard = CreateRect(root, "IconCard");
            iconCard.gameObject.AddComponent<Image>().color = new Color(0.18f, 0.22f, 0.31f, 0.96f);
            Image icon = CreateIcon(iconCard, "Icon", 18f);
            Center(icon.rectTransform);

            RectTransform fillBackground = CreateRect(root, "ProgressBackground");
            fillBackground.gameObject.AddComponent<Image>().color = new Color(0.04f, 0.05f, 0.08f, 0.94f);
            RectTransform fill = CreateRect(fillBackground, "ProgressFill");
            Stretch(fill, new Vector2(1f, 1f), new Vector2(-1f, -1f));
            Image fillImage = fill.gameObject.AddComponent<Image>();
            EnsureProgressFillGraphic(fillImage);

            TextMeshProUGUI label = CreateText(root, "Label", string.Empty, 8f, Color.white, TextAlignmentOptions.Center);
            label.raycastTarget = false;
            EnsureProgressLabelForeground(label);

            FloatingProgressCardRefs card = new FloatingProgressCardRefs
            {
                root = root,
                icon = icon,
                fill = fillImage,
                label = label
            };

            ApplyFloatingProgressVisualBaseline(card);
            _floatingCards.Add(card);
            return card;
        }

        private static void EnsureProgressLabelForeground(TextMeshProUGUI label)
        {
            if (label == null)
            {
                return;
            }

            label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Overflow;
            label.rectTransform.SetAsLastSibling();
            Shadow labelShadow = SpringDay1UiLayerUtility.EnsureComponent<Shadow>(label.gameObject);
            ApplyShadow(labelShadow, new Color(0f, 0f, 0f, 0.55f), new Vector2(0f, -1f));
        }

        private void EnsureProgressLabelBinding()
        {
            if (progressRoot == null)
            {
                return;
            }

            TextMeshProUGUI legacyLabel = progressLabelText;
            TextMeshProUGUI inlineLabel = null;
            for (int index = 0; index < progressRoot.childCount; index++)
            {
                Transform child = progressRoot.GetChild(index);
                if (child == null)
                {
                    continue;
                }

                TextMeshProUGUI candidate = child.GetComponent<TextMeshProUGUI>();
                if (candidate == null)
                {
                    continue;
                }

                inlineLabel = candidate;
                break;
            }

            if (inlineLabel == null)
            {
                inlineLabel = CreateText(progressRoot, "ProgressInlineLabel", "进度  0/0", 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.Center, true, true);
            }

            progressLabelText = inlineLabel;
            progressLabelText.rectTransform.anchorMin = Vector2.zero;
            progressLabelText.rectTransform.anchorMax = Vector2.one;
            progressLabelText.rectTransform.offsetMin = new Vector2(6f, 1f);
            progressLabelText.rectTransform.offsetMax = new Vector2(-6f, -1f);
            EnsureProgressLabelForeground(progressLabelText);

            if (legacyLabel != null && legacyLabel != progressLabelText && !legacyLabel.rectTransform.IsChildOf(progressRoot))
            {
                legacyLabel.gameObject.SetActive(false);
            }
        }

        private static void EnsureProgressFillGraphic(Image fillImage)
        {
            if (fillImage == null)
            {
                return;
            }

            fillImage.sprite = GetRuntimeProgressFillSprite();
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
            fillImage.fillAmount = Mathf.Clamp01(fillImage.fillAmount);
            fillImage.color = ActiveProgressFillColor;
            fillImage.preserveAspect = false;
            fillImage.raycastTarget = false;
        }

        private static Sprite GetRuntimeProgressFillSprite()
        {
            if (_runtimeProgressFillSprite != null)
            {
                return _runtimeProgressFillSprite;
            }

            Texture2D texture = Texture2D.whiteTexture;
            _runtimeProgressFillSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            _runtimeProgressFillSprite.name = "WorkbenchRuntimeProgressFillSprite";
            return _runtimeProgressFillSprite;
        }

        private void RefreshCompatibilityLayout(bool forceLayout = true)
        {
            if (_detailColumnRect == null || _materialsTitleRect == null || _quantityControlsRect == null
                || selectedNameText == null || selectedDescriptionText == null || stageHintText == null || progressLabelText == null
                || progressRoot == null || craftButton == null || materialsViewportRect == null)
            {
                return;
            }

            if (UsesPrefabDetailShell())
            {
                if (forceLayout)
                {
                    RefreshPrefabDetailShellGeometry();
                }

                return;
            }

            if (!UsesLegacyDetailManualLayout())
            {
                return;
            }

            if (!forceLayout)
            {
                return;
            }

            RefreshMaterialsContentGeometry();
            selectedNameText.enableAutoSizing = false;
            selectedNameText.textWrappingMode = TextWrappingModes.Normal;
            selectedNameText.overflowMode = TextOverflowModes.Overflow;
            selectedDescriptionText.enableAutoSizing = false;
            selectedDescriptionText.textWrappingMode = TextWrappingModes.Normal;
            selectedDescriptionText.overflowMode = TextOverflowModes.Truncate;
            bool hasStageHint = !string.IsNullOrWhiteSpace(stageHintText.text);
            RestoreBaselineRect(selectedNameText.rectTransform, _nameBaseline);
            RestoreBaselineRect(selectedDescriptionText.rectTransform, _descriptionBaseline);
            RestoreBaselineRect(_materialsTitleRect, _materialsTitleBaseline);
            RestoreBaselineRect(materialsViewportRect, _materialsViewportBaseline);
            RestoreBaselineRect(progressLabelText.rectTransform, _progressLabelBaseline);
            RestoreBaselineRect(_quantityControlsRect, _quantityControlsBaseline);
            RestoreBaselineRect(progressRoot, _progressBaseline);
            RestoreBaselineRect(craftButton.transform as RectTransform, _craftButtonBaseline);
            if (stageHintText != null)
            {
                stageHintText.gameObject.SetActive(hasStageHint);
                if (hasStageHint)
                {
                    RestoreBaselineRect(stageHintText.rectTransform, _stageHintBaseline);
                }
            }

            RefreshMaterialsContentGeometry();
            AdjustLegacyDetailLayoutToFitCurrentContent();
        }

        private void RefreshPrefabDetailShellGeometry()
        {
            if (_detailColumnRect == null || !UsesPrefabDetailShell())
            {
                return;
            }

            EnsurePrefabDetailTextChain();

            selectedNameText.enableAutoSizing = false;
            selectedNameText.textWrappingMode = TextWrappingModes.Normal;
            selectedNameText.overflowMode = TextOverflowModes.Overflow;
            selectedDescriptionText.enableAutoSizing = false;
            selectedDescriptionText.textWrappingMode = TextWrappingModes.Normal;
            selectedDescriptionText.overflowMode = TextOverflowModes.Truncate;
            selectedMaterialsText.enableAutoSizing = false;
            selectedMaterialsText.textWrappingMode = TextWrappingModes.Normal;

            if (_iconCardRect != null)
            {
                RestoreBaselineRect(_iconCardRect, _iconCardBaseline);
            }

            if (_quantityTitleRect != null && _quantityTitleRect.parent == _detailColumnRect)
            {
                RestoreBaselineRect(_quantityTitleRect, _quantityTitleBaseline);
            }

            if (_quantityControlsRect != null && _quantityControlsRect.parent == _detailColumnRect)
            {
                RestoreBaselineRect(_quantityControlsRect, _quantityControlsBaseline);
            }

            if (progressLabelText != null && progressLabelText.rectTransform.parent == _detailColumnRect)
            {
                RestoreBaselineRect(progressLabelText.rectTransform, _progressLabelBaseline);
            }

            if (progressRoot != null && progressRoot.parent == _detailColumnRect)
            {
                RestoreBaselineRect(progressRoot, _progressBaseline);
            }

            RectTransform craftButtonRect = craftButton != null ? craftButton.transform as RectTransform : null;
            if (craftButtonRect != null && craftButtonRect.parent == _detailColumnRect)
            {
                RestoreBaselineRect(craftButtonRect, _craftButtonBaseline);
            }

            bool hasStageHint = stageHintText != null && !string.IsNullOrWhiteSpace(stageHintText.text);
            if (stageHintText != null)
            {
                stageHintText.gameObject.SetActive(hasStageHint);
                if (hasStageHint && stageHintText.rectTransform.parent == _detailColumnRect)
                {
                    RestoreBaselineRect(stageHintText.rectTransform, _stageHintBaseline);
                }
            }

            float headerLeftInset = _iconCardRect != null
                ? Mathf.Max(54f, GetCurrentWidth(_iconCardRect, 40f) + 14f)
                : 54f;
            const float headerRightInset = 10f;
            float detailWidth = Mathf.Max(96f, _detailColumnRect.rect.width - headerLeftInset - headerRightInset);

            float nameTop = _nameBaseline.TopOr(GetTopInParent(selectedNameText.rectTransform));
            float nameHeight = Mathf.Max(_nameBaseline.HeightOr(20f), MeasureTextHeight(selectedNameText, detailWidth, 20f));
            SetTopStretchRect(selectedNameText.rectTransform, headerLeftInset, headerRightInset, nameTop, nameHeight);

            float descriptionTop = nameTop + nameHeight + 4f;
            float materialsTitleHeight = _materialsTitleBaseline.HeightOr(GetCurrentHeight(_materialsTitleRect, 14f));
            float materialsBottomLimit = _quantityTitleRect != null && _quantityTitleRect.parent == _detailColumnRect
                ? _quantityTitleBaseline.TopOr(GetTopInParent(_quantityTitleRect))
                : GetDetailContentFloorTop();
            float minimumMaterialsHeight = Mathf.Max(28f, Mathf.Min(44f, _materialsViewportBaseline.HeightOr(40f)));
            float availableForDescription = Mathf.Max(
                24f,
                materialsBottomLimit - descriptionTop - 8f - materialsTitleHeight - 6f - minimumMaterialsHeight - 8f);
            float measuredDescriptionHeight = MeasureTextHeight(selectedDescriptionText, detailWidth, _descriptionBaseline.HeightOr(24f));
            float descriptionHeight = Mathf.Clamp(measuredDescriptionHeight, 24f, availableForDescription);
            SetTopStretchRect(selectedDescriptionText.rectTransform, headerLeftInset, headerRightInset, descriptionTop, descriptionHeight);

            float materialsTitleTop = descriptionTop + descriptionHeight + 8f;
            SetTopFlowRect(_materialsTitleRect, materialsTitleTop, materialsTitleHeight);

            float materialsTop = materialsTitleTop + materialsTitleHeight + 6f;
            float materialsWidth = Mathf.Max(120f, _detailColumnRect.rect.width - 20f);
            float fullMaterialsHeight = MeasureTextHeight(selectedMaterialsText, materialsWidth, _materialsTextBaselineHeight > 0.01f ? _materialsTextBaselineHeight : 20f);
            float availableMaterialsHeight = Mathf.Max(24f, materialsBottomLimit - materialsTop - 8f);
            float assignedMaterialsHeight = Mathf.Min(fullMaterialsHeight, availableMaterialsHeight);
            SetTopFlowRect(materialsViewportRect, materialsTop, assignedMaterialsHeight);
            selectedMaterialsText.overflowMode = fullMaterialsHeight > assignedMaterialsHeight + 0.5f
                ? TextOverflowModes.Truncate
                : TextOverflowModes.Overflow;
            RefreshMaterialsContentGeometry();
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectedMaterialsText.rectTransform);
            if (materialsViewportRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(materialsViewportRect);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_detailColumnRect);
            Canvas.ForceUpdateCanvases();
        }

        private bool UsesManualRecipeShell()
        {
            if (recipeContentRect == null)
            {
                return false;
            }

            int manualRowCount = 0;
            for (int index = 0; index < recipeContentRect.childCount; index++)
            {
                if (recipeContentRect.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                manualRowCount++;
                if (IsGeneratedRecipeRow(rowRect))
                {
                    return false;
                }
            }

            return manualRowCount > 0;
        }

        private bool HasGeneratedRecipeRowChain()
        {
            return HasGeneratedRecipeRowChain(recipeContentRect);
        }

        private static bool HasGeneratedRecipeRowChain(RectTransform contentRoot)
        {
            if (contentRoot == null)
            {
                return false;
            }

            for (int index = 0; index < contentRoot.childCount; index++)
            {
                if (contentRoot.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                if (IsGeneratedRecipeRow(rowRect))
                {
                    return true;
                }
            }

            return false;
        }

        private void RefreshRecipeContentGeometry()
        {
            if (recipeContentRect == null)
            {
                return;
            }

            if (!UsesManualRecipeShell())
            {
                return;
            }

            int visibleIndex = 0;
            float currentTop = 0f;
            for (int index = 0; index < _rows.Count; index++)
            {
                RectTransform rowRect = _rows[index].rect;
                if (rowRect == null || !rowRect.gameObject.activeSelf)
                {
                    continue;
                }

                float currentRowHeight = Mathf.Max(
                    rowHeight,
                    _rows[index].preferredHeight > 0.01f ? _rows[index].preferredHeight : GetCurrentHeight(rowRect, rowHeight));
                rowRect.anchorMin = new Vector2(0f, 1f);
                rowRect.anchorMax = new Vector2(1f, 1f);
                rowRect.pivot = new Vector2(0.5f, 1f);
                rowRect.anchoredPosition = new Vector2(0f, -currentTop);
                rowRect.sizeDelta = new Vector2(0f, currentRowHeight);
                currentTop += currentRowHeight + rowSpacing;
                visibleIndex++;
            }

            float totalHeight = visibleIndex > 0 ? currentTop - rowSpacing : 0f;
            recipeContentRect.anchorMin = new Vector2(0f, 1f);
            recipeContentRect.anchorMax = new Vector2(1f, 1f);
            recipeContentRect.pivot = new Vector2(0.5f, 1f);
            recipeContentRect.anchoredPosition = Vector2.zero;
            recipeContentRect.sizeDelta = new Vector2(0f, totalHeight);

            LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContentRect);
            UpdateViewportMask(recipeViewportRect, totalHeight);
            Canvas.ForceUpdateCanvases();
        }

        private float CaptureRecipeScrollPosition()
        {
            return _recipeScrollRect != null ? _recipeScrollRect.verticalNormalizedPosition : 1f;
        }

        private void RestoreRecipeScrollPosition(float normalizedPosition)
        {
            if (_recipeScrollRect == null)
            {
                return;
            }

            _recipeScrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }

        private void RefreshMaterialsContentGeometry(bool forceLayoutUpdate = true)
        {
            if (selectedMaterialsText == null || materialsContentRect == null)
            {
                return;
            }

            float baselineHeight = 20f;
            float width = materialsViewportRect != null
                ? Mathf.Max(1f, materialsViewportRect.rect.width)
                : Mathf.Max(1f, selectedMaterialsText.rectTransform.rect.width);
            float textHeight = MeasureTextHeight(selectedMaterialsText, width, baselineHeight);

            RectTransform materialsTextRect = selectedMaterialsText.rectTransform;
            bool geometryChanged = !Mathf.Approximately(materialsTextRect.sizeDelta.y, textHeight);
            materialsTextRect.sizeDelta = new Vector2(materialsTextRect.sizeDelta.x, textHeight);
            if (materialsContentRect != materialsTextRect)
            {
                geometryChanged |= !Mathf.Approximately(materialsContentRect.sizeDelta.y, textHeight);
                materialsContentRect.sizeDelta = new Vector2(materialsContentRect.sizeDelta.x, textHeight);
            }

            LayoutElement materialsLabelLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(selectedMaterialsText.gameObject);
            geometryChanged |= !Mathf.Approximately(materialsLabelLayout.preferredHeight, textHeight);
            materialsLabelLayout.minHeight = baselineHeight;
            materialsLabelLayout.preferredHeight = textHeight;
            materialsLabelLayout.flexibleHeight = 0f;

            if (!forceLayoutUpdate && !geometryChanged)
            {
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(materialsTextRect);
            if (materialsContentRect != materialsTextRect)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(materialsContentRect);
                UpdateViewportMask(materialsViewportRect, textHeight);
            }
            Canvas.ForceUpdateCanvases();
        }

        private float MeasureTextHeight(TextMeshProUGUI text, float width, float minHeight)
        {
            if (text == null)
            {
                return minHeight;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                _fontAsset = ResolveFont(text.text);
                if (_fontAsset != null)
                {
                    ApplyResolvedFontToText(text, _fontAsset);
                }
            }

            Vector2 preferred = text.GetPreferredValues(text.text, Mathf.Max(1f, width), 0f);
            return Mathf.Max(minHeight, Mathf.Ceil(preferred.y));
        }

        private static bool SetTextIfChanged(TextMeshProUGUI text, string value)
        {
            if (text == null)
            {
                return false;
            }

            value ??= string.Empty;
            if (string.Equals(text.text, value, System.StringComparison.Ordinal))
            {
                return false;
            }

            text.text = value;
            return true;
        }

        private void EnsureRecipeRowTextChainReady(RowRefs row)
        {
            if (row == null)
            {
                return;
            }

            if (row.rect != null && !row.rect.gameObject.activeSelf)
            {
                row.rect.gameObject.SetActive(true);
            }

            if (row.rect != null && row.rect.TryGetComponent(out CanvasGroup rowGroup) && rowGroup.alpha < 0.98f)
            {
                rowGroup.alpha = 1f;
            }

            EnsureTextAncestorsVisible(row.name, row.rect, 0.98f);
            EnsureWorkbenchTextReady(row.name, forceActive: true, minAlpha: 0.98f);
            EnsureTextAncestorsVisible(row.summary, row.rect, 0.9f);
            EnsureWorkbenchTextReady(row.summary, forceActive: true, minAlpha: 0.9f);
        }

        private void EnsureWorkbenchTextReady(TextMeshProUGUI text, bool forceActive, float minAlpha)
        {
            if (text == null)
            {
                return;
            }

            if (forceActive && !text.gameObject.activeSelf)
            {
                text.gameObject.SetActive(true);
            }

            if (!text.enabled)
            {
                text.enabled = true;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                _fontAsset = ResolveFont(text.text);
                if (_fontAsset != null)
                {
                    ApplyResolvedFontToText(text, _fontAsset);
                }
            }

            if (text.color.a < minAlpha)
            {
                Color color = text.color;
                color.a = minAlpha;
                text.color = color;
            }
        }

        private void EnsureWorkbenchTextContent(TextMeshProUGUI text, string expected, float minAlpha)
        {
            if (text == null)
            {
                return;
            }

            bool changed = false;
            if (!string.IsNullOrWhiteSpace(expected) && !string.Equals(text.text, expected, System.StringComparison.Ordinal))
            {
                text.text = expected;
                changed = true;
            }

            EnsureWorkbenchTextReady(text, forceActive: true, minAlpha);
            if (!CanFontRenderText(text.font, text.text))
            {
                _fontAsset = ResolveFont(expected);
                if (_fontAsset != null)
                {
                    ApplyResolvedFontToText(text, _fontAsset);
                    return;
                }

                changed = true;
            }

            if (changed)
            {
                text.ForceMeshUpdate();
            }
        }

        private static void ApplyResolvedFontToText(TextMeshProUGUI text, TMP_FontAsset fontAsset)
        {
            if (text == null || fontAsset == null)
            {
                return;
            }

            text.font = fontAsset;
            text.UpdateMeshPadding();
            text.SetMaterialDirty();
            text.SetVerticesDirty();
            text.havePropertiesChanged = true;
            text.ForceMeshUpdate();
        }

        private static void EnsureTextAncestorsVisible(TextMeshProUGUI text, RectTransform stopAtExclusive, float minAlpha)
        {
            if (text == null)
            {
                return;
            }

            Transform current = text.transform.parent;
            while (current != null && current != stopAtExclusive)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }

                if (current.TryGetComponent(out CanvasGroup canvasGroup) && canvasGroup.alpha < minAlpha)
                {
                    canvasGroup.alpha = minAlpha;
                }

                current = current.parent;
            }
        }

        private static bool HasReadableRecipeText(TextMeshProUGUI text)
        {
            return text != null
                && text.enabled
                && text.gameObject.activeInHierarchy
                && text.color.a > 0.01f
                && text.rectTransform.rect.width > 2f
                && text.rectTransform.rect.height > 2f
                && CanFontRenderText(text.font, text.text)
                && !string.IsNullOrWhiteSpace(text.text);
        }

        private static void UpdateViewportMask(RectTransform viewportRect, float contentHeight)
        {
            if (viewportRect == null)
            {
                return;
            }

            Image viewportImage = viewportRect.GetComponent<Image>();
            if (viewportImage != null)
            {
                viewportImage.color = new Color(0f, 0f, 0f, 0.001f);
            }

            Mask mask = viewportRect.GetComponent<Mask>();
            if (mask == null)
            {
                return;
            }

            float viewportHeight = viewportRect.rect.height;
            bool shouldMask = viewportHeight > 0.01f && contentHeight > viewportHeight + 0.5f;
            mask.enabled = shouldMask;
        }

        private void CaptureDetailLayoutBaselines()
        {
            CaptureBaseline(ref _nameBaseline, selectedNameText != null ? selectedNameText.rectTransform : null);
            CaptureBaseline(ref _descriptionBaseline, selectedDescriptionText != null ? selectedDescriptionText.rectTransform : null);
            CaptureBaseline(ref _materialsTitleBaseline, _materialsTitleRect);
            CaptureBaseline(ref _materialsViewportBaseline, materialsViewportRect);
            CaptureBaseline(ref _quantityTitleBaseline, _quantityTitleRect);
            CaptureBaseline(ref _quantityControlsBaseline, _quantityControlsRect);
            CaptureBaseline(ref _stageHintBaseline, stageHintText != null ? stageHintText.rectTransform : null);
            CaptureBaseline(ref _progressLabelBaseline, progressLabelText != null ? progressLabelText.rectTransform : null);
            CaptureBaseline(ref _progressBaseline, progressRoot);
            CaptureBaseline(ref _craftButtonBaseline, craftButton != null ? craftButton.transform as RectTransform : null);
            CaptureBaseline(ref _iconCardBaseline, _iconCardRect);
        }

        private bool UsesLegacyDetailManualLayout()
        {
            return selectedNameText != null
                && selectedDescriptionText != null
                && _materialsTitleRect != null
                && materialsViewportRect != null
                && _quantityControlsRect != null
                && _detailColumnRect != null
                && selectedNameText.rectTransform.parent == _detailColumnRect
                && selectedDescriptionText.rectTransform.parent == _detailColumnRect
                && _materialsTitleRect.parent == _detailColumnRect
                && materialsViewportRect.parent == _detailColumnRect
                && _quantityControlsRect.parent == _detailColumnRect;
        }

        private float GetActionAreaFloorTop()
        {
            float floorTop = float.MaxValue;

            if (_quantityTitleRect != null && _quantityTitleRect.parent == _detailColumnRect)
            {
                floorTop = Mathf.Min(floorTop, _quantityTitleBaseline.TopOr(GetTopInParent(_quantityTitleRect)));
            }

            if (_quantityControlsRect != null && _quantityControlsRect.parent == _detailColumnRect)
            {
                floorTop = Mathf.Min(floorTop, _quantityControlsBaseline.TopOr(GetTopInParent(_quantityControlsRect)));
            }

            if (stageHintText != null && stageHintText.gameObject.activeSelf && stageHintText.rectTransform.parent == _detailColumnRect)
            {
                floorTop = Mathf.Min(floorTop, _stageHintBaseline.TopOr(GetTopInParent(stageHintText.rectTransform)));
            }

            RectTransform craftButtonRect = craftButton != null ? craftButton.transform as RectTransform : null;
            if (craftButtonRect != null && craftButtonRect.parent == _detailColumnRect)
            {
                floorTop = Mathf.Min(floorTop, _craftButtonBaseline.TopOr(GetTopInParent(craftButtonRect)));
            }

            if (progressRoot != null && progressRoot.parent == _detailColumnRect)
            {
                floorTop = Mathf.Min(floorTop, _progressBaseline.TopOr(GetTopInParent(progressRoot)));
            }

            return floorTop < float.MaxValue
                ? floorTop
                : (_stageHintBaseline.TopOr(GetTopInParent(_quantityControlsRect)) + _stageHintBaseline.HeightOr(18f) + 8f);
        }

        private float GetDetailContentFloorTop()
        {
            float floorTop = GetActionAreaFloorTop();
            if (floorTop < float.MaxValue)
            {
                return floorTop;
            }

            return _detailColumnRect != null ? _detailColumnRect.rect.height - 8f : 0f;
        }

        private static void CaptureBaseline(ref RectBaseline baseline, RectTransform rect)
        {
            if (baseline.IsCaptured || rect == null || rect.parent == null)
            {
                return;
            }

            baseline = new RectBaseline
            {
                top = GetTopInParent(rect),
                height = GetCurrentHeight(rect, 0f),
                captured = true
            };
        }

        private static float GetTopInParent(RectTransform rect)
        {
            RectTransform parent = rect != null ? rect.parent as RectTransform : null;
            if (rect == null || parent == null)
            {
                return 0f;
            }

            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector3 topLeft = parent.InverseTransformPoint(corners[1]);
            return parent.rect.yMax - topLeft.y;
        }

        private static float GetCurrentHeight(RectTransform rect, float fallback)
        {
            return rect != null && rect.rect.height > 0.01f ? rect.rect.height : fallback;
        }

        private static float GetCurrentWidth(RectTransform rect, float fallback)
        {
            return rect != null && rect.rect.width > 0.01f ? rect.rect.width : fallback;
        }

        private static void NormalizeToTopFlowRect(RectTransform rect)
        {
            if (rect == null || rect.parent == null)
            {
                return;
            }

            bool alreadyTopAnchored = Mathf.Abs(rect.anchorMin.y - 1f) < 0.001f
                && Mathf.Abs(rect.anchorMax.y - 1f) < 0.001f
                && Mathf.Abs(rect.pivot.y - 1f) < 0.001f;
            if (alreadyTopAnchored)
            {
                return;
            }

            float top = GetTopInParent(rect);
            float height = Mathf.Max(1f, GetCurrentHeight(rect, Mathf.Abs(rect.sizeDelta.y)));

            if (Mathf.Abs(rect.anchorMin.x - rect.anchorMax.x) > 0.001f)
            {
                Vector2 offsetMin = rect.offsetMin;
                Vector2 offsetMax = rect.offsetMax;
                rect.anchorMin = new Vector2(rect.anchorMin.x, 1f);
                rect.anchorMax = new Vector2(rect.anchorMax.x, 1f);
                rect.pivot = new Vector2(rect.pivot.x, 1f);
                rect.offsetMin = new Vector2(offsetMin.x, -top - height);
                rect.offsetMax = new Vector2(offsetMax.x, -top);
                return;
            }

            Vector2 anchoredPosition = rect.anchoredPosition;
            Vector2 sizeDelta = rect.sizeDelta;
            rect.anchorMin = new Vector2(rect.anchorMin.x, 1f);
            rect.anchorMax = new Vector2(rect.anchorMax.x, 1f);
            rect.pivot = new Vector2(rect.pivot.x, 1f);
            rect.anchoredPosition = new Vector2(anchoredPosition.x, -top);
            rect.sizeDelta = new Vector2(sizeDelta.x, height);
        }

        private static void SetTopFlowRect(RectTransform rect, float top, float height)
        {
            NormalizeToTopFlowRect(rect);
            SetTopKeepingHorizontal(rect, top, height);
        }

        private static void RestoreBaselineRect(RectTransform rect, RectBaseline baseline)
        {
            if (rect == null || !baseline.IsCaptured)
            {
                return;
            }

            SetTopFlowRect(rect, baseline.top, Mathf.Max(1f, baseline.height));
        }

        private static void SetTopKeepingHorizontal(RectTransform rect, float top, float height)
        {
            if (rect == null)
            {
                return;
            }

            Vector2 anchorMin = rect.anchorMin;
            Vector2 anchorMax = rect.anchorMax;
            Vector2 pivot = rect.pivot;
            bool stretchesVertically = Mathf.Abs(anchorMin.y - anchorMax.y) > 0.001f;

            rect.anchorMin = new Vector2(anchorMin.x, 1f);
            rect.anchorMax = new Vector2(anchorMax.x, 1f);
            rect.pivot = new Vector2(pivot.x, 1f);

            if (stretchesVertically)
            {
                Vector2 offsetMin = rect.offsetMin;
                Vector2 offsetMax = rect.offsetMax;
                rect.offsetMin = new Vector2(offsetMin.x, -top - height);
                rect.offsetMax = new Vector2(offsetMax.x, -top);
                return;
            }

            Vector2 anchoredPosition = rect.anchoredPosition;
            Vector2 sizeDelta = rect.sizeDelta;
            rect.anchoredPosition = new Vector2(anchoredPosition.x, -top);
            rect.sizeDelta = new Vector2(sizeDelta.x, height);
        }

        private static void CopyRectGeometry(RectTransform source, RectTransform target)
        {
            if (source == null || target == null)
            {
                return;
            }

            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.pivot = source.pivot;
            target.anchoredPosition = source.anchoredPosition;
            target.sizeDelta = source.sizeDelta;
            target.offsetMin = source.offsetMin;
            target.offsetMax = source.offsetMax;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        private bool EnsureRecipesLoaded()
        {
            if (_recipes.Count > 0)
            {
                return true;
            }

            RecipeData[] loadedRecipes = Resources.LoadAll<RecipeData>(RecipeResourceFolder);
            if (loadedRecipes == null || loadedRecipes.Length == 0)
            {
                Debug.LogWarning($"[SpringDay1WorkbenchCraftingOverlay] 未在 Resources/{RecipeResourceFolder} 找到工作台配方。");
                return false;
            }

            _recipes.Clear();
            _recipes.AddRange(
                loadedRecipes
                    .Where(recipe => recipe != null && recipe.requiredStation == CraftingStation.Workbench && recipe.resultItemID >= 0)
                    .OrderBy(GetRecipeSortOrder)
                    .ThenBy(recipe => recipe.recipeID));

            if (_recipes.Count == 0)
            {
                Debug.LogWarning("[SpringDay1WorkbenchCraftingOverlay] 工作台配方全部无效，无法显示制作列表。");
                return false;
            }

            return true;
        }

        private void EnsureRows()
        {
            if (_rows.Count == 0)
            {
                BindExistingRows();
            }

            while (_rows.Count < _recipes.Count)
            {
                if (_rows.Count > 0)
                {
                    RectTransform clone = Instantiate(_rows[0].rect, recipeContentRect);
                    clone.name = $"RecipeRow_{_rows.Count}";
                    EnsureRecipeRowCompatibility(clone);
                    RowRefs boundClone = BindRecipeRow(clone, _rows.Count);
                    if (boundClone != null)
                    {
                        _rows.Add(boundClone);
                        continue;
                    }
                }

                int rowIndex = _rows.Count;
                RowRefs createdRow = CreatePrefabStyleRecipeRow(rowIndex);
                if (createdRow != null)
                {
                    _rows.Add(createdRow);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContentRect);
        }

        private void RefreshAll(bool validateRecipeRows = true, bool rebuildRecipeGeometry = true, bool rebuildDetailGeometry = true)
        {
            float preservedRecipeScroll = CaptureRecipeScrollPosition();
            EnsureRows();
            RefreshRows(allowRecovery: validateRecipeRows, rebuildGeometry: rebuildRecipeGeometry);
            if (validateRecipeRows)
            {
                ForceRuntimeRecipeRowsIfNeeded();
            }

            RefreshSelection(forceDetailGeometry: rebuildDetailGeometry);
            UpdateQuantityUi();
            RefreshCompatibilityLayout(forceLayout: rebuildDetailGeometry);
            RestoreRecipeScrollPosition(preservedRecipeScroll);
        }

        private void RefreshVisibleWorkbenchUi(bool refreshRecipeRows = true)
        {
            if (!_isVisible)
            {
                return;
            }

            if (refreshRecipeRows)
            {
                RefreshRows(allowRecovery: false, rebuildGeometry: false);
            }

            RefreshSelection(forceDetailGeometry: false);
            UpdateQuantityUi();
        }

        private void RefreshRows(bool allowRecovery = true, bool rebuildGeometry = true)
        {
            bool usesManualRecipeShell = UsesManualRecipeShell();
            for (int i = 0; i < _rows.Count; i++)
            {
                bool hasRecipe = i < _recipes.Count;
                _rows[i].rect.gameObject.SetActive(hasRecipe);
                if (!hasRecipe)
                {
                    continue;
                }

                RecipeData recipe = _recipes[i];
                ItemData item = ResolveItem(recipe.resultItemID);
                bool selected = i == _selectedIndex;
                bool craftable = GetMaxCraftableCount(recipe) > 0;
                string displayName = GetRecipeDisplayName(recipe, item);
                string displaySummary = BuildRowSummary(recipe);
                EnsureRecipeRowTextChainReady(_rows[i]);
                _rows[i].icon.sprite = item != null ? item.GetBagSprite() : null;
                _rows[i].icon.color = _rows[i].icon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
                SetTextIfChanged(_rows[i].name, displayName);
                SetTextIfChanged(_rows[i].summary, displaySummary);
                EnsureWorkbenchTextContent(_rows[i].name, displayName, 0.98f);
                EnsureWorkbenchTextContent(_rows[i].summary, displaySummary, 0.9f);
                EnsureRecipeRowCompatibility(_rows[i].rect);
                if (rebuildGeometry)
                {
                    _rows[i].preferredHeight = usesManualRecipeShell ? GetCurrentHeight(_rows[i].rect, rowHeight) : rowHeight;
                }
                else if (_rows[i].preferredHeight <= 0.01f)
                {
                    _rows[i].preferredHeight = rowHeight;
                }

                _rows[i].background.color = selected ? new Color(0.31f, 0.23f, 0.14f, 0.98f) : craftable ? new Color(0.18f, 0.22f, 0.31f, 0.94f) : new Color(0.24f, 0.16f, 0.16f, 0.94f);
                _rows[i].accent.color = selected ? new Color(0.97f, 0.8f, 0.42f, 1f) : new Color(1f, 1f, 1f, 0f);
            }

            if (!rebuildGeometry)
            {
                return;
            }

            if (usesManualRecipeShell)
            {
                RefreshRecipeContentGeometry();
            }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContentRect);
                if (recipeViewportRect != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(recipeViewportRect);
                }

                Canvas.ForceUpdateCanvases();
            }

            bool unreadableRows = HasUnreadableVisibleRecipeRows();
            bool needsHardRecovery = NeedsRecipeRowHardRecovery();
            if (allowRecovery && (unreadableRows || needsHardRecovery))
            {
                RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true);
                RefreshRows(allowRecovery: false, rebuildGeometry: true);
            }
        }

        private void ForceRuntimeRecipeRowsIfNeeded()
        {
            if (recipeContentRect == null)
            {
                return;
            }

            if (_recipes.Count == 0)
            {
                return;
            }

            bool rowsNeedReplacement = !HasGeneratedRecipeRowChain()
                || ShouldRebuildRecipeRowsFromScratch()
                || HasUnreadableVisibleRecipeRows()
                || NeedsRecipeRowHardRecovery()
                || !HasReusableRecipeRowChain(recipeContentRect);
            if (!rowsNeedReplacement)
            {
                return;
            }

            RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true);
            RefreshRows(allowRecovery: false, rebuildGeometry: true);
        }

        private bool ShouldRebuildRecipeRowsFromScratch()
        {
            if (UsesPrefabRecipeShell())
            {
                return false;
            }

            if (_recipes.Count == 0 || _rows.Count == 0)
            {
                return false;
            }

            if (HasGeneratedRecipeRowChain())
            {
                return false;
            }

            bool hasVisibleRow = false;
            for (int index = 0; index < _rows.Count; index++)
            {
                RowRefs row = _rows[index];
                if (row?.rect == null || !row.rect.gameObject.activeSelf)
                {
                    continue;
                }

                hasVisibleRow = true;
                bool hasReadableName = HasReadableRecipeText(row.name);
                bool hasReadableSummary = HasReadableRecipeText(row.summary);
                if (!hasReadableName || !hasReadableSummary)
                {
                    return true;
                }
            }

            return !hasVisibleRow;
        }

        private bool HasUnreadableVisibleRecipeRows()
        {
            for (int index = 0; index < _rows.Count; index++)
            {
                RowRefs row = _rows[index];
                if (row?.rect == null || !row.rect.gameObject.activeSelf)
                {
                    continue;
                }

                if (!HasReadableRecipeText(row.name) || !HasReadableRecipeText(row.summary))
                {
                    return true;
                }
            }

            return false;
        }

        private bool NeedsRecipeRowHardRecovery()
        {
            if (recipeViewportRect == null)
            {
                return false;
            }

            bool hasVisibleRow = false;
            for (int index = 0; index < _rows.Count; index++)
            {
                RowRefs row = _rows[index];
                if (row?.rect == null || !row.rect.gameObject.activeSelf)
                {
                    continue;
                }

                hasVisibleRow = true;
                if (!IsRectReasonablyInsideViewport(row.rect, recipeViewportRect))
                {
                    return true;
                }

                if (row.background == null || row.background.color.a < 0.2f)
                {
                    return true;
                }

                if (row.background == null
                    || row.accent == null
                    || row.icon == null
                    || HasInvalidRecipeRowGraphicMaterial(row.background)
                    || HasInvalidRecipeRowGraphicMaterial(row.accent)
                    || HasInvalidRecipeRowGraphicMaterial(row.icon))
                {
                    return true;
                }

                if (!IsRecipeRowTextHierarchyValid(row)
                    || string.IsNullOrWhiteSpace(row.name.text)
                    || string.IsNullOrWhiteSpace(row.summary.text)
                    || !IsRectReasonablyInsideViewport(row.name.rectTransform, row.rect)
                    || !IsRectReasonablyInsideViewport(row.summary.rectTransform, row.rect))
                {
                    return true;
                }
            }

            return !hasVisibleRow;
        }

        private static bool IsRecipeRowTextHierarchyValid(RowRefs row)
        {
            if (row?.rect == null || row.name == null || row.summary == null)
            {
                return false;
            }

            if (!row.name.transform.IsChildOf(row.rect) || !row.summary.transform.IsChildOf(row.rect))
            {
                return false;
            }

            if (IsGeneratedRecipeRow(row.rect))
            {
                RectTransform textColumn = FindDescendantRect(row.rect, "TextColumn");
                return textColumn != null
                    && row.name.rectTransform.parent == textColumn
                    && row.summary.rectTransform.parent == textColumn
                    && GetCurrentWidth(textColumn, 0f) > 1f
                    && GetCurrentHeight(textColumn, 0f) > 1f;
            }

            return row.name.rectTransform.parent == row.rect
                && row.summary.rectTransform.parent == row.rect;
        }

        private static bool IsRectReasonablyInsideViewport(RectTransform rect, RectTransform viewport)
        {
            if (rect == null || viewport == null)
            {
                return false;
            }

            Vector3[] rectCorners = new Vector3[4];
            Vector3[] viewportCorners = new Vector3[4];
            rect.GetWorldCorners(rectCorners);
            viewport.GetWorldCorners(viewportCorners);

            float rectLeft = rectCorners[0].x;
            float rectRight = rectCorners[3].x;
            float rectBottom = rectCorners[0].y;
            float rectTop = rectCorners[1].y;

            float viewportLeft = viewportCorners[0].x;
            float viewportRight = viewportCorners[3].x;
            float viewportBottom = viewportCorners[0].y;
            float viewportTop = viewportCorners[1].y;

            bool overlapsHorizontally = rectRight >= viewportLeft - 1f && rectLeft <= viewportRight + 1f;
            bool overlapsVertically = rectTop >= viewportBottom - 1f && rectBottom <= viewportTop + 1f;
            return overlapsHorizontally && overlapsVertically;
        }

        private void RebuildRecipeRowsFromScratch(bool forceRuntimePrefabStyle = false)
        {
            if (recipeContentRect == null)
            {
                return;
            }

            RectTransform templateRow = forceRuntimePrefabStyle ? null : FindCloneableRecipeRowTemplate();

            for (int index = recipeContentRect.childCount - 1; index >= 0; index--)
            {
                if (recipeContentRect.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                if (templateRow != null && rowRect == templateRow)
                {
                    continue;
                }

                rowRect.name = $"Obsolete_{rowRect.name}";
                rowRect.gameObject.SetActive(false);
                if (Application.isPlaying)
                {
                    Destroy(rowRect.gameObject);
                }
                else
                {
                    DestroyImmediate(rowRect.gameObject);
                }
            }

            _rows.Clear();
            if (templateRow != null)
            {
                templateRow.name = "RecipeRow_0";
                templateRow.SetSiblingIndex(0);
                templateRow.gameObject.SetActive(true);
                EnsureRecipeRowCompatibility(templateRow);
            }

            EnsureRows();
        }

        private RectTransform FindCloneableRecipeRowTemplate()
        {
            if (recipeContentRect == null)
            {
                return null;
            }

            for (int index = 0; index < recipeContentRect.childCount; index++)
            {
                if (recipeContentRect.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                if (CanCloneRecipeRowTemplate(rowRect))
                {
                    return rowRect;
                }
            }

            return null;
        }

        private RowRefs CreatePrefabStyleRecipeRow(int rowIndex)
        {
            if (recipeContentRect == null)
            {
                return null;
            }

            RectTransform rowRect = CreateRect(recipeContentRect, $"RecipeRow_{rowIndex}");
            HorizontalLayoutGroup rowLayout = SpringDay1UiLayerUtility.EnsureComponent<HorizontalLayoutGroup>(rowRect.gameObject);
            rowLayout.padding = new RectOffset(4, 8, 6, 6);
            rowLayout.spacing = 5;
            rowLayout.childAlignment = TextAnchor.UpperLeft;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;
            LayoutElement rowLayoutElement = rowRect.gameObject.AddComponent<LayoutElement>();
            rowLayoutElement.preferredHeight = rowHeight;
            rowLayoutElement.minHeight = rowHeight;
            rowLayoutElement.flexibleHeight = 0f;

            Image background = rowRect.gameObject.AddComponent<Image>();
            background.color = new Color(0.18f, 0.22f, 0.31f, 0.94f);
            ApplyOutline(rowRect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.06f), new Vector2(1f, -1f));

            RectTransform accent = CreateRect(rowRect, "Accent");
            Image accentImage = accent.gameObject.AddComponent<Image>();
            LayoutElement accentLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(accent.gameObject);
            accentLayout.preferredWidth = 4f;
            accentLayout.minWidth = 4f;
            accentLayout.preferredHeight = rowHeight - 12f;
            accentLayout.minHeight = rowHeight - 12f;

            RectTransform iconCard = CreateRect(rowRect, "IconCard");
            iconCard.gameObject.AddComponent<Image>().color = new Color(0.23f, 0.28f, 0.38f, 0.94f);
            LayoutElement iconLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(iconCard.gameObject);
            iconLayout.preferredWidth = 36f;
            iconLayout.minWidth = 36f;
            iconLayout.preferredHeight = 36f;
            iconLayout.minHeight = 36f;
            Image icon = CreateIcon(iconCard, "Icon", 24f);
            Center(icon.rectTransform);

            RectTransform textColumn = CreateRect(rowRect, "TextColumn");
            VerticalLayoutGroup textLayout = SpringDay1UiLayerUtility.EnsureComponent<VerticalLayoutGroup>(textColumn.gameObject);
            textLayout.padding = new RectOffset(0, 0, 0, 0);
            textLayout.spacing = 2;
            textLayout.childAlignment = TextAnchor.UpperLeft;
            textLayout.childControlWidth = true;
            textLayout.childControlHeight = false;
            textLayout.childForceExpandWidth = true;
            textLayout.childForceExpandHeight = false;
            LayoutElement textColumnLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(textColumn.gameObject);
            textColumnLayout.flexibleWidth = 1f;
            textColumnLayout.minWidth = 78f;

            TextMeshProUGUI name = CreateText(textColumn, "Name", string.Empty, 13f, Color.white, TextAlignmentOptions.TopLeft, true, true);
            LayoutElement nameLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(name.gameObject);
            nameLayout.minHeight = 18f;
            nameLayout.preferredHeight = 18f;
            TextMeshProUGUI summary = CreateText(textColumn, "Summary", string.Empty, 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.TopLeft, true, true);
            LayoutElement summaryLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(summary.gameObject);
            summaryLayout.minHeight = 12f;

            Button button = rowRect.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(() => SelectRecipe(rowIndex));

            EnsureRecipeRowCompatibility(rowRect);
            return new RowRefs
            {
                rect = rowRect,
                background = background,
                accent = accentImage,
                icon = icon,
                name = name,
                summary = summary,
                button = button,
                preferredHeight = rowHeight
            };
        }

        private void SelectRecipe(int index)
        {
            if (index < 0 || index >= _recipes.Count)
            {
                return;
            }

            _selectedIndex = index;
            _selectedQuantity = 0;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            RefreshAll(validateRecipeRows: false, rebuildRecipeGeometry: false, rebuildDetailGeometry: true);
        }

        private void RefreshSelection(bool forceDetailGeometry = true)
        {
            RecipeData recipe = GetSelectedRecipe();
            if (recipe == null)
            {
                return;
            }

            ItemData item = ResolveItem(recipe.resultItemID);
            selectedIcon.sprite = item != null ? item.GetBagSprite() : null;
            selectedIcon.color = selectedIcon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            string displayName = GetRecipeDisplayName(recipe, item);
            string displayDescription = GetRecipeDisplayDescription(recipe, item);
            string materialsText = BuildMaterialsText(recipe, GetMaterialPreviewQuantity());
            SetTextIfChanged(selectedNameText, displayName);
            SetTextIfChanged(selectedDescriptionText, displayDescription);
            SetTextIfChanged(selectedMaterialsText, materialsText);
            ApplyDetailColumnFontTuning();
            EnsureWorkbenchTextContent(selectedNameText, displayName, 0.98f);
            EnsureWorkbenchTextContent(selectedDescriptionText, displayDescription, 0.9f);
            EnsureWorkbenchTextContent(selectedMaterialsText, materialsText, 0.9f);
            SetTextIfChanged(stageHintText, BuildStageHint(recipe));
            UpdateProgressLabel(recipe);

            if (UsesPrefabDetailShell())
            {
                EnsurePrefabDetailTextChain();
                if (forceDetailGeometry && _detailColumnRect != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_detailColumnRect);
                }
            }

            RefreshMaterialsContentGeometry(forceLayoutUpdate: forceDetailGeometry);
            if (_materialsScrollRect != null)
            {
                _materialsScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private static bool EntryHasReadyOutputs(WorkbenchQueueEntry entry)
        {
            return entry != null && entry.readyCount > 0;
        }

        private static bool EntryHasUnclaimedCompletedOutputs(WorkbenchQueueEntry entry)
        {
            return EntryHasReadyOutputs(entry) && !HasPendingCrafts(entry);
        }

        private static int GetPendingCraftCount(WorkbenchQueueEntry entry)
        {
            return entry != null ? Mathf.Max(0, entry.totalCount - entry.readyCount) : 0;
        }

        // 底部交互矩阵固定为两层互斥，且“动作意图”高于“领取意图”：
        // 1. 只要 selectedQuantity > 0，就进入动作层（开始 / 加入队列 / 追加），不允许被 readyCount 抢回领取态
        // 2. 只有没有动作意图时，进度层（进度 / 制作完成 / 领取 / 取消 / 中断）才接管
        // 3. 领取只允许出现在当前选中的那条 recipe 进度栏里，不通过通用 blocker 文案全局抢焦点
        private bool ShouldShowCraftButton(RecipeData recipe, WorkbenchQueueEntry entry)
        {
            return recipe != null && _selectedQuantity > 0;
        }

        private bool ShouldShowProgressFooter(RecipeData recipe, WorkbenchQueueEntry entry)
        {
            return recipe != null && !ShouldShowCraftButton(recipe, entry);
        }

        private void ApplyProgressLabelState(string labelText, Color labelColor)
        {
            if (progressLabelText == null)
            {
                return;
            }

            string resolvedText = labelText ?? string.Empty;
            bool changed = !string.Equals(progressLabelText.text, resolvedText, System.StringComparison.Ordinal);
            if (changed)
            {
                progressLabelText.text = resolvedText;
            }

            if (progressLabelText.color != labelColor)
            {
                progressLabelText.color = labelColor;
                changed = true;
            }

            progressLabelText.enabled = !string.IsNullOrWhiteSpace(progressLabelText.text);
            if (progressLabelText.enabled && changed)
            {
                EnsureWorkbenchTextContent(progressLabelText, progressLabelText.text, 0.96f);
            }
        }

        private void UpdateQuantityUi()
        {
            RecipeData recipe = GetSelectedRecipe();
            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            int maxSelectable = GetSelectableQuantityCap(recipe);
            _selectedQuantity = Mathf.Clamp(_selectedQuantity, 0, maxSelectable);
            quantitySlider.minValue = 0f;
            quantitySlider.maxValue = Mathf.Max(0f, maxSelectable);
            quantitySlider.wholeNumbers = true;
            quantitySlider.SetValueWithoutNotify(_selectedQuantity);
            quantityValueText.text = $"x{_selectedQuantity}";
            decreaseButton.interactable = _selectedQuantity > 0;
            increaseButton.interactable = _selectedQuantity < maxSelectable;
            quantitySlider.interactable = maxSelectable > 0;

            bool canCraft = CanCraftSelected(recipe, out string blockerMessage);
            bool hasAnyCraftRunning = HasActiveCraftQueue;
            bool canSubmitQuantity = canCraft && _selectedQuantity > 0;
            bool canPickupOutputs = CanPickupEntryOutputs(entry);
            bool canCancelFromButton = false;
            bool canCancelCraft = CanInterruptActiveEntry(entry);
            bool canCancelQueued = CanCancelQueuedEntry(entry);
            bool canShowQueueAppendHover = hasAnyCraftRunning && _selectedQuantity > 0 && canCraft;
            bool showCraftButton = ShouldShowCraftButton(recipe, entry);
            bool showProgressFooter = ShouldShowProgressFooter(recipe, entry);
            craftButton.gameObject.SetActive(showCraftButton);
            craftButton.interactable = showCraftButton && canSubmitQuantity;
            craftButtonLabel.text = showCraftButton ? BuildCraftButtonLabel(recipe, canCraft, blockerMessage) : string.Empty;
            craftButtonLabel.enabled = showCraftButton && !string.IsNullOrWhiteSpace(craftButtonLabel.text);
            if (craftButtonLabel.enabled)
            {
                EnsureWorkbenchTextContent(craftButtonLabel, craftButtonLabel.text, 0.98f);
            }
            if (!showCraftButton)
            {
                craftButtonBackground.color = new Color(0.25f, 0.39f, 0.55f, 0f);
            }
            else if (canPickupOutputs)
            {
                craftButtonBackground.color = new Color(0.25f, 0.39f, 0.55f, 0.04f);
            }
            else if (hasAnyCraftRunning)
            {
                craftButtonBackground.color = _craftButtonHovered && (canShowQueueAppendHover || canCancelFromButton)
                    ? (canShowQueueAppendHover
                        ? new Color(0.25f, 0.39f, 0.55f, 0.72f)
                        : new Color(0.52f, 0.17f, 0.17f, 0.78f))
                    : new Color(0.25f, 0.39f, 0.55f, 0.04f);
            }
            else
            {
                craftButtonBackground.color = canCraft ? new Color(0.33f, 0.53f, 0.44f, 0.98f) : new Color(0.27f, 0.3f, 0.34f, 0.9f);
            }

            if (progressRoot != null && progressBackgroundImage != null)
            {
                progressRoot.gameObject.SetActive(showProgressFooter);
                if (!showProgressFooter)
                {
                    progressBackgroundImage.color = new Color(0.04f, 0.05f, 0.08f, 0f);
                    if (progressFillImage != null)
                    {
                        progressFillImage.fillAmount = 0f;
                    }

                    ApplyProgressLabelState(string.Empty, ProgressLabelReadableColor);
                }
                else if (recipe == null)
                {
                    progressBackgroundImage.color = new Color(0.04f, 0.05f, 0.08f, 0f);
                }
                else if (canPickupOutputs && _progressBarHovered)
                {
                    progressBackgroundImage.color = new Color(0.32f, 0.24f, 0.08f, 0.98f);
                }
                else if ((canCancelCraft || canCancelQueued) && _progressBarHovered)
                {
                    progressBackgroundImage.color = new Color(0.32f, 0.08f, 0.08f, 0.98f);
                }
                else
                {
                    progressBackgroundImage.color = ShouldShowCompletedProgress(recipe, entry)
                        ? new Color(0.32f, 0.24f, 0.08f, 0.98f)
                        : new Color(0.04f, 0.05f, 0.08f, 0.94f);
                }
            }

            if (craftButtonLabel != null)
            {
                craftButtonLabel.alpha = craftButtonLabel.enabled ? 1f : 0f;
            }

            if (stageHintText != null)
            {
                if (!canCraft && !string.IsNullOrWhiteSpace(blockerMessage))
                {
                    stageHintText.text = blockerMessage;
                }
                else
                {
                    stageHintText.text = string.Empty;
                }

                EnsureWorkbenchTextContent(stageHintText, stageHintText.text, 0.9f);
            }
        }

        private void ChangeQuantity(int delta) => SetQuantity(_selectedQuantity + delta);

        private bool IsRecipeCurrentlySelected(RecipeData recipe)
        {
            return recipe != null && recipe == GetSelectedRecipe();
        }

        private void SetQuantity(int quantity, bool updateSlider = true)
        {
            RecipeData recipe = GetSelectedRecipe();
            int maxSelectable = GetSelectableQuantityCap(recipe);
            _selectedQuantity = Mathf.Clamp(quantity, 0, maxSelectable);
            if (updateSlider)
            {
                quantitySlider.SetValueWithoutNotify(_selectedQuantity);
            }

            RefreshSelection(forceDetailGeometry: false);
            UpdateQuantityUi();
        }

        private void RefreshRuntimeProgressVisuals(RecipeData recipe)
        {
            UpdateProgressLabel(recipe);
            UpdateFloatingProgressVisibility();
        }

        private void OnCraftButtonClicked()
        {
            RecipeData recipe = GetSelectedRecipe();
            if (recipe == null || _craftingService == null)
            {
                return;
            }

            WorkbenchQueueEntry selectedEntry = GetQueueEntry(recipe);
            bool isCrafting = HasActiveCraftQueue;
            if (isCrafting && _selectedQuantity <= 0)
            {
                if (IsActiveEntry(selectedEntry))
                {
                    CancelActiveCraftQueue();
                }

                UpdateQuantityUi();
                return;
            }

            if (_selectedQuantity <= 0)
            {
                UpdateQuantityUi();
                return;
            }

            if (!CanCraftSelected(recipe, out string blockerMessage))
            {
                if (!string.IsNullOrWhiteSpace(blockerMessage))
                {
                    SpringDay1PromptOverlay.EnsureRuntime();
                    SpringDay1PromptOverlay.Instance.Show(blockerMessage);
                    stageHintText.text = blockerMessage;
                }

                UpdateQuantityUi();
                return;
            }

            int requestedQuantity = Mathf.Max(0, _selectedQuantity);
            CraftResult reserveResult = _craftingService.TryReserveMaterials(recipe, requestedQuantity);
            if (!reserveResult.success)
            {
                string reserveBlockerMessage = !string.IsNullOrWhiteSpace(reserveResult.message)
                    ? reserveResult.message
                    : "材料还不够，先把清单里的材料收齐。";
                SpringDay1PromptOverlay.EnsureRuntime();
                SpringDay1PromptOverlay.Instance.Show(reserveBlockerMessage);
                stageHintText.text = reserveBlockerMessage;
                UpdateQuantityUi();
                return;
            }

            if (isCrafting)
            {
                WorkbenchQueueEntry queuedEntry = GetQueueEntry(recipe, createIfMissing: true);
                queuedEntry.totalCount += requestedQuantity;
                if (_activeQueueEntry == null)
                {
                    _activeQueueEntry = queuedEntry;
                    _craftingRecipe = queuedEntry.recipe;
                    SyncCountsFromActiveEntry();
                }

                _selectedQuantity = 0;
                RefreshSelection(forceDetailGeometry: false);
                UpdateQuantityUi();
                UpdateFloatingProgressVisibility();
                FlushCurrentRuntimeStateToPersistence();
                TryShowQueueOverflowToast();
                return;
            }

            _selectedQuantity = 0;
            _craftRoutine = StartCoroutine(CraftRoutine(recipe, requestedQuantity));
        }

        private void OnProgressBarClicked()
        {
            RecipeData recipe = GetSelectedRecipe();
            if (recipe == null)
            {
                return;
            }

            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            bool stateRefreshedByMutation = false;
            if (CanPickupEntryOutputs(entry))
            {
                stateRefreshedByMutation = TryPickupSelectedOutputs();
            }
            else if (CanInterruptActiveEntry(entry))
            {
                CancelActiveCraftQueue();
                stateRefreshedByMutation = true;
            }
            else if (CanCancelQueuedEntry(entry))
            {
                CancelQueuedRecipeEntry(entry);
                stateRefreshedByMutation = true;
            }

            if (!stateRefreshedByMutation)
            {
                UpdateQuantityUi();
                UpdateFloatingProgressVisibility();
            }
        }

        private IEnumerator CraftRoutine(RecipeData recipe, int initialCraftCount, bool resumeExistingEntry = false)
        {
            WorkbenchQueueEntry entry = GetQueueEntry(recipe, createIfMissing: true);
            if (!resumeExistingEntry)
            {
                entry.totalCount += Mathf.Max(0, initialCraftCount);
            }

            _activeQueueEntry = entry;
            _craftingRecipe = recipe;
            if (!resumeExistingEntry)
            {
                ResetEntryRuntimeState(entry);
            }

            SyncCountsFromActiveEntry();
            if (!resumeExistingEntry)
            {
                _lastCompletedQueueTotal = 0;
                _lastCompletedRecipeId = -1;
                SetActiveEntryProgress(0f);
            }
            else
            {
                SetActiveEntryProgress(GetEntryUnitProgress(entry));
            }

            _craftButtonHovered = false;
            _progressBarHovered = false;
            _selectedQuantity = 0;
            UpdateQuantityUi();
            RefreshSelection(forceDetailGeometry: false);
            PlayerMovement playerMovement = ResolvePlayerMovement();
            SetWorkbenchAnimating(true);
            FlushCurrentRuntimeStateToPersistence();

            int successCount = 0;
            while (true)
            {
                if (_activeQueueEntry == null || !HasPendingCrafts(_activeQueueEntry))
                {
                    RemoveQueueEntryIfEmpty(_activeQueueEntry);
                    _activeQueueEntry = FindNextPendingQueueEntry();

                    _craftingRecipe = _activeQueueEntry != null ? _activeQueueEntry.recipe : null;
                    SyncCountsFromActiveEntry();
                    if (_activeQueueEntry == null || _craftingRecipe == null)
                    {
                        break;
                    }

                    recipe = _craftingRecipe;
                    ResetEntryRuntimeState(_activeQueueEntry);
                    SetActiveEntryReserved(false);
                    SetActiveEntryProgress(0f);
                    RefreshSelection(forceDetailGeometry: false);
                    UpdateQuantityUi();
                    UpdateFloatingProgressVisibility();
                    FlushCurrentRuntimeStateToPersistence();
                }

                float duration = Mathf.Max(0.25f, recipe.craftingTime > 0f ? recipe.craftingTime : defaultCraftDuration);
                if (!resumeExistingEntry)
                {
                    SetActiveEntryProgress(0f);
                }
                else
                {
                    SetActiveEntryProgress(GetEntryUnitProgress(_activeQueueEntry));
                    resumeExistingEntry = false;
                }

                while (_craftProgress < 1f)
                {
                    SetActiveEntryProgress(_craftProgress + Time.deltaTime / duration);
                    MaintainWorkbenchPose(playerMovement);
                    RefreshRuntimeProgressVisuals(GetSelectedRecipe());
                    yield return null;
                }

                successCount++;
                SetActiveEntryReserved(false);
                _activeQueueEntry.readyCount = Mathf.Min(_activeQueueEntry.totalCount, _activeQueueEntry.readyCount + 1);
                _craftingService.NotifyCraftCompleted(recipe);
                SyncCountsFromActiveEntry();
                SetActiveEntryProgress(0f);
                HandleInventoryChanged();
                UpdateFloatingProgressVisibility();
                FlushCurrentRuntimeStateToPersistence();
            }

            _craftRoutine = null;
            SetActiveEntryProgress(0f);
            bool completedRecipeWasSelected = IsRecipeCurrentlySelected(recipe);
            if (completedRecipeWasSelected)
            {
                _selectedQuantity = 0;
            }
            _lastCompletedQueueTotal = _activeQueueEntry != null ? Mathf.Max(0, _activeQueueEntry.totalCount) : successCount;
            _lastCompletedRecipeId = successCount > 0 ? recipe.recipeID : -1;
            _craftingRecipe = null;
            SyncCountsFromActiveEntry();
            ResetEntryRuntimeState(_activeQueueEntry);
            _activeQueueEntry = null;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            SetActiveEntryReserved(false);
            SetWorkbenchAnimating(false);
            RefreshAll(
                validateRecipeRows: true,
                rebuildRecipeGeometry: true,
                rebuildDetailGeometry: completedRecipeWasSelected);
            UpdateFloatingProgressVisibility();
            FlushCurrentRuntimeStateToPersistence();
            if (!_isVisible && !HasRawWorkbenchState)
            {
                CleanupTransientState(resetSession: true);
            }
        }

        private void StopCraftRoutine()
        {
            if (_craftRoutine == null && !HasTrackedQueueEntries)
            {
                return;
            }

            if (_craftRoutine != null)
            {
                StopCoroutine(_craftRoutine);
            }

            RefundAllPendingReservedMaterials();

            _craftRoutine = null;
            SetActiveEntryProgress(0f);
            _craftingRecipe = null;
            ResetEntryRuntimeState(_activeQueueEntry);
            SetActiveEntryReserved(false);
            SyncCountsFromActiveEntry();
            if (_activeQueueEntry != null)
            {
                _lastCompletedQueueTotal = Mathf.Max(0, _activeQueueEntry.totalCount);
                _lastCompletedRecipeId = _lastCompletedQueueTotal > 0 ? _activeQueueEntry.recipeId : -1;
                RemoveQueueEntryIfEmpty(_activeQueueEntry);
            }

            _activeQueueEntry = null;
            _selectedQuantity = 0;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            SetWorkbenchAnimating(false);
            RefreshSelection(forceDetailGeometry: false);
            UpdateQuantityUi();
            UpdateFloatingProgressVisibility();
        }

        private void MaintainWorkbenchPose(PlayerMovement playerMovement)
        {
            if (playerMovement == null || _anchorTarget == null)
            {
                return;
            }

            if (GetOverlayAutoHideDistance(playerMovement.transform) > Mathf.Max(0.2f, GetCurrentAutoHideDistance()))
            {
                return;
            }

            Vector2 direction = GetAnchorBounds().center - playerMovement.transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                playerMovement.SetFacingDirection(direction);
            }
        }

        private bool CanCraftSelected(RecipeData recipe, out string blockerMessage)
        {
            blockerMessage = string.Empty;
            if (_craftingService == null || recipe == null)
            {
                blockerMessage = "工作台暂未接入制作服务。";
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            if (director != null && !director.CanPerformWorkbenchCraft(out blockerMessage))
            {
                return false;
            }

            if (!_craftingService.IsRecipeUnlocked(recipe))
            {
                blockerMessage = "这张配方目前还没有解锁。";
                return false;
            }

            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            if (EntryHasUnclaimedCompletedOutputs(entry))
            {
                blockerMessage = "先领取这件做好的物品。";
                return false;
            }

            if (GetMaxCraftableCount(recipe) <= 0)
            {
                blockerMessage = "材料还不够，先把清单里的材料收齐。";
                return false;
            }

            return true;
        }

        private int GetMaxCraftableCount(RecipeData recipe)
        {
            if (_craftingService == null || recipe == null || !_craftingService.IsRecipeUnlocked(recipe) || recipe.ingredients == null || recipe.ingredients.Count == 0)
            {
                return 0;
            }

            int materialCap = int.MaxValue;
            foreach (RecipeIngredient ingredient in recipe.ingredients)
            {
                if (ingredient.amount <= 0)
                {
                    continue;
                }

                materialCap = Mathf.Min(materialCap, _craftingService.GetMaterialCount(ingredient.itemID) / ingredient.amount);
            }

            if (materialCap == int.MaxValue)
            {
                materialCap = 0;
            }

            return Mathf.Max(0, materialCap);
        }

        private string GetRecipeDisplayName(RecipeData recipe, ItemData item)
        {
            int resultItemId = recipe != null ? recipe.resultItemID : item != null ? item.itemID : -1;
            string mappedName = GetWorkbenchPlayerFacingName(resultItemId);
            if (!string.IsNullOrWhiteSpace(mappedName))
            {
                return mappedName;
            }

            string recipeName = SanitizeWorkbenchDisplayName(recipe != null ? recipe.recipeName : string.Empty);
            string itemName = GetItemDisplayName(item != null ? item.itemID : resultItemId, item);

            if (!string.IsNullOrWhiteSpace(recipeName))
            {
                return recipeName;
            }

            if (!string.IsNullOrWhiteSpace(itemName))
            {
                return itemName;
            }

            return recipe != null ? $"配方 {recipe.recipeID}" : "未知配方";
        }

        private string GetRecipeDisplayDescription(RecipeData recipe, ItemData item)
        {
            int resultItemId = recipe != null ? recipe.resultItemID : item != null ? item.itemID : -1;
            string mappedDescription = GetWorkbenchPlayerFacingDescription(resultItemId);
            if (!string.IsNullOrWhiteSpace(mappedDescription))
            {
                return mappedDescription;
            }

            string recipeDescription = recipe != null ? recipe.description?.Trim() : string.Empty;
            if (!string.IsNullOrWhiteSpace(recipeDescription))
            {
                return recipeDescription;
            }

            string itemDescription = item != null ? item.description?.Trim() : string.Empty;
            return !string.IsNullOrWhiteSpace(itemDescription) ? itemDescription : "暂时还没有更多说明。";
        }

        private void ApplyDetailColumnFontTuning()
        {
            ApplyMinimumFontSize(selectedNameText, 17f);
            ApplyMinimumFontSize(selectedDescriptionText, 11.5f);
            ApplyMinimumFontSize(selectedMaterialsText, 11.5f);
            ApplyMinimumFontSize(stageHintText, 10.5f);
            ApplyMinimumFontSize(progressLabelText, 10.8f);
            ApplyMinimumFontSize(quantityValueText, 11.8f);
            ApplyMinimumFontSize(craftButtonLabel, 11.8f);

            TextMeshProUGUI materialsTitle = _materialsTitleRect != null
                ? _materialsTitleRect.GetComponent<TextMeshProUGUI>()
                : null;
            ApplyMinimumFontSize(materialsTitle, 10.8f);

            TextMeshProUGUI quantityTitle = _quantityTitleRect != null
                ? _quantityTitleRect.GetComponent<TextMeshProUGUI>()
                : null;
            ApplyMinimumFontSize(quantityTitle, 10.8f);
        }

        private static void ApplyMinimumFontSize(TextMeshProUGUI text, float minimumFontSize)
        {
            if (text == null)
            {
                return;
            }

            text.enableAutoSizing = false;
            if (text.fontSize < minimumFontSize)
            {
                text.fontSize = minimumFontSize;
            }
        }

        private string GetItemDisplayName(int itemId, ItemData item)
        {
            string mappedName = GetWorkbenchPlayerFacingName(itemId);
            if (!string.IsNullOrWhiteSpace(mappedName))
            {
                return mappedName;
            }

            string source = item != null ? item.itemName : string.Empty;
            string sanitized = SanitizeWorkbenchDisplayName(source);
            if (!string.IsNullOrWhiteSpace(sanitized))
            {
                return sanitized;
            }

            if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(BuildPlayerFacingInternalName(source, itemId)))
            {
                return BuildPlayerFacingInternalName(source, itemId);
            }

            return item != null ? $"材料 {item.itemID}" : $"材料 {itemId}";
        }

        private static string GetWorkbenchPlayerFacingName(int itemId)
        {
            return itemId switch
            {
                0 => "木斧",
                1 => "石斧",
                2 => "铁斧",
                3 => "黄铜斧",
                4 => "钢斧",
                5 => "金斧",
                6 => "木镐",
                7 => "石镐",
                8 => "铁镐",
                9 => "黄铜镐",
                10 => "钢镐",
                11 => "金镐",
                12 => "木锄",
                13 => "石锄",
                14 => "铁锄",
                15 => "黄铜锄",
                16 => "钢锄",
                17 => "金锄",
                18 => "洒水壶",
                200 => "木剑",
                201 => "石剑",
                202 => "铁剑",
                203 => "黄铜剑",
                204 => "钢剑",
                205 => "金剑",
                1400 => "小木箱子",
                1401 => "大木箱子",
                1402 => "小铁箱子",
                1403 => "大铁箱子",
                _ => string.Empty
            };
        }

        private static string GetWorkbenchPlayerFacingDescription(int itemId)
        {
            return itemId switch
            {
                0 => "用木料拼成的简易斧头，足够砍下最开始需要的木材。",
                1 => "嵌了石料的斧头，比木斧更结实，砍伐更顺手。",
                2 => "用生铁加固的斧头，适合更稳定地砍树。",
                3 => "黄铜包口的斧头，挥起来更稳当。",
                4 => "钢制斧头，更适合高强度砍伐。",
                5 => "做工精细的金斧，砍伐时更省力。",
                6 => "简单拼成的木镐，先拿它敲开最基础的矿石。",
                7 => "石料补强的镐子，比木镐更耐用。",
                8 => "生铁加固的镐子，挖矿时更稳更顺手。",
                9 => "黄铜包头的镐子，敲击更利落。",
                10 => "钢制镐子，适合更高强度的采集。",
                11 => "做工精细的金镐，适合更高阶的采集。",
                12 => "最基础的木锄，先把地翻开再说。",
                13 => "石料补强的锄头，翻地更省力。",
                14 => "生铁加固的锄头，整理田地更顺手。",
                15 => "黄铜包口的锄头，翻土更利落。",
                16 => "钢制锄头，适合更高强度的耕作。",
                17 => "做工精细的金锄，适合长时间耕作。",
                18 => "装上水后，就能用来浇灌作物。",
                200 => "练习用的木剑，遇到危险时也能先护身。",
                201 => "嵌了石料的剑，比木剑更结实一些。",
                202 => "生铁打造的剑，挥起来更有分量。",
                203 => "黄铜加固的剑，手感更稳。",
                204 => "钢制长剑，适合更激烈的战斗。",
                205 => "做工精细的金剑，是更高阶的护身武器。",
                1400 => "简单的木箱，能收纳常用物品。",
                1401 => "更大的木箱，能放下更多材料和工具。",
                1402 => "包了铁边的小箱子，更结实也更能装。",
                1403 => "容量更大的铁箱，适合集中收纳工具和材料。",
                _ => string.Empty
            };
        }

        private static string BuildPlayerFacingInternalName(string source, int itemId)
        {
            string mappedName = GetWorkbenchPlayerFacingName(itemId);
            if (!string.IsNullOrWhiteSpace(mappedName))
            {
                return mappedName;
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            string lower = source.ToLowerInvariant();
            if (lower.Contains("wateringcan"))
            {
                return "洒水壶";
            }

            if (lower.Contains("pickaxe"))
            {
                return "镐子";
            }

            if (lower.Contains("axe"))
            {
                return "斧头";
            }

            if (lower.Contains("hoe"))
            {
                return "锄头";
            }

            if (lower.Contains("storage"))
            {
                return "箱子";
            }

            if (lower.Contains("sword"))
            {
                return "剑";
            }

            return source.Replace('_', ' ');
        }

        private static string SanitizeWorkbenchDisplayName(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            string trimmed = rawValue.Trim();
            string withoutIdPrefix = StripLeadingNumericPrefix(trimmed);
            string candidate = !string.IsNullOrWhiteSpace(withoutIdPrefix) ? withoutIdPrefix : trimmed;

            if (LooksLikeInternalRecipeName(candidate))
            {
                return string.Empty;
            }

            return candidate;
        }

        private static string StripLeadingNumericPrefix(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            int index = 0;
            while (index < rawValue.Length && char.IsDigit(rawValue[index]))
            {
                index++;
            }

            if (index > 0 && index < rawValue.Length && rawValue[index] == '_')
            {
                return rawValue.Substring(index + 1).Trim();
            }

            return rawValue.Trim();
        }

        private static bool LooksLikeInternalRecipeName(string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return false;
            }

            bool hasAsciiUnderscore = false;
            bool hasAsciiDigit = false;
            bool hasWhitespace = false;
            for (int index = 0; index < recipeName.Length; index++)
            {
                char current = recipeName[index];
                if (current == '_')
                {
                    hasAsciiUnderscore = true;
                    continue;
                }

                if (char.IsDigit(current))
                {
                    hasAsciiDigit = true;
                }

                if (char.IsWhiteSpace(current))
                {
                    hasWhitespace = true;
                }

                if (current > 127)
                {
                    return false;
                }

                if (!(char.IsLetterOrDigit(current) || current == '-' || current == ' '))
                {
                    return false;
                }
            }

            return hasAsciiUnderscore || hasAsciiDigit || !hasWhitespace;
        }

        private WorkbenchQueueEntry GetQueueEntry(RecipeData recipe, bool createIfMissing = false)
        {
            if (recipe == null)
            {
                return null;
            }

            for (int index = 0; index < _queueEntries.Count; index++)
            {
                WorkbenchQueueEntry entry = _queueEntries[index];
                if (entry != null && entry.recipe == recipe)
                {
                    return entry;
                }
            }

            if (!createIfMissing)
            {
                return null;
            }

            WorkbenchQueueEntry created = new WorkbenchQueueEntry
            {
                recipe = recipe,
                recipeId = recipe.recipeID,
                resultItemId = recipe.resultItemID,
                resultAmountPerCraft = Mathf.Max(1, recipe.resultAmount)
            };
            _queueEntries.Add(created);
            return created;
        }

        private WorkbenchQueueEntry GetSelectedQueueEntry()
        {
            return GetQueueEntry(GetSelectedRecipe());
        }

        private void RemoveQueueEntryIfEmpty(WorkbenchQueueEntry entry)
        {
            if (entry == null || entry.totalCount > 0 || entry.readyCount > 0)
            {
                return;
            }

            _queueEntries.Remove(entry);
            if (_activeQueueEntry == entry)
            {
                _activeQueueEntry = null;
            }
        }

        private bool HasReadyOutputs()
        {
            return _queueEntries.Any(entry => entry != null && entry.readyCount > 0);
        }

        private bool HasTrackedQueueEntries =>
            _queueEntries.Any(entry => entry != null && (entry.totalCount > 0 || entry.readyCount > 0));

        private void SyncCountsFromActiveEntry()
        {
            if (_activeQueueEntry == null)
            {
                _craftQueueTotal = 0;
                _craftQueueCompleted = 0;
                return;
            }

            _craftQueueTotal = Mathf.Max(0, _activeQueueEntry.totalCount);
            _craftQueueCompleted = Mathf.Clamp(_activeQueueEntry.readyCount, 0, _craftQueueTotal);
        }

        private static bool HasPendingCrafts(WorkbenchQueueEntry entry)
        {
            return entry != null && entry.totalCount > entry.readyCount;
        }

        private bool IsActiveEntry(WorkbenchQueueEntry entry)
        {
            return entry != null && entry == _activeQueueEntry && HasActiveCraftQueue;
        }

        private static float GetEntryUnitProgress(WorkbenchQueueEntry entry)
        {
            return entry != null ? Mathf.Clamp01(entry.currentUnitProgress) : 0f;
        }

        private static void ResetEntryRuntimeState(WorkbenchQueueEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            entry.currentUnitProgress = 0f;
            entry.hasReservedCurrentCraft = false;
        }

        private void SetActiveEntryProgress(float progress)
        {
            _craftProgress = Mathf.Clamp01(progress);
            if (_activeQueueEntry != null)
            {
                _activeQueueEntry.currentUnitProgress = _craftProgress;
            }
        }

        private void SetActiveEntryReserved(bool reserved)
        {
            _hasReservedActiveCraft = reserved;
            if (_activeQueueEntry != null)
            {
                _activeQueueEntry.hasReservedCurrentCraft = reserved;
            }
        }

        private bool CanPickupEntryOutputs(WorkbenchQueueEntry entry)
        {
            return entry != null && entry.readyCount > 0 && _selectedQuantity <= 0;
        }

        private bool CanInterruptActiveEntry(WorkbenchQueueEntry entry)
        {
            return IsActiveEntry(entry) && !CanPickupEntryOutputs(entry) && _selectedQuantity <= 0;
        }

        private bool CanCancelQueuedEntry(WorkbenchQueueEntry entry)
        {
            return entry != null
                && !IsActiveEntry(entry)
                && !EntryHasReadyOutputs(entry)
                && HasPendingCrafts(entry)
                && _selectedQuantity <= 0;
        }

        private WorkbenchQueueEntry FindNextPendingQueueEntry()
        {
            for (int queueIndex = 0; queueIndex < _queueEntries.Count; queueIndex++)
            {
                WorkbenchQueueEntry pendingEntry = _queueEntries[queueIndex];
                if (HasPendingCrafts(pendingEntry))
                {
                    return pendingEntry;
                }
            }

            return null;
        }

        private void ResumeNextPendingCraftQueueIfAny()
        {
            if (_craftRoutine != null || _craftingService == null)
            {
                return;
            }

            WorkbenchQueueEntry nextEntry = FindNextPendingQueueEntry();
            if (nextEntry == null || nextEntry.recipe == null)
            {
                _activeQueueEntry = null;
                _craftingRecipe = null;
                SyncCountsFromActiveEntry();
                SetWorkbenchAnimating(false);
                FlushCurrentRuntimeStateToPersistence();
                return;
            }

            _activeQueueEntry = nextEntry;
            _craftingRecipe = nextEntry.recipe;
            ResetEntryRuntimeState(nextEntry);
            SetActiveEntryReserved(false);
            SetActiveEntryProgress(0f);
            _selectedQuantity = 0;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            SyncCountsFromActiveEntry();
            RefreshSelection(forceDetailGeometry: false);
            UpdateQuantityUi();
            UpdateFloatingProgressVisibility();
            _craftRoutine = StartCoroutine(CraftRoutine(nextEntry.recipe, 0));
            FlushCurrentRuntimeStateToPersistence();
        }

        private int GetDisplayedReadyCount(RecipeData recipe)
        {
            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            return entry != null ? Mathf.Max(0, entry.readyCount) : 0;
        }

        private int GetDisplayedTotalCount(RecipeData recipe)
        {
            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            if (entry != null)
            {
                return Mathf.Max(0, entry.totalCount);
            }

            if (recipe != null && recipe.recipeID == _lastCompletedRecipeId)
            {
                return Mathf.Max(0, _lastCompletedQueueTotal);
            }

            return 0;
        }

        private bool TryPickupSelectedOutputs()
        {
            RecipeData recipe = GetSelectedRecipe();
            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            if (recipe == null || entry == null || entry.readyCount <= 0 || _inventoryService == null)
            {
                return false;
            }

            int readyCraftCount = Mathf.Max(0, entry.readyCount);
            int totalAmount = readyCraftCount * Mathf.Max(1, entry.resultAmountPerCraft);
            int remaining = _inventoryService.AddItem(entry.resultItemId, 0, totalAmount);
            if (remaining > 0)
            {
                ItemDropHelper.DropAtPosition(new ItemStack(entry.resultItemId, 0, remaining), GetPickupDropPosition());
            }

            entry.totalCount = Mathf.Max(0, entry.totalCount - readyCraftCount);
            entry.readyCount = 0;
            if (_activeQueueEntry == entry)
            {
                SyncCountsFromActiveEntry();
            }

            _lastCompletedQueueTotal = Mathf.Max(0, entry.totalCount);
            _lastCompletedRecipeId = entry.totalCount > 0 ? entry.recipeId : -1;
            RemoveQueueEntryIfEmpty(entry);

            if (!HasRawWorkbenchState && !_isVisible)
            {
                CleanupTransientState(resetSession: true);
            }

            HandleInventoryChanged();
            FlushCurrentRuntimeStateToPersistence();
            return true;
        }

        private void CancelActiveCraftQueue()
        {
            if (_craftRoutine == null || _activeQueueEntry == null)
            {
                return;
            }

            WorkbenchQueueEntry canceledEntry = _activeQueueEntry;
            bool refundedReservedCraft = RefundPendingReservedMaterials(canceledEntry);

            canceledEntry.totalCount = Mathf.Max(0, canceledEntry.readyCount);
            ResetEntryRuntimeState(canceledEntry);
            SetActiveEntryReserved(false);

            StopCoroutine(_craftRoutine);
            _craftRoutine = null;
            SetActiveEntryProgress(0f);
            _craftingRecipe = null;
            SyncCountsFromActiveEntry();
            _lastCompletedQueueTotal = Mathf.Max(0, canceledEntry.totalCount);
            _lastCompletedRecipeId = _lastCompletedQueueTotal > 0 ? canceledEntry.recipeId : -1;
            RemoveQueueEntryIfEmpty(canceledEntry);
            _activeQueueEntry = null;
            _selectedQuantity = 0;
            _craftButtonHovered = false;
            _progressBarHovered = false;
            RefreshVisibleWorkbenchUi();
            UpdateFloatingProgressVisibility();
            if (refundedReservedCraft)
            {
                HandleInventoryChanged();
            }

            ResumeNextPendingCraftQueueIfAny();
            FlushCurrentRuntimeStateToPersistence();

            if (!HasRawWorkbenchState && !_isVisible)
            {
                CleanupTransientState(resetSession: true);
            }
        }

        private void CancelQueuedRecipeEntry(WorkbenchQueueEntry entry)
        {
            if (entry == null || entry == _activeQueueEntry)
            {
                return;
            }

            bool refundedReservedCraft = RefundPendingReservedMaterials(entry);
            entry.totalCount = Mathf.Max(0, entry.readyCount);
            _lastCompletedQueueTotal = Mathf.Max(0, entry.totalCount);
            _lastCompletedRecipeId = entry.totalCount > 0 ? entry.recipeId : -1;
            RemoveQueueEntryIfEmpty(entry);
            _selectedQuantity = 0;
            RefreshVisibleWorkbenchUi();
            UpdateFloatingProgressVisibility();
            if (refundedReservedCraft)
            {
                HandleInventoryChanged();
            }

            FlushCurrentRuntimeStateToPersistence();

            if (!HasRawWorkbenchState && !_isVisible)
            {
                CleanupTransientState(resetSession: true);
            }
        }

        private void RefundReservedCraftMaterials(RecipeData recipe, int craftCount)
        {
            if (recipe == null || recipe.ingredients == null || _inventoryService == null || craftCount <= 0)
            {
                return;
            }

            for (int index = 0; index < recipe.ingredients.Count; index++)
            {
                RecipeIngredient ingredient = recipe.ingredients[index];
                int refundAmount = Mathf.Max(0, ingredient.amount) * craftCount;
                if (refundAmount <= 0)
                {
                    continue;
                }

                int remaining = _inventoryService.AddItem(ingredient.itemID, 0, refundAmount);
                if (remaining > 0)
                {
                    ItemDropHelper.DropAtPosition(new ItemStack(ingredient.itemID, 0, remaining), GetPickupDropPosition());
                }
            }
        }

        private bool RefundPendingReservedMaterials(WorkbenchQueueEntry entry)
        {
            if (entry == null || entry.recipe == null)
            {
                return false;
            }

            int pendingCount = GetPendingCraftCount(entry);
            if (pendingCount <= 0)
            {
                return false;
            }

            RefundReservedCraftMaterials(entry.recipe, pendingCount);
            return true;
        }

        private void RefundAllPendingReservedMaterials()
        {
            for (int index = 0; index < _queueEntries.Count; index++)
            {
                WorkbenchQueueEntry entry = _queueEntries[index];
                if (!RefundPendingReservedMaterials(entry))
                {
                    continue;
                }

                entry.totalCount = Mathf.Max(0, entry.readyCount);
                ResetEntryRuntimeState(entry);
            }
        }

        private Vector3 GetPickupDropPosition()
        {
            if (_anchorTarget != null)
            {
                return _anchorTarget.position;
            }

            if (_playerTransform != null)
            {
                return _playerTransform.position;
            }

            return Vector3.zero;
        }

        private string BuildRowSummary(RecipeData recipe)
        {
            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            if (entry != null)
            {
                int readyCount = Mathf.Max(0, entry.readyCount);
                int totalCount = Mathf.Max(0, entry.totalCount);
                if (IsActiveEntry(entry))
                {
                    return $"进度 {readyCount}/{Mathf.Max(1, totalCount)}";
                }

                if (readyCount > 0)
                {
                    return $"制作完成 {readyCount}个";
                }

                if (HasPendingCrafts(entry))
                {
                    return $"排队 {Mathf.Max(0, totalCount - readyCount)}个";
                }
            }

            float previewDuration = Mathf.Max(0.25f, recipe.craftingTime > 0f ? recipe.craftingTime : defaultCraftDuration);
            string materialSummary = recipe.ingredients == null || recipe.ingredients.Count == 0
                ? "免材料"
                : $"{recipe.ingredients.Count}项材料";
            return $"{previewDuration:0.0}s · {materialSummary}";
        }

        private string BuildMaterialsText(RecipeData recipe, int quantity)
        {
            int effectiveQuantity = Mathf.Max(1, quantity);
            List<string> lines = new();
            foreach (RecipeIngredient ingredient in recipe.ingredients)
            {
                int owned = _craftingService != null ? _craftingService.GetMaterialCount(ingredient.itemID) : 0;
                int required = ingredient.amount * effectiveQuantity;
                string color = owned >= required ? "#B7E6C2" : "#F0B49E";
                ItemData materialItem = ResolveItem(ingredient.itemID);
                string materialName = GetItemDisplayName(ingredient.itemID, materialItem);
                lines.Add($"<color={color}>{materialName}</color>  {owned}/{required}");
            }

            return string.Join("\n", lines);
        }

        private string BuildStageHint(RecipeData recipe)
        {
            return string.Empty;
        }

        private void UpdateProgressLabel(RecipeData recipe)
        {
            EnsureProgressLabelBinding();
            EnsureProgressFillGraphic(progressFillImage);
            if (progressFillImage == null || progressLabelText == null)
            {
                return;
            }

            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            int totalCount = entry != null ? Mathf.Max(0, entry.totalCount) : 0;
            int readyCount = entry != null ? Mathf.Max(0, entry.readyCount) : 0;
            bool hasReadyOutputs = EntryHasReadyOutputs(entry);
            bool hasPendingCrafts = HasPendingCrafts(entry);
            bool isSelectedEntryActive = IsActiveEntry(entry);
            bool showCraftButton = ShouldShowCraftButton(recipe, entry);
            string labelText;
            Color fillColor;
            float fillAmount;

            if (isSelectedEntryActive)
            {
                if (_progressBarHovered && CanPickupEntryOutputs(entry))
                {
                    fillAmount = 1f;
                    fillColor = CompletedProgressFillColor;
                    labelText = "领取产物";
                }
                else if (_progressBarHovered && CanInterruptActiveEntry(entry))
                {
                    fillAmount = 1f;
                    fillColor = CancelProgressFillColor;
                    labelText = "中断制作";
                }
                else
                {
                    fillAmount = GetEntryUnitProgress(entry);
                    fillColor = ActiveProgressFillColor;
                    labelText = $"进度  {readyCount}/{Mathf.Max(1, totalCount)}";
                }

                progressFillImage.fillAmount = fillAmount;
                progressFillImage.color = fillColor;
                ApplyProgressLabelState(labelText, ProgressLabelReadableColor);
                PushDirectorCraftProgress();
                return;
            }

            if (hasReadyOutputs)
            {
                fillAmount = 1f;
                fillColor = CompletedProgressFillColor;
                labelText = _progressBarHovered ? "领取产物" : $"制作完成 {readyCount}个";
            }
            else if (CanCancelQueuedEntry(entry))
            {
                fillAmount = 0f;
                fillColor = _progressBarHovered ? CancelProgressFillColor : ActiveProgressIdleColor;
                labelText = _progressBarHovered ? "取消排队" : $"排队  {GetPendingCraftCount(entry)}个";
            }
            else if (hasPendingCrafts)
            {
                fillAmount = 0f;
                fillColor = ActiveProgressIdleColor;
                labelText = $"排队  {GetPendingCraftCount(entry)}个";
            }
            else if (showCraftButton)
            {
                fillAmount = 0f;
                fillColor = ActiveProgressIdleColor;
                labelText = string.Empty;
            }
            else
            {
                fillAmount = 0f;
                fillColor = ActiveProgressEmptyColor;
                labelText = "进度  0/0";
            }

            progressFillImage.fillAmount = fillAmount;
            progressFillImage.color = fillColor;
            ApplyProgressLabelState(labelText, ProgressLabelReadableColor);
            PushDirectorCraftProgress();
        }

        private bool ShouldShowCompletedProgress(RecipeData recipe, WorkbenchQueueEntry entry = null)
        {
            entry ??= GetQueueEntry(recipe);
            return entry != null && entry.readyCount > 0 && !HasPendingCrafts(entry);
        }

        private void SetWorkbenchAnimating(bool active)
        {
            if (_workbenchAnimator != null && !string.IsNullOrWhiteSpace(_workbenchAnimatorBoolName))
            {
                _workbenchAnimator.SetBool(_workbenchAnimatorBoolName, active);
                if (!active)
                {
                    _workbenchAnimator = null;
                    _workbenchAnimatorBoolName = null;
                }

                return;
            }

            if (_anchorTarget == null)
            {
                return;
            }

            if (_workbenchAnimator == null)
            {
                _workbenchAnimator = _anchorTarget.GetComponent<Animator>() ?? _anchorTarget.GetComponentInChildren<Animator>(true);
                if (_workbenchAnimator != null)
                {
                    for (int index = 0; index < WorkbenchAnimatorBoolNames.Length; index++)
                    {
                        if (_workbenchAnimator.parameters.Any(parameter => parameter.type == AnimatorControllerParameterType.Bool && parameter.name == WorkbenchAnimatorBoolNames[index]))
                        {
                            _workbenchAnimatorBoolName = WorkbenchAnimatorBoolNames[index];
                            break;
                        }
                    }
                }
            }

            if (_workbenchAnimator != null && !string.IsNullOrWhiteSpace(_workbenchAnimatorBoolName))
            {
                _workbenchAnimator.SetBool(_workbenchAnimatorBoolName, active);
            }
        }

        private RecipeData GetSelectedRecipe() => _selectedIndex >= 0 && _selectedIndex < _recipes.Count ? _recipes[_selectedIndex] : null;

        private RecipeData ResolveRecipeById(int recipeId)
        {
            if (recipeId < 0 || !EnsureRecipesLoaded())
            {
                return null;
            }

            for (int index = 0; index < _recipes.Count; index++)
            {
                RecipeData recipe = _recipes[index];
                if (recipe != null && recipe.recipeID == recipeId)
                {
                    return recipe;
                }
            }

            return null;
        }

        private int FindRecipeIndex(RecipeData recipe)
        {
            if (recipe == null)
            {
                return -1;
            }

            for (int index = 0; index < _recipes.Count; index++)
            {
                if (_recipes[index] == recipe || _recipes[index].recipeID == recipe.recipeID)
                {
                    return index;
                }
            }

            return -1;
        }

        private static int GetRecipeSortOrder(RecipeData recipe)
        {
            return recipe != null ? recipe.resultItemID : int.MaxValue;
        }

        private ItemData ResolveItem(int itemId)
        {
            ItemDatabase database = _craftingService != null ? _craftingService.Database : _inventoryService != null ? _inventoryService.Database : null;
            if (database == null)
            {
                _fallbackItemDatabase ??= AssetLocator.LoadItemDatabase();
                database = _fallbackItemDatabase;
            }

            return database != null ? database.GetItemByID(itemId) : null;
        }

        private void BindInventory(InventoryService inventory)
        {
            if (_inventoryService == inventory)
            {
                return;
            }

            UnbindInventory();
            _inventoryService = inventory;
            if (_inventoryService != null)
            {
                _inventoryService.OnSlotChanged += HandleInventorySlotChanged;
                _inventoryService.OnInventoryChanged += HandleInventoryChanged;
            }
        }

        private void UnbindInventory()
        {
            if (_inventoryService != null)
            {
                _inventoryService.OnSlotChanged -= HandleInventorySlotChanged;
                _inventoryService.OnInventoryChanged -= HandleInventoryChanged;
                _inventoryService = null;
            }
        }

        private void HandleInventorySlotChanged(int _)
        {
            HandleInventoryChanged();
        }

        private void HandleInventoryChanged()
        {
            if (_isVisible)
            {
                RefreshRows(allowRecovery: false, rebuildGeometry: false);
                RefreshSelection(forceDetailGeometry: false);
                UpdateQuantityUi();
            }

            UpdateFloatingProgressVisibility();
        }

        private void ApplyDisplayDirection(bool displayBelow)
        {
            bool directionChanged = false;
            if (_hasDisplayDirectionDecision &&
                displayBelow != _displayBelow &&
                _playerTransform != null)
            {
                float verticalDelta = GetDisplayDecisionSamplePoint(_playerTransform).y - GetAnchorBounds().center.y;
                if (Mathf.Abs(verticalDelta) < displayDirectionHysteresis)
                {
                    displayBelow = _displayBelow;
                }
            }

            if (_hasDisplayDirectionDecision && displayBelow != _displayBelow)
            {
                directionChanged = true;
            }

            _hasDisplayDirectionDecision = true;
            _displayBelow = displayBelow;
            if (directionChanged)
            {
                _animateDisplayDirectionTransition = true;
                _panelVerticalVelocity = 0f;
            }

            panelRect.pivot = _displayBelow ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            pointerRect.anchorMin = _displayBelow ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            pointerRect.anchorMax = pointerRect.anchorMin;
            pointerRect.pivot = new Vector2(0.5f, 0.5f);
            pointerRect.anchoredPosition = new Vector2(0f, _displayBelow ? 5f : -5f);
        }

        private void SetWorkbenchPanelVisible(bool visible)
        {
            if (panelRect != null && panelRect.gameObject.activeSelf != visible)
            {
                panelRect.gameObject.SetActive(visible);
            }

            if (visible)
            {
                for (int index = 0; index < _floatingCards.Count; index++)
                {
                    if (_floatingCards[index]?.root != null && _floatingCards[index].root.gameObject.activeSelf)
                    {
                        _floatingCards[index].root.gameObject.SetActive(false);
                    }
                }
            }

            if (pointerRect != null &&
                pointerRect.parent != panelRect &&
                pointerRect.gameObject.activeSelf != visible)
            {
                pointerRect.gameObject.SetActive(visible);
            }
        }

        private void Reposition(bool immediate = false)
        {
            if (panelRect == null || rootRect == null || _anchorTarget == null)
            {
                return;
            }

            Bounds bounds = GetAnchorBounds();
            Vector3 worldAnchor = _displayBelow ? new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) : new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Vector2 screenOffset = _displayBelow ? belowOffset : aboveOffset;
            if (!SpringDay1UiLayerUtility.TryProjectWorldToCanvas(overlayCanvas, rootRect, worldAnchor, screenOffset, out Vector2 localPoint))
            {
                SetWorkbenchPanelVisible(false);
                return;
            }

            if (_isVisible)
            {
                SetWorkbenchPanelVisible(true);
            }

            Rect rect = rootRect.rect;
            float minX = rect.xMin + panelWidth * panelRect.pivot.x + screenMargin.x;
            float maxX = rect.xMax - panelWidth * (1f - panelRect.pivot.x) - screenMargin.x;
            float minY = rect.yMin + panelHeight * panelRect.pivot.y + screenMargin.y;
            float maxY = rect.yMax - panelHeight * (1f - panelRect.pivot.y) - screenMargin.y;
            Vector2 target = new(Mathf.Clamp(localPoint.x, minX, maxX), Mathf.Clamp(localPoint.y, minY, maxY));
            Vector2 resolved = target;
            if (!immediate && _animateDisplayDirectionTransition)
            {
                float transitionY = Mathf.SmoothDamp(
                    panelRect.anchoredPosition.y,
                    target.y,
                    ref _panelVerticalVelocity,
                    1f / Mathf.Max(1f, panelFollowSharpness),
                    Mathf.Infinity,
                    Time.unscaledDeltaTime);
                resolved = new Vector2(target.x, transitionY);
                if (Mathf.Abs(transitionY - target.y) <= 0.5f)
                {
                    resolved.y = target.y;
                    _animateDisplayDirectionTransition = false;
                    _panelVerticalVelocity = 0f;
                }
            }
            else if (immediate)
            {
                _animateDisplayDirectionTransition = false;
                _panelVerticalVelocity = 0f;
            }

            panelRect.anchoredPosition = SpringDay1UiLayerUtility.SnapToCanvasPixel(overlayCanvas, resolved);
        }

        private void UpdateFloatingProgressVisibility()
        {
            EnsureFloatingCardRegistry();
            if (_floatingCards.Count == 0)
            {
                return;
            }

            List<FloatingProgressDisplayState> floatingStates = BuildFloatingProgressStates();

            bool panelActuallyVisible = _isVisible || (panelRect != null && panelRect.gameObject.activeInHierarchy);
            bool shouldShow = !panelActuallyVisible
                && floatingStates.Count > 0
                && (HasActiveCraftQueue || floatingStates.Any(state => state.TotalCount > 0))
                && _anchorTarget != null
                && !SpringDay1UiLayerUtility.IsBlockingPageUiOpen()
                && (DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive);

            bool shouldKeepCanvasVisible = _isVisible || shouldShow;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = shouldKeepCanvasVisible ? 1f : 0f;
                canvasGroup.interactable = _isVisible;
                canvasGroup.blocksRaycasts = _isVisible;
            }

            SetWorkbenchPanelVisible(_isVisible);
            if (!shouldShow)
            {
                for (int index = 0; index < _floatingCards.Count; index++)
                {
                    if (_floatingCards[index]?.root != null)
                    {
                        _floatingCards[index].root.gameObject.SetActive(false);
                    }
                }

                return;
            }

            while (_floatingCards.Count < floatingStates.Count)
            {
                CreateFloatingProgressCard(_floatingCards.Count);
            }

            for (int index = 0; index < _floatingCards.Count; index++)
            {
                FloatingProgressCardRefs card = _floatingCards[index];
                if (card?.root == null)
                {
                    continue;
                }

                bool active = index < floatingStates.Count;
                card.root.gameObject.SetActive(active);
                if (!active)
                {
                    continue;
                }

                FloatingProgressDisplayState state = floatingStates[index];
                ItemData item = ResolveItem(state.ResultItemId);
                Sprite iconSprite = item != null ? item.GetBagSprite() : null;
                if (card.icon.sprite != iconSprite)
                {
                    card.icon.sprite = iconSprite;
                }

                Color iconColor = card.icon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
                if (card.icon.color != iconColor)
                {
                    card.icon.color = iconColor;
                }

                card.icon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
                card.icon.rectTransform.sizeDelta = new Vector2(46f, 46f);
                EnsureProgressFillGraphic(card.fill);
                EnsureProgressLabelForeground(card.label);

                if (state.HasActiveCraft)
                {
                    card.fill.fillAmount = state.HasPendingCrafts ? state.ActiveUnitProgress : 1f;
                    card.fill.color = state.HasPendingCrafts
                        ? ActiveProgressFillColor
                        : CompletedProgressFillColor;
                    string labelText = state.HasPendingCrafts
                        ? $"进度  {state.ReadyCount}/{Mathf.Max(1, state.TotalCount)}"
                        : $"制作完成 {state.ReadyCount}个";
                    if (!string.Equals(card.label.text, labelText, System.StringComparison.Ordinal))
                    {
                        card.label.text = labelText;
                    }
                }
                else if (state.ReadyCount > 0 && state.HasPendingCrafts)
                {
                    card.fill.fillAmount = 1f;
                    card.fill.color = CompletedProgressFillColor;
                    string labelText = $"进度  {state.ReadyCount}/{Mathf.Max(1, state.TotalCount)}";
                    if (!string.Equals(card.label.text, labelText, System.StringComparison.Ordinal))
                    {
                        card.label.text = labelText;
                    }
                }
                else if (state.ReadyCount > 0)
                {
                    card.fill.fillAmount = 1f;
                    card.fill.color = CompletedProgressFillColor;
                    string labelText = $"制作完成 {state.ReadyCount}个";
                    if (!string.Equals(card.label.text, labelText, System.StringComparison.Ordinal))
                    {
                        card.label.text = labelText;
                    }
                }
                else
                {
                    card.fill.fillAmount = 0f;
                    card.fill.color = ActiveProgressIdleColor;
                    string labelText = $"进度  0/{Mathf.Max(1, state.TotalCount)}";
                    if (!string.Equals(card.label.text, labelText, System.StringComparison.Ordinal))
                    {
                        card.label.text = labelText;
                    }
                }

                card.label.color = ProgressLabelReadableColor;
            }

            RepositionFloatingProgressCards(floatingStates.Count);
        }

        private List<FloatingProgressDisplayState> BuildFloatingProgressStates()
        {
            _floatingStateBuffer.Clear();
            for (int index = 0; index < _queueEntries.Count; index++)
            {
                WorkbenchQueueEntry entry = _queueEntries[index];
                if (entry == null || (entry.totalCount <= 0 && entry.readyCount <= 0))
                {
                    continue;
                }

                FloatingProgressDisplayState state = GetReusableFloatingState(_floatingStateBuffer.Count);
                state.RecipeId = entry.recipeId;
                state.ResultItemId = entry.resultItemId;
                state.TotalCount = Mathf.Max(0, entry.totalCount);
                state.ReadyCount = Mathf.Max(0, entry.readyCount);
                state.HasActiveCraft = false;
                state.ActiveUnitProgress = 0f;
                if (entry == _activeQueueEntry)
                {
                    state.HasActiveCraft = true;
                    state.ActiveUnitProgress = GetEntryUnitProgress(entry);
                }

                _floatingStateBuffer.Add(state);
            }

            _floatingStateBuffer.Sort(CompareFloatingProgressStates);
            if (_floatingStateBuffer.Count > MaxFloatingProgressCards)
            {
                _floatingStateBuffer.RemoveRange(MaxFloatingProgressCards, _floatingStateBuffer.Count - MaxFloatingProgressCards);
            }

            return _floatingStateBuffer;
        }

        private FloatingProgressDisplayState GetReusableFloatingState(int index)
        {
            while (_floatingStatePool.Count <= index)
            {
                _floatingStatePool.Add(new FloatingProgressDisplayState());
            }

            return _floatingStatePool[index];
        }

        private static int CompareFloatingProgressStates(FloatingProgressDisplayState left, FloatingProgressDisplayState right)
        {
            int compare = right.HasActiveCraft.CompareTo(left.HasActiveCraft);
            if (compare != 0)
            {
                return compare;
            }

            compare = (right.ReadyCount > 0).CompareTo(left.ReadyCount > 0);
            if (compare != 0)
            {
                return compare;
            }

            compare = left.RecipeId.CompareTo(right.RecipeId);
            if (compare != 0)
            {
                return compare;
            }

            return left.ResultItemId.CompareTo(right.ResultItemId);
        }

        private void RepositionFloatingProgressCards(int visibleCardCount)
        {
            if (_floatingCards.Count == 0 || rootRect == null || _anchorTarget == null)
            {
                return;
            }

            Bounds bounds = GetAnchorBounds();
            Vector3 worldAnchor = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            if (!SpringDay1UiLayerUtility.TryProjectWorldToCanvas(
                    overlayCanvas,
                    rootRect,
                    worldAnchor,
                    new Vector2(0f, 26f),
                    out Vector2 localPoint))
            {
                for (int index = 0; index < _floatingCards.Count; index++)
                {
                    if (_floatingCards[index]?.root != null)
                    {
                        _floatingCards[index].root.gameObject.SetActive(false);
                    }
                }

                return;
            }

            const int columns = 3;
            const float cardSpacingX = 74f;
            const float cardSpacingY = 78f;
            float rowWidth = (Mathf.Min(columns, Mathf.Max(1, visibleCardCount)) - 1) * cardSpacingX;
            for (int index = 0; index < _floatingCards.Count; index++)
            {
                FloatingProgressCardRefs card = _floatingCards[index];
                if (card?.root == null || !card.root.gameObject.activeSelf)
                {
                    continue;
                }

                int row = index / columns;
                int column = index % columns;
                float x = localPoint.x - rowWidth * 0.5f + column * cardSpacingX;
                float y = localPoint.y - row * cardSpacingY;
                card.root.anchoredPosition = SpringDay1UiLayerUtility.SnapToCanvasPixel(overlayCanvas, new Vector2(x, y));
            }
        }

        private int GetRemainingCraftCount()
        {
            return _activeQueueEntry != null
                ? Mathf.Max(0, _activeQueueEntry.totalCount - _activeQueueEntry.readyCount)
                : 0;
        }

        private int GetQueuedCraftCountAfterCurrent()
        {
            if (!HasActiveCraftQueue)
            {
                return 0;
            }

            return Mathf.Max(0, GetRemainingCraftCount() - 1);
        }

        private int GetMaterialPreviewQuantity()
        {
            if (IsActiveEntry(GetSelectedQueueEntry()) && _selectedQuantity <= 0)
            {
                return Mathf.Max(1, GetQueuedCraftCountAfterCurrent());
            }

            return Mathf.Max(1, _selectedQuantity);
        }

        private int GetSelectableQuantityCap(RecipeData recipe)
        {
            if (recipe == null)
            {
                return 0;
            }

            int maxCraftable = Mathf.Max(0, GetMaxCraftableCount(recipe));
            WorkbenchQueueEntry entry = GetQueueEntry(recipe);
            if (EntryHasUnclaimedCompletedOutputs(entry))
            {
                return 0;
            }

            return maxCraftable;
        }

        private string BuildCraftButtonLabel(RecipeData recipe, bool canCraft, string blockerMessage)
        {
            if (recipe == null || _selectedQuantity <= 0)
            {
                return string.Empty;
            }

            bool isSelectedEntryActive = IsActiveEntry(GetQueueEntry(recipe));
            if (HasActiveCraftQueue)
            {
                if (_craftButtonHovered)
                {
                    if (_selectedQuantity > 0)
                    {
                        return isSelectedEntryActive ? $"追加制作 x{_selectedQuantity}" : $"加入队列 x{_selectedQuantity}";
                    }

                    return isSelectedEntryActive ? "中断制作" : string.Empty;
                }

                if (_selectedQuantity > 0 && canCraft)
                {
                    return isSelectedEntryActive
                        ? $"追加制作 x{_selectedQuantity}"
                        : $"加入队列 x{_selectedQuantity}";
                }

                return string.Empty;
            }

            if (!canCraft)
            {
                return string.IsNullOrWhiteSpace(blockerMessage) ? "暂不可制作" : "材料不足";
            }

            return $"开始制作 x{_selectedQuantity}";
        }

        private Bounds GetAnchorBounds()
        {
            CraftingStationInteractable interactable = _anchorTarget != null ? _anchorTarget.GetComponent<CraftingStationInteractable>() : null;
            if (interactable != null)
            {
                return interactable.GetVisualBounds();
            }

            if (SpringDay1UiLayerUtility.TryGetPresentationBounds(_anchorTarget, out Bounds bounds))
            {
                return bounds;
            }

            return new Bounds(_anchorTarget != null ? _anchorTarget.position : Vector3.zero, Vector3.one);
        }

        private float GetBoundaryDistance(Vector2 playerPosition)
        {
            CraftingStationInteractable interactable = _anchorTarget != null ? _anchorTarget.GetComponent<CraftingStationInteractable>() : null;
            if (interactable != null)
            {
                return interactable.GetBoundaryDistance(playerPosition);
            }

            return Vector2.Distance(playerPosition, _anchorTarget != null ? (Vector2)_anchorTarget.position : Vector2.zero);
        }

        private float GetOverlayAutoHideDistance(Vector2 playerPosition)
        {
            float interactionBoundaryDistance = GetBoundaryDistance(playerPosition);
            Bounds visualBounds = GetAnchorBounds();
            Vector3 closestVisualPoint = visualBounds.ClosestPoint(new Vector3(playerPosition.x, playerPosition.y, visualBounds.center.z));
            float visualBoundaryDistance = Vector2.Distance(playerPosition, new Vector2(closestVisualPoint.x, closestVisualPoint.y));
            return Mathf.Max(interactionBoundaryDistance, visualBoundaryDistance);
        }

        private float GetOverlayAutoHideDistance(Transform playerTransform)
        {
            if (playerTransform == null)
            {
                return float.MaxValue;
            }

            return GetOverlayAutoHideDistance(GetAutoHideSamplePoint(playerTransform));
        }

        private bool ShouldDisplayBelow(Vector2 playerPosition)
        {
            CraftingStationInteractable interactable = _anchorTarget != null ? _anchorTarget.GetComponent<CraftingStationInteractable>() : null;
            return interactable != null ? interactable.ShouldDisplayOverlayBelow(playerPosition) : playerPosition.y > GetAnchorBounds().center.y;
        }

        private float GetCurrentAutoHideDistance()
        {
            if (_craftRoutine == null)
            {
                return _autoHideDistance;
            }

            return Mathf.Max(0.58f, Mathf.Min(_autoHideDistance, CraftExitAutoHideDistance));
        }

        private static Vector2 GetAutoHideSamplePoint(Transform target)
        {
            return SpringDay1UiLayerUtility.GetInteractionSamplePoint(target);
        }

        private static Vector2 GetDisplayDecisionSamplePoint(Transform target)
        {
            return TryGetCenterSamplePoint(target, out Vector2 samplePoint)
                ? samplePoint
                : SpringDay1UiLayerUtility.GetInteractionSamplePoint(target);
        }

        private void PushDirectorCraftProgress()
        {
            SpringDay1Director director = SpringDay1Director.Instance;
            if (director == null)
            {
                return;
            }

            WorkbenchQueueEntry activeEntry = _activeQueueEntry;
            RecipeData activeRecipe = activeEntry != null ? activeEntry.recipe : null;
            bool active = HasActiveCraftQueue && activeEntry != null && activeRecipe != null;
            director.NotifyWorkbenchCraftProgress(
                activeRecipe,
                active ? Mathf.Max(1, activeEntry.totalCount) : 0,
                active ? Mathf.Clamp(activeEntry.readyCount, 0, Mathf.Max(1, activeEntry.totalCount)) : 0,
                active ? GetEntryUnitProgress(activeEntry) : 0f,
                active);
        }

        private static bool TryGetCenterSamplePoint(Transform target, out Vector2 samplePoint)
        {
            samplePoint = Vector2.zero;
            if (target == null)
            {
                return false;
            }

            Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(includeInactive: true);
            bool hasCollider = false;
            Bounds combinedBounds = default;

            for (int index = 0; index < colliders.Length; index++)
            {
                Collider2D collider2D = colliders[index];
                if (collider2D == null || !collider2D.enabled)
                {
                    continue;
                }

                if (!hasCollider)
                {
                    combinedBounds = collider2D.bounds;
                    hasCollider = true;
                }
                else
                {
                    combinedBounds.Encapsulate(collider2D.bounds);
                }
            }

            if (hasCollider)
            {
                samplePoint = combinedBounds.center;
                return true;
            }

            if (SpringDay1UiLayerUtility.TryGetPresentationBounds(target, out Bounds presentationBounds))
            {
                samplePoint = presentationBounds.center;
                return true;
            }

            samplePoint = target.position;
            return true;
        }

        private TMP_FontAsset ResolveFont(string preferredText = null)
        {
            return DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(
                preferredText,
                _fontAsset,
                FontCoverageProbeText);
        }

        private static bool CanFontRenderText(TMP_FontAsset fontAsset, string currentText)
        {
            return DialogueChineseFontRuntimeBootstrap.CanRenderText(
                fontAsset,
                currentText,
                FontCoverageProbeText);
        }

        private static string GetFontProbeText(string currentText)
        {
            if (string.IsNullOrWhiteSpace(currentText))
            {
                return FontCoverageProbeText;
            }

            var builder = new System.Text.StringBuilder(currentText.Length);
            bool insideTag = false;
            for (int index = 0; index < currentText.Length; index++)
            {
                char current = currentText[index];
                if (current == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (current == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (!char.IsControl(current))
                {
                    builder.Append(current);
                }
            }

            return builder.Length > 0 ? builder.ToString() : FontCoverageProbeText;
        }

        private static bool IsFontAssetUsable(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null || fontAsset.material == null)
            {
                return false;
            }

            Texture[] atlasTextures = fontAsset.atlasTextures;
            if (atlasTextures == null || atlasTextures.Length == 0)
            {
                return false;
            }

            for (int index = 0; index < atlasTextures.Length; index++)
            {
                Texture atlasTexture = atlasTextures[index];
                if (atlasTexture != null && atlasTexture.width > 1 && atlasTexture.height > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyNavigationBlock(bool enable)
        {
            if (BlockNavOverUiField == null)
            {
                return;
            }

            if (enable)
            {
                if (_navBlockOverrideApplied)
                {
                    return;
                }

                _gameInputManager = FindFirstObjectByType<GameInputManager>(FindObjectsInactive.Include);
                if (_gameInputManager == null)
                {
                    return;
                }

                _navBlockWasEnabled = (bool)BlockNavOverUiField.GetValue(_gameInputManager);
                if (!_navBlockWasEnabled)
                {
                    BlockNavOverUiField.SetValue(_gameInputManager, true);
                }

                _navBlockOverrideApplied = true;
                return;
            }

            if (!_navBlockOverrideApplied || _gameInputManager == null)
            {
                return;
            }

            BlockNavOverUiField.SetValue(_gameInputManager, _navBlockWasEnabled);
            _gameInputManager = null;
            _navBlockOverrideApplied = false;
        }

        private static Transform ResolveParent()
        {
            return SpringDay1UiLayerUtility.ResolveUiParent();
        }

        private static SpringDay1WorkbenchCraftingOverlay InstantiateRuntimePrefab()
        {
            GameObject prefab = LoadRuntimePrefabAsset();
            if (prefab == null || !CanInstantiateRuntimePrefab(prefab))
            {
                return null;
            }

            Transform parent = ResolveParent();
            GameObject instance = parent != null ? Instantiate(prefab, parent, false) : Instantiate(prefab);
            instance.name = prefab.name;
            return instance.GetComponent<SpringDay1WorkbenchCraftingOverlay>();
        }

        private static SpringDay1WorkbenchCraftingOverlay FindReusableRuntimeInstance(bool requireScreenOverlay)
        {
            SpringDay1WorkbenchCraftingOverlay[] candidates = FindObjectsByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < candidates.Length; index++)
            {
                SpringDay1WorkbenchCraftingOverlay candidate = candidates[index];
                if (CanReuseRuntimeInstance(candidate, requireScreenOverlay))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static GameObject LoadRuntimePrefabAsset()
        {
            GameObject prefab = SpringDay1UiPrefabRegistry.LoadWorkbenchOverlayPrefab();
            if (prefab != null)
            {
                return prefab;
            }

#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);
#else
            return null;
#endif
        }

        private static bool CanInstantiateRuntimePrefab(GameObject prefab)
        {
            return prefab != null && prefab.GetComponent<SpringDay1WorkbenchCraftingOverlay>() != null;
        }

        private static bool CanReuseRuntimeInstance(SpringDay1WorkbenchCraftingOverlay candidate, bool requireScreenOverlay)
        {
            if (candidate == null || candidate.gameObject == null || !candidate.gameObject.scene.IsValid())
            {
                return false;
            }

            RectTransform candidateRoot = candidate.rootRect != null ? candidate.rootRect : candidate.transform as RectTransform;
            Canvas candidateCanvas = candidate.overlayCanvas != null ? candidate.overlayCanvas : candidate.GetComponent<Canvas>();
            CanvasGroup candidateCanvasGroup = candidate.canvasGroup != null ? candidate.canvasGroup : candidate.GetComponent<CanvasGroup>();

            if (candidateRoot == null || candidateCanvas == null || candidateCanvasGroup == null)
            {
                return false;
            }

            if (requireScreenOverlay && candidateCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return false;
            }

            RectTransform candidatePanel = candidate.panelRect != null ? candidate.panelRect : FindDirectChildRect(candidateRoot, "PanelRoot");
            if (candidatePanel == null)
            {
                return false;
            }

            RectTransform candidateViewport = candidate.recipeViewportRect != null
                ? candidate.recipeViewportRect
                : FindDescendantRect(candidatePanel, "Viewport");
            RectTransform candidateContent = candidate.recipeContentRect != null
                ? candidate.recipeContentRect
                : (candidateViewport != null ? FindDirectChildRect(candidateViewport, "Content") : null);

            return candidateViewport != null
                && candidateContent != null
                && (candidate.UsesPrefabRecipeShell() || HasGeneratedRecipeRowChain(candidateContent))
                && candidate.UsesPrefabDetailShell()
                && HasReusableRecipeRowChain(candidateContent);
        }

        private bool UsesPrefabRecipeShell()
        {
            RectTransform contentRoot = recipeContentRect;
            if (contentRoot == null)
            {
                RectTransform panel = panelRect != null ? panelRect : FindDirectChildRect(rootRect, "PanelRoot");
                RectTransform viewport = panel != null ? FindDescendantRect(panel, "Viewport") : null;
                contentRoot = viewport != null ? FindDirectChildRect(viewport, "Content") : null;
            }

            if (contentRoot == null)
            {
                return false;
            }

            bool hasRow = false;
            for (int index = 0; index < contentRoot.childCount; index++)
            {
                if (contentRoot.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                hasRow = true;
                if (rowRect.GetComponent<HorizontalLayoutGroup>() != null
                    || FindDescendantRect(rowRect, "TextColumn") != null)
                {
                    return false;
                }
            }

            return hasRow;
        }

        private bool UsesPrefabDetailShell()
        {
            RectTransform detailColumn = _detailColumnRect;
            if (detailColumn == null)
            {
                RectTransform panel = panelRect != null ? panelRect : FindDirectChildRect(rootRect, "PanelRoot");
                detailColumn = panel != null ? FindDescendantRect(panel, "DetailColumn") : null;
            }

            TextMeshProUGUI nameText = selectedNameText ?? FindDescendantComponent<TextMeshProUGUI>(detailColumn, "SelectedName");
            TextMeshProUGUI descriptionText = selectedDescriptionText ?? FindDescendantComponent<TextMeshProUGUI>(detailColumn, "SelectedDescription");
            TextMeshProUGUI materialsText = selectedMaterialsText ?? FindDescendantComponent<TextMeshProUGUI>(detailColumn, "SelectedMaterials");
            RectTransform materialsTitle = _materialsTitleRect ?? FindDescendantRect(detailColumn, "MaterialsTitle");
            RectTransform quantityControls = _quantityControlsRect ?? FindDescendantRect(detailColumn, "QuantityControls");
            RectTransform progressRect = progressRoot ?? FindDescendantRect(detailColumn, "ProgressBackground");
            Button craftActionButton = craftButton ?? FindDescendantComponent<Button>(detailColumn, "CraftButton");
            bool directMaterialsBinding = materialsText != null && materialsText.rectTransform.parent == detailColumn;
            bool wrappedMaterialsBinding =
                materialsText != null
                && materialsViewportRect != null
                && materialsContentRect != null
                && materialsViewportRect.parent == detailColumn
                && materialsContentRect.parent == materialsViewportRect
                && materialsText.rectTransform.parent == materialsContentRect;

            return detailColumn != null
                && FindDirectChildRect(detailColumn, "DetailLayout") == null
                && nameText != null
                && nameText.rectTransform.parent == detailColumn
                && descriptionText != null
                && descriptionText.rectTransform.parent == detailColumn
                && materialsText != null
                && (directMaterialsBinding || wrappedMaterialsBinding)
                && materialsTitle != null
                && materialsTitle.parent == detailColumn
                && quantityControls != null
                && quantityControls.parent == detailColumn
                && progressRect != null
                && progressRect.parent == detailColumn
                && craftActionButton != null
                && craftActionButton.transform.parent == detailColumn;
        }

        private static bool HasReusableRecipeRowChain(RectTransform contentRoot)
        {
            if (contentRoot == null)
            {
                return false;
            }

            for (int index = 0; index < contentRoot.childCount; index++)
            {
                if (contentRoot.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("RecipeRow_"))
                {
                    continue;
                }

                if (!rowRect.gameObject.activeSelf)
                {
                    continue;
                }

                if (!CanBindRecipeRowRect(rowRect))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanBindRecipeRowRect(RectTransform rowRect)
        {
            TextMeshProUGUI name = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Name");
            TextMeshProUGUI summary = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Summary");
            return rowRect != null
                && rowRect.GetComponent<Image>() != null
                && FindDescendantComponent<Image>(rowRect, "Accent") != null
                && FindDescendantComponent<Image>(rowRect, "Icon") != null
                && CanReuseRecipeText(name)
                && CanReuseRecipeText(summary)
                && rowRect.GetComponent<Button>() != null;
        }

        private static bool CanCloneRecipeRowTemplate(RectTransform rowRect)
        {
            return rowRect != null
                && rowRect.GetComponent<Image>() != null
                && FindDescendantComponent<Image>(rowRect, "Accent") != null
                && FindDescendantComponent<Image>(rowRect, "Icon") != null
                && FindDescendantComponent<TextMeshProUGUI>(rowRect, "Name") != null
                && FindDescendantComponent<TextMeshProUGUI>(rowRect, "Summary") != null
                && rowRect.GetComponent<Button>() != null;
        }

        private static bool IsGeneratedRecipeRow(RectTransform rowRect)
        {
            return rowRect != null
                && (rowRect.GetComponent<HorizontalLayoutGroup>() != null
                    || FindDescendantRect(rowRect, "TextColumn") != null);
        }

        private static bool CanReuseRecipeText(TextMeshProUGUI text)
        {
            return text != null
                && text.enabled
                && text.gameObject.activeInHierarchy
                && text.color.a > 0.01f
                && text.rectTransform.rect.width > 2f
                && text.rectTransform.rect.height > 2f
                && !string.IsNullOrWhiteSpace(text.text)
                && CanFontRenderText(text.font, text.text);
        }

        private static void RetireIncompatibleRuntimeInstances()
        {
            SpringDay1WorkbenchCraftingOverlay[] candidates = FindObjectsByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < candidates.Length; index++)
            {
                SpringDay1WorkbenchCraftingOverlay candidate = candidates[index];
                if (candidate == null || CanReuseRuntimeInstance(candidate, requireScreenOverlay: true))
                {
                    continue;
                }

                RetireRuntimeInstance(candidate);
            }
        }

        private static void RetireOtherRuntimeInstances(SpringDay1WorkbenchCraftingOverlay keep)
        {
            SpringDay1WorkbenchCraftingOverlay[] candidates = FindObjectsByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < candidates.Length; index++)
            {
                SpringDay1WorkbenchCraftingOverlay candidate = candidates[index];
                if (candidate == null || candidate == keep)
                {
                    continue;
                }

                RetireRuntimeInstance(candidate);
            }
        }

        private static void RetireRuntimeInstance(SpringDay1WorkbenchCraftingOverlay candidate)
        {
            if (candidate == null || candidate.gameObject == null)
            {
                return;
            }

            if (candidate.canvasGroup != null)
            {
                candidate.canvasGroup.alpha = 0f;
                candidate.canvasGroup.interactable = false;
                candidate.canvasGroup.blocksRaycasts = false;
            }

            if (candidate.overlayCanvas != null)
            {
                candidate.overlayCanvas.gameObject.SetActive(false);
            }
            else
            {
                candidate.gameObject.SetActive(false);
            }
        }

        private static Transform ResolvePlayerTransform()
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            return playerMovement != null ? playerMovement.transform : null;
        }

        private static PlayerMovement ResolvePlayerMovement() => FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);

        private static RectTransform FindDirectChildRect(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            Transform child = parent.Find(childName);
            return child as RectTransform;
        }

        private static RectTransform FindDescendantRect(Transform parent, string childName)
        {
            Transform child = FindDescendant(parent, childName);
            return child as RectTransform;
        }

        private static T FindDescendantComponent<T>(Transform parent, string childName) where T : Component
        {
            Transform child = FindDescendant(parent, childName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private static Transform FindDescendant(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform child = parent.GetChild(index);
                if (child.name == childName)
                {
                    return child;
                }

                Transform nested = FindDescendant(child, childName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        private static int ParseTrailingIndex(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return int.MaxValue;
            }

            int underscoreIndex = name.LastIndexOf('_');
            if (underscoreIndex < 0 || underscoreIndex >= name.Length - 1)
            {
                return int.MaxValue;
            }

            int value;
            return int.TryParse(name.Substring(underscoreIndex + 1), out value) ? value : int.MaxValue;
        }

        private RectTransform CreateSection(Transform parent, string name)
        {
            RectTransform rect = CreateRect(parent, name);
            rect.gameObject.AddComponent<Image>().color = new Color(0.13f, 0.16f, 0.24f, 0.9f);
            ApplyOutline(rect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.05f), new Vector2(1f, -1f));
            return rect;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment, bool wrap = false, bool stretch = false)
        {
            RectTransform rect = CreateRect(parent, name);
            if (stretch)
            {
                Stretch(rect, Vector2.zero, Vector2.zero);
            }

            TextMeshProUGUI tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.font = _fontAsset;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            tmp.overflowMode = wrap ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis;
            tmp.raycastTarget = false;
            return tmp;
        }

        private Image CreateIcon(Transform parent, string name, float size)
        {
            RectTransform rect = CreateRect(parent, name);
            rect.sizeDelta = new Vector2(size, size);
            Image image = rect.gameObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0f);
            return image;
        }

        private Button CreateButton(Transform parent, string name, string label, Vector2 size, float fontSize)
        {
            RectTransform rect = CreateRect(parent, name);
            if (size.x > 0f)
            {
                rect.sizeDelta = size;
            }

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.28f, 0.36f, 0.44f, 0.96f);
            image.raycastTarget = true;
            ApplyOutline(rect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.1f), new Vector2(1f, -1f));
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            CreateText(rect, "Label", label, fontSize, Color.white, TextAlignmentOptions.Center, false, true);
            return button;
        }

        private void AddHoverRelay(GameObject target)
        {
            HoverRelay hoverRelay = target.GetComponent<HoverRelay>();
            if (hoverRelay == null)
            {
                hoverRelay = target.AddComponent<HoverRelay>();
            }

            hoverRelay.Initialize(
                () =>
                {
                    _craftButtonHovered = true;
                    UpdateQuantityUi();
                },
                () =>
                {
                    _craftButtonHovered = false;
                    UpdateQuantityUi();
                });
        }

        private void AddProgressHoverRelay(GameObject target)
        {
            HoverRelay hoverRelay = target.GetComponent<HoverRelay>();
            if (hoverRelay == null)
            {
                hoverRelay = target.AddComponent<HoverRelay>();
            }

            hoverRelay.Initialize(
                () =>
                {
                    _progressBarHovered = true;
                    UpdateQuantityUi();
                },
                () =>
                {
                    _progressBarHovered = false;
                    UpdateQuantityUi();
                });
        }

        private Slider CreateSlider(Transform parent)
        {
            RectTransform rect = CreateRect(parent, "Slider");
            Slider slider = rect.gameObject.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;
            RectTransform bg = CreateRect(rect, "Background");
            Stretch(bg, new Vector2(0f, 4f), new Vector2(0f, -4f));
            bg.gameObject.AddComponent<Image>().color = new Color(0.19f, 0.23f, 0.31f, 0.96f);
            RectTransform fillArea = CreateRect(rect, "FillArea");
            Stretch(fillArea, new Vector2(3f, 4f), new Vector2(-12f, -4f));
            RectTransform fill = CreateRect(fillArea, "Fill");
            Stretch(fill, Vector2.zero, Vector2.zero);
            fill.gameObject.AddComponent<Image>().color = new Color(0.44f, 0.71f, 0.56f, 0.98f);
            RectTransform handleArea = CreateRect(rect, "HandleSlideArea");
            Stretch(handleArea, new Vector2(9f, 0f), new Vector2(-9f, 0f));
            RectTransform handle = CreateRect(handleArea, "Handle");
            handle.sizeDelta = new Vector2(12f, 18f);
            Image handleImage = handle.gameObject.AddComponent<Image>();
            handleImage.color = new Color(0.97f, 0.8f, 0.42f, 1f);
            ApplyOutline(handle.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.12f), new Vector2(1f, -1f));
            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = handleImage;
            return slider;
        }

        private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void Place(RectTransform rect, float left, float top, float right, float bottom)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.offsetMin = new Vector2(left, -bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void AnchorTopLeft(RectTransform rect, float x, float y)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x, -y);
        }

        private static void Center(RectTransform rect)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }

        private static void ApplyOutline(Outline outline, Color color, Vector2 distance)
        {
            outline.effectColor = color;
            outline.effectDistance = distance;
            outline.useGraphicAlpha = true;
        }

        private static void ApplyShadow(Shadow shadow, Color color, Vector2 distance)
        {
            shadow.effectColor = color;
            shadow.effectDistance = distance;
            shadow.useGraphicAlpha = true;
        }

        private Scrollbar EnsureRecipeScrollbar()
        {
            if (recipeScrollbar != null)
            {
                return recipeScrollbar;
            }

            RectTransform parent = recipeViewportRect != null ? recipeViewportRect.parent as RectTransform : null;
            if (parent == null)
            {
                return null;
            }

            recipeScrollbar = FindDescendantComponent<Scrollbar>(parent, "RecipeScrollbar");
            if (recipeScrollbar == null)
            {
                recipeScrollbar = CreateRecipeScrollbar(parent);
            }

            return recipeScrollbar;
        }

        private Scrollbar CreateRecipeScrollbar(Transform parent)
        {
            RectTransform scrollbarRect = CreateRect(parent, "RecipeScrollbar");
            scrollbarRect.anchorMin = new Vector2(1f, 0f);
            scrollbarRect.anchorMax = new Vector2(1f, 1f);
            scrollbarRect.pivot = new Vector2(1f, 0.5f);
            scrollbarRect.anchoredPosition = new Vector2(-6f, 0f);
            scrollbarRect.sizeDelta = new Vector2(8f, -48f);

            Image background = scrollbarRect.gameObject.AddComponent<Image>();
            background.color = new Color(0.18f, 0.22f, 0.31f, 0.9f);
            background.raycastTarget = true;

            RectTransform slidingArea = CreateRect(scrollbarRect, "Sliding Area");
            Stretch(slidingArea, new Vector2(1f, 2f), new Vector2(-1f, -2f));

            RectTransform handle = CreateRect(slidingArea, "Handle");
            Stretch(handle, Vector2.zero, Vector2.zero);
            Image handleImage = handle.gameObject.AddComponent<Image>();
            handleImage.color = new Color(0.97f, 0.8f, 0.42f, 0.95f);
            handleImage.raycastTarget = true;

            Scrollbar scrollbar = scrollbarRect.gameObject.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            scrollbar.targetGraphic = handleImage;
            scrollbar.handleRect = handle;
            scrollbar.size = 0.25f;
            scrollbar.numberOfSteps = 0;
            return scrollbar;
        }

        private void HideImmediate()
        {
            if (canvasGroup == null)
            {
                return;
            }

            StopQueueOverflowToastRoutine();
            HideQueueOverflowToastImmediate();
            SetWorkbenchPanelVisible(false);
            for (int index = 0; index < _floatingCards.Count; index++)
            {
                if (_floatingCards[index]?.root != null)
                {
                    _floatingCards[index].root.gameObject.SetActive(false);
                }
            }
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private sealed class WorkbenchQueueEntry
        {
            public RecipeData recipe;
            public int recipeId;
            public int resultItemId;
            public int resultAmountPerCraft = 1;
            public int totalCount;
            public int readyCount;
            public float currentUnitProgress;
            public bool hasReservedCurrentCraft;
        }

        private sealed class FloatingProgressCardRefs
        {
            public RectTransform root;
            public Image icon;
            public Image fill;
            public TextMeshProUGUI label;
        }

        private sealed class FloatingProgressDisplayState
        {
            public int RecipeId { get; set; }
            public int ResultItemId { get; set; }
            public int TotalCount { get; set; }
            public int ReadyCount { get; set; }
            public bool HasActiveCraft { get; set; }
            public float ActiveUnitProgress { get; set; }
            public bool HasPendingCrafts => TotalCount > ReadyCount;
        }

        private struct RectBaseline
        {
            public float top;
            public float height;
            public bool captured;

            public bool IsCaptured => captured;

            public float TopOr(float fallback) => captured ? top : fallback;

            public float HeightOr(float fallback) => captured ? height : fallback;
        }

        private sealed class RowRefs
        {
            public RectTransform rect;
            public Image background;
            public Image accent;
            public Image icon;
            public TextMeshProUGUI name;
            public TextMeshProUGUI summary;
            public Button button;
            public float preferredHeight;
        }

        private sealed class HoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            private System.Action _onEnter;
            private System.Action _onExit;

            public void Initialize(System.Action onEnter, System.Action onExit)
            {
                _onEnter = onEnter;
                _onExit = onExit;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                _onEnter?.Invoke();
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                _onExit?.Invoke();
            }
        }
    }
}
