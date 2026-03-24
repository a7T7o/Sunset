using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FarmGame.Data;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1WorkbenchCraftingOverlay : MonoBehaviour
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese BitmapSong SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF"
        };

        private const string RecipeResourceFolder = "Story/SpringDay1Workbench";
        private const float PanelWidth = 448f;
        private const float PanelHeight = 236f;
        private const float RecipeColumnWidth = 158f;
        private const float ScreenMargin = 20f;
        private const float AboveOffsetPixels = 46f;
        private const float BelowOffsetPixels = 38f;
        private const float PointerSize = 16f;
        private const int OverlaySortingOrder = 180;

        private static readonly Color OuterFrameColor = new(0.78f, 0.52f, 0.18f, 0.98f);
        private static readonly Color FrameHighlightColor = new(1f, 0.91f, 0.72f, 0.26f);
        private static readonly Color SurfaceColor = new(0.11f, 0.13f, 0.2f, 0.96f);
        private static readonly Color SectionColor = new(0.16f, 0.19f, 0.27f, 0.98f);
        private static readonly Color SectionAccentColor = new(0.24f, 0.29f, 0.39f, 0.98f);
        private static readonly Color SelectedRowColor = new(0.3f, 0.22f, 0.13f, 0.98f);
        private static readonly Color ReadyRowColor = new(0.2f, 0.24f, 0.33f, 0.98f);
        private static readonly Color DisabledRowColor = new(0.27f, 0.17f, 0.16f, 0.98f);
        private static readonly Color AccentColor = new(0.98f, 0.81f, 0.45f, 1f);
        private static readonly Color SoftTextColor = new(0.9f, 0.92f, 0.97f, 1f);
        private static readonly Color MutedTextColor = new(0.73f, 0.78f, 0.87f, 0.96f);
        private static readonly Color SecondaryAccentColor = new(0.65f, 0.87f, 0.76f, 1f);

        private static readonly FieldInfo BlockNavOverUiField =
            typeof(GameInputManager).GetField("blockNavOverUI", BindingFlags.Instance | BindingFlags.NonPublic);

        private static SpringDay1WorkbenchCraftingOverlay _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform pointerRect;
        [SerializeField] private RectTransform recipeListContent;
        [SerializeField] private Image selectedIcon;
        [SerializeField] private TextMeshProUGUI selectedNameText;
        [SerializeField] private TextMeshProUGUI selectedDescriptionText;
        [SerializeField] private TextMeshProUGUI selectedMaterialsText;
        [SerializeField] private Slider quantitySlider;
        [SerializeField] private TextMeshProUGUI quantityValueText;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button craftButton;
        [SerializeField] private Image craftButtonBackground;
        [SerializeField] private TextMeshProUGUI craftButtonLabel;

        private readonly List<RecipeData> _recipes = new();
        private readonly List<RowRefs> _rows = new();

        private TMP_FontAsset _fontAsset;
        private Transform _anchorTarget;
        private Transform _playerTransform;
        private CraftingService _craftingService;
        private InventoryService _inventoryService;
        private GameInputManager _gameInputManager;
        private int _selectedIndex = -1;
        private int _selectedQuantity = 1;
        private float _autoHideDistance = 1.5f;
        private bool _displayBelow;
        private bool _isVisible;
        private bool _navBlockWasEnabled;
        private bool _navBlockOverrideApplied;
        private Canvas _canvas => overlayCanvas;
        private RectTransform _canvasRect => rootRect;

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

            Transform parent = ResolveParent();
            GameObject root = new GameObject(
                nameof(SpringDay1WorkbenchCraftingOverlay),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(GraphicRaycaster),
                typeof(CanvasGroup));

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
            CleanupTransientState();
        }

        private void LateUpdate()
        {
            if (!_isVisible || _anchorTarget == null)
            {
                return;
            }

            if (_playerTransform != null)
            {
                float distance = Vector2.Distance(_playerTransform.position, GetClosestInteractionPoint(_playerTransform.position));
                if (distance > _autoHideDistance)
                {
                    Hide();
                    return;
                }
            }

            bool shouldDisplayBelow = _playerTransform != null && _playerTransform.position.y > GetAnchorBounds().center.y;
            if (shouldDisplayBelow != _displayBelow)
            {
                ApplyDisplayDirection(shouldDisplayBelow);
            }

            Reposition();
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
            if (panelRect == null)
            {
                BuildUi();
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
            ApplyDisplayDirection(_playerTransform != null && _playerTransform.position.y > GetAnchorBounds().center.y);
            BindInventory(craftingService.Inventory);
            ApplyNavigationBlock(true);

            if (_selectedIndex < 0 || _selectedIndex >= _recipes.Count)
            {
                _selectedIndex = 0;
            }

            _selectedQuantity = 1;
            RefreshAll();
            _isVisible = true;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            Reposition();
        }

        public void Hide()
        {
            _isVisible = false;
            _anchorTarget = null;
            _playerTransform = null;
            _craftingService = null;
            CleanupTransientState();
            HideImmediate();
        }

        private void CleanupTransientState()
        {
            UnbindInventory();
            ApplyNavigationBlock(false);
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

            Canvas parentCanvas = transform.parent != null ? transform.parent.GetComponentInParent<Canvas>() : null;
            overlayCanvas.renderMode = parentCanvas != null ? parentCanvas.renderMode : RenderMode.ScreenSpaceOverlay;
            overlayCanvas.worldCamera = overlayCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : (parentCanvas != null ? parentCanvas.worldCamera : Camera.main);
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = OverlaySortingOrder;
            overlayCanvas.pixelPerfect = true;
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            GameObject panelRoot = new GameObject("PanelRoot", typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow));
            panelRoot.transform.SetParent(transform, false);
            panelRect = panelRoot.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);
            Image panelBackground = panelRoot.GetComponent<Image>();
            panelBackground.color = OuterFrameColor;
            panelBackground.raycastTarget = true;
            ApplyOutline(panelRoot.GetComponent<Outline>(), FrameHighlightColor, new Vector2(1f, -1f));
            ApplyShadow(panelRoot.GetComponent<Shadow>(), new Color(0f, 0f, 0f, 0.35f), new Vector2(0f, -6f));

            pointerRect = CreateRect(panelRect, "Pointer");
            pointerRect.sizeDelta = new Vector2(PointerSize, PointerSize);
            pointerRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Image pointerImage = pointerRect.gameObject.AddComponent<Image>();
            pointerImage.color = OuterFrameColor;
            pointerImage.raycastTarget = false;
            ApplyOutline(pointerRect.gameObject.AddComponent<Outline>(), FrameHighlightColor, new Vector2(1f, -1f));

            RectTransform surface = CreateRect(panelRect, "Surface");
            surface.anchorMin = Vector2.zero;
            surface.anchorMax = Vector2.one;
            surface.offsetMin = new Vector2(4f, 4f);
            surface.offsetMax = new Vector2(-4f, -4f);
            Image surfaceImage = surface.gameObject.AddComponent<Image>();
            surfaceImage.color = SurfaceColor;
            surfaceImage.raycastTarget = true;

            RectTransform body = CreateRect(surface, "Body");
            body.anchorMin = Vector2.zero;
            body.anchorMax = Vector2.one;
            body.offsetMin = new Vector2(10f, 10f);
            body.offsetMax = new Vector2(-10f, -10f);
            var bodyLayout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            bodyLayout.spacing = 10f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = true;

            RectTransform left = CreatePanel(body, "RecipeColumn", RecipeColumnWidth);
            LayoutElement leftLayout = left.gameObject.AddComponent<LayoutElement>();
            leftLayout.preferredWidth = RecipeColumnWidth;
            leftLayout.flexibleHeight = 1f;

            RectTransform scrollRoot = CreateRect(left, "RecipeScrollRoot");
            scrollRoot.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.inertia = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 20f;

            RectTransform viewport = CreateRect(scrollRoot, "Viewport");
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = Vector2.zero;
            viewport.offsetMax = Vector2.zero;
            var viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0.001f);
            viewportImage.raycastTarget = true;
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;

            recipeListContent = CreateRect(viewport, "Content");
            recipeListContent.anchorMin = new Vector2(0f, 1f);
            recipeListContent.anchorMax = new Vector2(1f, 1f);
            recipeListContent.pivot = new Vector2(0.5f, 1f);
            recipeListContent.offsetMin = Vector2.zero;
            recipeListContent.offsetMax = Vector2.zero;
            var recipeListLayout = recipeListContent.gameObject.AddComponent<VerticalLayoutGroup>();
            recipeListLayout.spacing = 6f;
            recipeListLayout.childControlWidth = true;
            recipeListLayout.childControlHeight = true;
            recipeListLayout.childForceExpandWidth = true;
            recipeListLayout.childForceExpandHeight = false;
            recipeListContent.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.viewport = viewport;
            scrollRect.content = recipeListContent;

            RectTransform right = CreatePanel(body, "DetailColumn");
            LayoutElement rightLayout = right.gameObject.AddComponent<LayoutElement>();
            rightLayout.flexibleWidth = 1f;
            rightLayout.flexibleHeight = 1f;

            RectTransform detailHeader = CreateRect(right, "DetailHeader");
            detailHeader.gameObject.AddComponent<LayoutElement>().preferredHeight = 58f;
            HorizontalLayoutGroup detailHeaderLayout = detailHeader.gameObject.AddComponent<HorizontalLayoutGroup>();
            detailHeaderLayout.spacing = 10f;
            detailHeaderLayout.childControlWidth = false;
            detailHeaderLayout.childControlHeight = true;

            RectTransform iconCard = CreateRect(detailHeader, "SelectedIconCard");
            LayoutElement iconCardLayout = iconCard.gameObject.AddComponent<LayoutElement>();
            iconCardLayout.preferredWidth = 44f;
            iconCardLayout.preferredHeight = 44f;
            Image iconCardImage = iconCard.gameObject.AddComponent<Image>();
            iconCardImage.color = SectionAccentColor;
            iconCardImage.raycastTarget = false;
            ApplyOutline(iconCard.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.08f), new Vector2(1f, -1f));
            selectedIcon = CreateIcon(iconCard, "SelectedIcon", 28f);
            CenterRect(selectedIcon.rectTransform);

            RectTransform titleBlock = CreateRect(detailHeader, "TitleBlock");
            titleBlock.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            VerticalLayoutGroup titleLayout = titleBlock.gameObject.AddComponent<VerticalLayoutGroup>();
            titleLayout.spacing = 4f;
            selectedNameText = CreateText(titleBlock, "SelectedName", string.Empty, 19f, SoftTextColor, TextAlignmentOptions.Left);
            selectedDescriptionText = CreateText(titleBlock, "SelectedDescription", string.Empty, 11f, MutedTextColor, TextAlignmentOptions.TopLeft, true);

            RectTransform topDivider = CreateDivider(right, "TopDivider");
            topDivider.gameObject.AddComponent<LayoutElement>().preferredHeight = 1f;

            selectedMaterialsText = CreateText(right, "SelectedMaterials", string.Empty, 13f, SoftTextColor, TextAlignmentOptions.TopLeft, true);
            selectedMaterialsText.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;

            RectTransform bottomDivider = CreateDivider(right, "BottomDivider");
            bottomDivider.gameObject.AddComponent<LayoutElement>().preferredHeight = 1f;

            RectTransform controls = CreateRect(right, "QuantityControls");
            controls.gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;
            HorizontalLayoutGroup controlsLayout = controls.gameObject.AddComponent<HorizontalLayoutGroup>();
            controlsLayout.spacing = 6f;
            controlsLayout.childControlWidth = false;
            controlsLayout.childControlHeight = true;
            decreaseButton = CreateButton(controls, "Decrease", "-", 34f, 34f, 18f);
            quantitySlider = CreateSlider(controls);
            quantityValueText = CreateText(controls, "QuantityValue", "x1", 13f, SecondaryAccentColor, TextAlignmentOptions.Center);
            quantityValueText.gameObject.AddComponent<LayoutElement>().preferredWidth = 44f;
            increaseButton = CreateButton(controls, "Increase", "+", 34f, 34f, 18f);

            RectTransform footer = CreateRect(right, "Footer");
            footer.gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;
            HorizontalLayoutGroup footerLayout = footer.gameObject.AddComponent<HorizontalLayoutGroup>();
            footerLayout.spacing = 0f;
            footerLayout.childControlWidth = true;
            footerLayout.childControlHeight = true;
            footerLayout.childForceExpandWidth = true;
            craftButton = CreateButton(footer, "Craft", "制作", 0f, 36f, 15f);
            craftButtonBackground = craftButton.targetGraphic as Image;
            craftButtonLabel = craftButton.GetComponentInChildren<TextMeshProUGUI>();

            decreaseButton.onClick.AddListener(() => ChangeQuantity(-1));
            increaseButton.onClick.AddListener(() => ChangeQuantity(1));
            quantitySlider.onValueChanged.AddListener(v => SetQuantity(Mathf.RoundToInt(v), false));
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }

        private void ApplyDisplayDirection(bool displayBelow)
        {
            _displayBelow = displayBelow;
            panelRect.pivot = _displayBelow ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);

            if (pointerRect == null)
            {
                return;
            }

            if (_displayBelow)
            {
                pointerRect.anchorMin = new Vector2(0.5f, 1f);
                pointerRect.anchorMax = new Vector2(0.5f, 1f);
                pointerRect.pivot = new Vector2(0.5f, 0.5f);
                pointerRect.anchoredPosition = new Vector2(0f, PointerSize * 0.32f);
            }
            else
            {
                pointerRect.anchorMin = new Vector2(0.5f, 0f);
                pointerRect.anchorMax = new Vector2(0.5f, 0f);
                pointerRect.pivot = new Vector2(0.5f, 0.5f);
                pointerRect.anchoredPosition = new Vector2(0f, -PointerSize * 0.32f);
            }
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

        private bool EnsureRecipesLoaded()
        {
            if (_recipes.Count > 0)
            {
                return true;
            }

            RecipeData[] loaded = Resources.LoadAll<RecipeData>(RecipeResourceFolder);
            if (loaded == null || loaded.Length == 0)
            {
                return false;
            }

            _recipes.AddRange(loaded.Where(r => r != null && r.requiredStation == CraftingStation.Workbench).OrderBy(r => r.recipeID));
            return _recipes.Count > 0;
        }

        private void RefreshAll()
        {
            EnsureRows();
            RefreshRows();
            RefreshSelection();
            UpdateQuantityUi();
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        }

        private void EnsureRows()
        {
            while (_rows.Count < _recipes.Count)
            {
                int capturedIndex = _rows.Count;
                RectTransform row = CreateRecipeRowRoot(recipeListContent, $"RecipeRow_{capturedIndex}");
                row.gameObject.AddComponent<LayoutElement>().preferredHeight = 58f;
                var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 8f;
                layout.padding = new RectOffset(0, 10, 8, 8);
                layout.childControlWidth = false;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;

                RectTransform accentBarRect = CreateRect(row, "AccentBar");
                accentBarRect.gameObject.AddComponent<LayoutElement>().preferredWidth = 4f;
                Image accentBar = accentBarRect.gameObject.AddComponent<Image>();
                accentBar.color = new Color(1f, 1f, 1f, 0f);
                accentBar.raycastTarget = false;

                Image icon = CreateIcon(row, "Icon", 24f);
                RectTransform info = CreateRect(row, "Info");
                info.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
                info.gameObject.AddComponent<VerticalLayoutGroup>().spacing = 2f;
                TextMeshProUGUI name = CreateText(info, "Name", "配方", 15f, new Color(0.96f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Left);
                TextMeshProUGUI summary = CreateText(info, "Summary", "材料摘要", 11f, new Color(0.78f, 0.84f, 0.9f, 0.95f), TextAlignmentOptions.Left, true);

                Button button = row.gameObject.AddComponent<Button>();
                button.targetGraphic = row.GetComponent<Image>();
                button.onClick.AddListener(() => SelectRecipe(capturedIndex));

                _rows.Add(new RowRefs { root = row.gameObject, background = row.GetComponent<Image>(), accentBar = accentBar, icon = icon, name = name, summary = summary });
            }
        }

        private void RefreshRows()
        {
            for (int index = 0; index < _rows.Count; index++)
            {
                RecipeData recipe = _recipes[index];
                ItemData item = ResolveItem(recipe.resultItemID);
                bool selected = index == _selectedIndex;
                bool canCraft = GetMaxCraftableCount(recipe) > 0;
                _rows[index].icon.sprite = item != null ? item.GetBagSprite() : null;
                _rows[index].icon.color = _rows[index].icon.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0f);
                _rows[index].name.text = string.IsNullOrWhiteSpace(recipe.recipeName) ? (item != null ? item.itemName : $"配方 {recipe.recipeID}") : recipe.recipeName;
                _rows[index].summary.text = BuildSummary(recipe);
                _rows[index].background.color = selected
                    ? SelectedRowColor
                    : canCraft ? ReadyRowColor : DisabledRowColor;
                _rows[index].accentBar.color = selected ? AccentColor : new Color(1f, 1f, 1f, 0f);
            }
        }

        private void SelectRecipe(int index)
        {
            if (index < 0 || index >= _recipes.Count)
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
            selectedNameText.text = string.IsNullOrWhiteSpace(recipe.recipeName) ? (item != null ? item.itemName : $"配方 {recipe.recipeID}") : recipe.recipeName;
            selectedDescriptionText.text = string.IsNullOrWhiteSpace(recipe.description)
                ? (item != null && !string.IsNullOrWhiteSpace(item.description) ? item.description : "暂无说明")
                : recipe.description;
            selectedMaterialsText.text = BuildDetailText(recipe, _selectedQuantity);
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
            decreaseButton.interactable = _selectedQuantity > 1;
            increaseButton.interactable = _selectedQuantity < maxCraftable;

            bool canCraft = recipe != null && _selectedQuantity <= GetMaxCraftableCount(recipe);
            craftButton.interactable = canCraft;
            craftButtonLabel.text = canCraft ? $"制作 x{_selectedQuantity}" : "材料不足";
            if (craftButtonBackground != null)
            {
                craftButtonBackground.color = canCraft
                    ? new Color(0.33f, 0.53f, 0.44f, 0.98f)
                    : new Color(0.27f, 0.3f, 0.34f, 0.9f);
            }
        }

        private void ChangeQuantity(int delta)
        {
            SetQuantity(_selectedQuantity + delta);
        }

        private void SetQuantity(int quantity, bool updateSlider = true)
        {
            RecipeData recipe = GetSelectedRecipe();
            int maxCraftable = recipe != null ? Mathf.Max(1, GetMaxCraftableCount(recipe)) : 1;
            _selectedQuantity = Mathf.Clamp(quantity, 1, maxCraftable);
            if (updateSlider)
            {
                quantitySlider.SetValueWithoutNotify(_selectedQuantity);
            }

            RefreshSelection();
            UpdateQuantityUi();
        }

        private void OnCraftButtonClicked()
        {
            RecipeData recipe = GetSelectedRecipe();
            if (_craftingService == null || recipe == null)
            {
                return;
            }

            int successCount = 0;
            for (int index = 0; index < _selectedQuantity; index++)
            {
                CraftResult result = _craftingService.TryCraft(recipe);
                if (!result.success)
                {
                    break;
                }

                successCount++;
            }

            if (successCount > 0)
            {
                _selectedQuantity = 1;
                RefreshAll();
            }
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
            for (int slotIndex = 0; slotIndex < inventory.Size; slotIndex++)
            {
                ItemStack slot = inventory.GetSlot(slotIndex);
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

        private string BuildSummary(RecipeData recipe)
        {
            return recipe.ingredients == null
                ? "暂无材料"
                : string.Join(" · ", recipe.ingredients.Select(i => $"{ResolveItem(i.itemID)?.itemName ?? $"材料{i.itemID}"} x{i.amount}"));
        }

        private string BuildDetailText(RecipeData recipe, int quantity)
        {
            List<string> lines = new List<string>();
            foreach (RecipeIngredient ingredient in recipe.ingredients)
            {
                ItemData item = ResolveItem(ingredient.itemID);
                int owned = _craftingService != null ? _craftingService.GetMaterialCount(ingredient.itemID) : 0;
                int required = ingredient.amount * quantity;
                string color = owned >= required ? "#B7E6C2" : "#F0B49E";
                lines.Add($"<color={color}>{item?.itemName ?? $"材料 {ingredient.itemID}"}</color>    {owned}/{required}");
            }

            int maxCraftable = GetMaxCraftableCount(recipe);
            lines.Add(string.Empty);
            lines.Add($"最多可制作  {Mathf.Max(0, maxCraftable)}");
            return string.Join("\n", lines);
        }

        private RecipeData GetSelectedRecipe()
        {
            return _selectedIndex >= 0 && _selectedIndex < _recipes.Count ? _recipes[_selectedIndex] : null;
        }

        private ItemData ResolveItem(int itemId)
        {
            ItemDatabase database = _craftingService != null ? _craftingService.Database : null;
            if (database == null && _inventoryService != null)
            {
                database = _inventoryService.Database;
            }

            return database != null ? database.GetItemByID(itemId) : null;
        }

        private void BindInventory(InventoryService inventoryService)
        {
            if (_inventoryService == inventoryService)
            {
                return;
            }

            UnbindInventory();
            _inventoryService = inventoryService;
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
        }

        private void Reposition()
        {
            if (panelRect == null || rootRect == null || _anchorTarget == null)
            {
                return;
            }

            Camera projectionCamera = GetWorldProjectionCamera();
            if (projectionCamera == null)
            {
                return;
            }

            Bounds bounds = GetAnchorBounds();
            Vector3 worldAnchor = _displayBelow
                ? new Vector3(bounds.center.x, bounds.min.y, bounds.center.z)
                : new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(projectionCamera, worldAnchor);
            screenPoint.y += _displayBelow ? -BelowOffsetPixels : AboveOffsetPixels;

            Camera uiCamera = GetUiEventCamera();
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, uiCamera, out Vector2 localPoint))
            {
                return;
            }

            Rect rootBounds = rootRect.rect;
            float width = panelRect.rect.width > 0f ? panelRect.rect.width : PanelWidth;
            float height = panelRect.rect.height > 0f ? panelRect.rect.height : PanelHeight;
            float minX = rootBounds.xMin + width * panelRect.pivot.x + ScreenMargin;
            float maxX = rootBounds.xMax - width * (1f - panelRect.pivot.x) - ScreenMargin;
            float minY = rootBounds.yMin + height * panelRect.pivot.y + ScreenMargin;
            float maxY = rootBounds.yMax - height * (1f - panelRect.pivot.y) - ScreenMargin;

            panelRect.anchoredPosition = new Vector2(
                Mathf.Clamp(localPoint.x, minX, maxX),
                Mathf.Clamp(localPoint.y, minY, maxY));
        }

        private Camera GetWorldProjectionCamera()
        {
            if (overlayCanvas != null && overlayCanvas.worldCamera != null)
            {
                return overlayCanvas.worldCamera;
            }

            Canvas parentCanvas = transform.parent != null ? transform.parent.GetComponentInParent<Canvas>() : null;
            if (parentCanvas != null && parentCanvas.worldCamera != null)
            {
                return parentCanvas.worldCamera;
            }

            return Camera.main;
        }

        private Camera GetUiEventCamera()
        {
            if (overlayCanvas == null || overlayCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            if (overlayCanvas.worldCamera != null)
            {
                return overlayCanvas.worldCamera;
            }

            Canvas parentCanvas = transform.parent != null ? transform.parent.GetComponentInParent<Canvas>() : null;
            if (parentCanvas != null && parentCanvas.worldCamera != null)
            {
                return parentCanvas.worldCamera;
            }

            return Camera.main;
        }

        private Bounds GetAnchorBounds()
        {
            if (_anchorTarget == null)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }

            Collider2D col = _anchorTarget.GetComponent<Collider2D>() ?? _anchorTarget.GetComponentInChildren<Collider2D>();
            if (col != null)
            {
                return col.bounds;
            }

            SpriteRenderer sr = _anchorTarget.GetComponent<SpriteRenderer>() ?? _anchorTarget.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                return sr.bounds;
            }

            return new Bounds(_anchorTarget.position, Vector3.one);
        }

        private Vector2 GetClosestInteractionPoint(Vector2 playerPosition)
        {
            Collider2D col = _anchorTarget != null ? (_anchorTarget.GetComponent<Collider2D>() ?? _anchorTarget.GetComponentInChildren<Collider2D>()) : null;
            return col != null ? col.ClosestPoint(playerPosition) : (_anchorTarget != null ? (Vector2)_anchorTarget.position : Vector2.zero);
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

                _gameInputManager = Object.FindFirstObjectByType<GameInputManager>(FindObjectsInactive.Include);
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
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                Canvas canvas = uiRoot.GetComponent<Canvas>() ?? uiRoot.GetComponentInChildren<Canvas>(true);
                return canvas != null ? canvas.transform : uiRoot.transform;
            }

            Canvas fallback = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            return fallback != null ? fallback.transform : null;
        }

        private static Transform ResolvePlayerTransform()
        {
            PlayerMovement playerMovement = Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            return playerMovement != null ? playerMovement.transform : null;
        }

        private RectTransform CreatePanel(Transform parent, string name, float preferredWidth = -1f)
        {
            RectTransform rect = CreateRect(parent, name);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = SectionColor;
            image.raycastTarget = true;
            Outline outline = rect.gameObject.AddComponent<Outline>();
            ApplyOutline(outline, new Color(1f, 1f, 1f, 0.05f), new Vector2(1f, -1f));
            var layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            if (preferredWidth > 0f)
            {
                rect.gameObject.AddComponent<LayoutElement>().preferredWidth = preferredWidth;
            }

            return rect;
        }

        private RectTransform CreateRecipeRowRoot(Transform parent, string name)
        {
            RectTransform rect = CreateRect(parent, name);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = ReadyRowColor;
            image.raycastTarget = true;
            Outline outline = rect.gameObject.AddComponent<Outline>();
            ApplyOutline(outline, new Color(1f, 1f, 1f, 0.06f), new Vector2(1f, -1f));
            return rect;
        }

        private RectTransform CreateDivider(Transform parent, string name)
        {
            RectTransform rect = CreateRect(parent, name);
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);
            image.raycastTarget = false;
            rect.sizeDelta = new Vector2(0f, 1f);
            return rect;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment, bool wrap = false)
        {
            RectTransform rect = CreateRect(parent, name);
            StretchRect(rect);
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
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = size;
            layout.preferredHeight = size;
            Image image = rect.gameObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0f);
            return image;
        }

        private Button CreateButton(Transform parent, string name, string label, float width, float height, float fontSize)
        {
            RectTransform rect = CreateRect(parent, name);
            LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
            if (width > 0f)
            {
                layout.preferredWidth = width;
            }

            layout.preferredHeight = height;
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.27f, 0.36f, 0.43f, 0.98f);
            image.raycastTarget = true;
            Outline outline = rect.gameObject.AddComponent<Outline>();
            ApplyOutline(outline, new Color(1f, 1f, 1f, 0.1f), new Vector2(1f, -1f));
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            TextMeshProUGUI labelText = CreateText(rect, "Label", label, fontSize, Color.white, TextAlignmentOptions.Center);
            StretchRect(labelText.rectTransform, new Vector2(6f, 4f), new Vector2(-6f, -4f));
            return button;
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

        private Slider CreateSlider(Transform parent)
        {
            RectTransform rect = CreateRect(parent, "Slider");
            LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            layout.preferredHeight = 34f;
            Slider slider = rect.gameObject.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;

            RectTransform background = CreateRect(rect, "Background");
            StretchRect(background, new Vector2(4f, 13f), new Vector2(-4f, -13f));
            Image backgroundImage = background.gameObject.AddComponent<Image>();
            backgroundImage.color = new Color(0.18f, 0.22f, 0.3f, 0.94f);
            backgroundImage.raycastTarget = true;

            RectTransform fillArea = CreateRect(rect, "FillArea");
            StretchRect(fillArea, new Vector2(5f, 13f), new Vector2(-18f, -13f));
            RectTransform fill = CreateRect(fillArea, "Fill");
            StretchRect(fill);
            fill.gameObject.AddComponent<Image>().color = new Color(0.46f, 0.71f, 0.56f, 0.96f);

            RectTransform handleArea = CreateRect(rect, "HandleSlideArea");
            StretchRect(handleArea, new Vector2(12f, 6f), new Vector2(-12f, -6f));
            RectTransform handle = CreateRect(handleArea, "Handle");
            handle.sizeDelta = new Vector2(16f, 24f);
            Image handleImage = handle.gameObject.AddComponent<Image>();
            handleImage.color = AccentColor;
            handleImage.raycastTarget = true;
            ApplyOutline(handle.gameObject.AddComponent<Outline>(), new Color(1f, 1f, 1f, 0.12f), new Vector2(1f, -1f));

            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = handleImage;
            return slider;
        }

        private static void StretchRect(RectTransform rect, Vector2? offsetMin = null, Vector2? offsetMax = null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = offsetMin ?? Vector2.zero;
            rect.offsetMax = offsetMax ?? Vector2.zero;
        }

        private static void CenterRect(RectTransform rect)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }

        private sealed class RowRefs
        {
            public GameObject root;
            public Image background;
            public Image accentBar;
            public Image icon;
            public TextMeshProUGUI name;
            public TextMeshProUGUI summary;
        }
    }
}
