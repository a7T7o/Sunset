using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FarmGame.Data;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1WorkbenchCraftingOverlay : MonoBehaviour
    {
        private const string PrefabAssetPath = "Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab";

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese SDF"
        };

        private static readonly FieldInfo BlockNavOverUiField =
            typeof(GameInputManager).GetField("blockNavOverUI", BindingFlags.Instance | BindingFlags.NonPublic);

        private const string RecipeResourceFolder = "Story/SpringDay1Workbench";
        private const int OverlaySortingOrder = 168;

        private static SpringDay1WorkbenchCraftingOverlay _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform pointerRect;
        [SerializeField] private RectTransform recipeViewportRect;
        [SerializeField] private RectTransform recipeContentRect;
        [SerializeField] private RectTransform materialsViewportRect;
        [SerializeField] private RectTransform materialsContentRect;
        [SerializeField] private Image selectedIcon;
        [SerializeField] private TextMeshProUGUI selectedNameText;
        [SerializeField] private TextMeshProUGUI selectedDescriptionText;
        [SerializeField] private TextMeshProUGUI selectedMaterialsText;
        [SerializeField] private TextMeshProUGUI stageHintText;
        [SerializeField] private TextMeshProUGUI progressLabelText;
        [SerializeField] private RectTransform progressRoot;
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
        [SerializeField] private float extraCraftDurationPerItem = 0.22f;

        private readonly List<RecipeData> _recipes = new();
        private readonly List<RowRefs> _rows = new();
        private static readonly string[] WorkbenchAnimatorBoolNames = { "IsWorking", "Working", "IsCrafting", "Crafting" };

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private Transform _playerTransform;
        private CraftingService _craftingService;
        private InventoryService _inventoryService;
        private GameInputManager _gameInputManager;
        private ScrollRect _recipeScrollRect;
        private ScrollRect _materialsScrollRect;
        private int _selectedIndex = -1;
        private int _selectedQuantity = 1;
        private float _autoHideDistance = 1.5f;
        private bool _displayBelow;
        private bool _isVisible;
        private bool _navBlockWasEnabled;
        private bool _navBlockOverrideApplied;
        private Coroutine _craftRoutine;
        private float _craftProgress;
        private int _craftQueueTotal;
        private int _craftQueueCompleted;
        private RecipeData _craftingRecipe;
        private bool _craftButtonHovered;
        private Animator _workbenchAnimator;
        private string _workbenchAnimatorBoolName;
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

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            SpringDay1WorkbenchCraftingOverlay existing = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            if (existing != null)
            {
                _instance = existing;
                _instance.EnsureBuilt();
                _instance.HideImmediate();
                return;
            }

            SpringDay1WorkbenchCraftingOverlay prefabInstance = InstantiateRuntimePrefab();
            if (prefabInstance != null)
            {
                _instance = prefabInstance;
                _instance.EnsureBuilt();
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
            _instance.HideImmediate();
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
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            StopCraftRoutine();
            CleanupTransientState(resetSession: true);
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

            if (_isVisible && _playerTransform != null && GetBoundaryDistance(SpringDay1UiLayerUtility.GetInteractionSamplePoint(_playerTransform)) > _autoHideDistance)
            {
                Hide();
            }

            if (_isVisible)
            {
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
            _playerTransform = playerTransform != null ? playerTransform : ResolvePlayerTransform();
            _craftingService = craftingService;
            _craftingService.SetStation(CraftingStation.Workbench);
            _autoHideDistance = Mathf.Max(0.6f, autoHideDistance);
            ApplyDisplayDirection(_playerTransform != null && ShouldDisplayBelow(SpringDay1UiLayerUtility.GetInteractionSamplePoint(_playerTransform)));
            BindInventory(craftingService.Inventory);
            ApplyNavigationBlock(true);

            if (_selectedIndex < 0 || _selectedIndex >= _recipes.Count)
            {
                _selectedIndex = 0;
            }

            _selectedQuantity = 1;
            _isVisible = true;
            RefreshAll();
            UpdateFloatingProgressVisibility();
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
                CleanupTransientState(resetSession: true);
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

            if (progressLabelText != null)
            {
                progressLabelText.text = "选择配方后即可开始制作";
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
                _craftQueueTotal = 0;
                _craftQueueCompleted = 0;
            }

            UpdateFloatingProgressVisibility();
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            Hide();
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFont();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = OverlaySortingOrder;
            overlayCanvas.pixelPerfect = true;

            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

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
            recipeViewportRect.offsetMax = new Vector2(-8f, -38f);
            Image viewportImage = recipeViewportRect.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(0.05f, 0.07f, 0.1f, 0.45f);
            viewportImage.raycastTarget = true;
            recipeViewportRect.gameObject.AddComponent<Mask>().showMaskGraphic = false;
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
            _recipeScrollRect.viewport = recipeViewportRect;
            _recipeScrollRect.content = recipeContentRect;

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

            selectedNameText = CreateText(metaColumn, "SelectedName", string.Empty, 16f, Color.white, TextAlignmentOptions.TopLeft, false);
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
            quantityValueText = CreateText(quantityControls, "QuantityValue", "x1", 11f, Color.white, TextAlignmentOptions.Center);
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
            progressBackgroundImage.raycastTarget = false;
            RectTransform progressFill = CreateRect(progressRoot, "ProgressFill");
            Stretch(progressFill, new Vector2(1f, 1f), new Vector2(-1f, -1f));
            progressFillImage = progressFill.gameObject.AddComponent<Image>();
            progressFillImage.type = Image.Type.Filled;
            progressFillImage.fillMethod = Image.FillMethod.Horizontal;
            progressFillImage.fillAmount = 0f;
            progressFillImage.color = new Color(0.43f, 0.73f, 0.56f, 0.96f);

            progressLabelText = CreateText(progressRoot, "ProgressLabel", "准备好后即可开始打造", 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.Center, true, true);

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

            if (panelRect == null || quantitySlider == null || craftButton == null)
            {
                BuildUi();
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

            if (panelRect == null)
            {
                panelRect = FindDirectChildRect(rootRect, "PanelRoot");
            }

            if (pointerRect == null && panelRect != null)
            {
                pointerRect = FindDirectChildRect(panelRect, "Pointer");
            }

            if (recipeViewportRect == null)
            {
                recipeViewportRect = FindDescendantRect(panelRect, "Viewport");
            }

            if (recipeContentRect == null && recipeViewportRect != null)
            {
                recipeContentRect = FindDirectChildRect(recipeViewportRect, "Content");
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

            if (progressFillImage == null)
            {
                progressFillImage = FindDescendantComponent<Image>(progressRoot, "ProgressFill");
            }

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

            EnsureRecipeViewportCompatibility();
            EnsureMaterialsViewportCompatibility();
            EnsureFloatingProgressCompatibility();
            CaptureDetailLayoutBaselines();

            if (panelRect == null || pointerRect == null || recipeContentRect == null || selectedIcon == null || selectedNameText == null
                || selectedDescriptionText == null || selectedMaterialsText == null || stageHintText == null || progressLabelText == null
                || progressFillImage == null || quantitySlider == null || quantityValueText == null || decreaseButton == null
                || increaseButton == null || craftButton == null || craftButtonBackground == null || craftButtonLabel == null)
            {
                return false;
            }

            panelWidth = panelRect.rect.width;
            panelHeight = panelRect.rect.height;

            BindExistingRows();
            if (_rows.Count == 0)
            {
                return false;
            }

            decreaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.RemoveAllListeners();
            quantitySlider.onValueChanged.RemoveAllListeners();
            craftButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(() => ChangeQuantity(-1));
            increaseButton.onClick.AddListener(() => ChangeQuantity(1));
            quantitySlider.onValueChanged.AddListener(v => SetQuantity(Mathf.RoundToInt(v), false));
            craftButton.onClick.AddListener(OnCraftButtonClicked);
            AddHoverRelay(craftButton.gameObject);
            RefreshCompatibilityLayout();
            return true;
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
            viewportImage.color = new Color(0f, 0f, 0f, 0.001f);
            viewportImage.raycastTarget = true;

            Mask mask = SpringDay1UiLayerUtility.EnsureComponent<Mask>(recipeViewportRect.gameObject);
            mask.showMaskGraphic = false;

            _recipeScrollRect = SpringDay1UiLayerUtility.EnsureComponent<ScrollRect>(recipeViewportRect.gameObject);
            _recipeScrollRect.horizontal = false;
            _recipeScrollRect.vertical = true;
            _recipeScrollRect.inertia = false;
            _recipeScrollRect.movementType = ScrollRect.MovementType.Clamped;
            _recipeScrollRect.scrollSensitivity = 18f;
            _recipeScrollRect.viewport = recipeViewportRect;
            _recipeScrollRect.content = recipeContentRect;

            if (UsesManualRecipeShell())
            {
                if (recipeContentRect.TryGetComponent(out VerticalLayoutGroup existingLayout))
                {
                    existingLayout.enabled = false;
                }

                if (recipeContentRect.TryGetComponent(out ContentSizeFitter existingFitter))
                {
                    existingFitter.enabled = false;
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

            if (usesGeneratedLayout)
            {
                rowRect.anchorMin = new Vector2(0f, 1f);
                rowRect.anchorMax = new Vector2(1f, 1f);
                rowRect.pivot = new Vector2(0.5f, 1f);
                rowRect.sizeDelta = new Vector2(0f, rowHeight);
            }
            else if (rowRect.sizeDelta.y <= 0.01f)
            {
                rowRect.sizeDelta = new Vector2(rowRect.sizeDelta.x, rowHeight);
            }

            TextMeshProUGUI name = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Name");
            if (name != null)
            {
                RectTransform nameRect = name.rectTransform;
                name.enableAutoSizing = false;
                name.textWrappingMode = TextWrappingModes.NoWrap;
                name.overflowMode = TextOverflowModes.Ellipsis;
                if (usesGeneratedLayout && nameRect.sizeDelta.y <= 0.01f)
                {
                    nameRect.sizeDelta = new Vector2(nameRect.sizeDelta.x, 18f);
                }
            }

            TextMeshProUGUI summary = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Summary");
            if (summary != null)
            {
                RectTransform summaryRect = summary.rectTransform;
                summary.enableAutoSizing = false;
                summary.textWrappingMode = TextWrappingModes.Normal;
                summary.overflowMode = TextOverflowModes.Overflow;
                if (usesGeneratedLayout && summaryRect.sizeDelta.y <= 0.01f)
                {
                    summaryRect.sizeDelta = new Vector2(summaryRect.sizeDelta.x, 12f);
                }
            }

            if (!usesGeneratedLayout && name != null && summary != null)
            {
                RectTransform nameRect = name.rectTransform;
                RectTransform summaryRect = summary.rectTransform;
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

                RectTransform accentRect = FindDescendantRect(rowRect, "Accent");
                if (accentRect != null)
                {
                    accentRect.sizeDelta = new Vector2(accentRect.sizeDelta.x, Mathf.Max(32f, dynamicRowHeight - 10f));
                }
            }
        }

        private void EnsureMaterialsViewportCompatibility()
        {
            if (_detailColumnRect == null || selectedMaterialsText == null)
            {
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
            RefreshMaterialsContentGeometry();
        }

        private void EnsureFloatingProgressCompatibility()
        {
            if (floatingProgressRoot != null && floatingProgressIcon != null && floatingProgressFillImage != null && floatingProgressLabel != null)
            {
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

            if (floatingProgressLabel == null)
            {
                floatingProgressLabel = FindDescendantComponent<TextMeshProUGUI>(floatingProgressRoot, "Label");
            }
        }

        private void RefreshCompatibilityLayout()
        {
            if (_detailColumnRect == null || _materialsTitleRect == null || _quantityControlsRect == null
                || selectedNameText == null || selectedDescriptionText == null || stageHintText == null || progressLabelText == null
                || progressRoot == null || craftButton == null || materialsViewportRect == null)
            {
                return;
            }

            RefreshMaterialsContentGeometry();

            if (!UsesLegacyDetailManualLayout())
            {
                return;
            }

            NormalizeToTopFlowRect(selectedNameText.rectTransform);
            NormalizeToTopFlowRect(selectedDescriptionText.rectTransform);
            NormalizeToTopFlowRect(_materialsTitleRect);
            NormalizeToTopFlowRect(materialsViewportRect);
            NormalizeToTopFlowRect(progressLabelText.rectTransform);
            NormalizeToTopFlowRect(_quantityControlsRect);
            NormalizeToTopFlowRect(stageHintText.rectTransform);

            float nameWidth = GetCurrentWidth(selectedNameText.rectTransform, 120f);
            float descriptionWidth = GetCurrentWidth(selectedDescriptionText.rectTransform, nameWidth);
            float materialsWidth = GetCurrentWidth(materialsViewportRect, descriptionWidth);
            float progressLabelWidth = GetCurrentWidth(progressLabelText.rectTransform, materialsWidth);
            float stageHintWidth = GetCurrentWidth(stageHintText.rectTransform, progressLabelWidth);
            float nameHeight = MeasureTextHeight(selectedNameText, nameWidth, Mathf.Max(18f, _nameBaseline.HeightOr(18f)));
            float descriptionHeight = Mathf.Min(
                MeasureTextHeight(selectedDescriptionText, descriptionWidth, Mathf.Max(26f, _descriptionBaseline.HeightOr(26f))),
                60f);
            float materialsTextHeight = Mathf.Max(_materialsTextBaselineHeight, MeasureTextHeight(selectedMaterialsText, materialsWidth, Mathf.Max(_materialsTextBaselineHeight, 20f)));
            float progressLabelHeight = MeasureTextHeight(progressLabelText, progressLabelWidth, Mathf.Max(18f, _progressLabelBaseline.HeightOr(18f)));
            float quantityControlsHeight = Mathf.Max(_quantityControlsBaseline.HeightOr(GetCurrentHeight(_quantityControlsRect, 20f)), 20f);
            float stageHintHeight = MeasureTextHeight(stageHintText, stageHintWidth, Mathf.Max(18f, _stageHintBaseline.HeightOr(18f)));

            float nameTop = _nameBaseline.TopOr(GetTopInParent(selectedNameText.rectTransform));
            float descriptionTop = nameTop + nameHeight + 4f;

            float iconBottom = _iconCardBaseline.IsCaptured
                ? _iconCardBaseline.top + _iconCardBaseline.height
                : descriptionTop;
            float headerBottom = Mathf.Max(iconBottom, descriptionTop + descriptionHeight);

            float materialsTitleTop = Mathf.Max(
                _materialsTitleBaseline.TopOr(GetTopInParent(_materialsTitleRect)),
                headerBottom + 6f);
            float materialsTitleHeight = Mathf.Max(14f, _materialsTitleBaseline.HeightOr(14f));
            float materialsViewportTop = materialsTitleTop + materialsTitleHeight + 4f;
            float detailFloorTop = GetActionAreaFloorTop();
            const float materialsToProgressGap = 6f;
            const float progressToQuantityGap = 8f;
            const float quantityToHintGap = 8f;
            const float hintToActionGap = 6f;
            float reservedAfterViewport = materialsToProgressGap + progressLabelHeight + progressToQuantityGap + quantityControlsHeight + quantityToHintGap + stageHintHeight + hintToActionGap;
            float maxViewportHeight = Mathf.Max(20f, detailFloorTop - materialsViewportTop - reservedAfterViewport);
            float materialsViewportHeight = Mathf.Clamp(Mathf.Max(20f, materialsTextHeight), 20f, maxViewportHeight);
            float progressLabelTop = materialsViewportTop + materialsViewportHeight + materialsToProgressGap;
            float quantityControlsTop = progressLabelTop + progressLabelHeight + progressToQuantityGap;
            float stageHintTop = quantityControlsTop + quantityControlsHeight + quantityToHintGap;

            SetTopFlowRect(selectedNameText.rectTransform, nameTop, nameHeight);
            SetTopFlowRect(selectedDescriptionText.rectTransform, descriptionTop, descriptionHeight);
            SetTopFlowRect(_materialsTitleRect, materialsTitleTop, materialsTitleHeight);
            SetTopFlowRect(materialsViewportRect, materialsViewportTop, materialsViewportHeight);
            SetTopFlowRect(progressLabelText.rectTransform, progressLabelTop, progressLabelHeight);
            SetTopFlowRect(_quantityControlsRect, quantityControlsTop, quantityControlsHeight);
            SetTopFlowRect(stageHintText.rectTransform, stageHintTop, stageHintHeight);

            RefreshMaterialsContentGeometry();
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
                if (rowRect.GetComponent<HorizontalLayoutGroup>() != null)
                {
                    return false;
                }
            }

            return manualRowCount > 0;
        }

        private void RefreshRecipeContentGeometry()
        {
            if (recipeContentRect == null)
            {
                return;
            }

            if (!UsesManualRecipeShell())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContentRect);
                Canvas.ForceUpdateCanvases();
                if (_recipeScrollRect != null)
                {
                    _recipeScrollRect.verticalNormalizedPosition = 1f;
                }

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
            recipeContentRect.sizeDelta = new Vector2(recipeContentRect.sizeDelta.x, totalHeight);

            LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContentRect);
            UpdateViewportMask(recipeViewportRect, totalHeight);
            Canvas.ForceUpdateCanvases();
            if (_recipeScrollRect != null)
            {
                _recipeScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private void RefreshMaterialsContentGeometry()
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
            materialsTextRect.sizeDelta = new Vector2(materialsTextRect.sizeDelta.x, textHeight);
            materialsContentRect.sizeDelta = new Vector2(materialsContentRect.sizeDelta.x, textHeight);

            LayoutElement materialsLabelLayout = SpringDay1UiLayerUtility.EnsureComponent<LayoutElement>(selectedMaterialsText.gameObject);
            materialsLabelLayout.minHeight = baselineHeight;
            materialsLabelLayout.preferredHeight = textHeight;
            materialsLabelLayout.flexibleHeight = 0f;

            LayoutRebuilder.ForceRebuildLayoutImmediate(materialsTextRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(materialsContentRect);
            UpdateViewportMask(materialsViewportRect, textHeight);
            Canvas.ForceUpdateCanvases();
        }

        private static float MeasureTextHeight(TextMeshProUGUI text, float width, float minHeight)
        {
            if (text == null)
            {
                return minHeight;
            }

            Vector2 preferred = text.GetPreferredValues(text.text, Mathf.Max(1f, width), 0f);
            return Mathf.Max(minHeight, Mathf.Ceil(preferred.y));
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

        private static void SetTopKeepingHorizontal(RectTransform rect, float top, float height)
        {
            if (rect == null)
            {
                return;
            }

            if (Mathf.Abs(rect.anchorMin.x - rect.anchorMax.x) > 0.001f)
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
                RectTransform rowRect = CreateRect(recipeContentRect, $"RecipeRow_{rowIndex}");
                LayoutElement rowLayoutElement = rowRect.gameObject.AddComponent<LayoutElement>();
                rowLayoutElement.preferredHeight = rowHeight;
                rowLayoutElement.minHeight = rowHeight;
                Image background = rowRect.gameObject.AddComponent<Image>();
                background.color = new Color(0.18f, 0.22f, 0.31f, 0.94f);
                ApplyOutline(rowRect.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.06f), new Vector2(1f, -1f));
                HorizontalLayoutGroup rowLayout = rowRect.gameObject.AddComponent<HorizontalLayoutGroup>();
                rowLayout.padding = new RectOffset(10, 10, 6, 6);
                rowLayout.spacing = 8f;
                rowLayout.childAlignment = TextAnchor.UpperLeft;
                rowLayout.childControlWidth = false;
                rowLayout.childControlHeight = false;
                rowLayout.childForceExpandWidth = false;
                rowLayout.childForceExpandHeight = false;
                RectTransform accent = CreateRect(rowRect, "Accent");
                LayoutElement accentLayout = accent.gameObject.AddComponent<LayoutElement>();
                accentLayout.preferredWidth = 4f;
                accentLayout.minWidth = 4f;
                accentLayout.preferredHeight = rowHeight - 10f;
                Image accentImage = accent.gameObject.AddComponent<Image>();
                RectTransform iconCard = CreateRect(rowRect, "IconCard");
                LayoutElement iconCardLayout = iconCard.gameObject.AddComponent<LayoutElement>();
                iconCardLayout.preferredWidth = 36f;
                iconCardLayout.preferredHeight = 36f;
                iconCard.gameObject.AddComponent<Image>().color = new Color(0.23f, 0.28f, 0.38f, 0.94f);
                Image icon = CreateIcon(iconCard, "Icon", 24f);
                Center(icon.rectTransform);
                RectTransform textColumn = CreateRect(rowRect, "TextColumn");
                LayoutElement textColumnLayout = textColumn.gameObject.AddComponent<LayoutElement>();
                textColumnLayout.flexibleWidth = 1f;
                VerticalLayoutGroup textLayout = textColumn.gameObject.AddComponent<VerticalLayoutGroup>();
                textLayout.spacing = 1f;
                textLayout.childAlignment = TextAnchor.UpperLeft;
                textLayout.childControlWidth = true;
                textLayout.childControlHeight = false;
                textLayout.childForceExpandWidth = true;
                textLayout.childForceExpandHeight = false;
                ContentSizeFitter textFitter = textColumn.gameObject.AddComponent<ContentSizeFitter>();
                textFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                TextMeshProUGUI name = CreateText(textColumn, "Name", string.Empty, 13f, Color.white, TextAlignmentOptions.TopLeft);
                name.gameObject.AddComponent<LayoutElement>().minHeight = 16f;
                TextMeshProUGUI summary = CreateText(textColumn, "Summary", string.Empty, 10f, new Color(0.77f, 0.82f, 0.9f, 0.94f), TextAlignmentOptions.TopLeft, true);
                summary.gameObject.AddComponent<LayoutElement>().minHeight = 12f;
                Button button = rowRect.gameObject.AddComponent<Button>();
                button.targetGraphic = background;
                button.onClick.AddListener(() => SelectRecipe(rowIndex));
                EnsureRecipeRowCompatibility(rowRect);
                _rows.Add(new RowRefs { rect = rowRect, background = background, accent = accentImage, icon = icon, name = name, summary = summary, button = button });
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContentRect);
        }

        private void RefreshAll()
        {
            EnsureRows();
            RefreshRows();
            RefreshSelection();
            UpdateQuantityUi();
            RefreshCompatibilityLayout();
        }

        private void RefreshRows()
        {
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
                _rows[i].icon.sprite = item != null ? item.GetBagSprite() : null;
                _rows[i].icon.color = _rows[i].icon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
                _rows[i].name.text = GetRecipeDisplayName(recipe, item);
                _rows[i].summary.text = BuildRowSummary(recipe);
                _rows[i].name.ForceMeshUpdate();
                _rows[i].summary.ForceMeshUpdate();
                EnsureRecipeRowCompatibility(_rows[i].rect);
                _rows[i].preferredHeight = GetCurrentHeight(_rows[i].rect, rowHeight);
                _rows[i].background.color = selected ? new Color(0.31f, 0.23f, 0.14f, 0.98f) : craftable ? new Color(0.18f, 0.22f, 0.31f, 0.94f) : new Color(0.24f, 0.16f, 0.16f, 0.94f);
                _rows[i].accent.color = selected ? new Color(0.97f, 0.8f, 0.42f, 1f) : new Color(1f, 1f, 1f, 0f);
            }

            RefreshRecipeContentGeometry();
        }

        private void SelectRecipe(int index)
        {
            if (index < 0 || index >= _recipes.Count || _craftRoutine != null)
            {
                return;
            }

            _selectedIndex = index;
            _selectedQuantity = 1;
            RefreshAll();
        }

        private void RefreshSelection()
        {
            RecipeData recipe = GetSelectedRecipe();
            if (recipe == null)
            {
                return;
            }

            ItemData item = ResolveItem(recipe.resultItemID);
            selectedIcon.sprite = item != null ? item.GetBagSprite() : null;
            selectedIcon.color = selectedIcon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            selectedNameText.text = GetRecipeDisplayName(recipe, item);
            selectedDescriptionText.text = string.IsNullOrWhiteSpace(recipe.description) ? (item != null ? item.description : "这件工具还没有额外说明。") : recipe.description;
            selectedMaterialsText.text = BuildMaterialsText(recipe, _selectedQuantity);
            RefreshMaterialsContentGeometry();
            if (_materialsScrollRect != null)
            {
                _materialsScrollRect.verticalNormalizedPosition = 1f;
            }
            stageHintText.text = BuildStageHint(recipe);
            UpdateProgressLabel(recipe);
        }

        private void UpdateQuantityUi()
        {
            RecipeData recipe = GetSelectedRecipe();
            int maxCraftable = recipe != null ? Mathf.Max(1, GetMaxCraftableCount(recipe)) : 1;
            _selectedQuantity = Mathf.Clamp(_selectedQuantity, 1, maxCraftable);
            quantitySlider.minValue = 1f;
            quantitySlider.maxValue = maxCraftable;
            quantitySlider.wholeNumbers = true;
            quantitySlider.SetValueWithoutNotify(_selectedQuantity);
            quantityValueText.text = $"x{_selectedQuantity}";
            decreaseButton.interactable = _selectedQuantity > 1 && _craftRoutine == null;
            increaseButton.interactable = _selectedQuantity < maxCraftable && _craftRoutine == null;
            quantitySlider.interactable = maxCraftable > 1 && _craftRoutine == null;

            bool canCraft = CanCraftSelected(recipe, out string blockerMessage);
            bool isCrafting = _craftRoutine != null;
            craftButton.interactable = canCraft && !isCrafting;
            craftButtonLabel.text = isCrafting
                ? GetCraftHoverLabel()
                : canCraft ? $"开始制作 x{_selectedQuantity}" : "暂不可制作";
            if (isCrafting)
            {
                craftButtonBackground.color = _craftButtonHovered
                    ? new Color(0.25f, 0.39f, 0.55f, 0.58f)
                    : new Color(0.25f, 0.39f, 0.55f, 0f);
            }
            else
            {
                craftButtonBackground.color = canCraft ? new Color(0.33f, 0.53f, 0.44f, 0.98f) : new Color(0.27f, 0.3f, 0.34f, 0.9f);
            }

            if (progressRoot != null && progressBackgroundImage != null)
            {
                progressRoot.gameObject.SetActive(isCrafting);
                progressBackgroundImage.color = isCrafting ? new Color(0.04f, 0.05f, 0.08f, 0.94f) : new Color(0.04f, 0.05f, 0.08f, 0f);
            }

            if (craftButtonLabel != null)
            {
                craftButtonLabel.alpha = isCrafting && !_craftButtonHovered ? 0f : 1f;
            }

            if (!canCraft && !string.IsNullOrWhiteSpace(blockerMessage))
            {
                stageHintText.text = blockerMessage;
            }
        }

        private void ChangeQuantity(int delta) => SetQuantity(_selectedQuantity + delta);

        private void SetQuantity(int quantity, bool updateSlider = true)
        {
            if (_craftRoutine != null)
            {
                return;
            }

            RecipeData recipe = GetSelectedRecipe();
            int maxCraftable = recipe != null ? Mathf.Max(1, GetMaxCraftableCount(recipe)) : 1;
            _selectedQuantity = Mathf.Clamp(quantity, 1, maxCraftable);
            if (updateSlider)
            {
                quantitySlider.SetValueWithoutNotify(_selectedQuantity);
            }

            RefreshSelection();
            UpdateQuantityUi();
            RefreshCompatibilityLayout();
        }

        private void OnCraftButtonClicked()
        {
            RecipeData recipe = GetSelectedRecipe();
            if (_craftRoutine != null || recipe == null || _craftingService == null)
            {
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

            _craftRoutine = StartCoroutine(CraftRoutine(recipe, _selectedQuantity));
        }

        private IEnumerator CraftRoutine(RecipeData recipe, int quantity)
        {
            _craftingRecipe = recipe;
            _craftQueueTotal = quantity;
            _craftQueueCompleted = 0;
            _craftProgress = 0f;
            _craftProgress = 0f;
            _craftButtonHovered = false;
            UpdateQuantityUi();
            RefreshSelection();
            PlayerMovement playerMovement = ResolvePlayerMovement();
            SetWorkbenchAnimating(true);

            int successCount = 0;
            for (int craftIndex = 0; craftIndex < quantity; craftIndex++)
            {
                float duration = Mathf.Max(0.25f, (recipe.craftingTime > 0f ? recipe.craftingTime : defaultCraftDuration) + Mathf.Max(0, quantity - 1) * extraCraftDurationPerItem * 0.2f);
                _craftProgress = 0f;
                while (_craftProgress < 1f)
                {
                    _craftProgress += Time.deltaTime / duration;
                    MaintainWorkbenchPose(playerMovement);
                    UpdateProgressLabel(recipe);
                    UpdateQuantityUi();
                    UpdateFloatingProgressVisibility();
                    yield return null;
                }

                CraftResult result = _craftingService.TryCraft(recipe);
                if (!result.success)
                {
                    break;
                }

                successCount++;
                _craftQueueCompleted = successCount;
                HandleInventoryChanged();
            }

            _craftRoutine = null;
            _craftProgress = 0f;
            _selectedQuantity = 1;
            _craftingRecipe = null;
            _craftQueueTotal = 0;
            _craftQueueCompleted = 0;
            _craftButtonHovered = false;
            SetWorkbenchAnimating(false);
            RefreshAll();
            UpdateFloatingProgressVisibility();
            if (successCount > 0)
            {
                SpringDay1PromptOverlay.EnsureRuntime();
                SpringDay1PromptOverlay.Instance.Show($"{GetRecipeDisplayName(recipe, ResolveItem(recipe.resultItemID))} 已完成制作。");
                if (!_isVisible)
                {
                    CleanupTransientState(resetSession: true);
                }
            }
            else if (!_isVisible)
            {
                CleanupTransientState(resetSession: true);
            }
        }

        private void StopCraftRoutine()
        {
            if (_craftRoutine == null)
            {
                return;
            }

            StopCoroutine(_craftRoutine);
            _craftRoutine = null;
            _craftProgress = 0f;
            _craftingRecipe = null;
            _craftQueueTotal = 0;
            _craftQueueCompleted = 0;
            _craftButtonHovered = false;
            SetWorkbenchAnimating(false);
            UpdateFloatingProgressVisibility();
        }

        private void MaintainWorkbenchPose(PlayerMovement playerMovement)
        {
            if (playerMovement == null || _anchorTarget == null)
            {
                return;
            }

            playerMovement.StopMovement();
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

            if (!_craftingService.IsRecipeUnlocked(recipe))
            {
                blockerMessage = "这张配方目前还没有解锁。";
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            if (director != null && !director.CanPerformWorkbenchCraft(out blockerMessage))
            {
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

            InventoryService inventory = _craftingService.Inventory;
            if (inventory == null || recipe.resultAmount <= 0)
            {
                return 0;
            }

            int capacity = 0;
            int maxStack = Mathf.Max(1, inventory.GetMaxStack(recipe.resultItemID));
            for (int i = 0; i < inventory.Size; i++)
            {
                ItemStack slot = inventory.GetSlot(i);
                if (slot.IsEmpty)
                {
                    capacity += maxStack;
                }
                else if (slot.itemId == recipe.resultItemID && slot.amount < maxStack)
                {
                    capacity += maxStack - slot.amount;
                }
            }

            return Mathf.Max(0, Mathf.Min(materialCap, capacity / recipe.resultAmount));
        }

        private string GetRecipeDisplayName(RecipeData recipe, ItemData item)
        {
            return string.IsNullOrWhiteSpace(recipe.recipeName) ? (item != null ? item.itemName : $"配方 {recipe.recipeID}") : recipe.recipeName;
        }

        private string BuildRowSummary(RecipeData recipe)
        {
            float previewDuration = Mathf.Max(0.25f, recipe.craftingTime > 0f ? recipe.craftingTime : defaultCraftDuration);
            string materialSummary = recipe.ingredients == null || recipe.ingredients.Count == 0
                ? "免材料"
                : $"{recipe.ingredients.Count}项材料";
            return $"{previewDuration:0.0}s · {materialSummary}";
        }

        private string BuildMaterialsText(RecipeData recipe, int quantity)
        {
            List<string> lines = new();
            foreach (RecipeIngredient ingredient in recipe.ingredients)
            {
                int owned = _craftingService != null ? _craftingService.GetMaterialCount(ingredient.itemID) : 0;
                int required = ingredient.amount * quantity;
                string color = owned >= required ? "#B7E6C2" : "#F0B49E";
                lines.Add($"<color={color}>{ResolveItem(ingredient.itemID)?.itemName ?? $"材料 {ingredient.itemID}"}</color>  {owned}/{required}");
            }

            return string.Join("\n", lines);
        }

        private string BuildStageHint(RecipeData recipe)
        {
            if (recipe == null)
            {
                return string.Empty;
            }

            if (!CanCraftSelected(recipe, out string blockerMessage))
            {
                return blockerMessage;
            }

            return "材料齐全，点击下方按钮开始制作。";
        }

        private void UpdateProgressLabel(RecipeData recipe)
        {
            if (_craftRoutine != null)
            {
                progressFillImage.fillAmount = _craftProgress;
                int remaining = Mathf.Max(1, _craftQueueTotal - _craftQueueCompleted);
                progressLabelText.text = $"正在打造 {GetRecipeDisplayName(recipe, ResolveItem(recipe.resultItemID))} · 剩余 {remaining} · {Mathf.RoundToInt(_craftProgress * 100f)}%";
                return;
            }

            progressFillImage.fillAmount = 0f;
            float preview = Mathf.Max(0.25f, (recipe.craftingTime > 0f ? recipe.craftingTime : defaultCraftDuration) + Mathf.Max(0, _selectedQuantity - 1) * extraCraftDurationPerItem);
            progressLabelText.text = $"预计耗时 {preview:0.0} 秒";
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

        private static int GetRecipeSortOrder(RecipeData recipe)
        {
            string name = (recipe.recipeName ?? string.Empty).ToLowerInvariant();
            if (name.Contains("axe"))
            {
                return 0;
            }

            if (name.Contains("hoe"))
            {
                return 1;
            }

            if (name.Contains("pickaxe"))
            {
                return 2;
            }

            return 99;
        }

        private ItemData ResolveItem(int itemId)
        {
            ItemDatabase database = _craftingService != null ? _craftingService.Database : _inventoryService != null ? _inventoryService.Database : null;
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
                _inventoryService.OnInventoryChanged += HandleInventoryChanged;
            }
        }

        private void UnbindInventory()
        {
            if (_inventoryService != null)
            {
                _inventoryService.OnInventoryChanged -= HandleInventoryChanged;
                _inventoryService = null;
            }
        }

        private void HandleInventoryChanged()
        {
            if (_isVisible)
            {
                RefreshAll();
            }

            UpdateFloatingProgressVisibility();
        }

        private void ApplyDisplayDirection(bool displayBelow)
        {
            _displayBelow = displayBelow;
            panelRect.pivot = _displayBelow ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            pointerRect.anchorMin = _displayBelow ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            pointerRect.anchorMax = pointerRect.anchorMin;
            pointerRect.pivot = new Vector2(0.5f, 0.5f);
            pointerRect.anchoredPosition = new Vector2(0f, _displayBelow ? 5f : -5f);
        }

        private void Reposition(bool immediate = false)
        {
            if (panelRect == null || rootRect == null || _anchorTarget == null)
            {
                return;
            }

            Bounds bounds = GetAnchorBounds();
            Vector3 worldAnchor = _displayBelow ? new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) : new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Camera worldCamera = GetWorldProjectionCamera();
            if (worldCamera == null)
            {
                return;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldAnchor) + (_displayBelow ? belowOffset : aboveOffset);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, GetUiEventCamera(), out Vector2 localPoint))
            {
                return;
            }

            Rect rect = rootRect.rect;
            float minX = rect.xMin + panelWidth * panelRect.pivot.x + screenMargin.x;
            float maxX = rect.xMax - panelWidth * (1f - panelRect.pivot.x) - screenMargin.x;
            float minY = rect.yMin + panelHeight * panelRect.pivot.y + screenMargin.y;
            float maxY = rect.yMax - panelHeight * (1f - panelRect.pivot.y) - screenMargin.y;
            Vector2 target = new(Mathf.Clamp(localPoint.x, minX, maxX), Mathf.Clamp(localPoint.y, minY, maxY));
            panelRect.anchoredPosition = SpringDay1UiLayerUtility.SnapToCanvasPixel(overlayCanvas, target);
        }

        private void UpdateFloatingProgressVisibility()
        {
            if (floatingProgressRoot == null)
            {
                return;
            }

            bool shouldShow = !_isVisible
                && _craftRoutine != null
                && _craftingRecipe != null
                && _anchorTarget != null
                && !SpringDay1UiLayerUtility.IsBlockingPageUiOpen()
                && (DialogueManager.Instance == null || !DialogueManager.Instance.IsDialogueActive);
            floatingProgressRoot.gameObject.SetActive(shouldShow);
            if (!shouldShow)
            {
                return;
            }

            ItemData item = ResolveItem(_craftingRecipe.resultItemID);
            floatingProgressIcon.sprite = item != null ? item.GetBagSprite() : null;
            floatingProgressIcon.color = floatingProgressIcon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            floatingProgressFillImage.fillAmount = _craftProgress;
            floatingProgressLabel.text = $"{Mathf.Max(1, _craftQueueTotal - _craftQueueCompleted)}";
            RepositionFloatingProgress();
        }

        private void RepositionFloatingProgress()
        {
            if (floatingProgressRoot == null || rootRect == null || _anchorTarget == null)
            {
                return;
            }

            Bounds bounds = GetAnchorBounds();
            Vector3 worldAnchor = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Camera worldCamera = GetWorldProjectionCamera();
            if (worldCamera == null)
            {
                return;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, worldAnchor) + new Vector2(0f, 26f);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, GetUiEventCamera(), out Vector2 localPoint))
            {
                return;
            }

            floatingProgressRoot.anchoredPosition = SpringDay1UiLayerUtility.SnapToCanvasPixel(overlayCanvas, localPoint);
        }

        private string GetCraftHoverLabel()
        {
            int remaining = Mathf.Max(1, _craftQueueTotal - _craftQueueCompleted);
            return $"剩余 {remaining} · {Mathf.RoundToInt(_craftProgress * 100f)}%";
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

        private bool ShouldDisplayBelow(Vector2 playerPosition)
        {
            CraftingStationInteractable interactable = _anchorTarget != null ? _anchorTarget.GetComponent<CraftingStationInteractable>() : null;
            return interactable != null ? interactable.ShouldDisplayOverlayBelow(playerPosition) : playerPosition.y > GetAnchorBounds().center.y;
        }

        private Camera GetWorldProjectionCamera()
        {
            return SpringDay1UiLayerUtility.GetWorldProjectionCamera(overlayCanvas);
        }

        private Camera GetUiEventCamera()
        {
            return SpringDay1UiLayerUtility.GetUiEventCamera(overlayCanvas);
        }

        private TMP_FontAsset ResolveFont()
        {
            foreach (string path in PreferredFontResourcePaths)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(path);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return TMP_Settings.defaultFontAsset;
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
            if (prefab == null)
            {
                return null;
            }

            Transform parent = ResolveParent();
            GameObject instance = parent != null ? Instantiate(prefab, parent, false) : Instantiate(prefab);
            instance.name = prefab.name;
            return instance.GetComponent<SpringDay1WorkbenchCraftingOverlay>();
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

        private void HideImmediate()
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
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
