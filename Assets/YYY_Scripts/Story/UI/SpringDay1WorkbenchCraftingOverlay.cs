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
        private const float PanelWidth = 512f;
        private const float PanelHeight = 292f;
        private const float RecipeColumnWidth = 176f;
        private const float ScreenMargin = 18f;
        private const float AboveOffsetPixels = 62f;
        private const float BelowOffsetPixels = 54f;
        private const float AboveOffset = AboveOffsetPixels;
        private const float BelowOffset = BelowOffsetPixels;
        private const float PointerSize = 16f;
        private const int OverlaySortingOrder = 180;

        private static readonly Color CardColor = new(0.055f, 0.075f, 0.11f, 0.965f);
        private static readonly Color SectionColor = new(0.09f, 0.12f, 0.18f, 0.92f);
        private static readonly Color SelectedRowColor = new(0.2f, 0.31f, 0.28f, 0.97f);
        private static readonly Color ReadyRowColor = new(0.13f, 0.17f, 0.22f, 0.94f);
        private static readonly Color DisabledRowColor = new(0.2f, 0.14f, 0.12f, 0.92f);
        private static readonly Color AccentColor = new(0.62f, 0.84f, 0.74f, 1f);

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

            GameObject root = new GameObject(
                nameof(SpringDay1WorkbenchCraftingOverlay),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(GraphicRaycaster),
                typeof(CanvasGroup));
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

            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = OverlaySortingOrder;
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            GameObject panelRoot = new GameObject("PanelRoot", typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow));
            panelRoot.transform.SetParent(transform, false);
            panelRect = panelRoot.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);
            Image panelBackground = panelRoot.GetComponent<Image>();
            panelBackground.color = CardColor;
            panelBackground.raycastTarget = true;
            ApplyOutline(panelRoot.GetComponent<Outline>(), new Color(0.86f, 0.94f, 1f, 0.08f), new Vector2(1f, -1f));
            ApplyShadow(panelRoot.GetComponent<Shadow>(), new Color(0f, 0f, 0f, 0.42f), new Vector2(0f, -6f));

            pointerRect = CreateRect(panelRect, "Pointer");
            pointerRect.sizeDelta = new Vector2(PointerSize, PointerSize);
            pointerRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
            Image pointerImage = pointerRect.gameObject.AddComponent<Image>();
            pointerImage.color = CardColor;
            pointerImage.raycastTarget = false;
            ApplyOutline(pointerRect.gameObject.AddComponent<Outline>(), new Color(0.86f, 0.94f, 1f, 0.08f), new Vector2(1f, -1f));

            RectTransform body = CreateRect(panelRect, "Body");
            body.anchorMin = Vector2.zero;
            body.anchorMax = Vector2.one;
            body.offsetMin = new Vector2(14f, 14f);
            body.offsetMax = new Vector2(-14f, -14f);
            var bodyLayout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            bodyLayout.spacing = 12f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = true;

            RectTransform left = CreatePanel(body, "RecipeColumn", RecipeColumnWidth);
            RectTransform scrollRoot = CreateRect(left, "RecipeScrollRoot");
            scrollRoot.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            scrollRoot.anchorMin = Vector2.zero;
            scrollRoot.anchorMax = Vector2.one;
            ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.scrollSensitivity = 24f;

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
            var recipeListLayout = recipeListContent.gameObject.AddComponent<VerticalLayoutGroup>();
            recipeListLayout.spacing = 8f;
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

            RectTransform detailHeader = CreateRect(right, "DetailHeader");
            detailHeader.gameObject.AddComponent<LayoutElement>().preferredHeight = 56f;
            HorizontalLayoutGroup detailHeaderLayout = detailHeader.gameObject.AddComponent<HorizontalLayoutGroup>();
            detailHeaderLayout.spacing = 10f;
            detailHeaderLayout.childControlWidth = false;
            detailHeaderLayout.childControlHeight = true;
            selectedIcon = CreateIcon(detailHeader, "SelectedIcon", 42f);
            RectTransform titleBlock = CreateRect(detailHeader, "TitleBlock");
            titleBlock.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            titleBlock.gameObject.AddComponent<VerticalLayoutGroup>().spacing = 4f;
            selectedNameText = CreateText(titleBlock, "SelectedName", string.Empty, 22f, new Color(0.98f, 0.96f, 0.9f, 1f), TextAlignmentOptions.Left);
            selectedDescriptionText = CreateText(titleBlock, "SelectedDescription", string.Empty, 13f, new Color(0.82f, 0.88f, 0.94f, 0.95f), TextAlignmentOptions.TopLeft, true);

            RectTransform topDivider = CreateRect(right, "TopDivider");
            topDivider.gameObject.AddComponent<LayoutElement>().preferredHeight = 1f;
            Image topDividerImage = topDivider.gameObject.AddComponent<Image>();
            topDividerImage.color = new Color(1f, 1f, 1f, 0.08f);
            topDividerImage.raycastTarget = false;

            selectedMaterialsText = CreateText(right, "SelectedMaterials", string.Empty, 15f, new Color(0.9f, 0.94f, 0.98f, 1f), TextAlignmentOptions.TopLeft, true);
            selectedMaterialsText.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;

            RectTransform bottomDivider = CreateRect(right, "BottomDivider");
            bottomDivider.gameObject.AddComponent<LayoutElement>().preferredHeight = 1f;
            Image bottomDividerImage = bottomDivider.gameObject.AddComponent<Image>();
            bottomDividerImage.color = new Color(1f, 1f, 1f, 0.08f);
            bottomDividerImage.raycastTarget = false;

            RectTransform controls = CreateRect(right, "QuantityControls");
            controls.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
            controls.gameObject.AddComponent<HorizontalLayoutGroup>().spacing = 8f;
            decreaseButton = CreateButton(controls, "Decrease", "−", 34f);
            quantitySlider = CreateSlider(controls);
            quantityValueText = CreateText(controls, "QuantityValue", "x1", 15f, AccentColor, TextAlignmentOptions.Center);
            quantityValueText.gameObject.AddComponent<LayoutElement>().preferredWidth = 52f;
            increaseButton = CreateButton(controls, "Increase", "+", 34f);

            RectTransform footer = CreateRect(right, "Footer");
            footer.gameObject.AddComponent<HorizontalLayoutGroup>().spacing = 10f;
            RectTransform spacer = CreateRect(footer, "Spacer");
            spacer.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            craftButton = CreateButton(footer, "Craft", "制作", 132f);
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
            if (rootRect == null)
            {
                return;
            }

            Camera projectionCamera = overlayCanvas != null && overlayCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? (overlayCanvas.worldCamera != null ? overlayCanvas.worldCamera : Camera.main)
                : null;
            Bounds bounds = GetAnchorBounds();
            Vector3 worldAnchor = _displayBelow
                ? new Vector3(bounds.center.x, bounds.min.y, bounds.center.z)
                : new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(projectionCamera, worldAnchor);
            screenPoint.y += _displayBelow ? -BelowOffsetPixels : AboveOffsetPixels;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, projectionCamera, out Vector2 localPoint))
            {
                return;
            }

            float width = panelRect.rect.width > 0f ? panelRect.rect.width : PanelWidth;
            float height = panelRect.rect.height > 0f ? panelRect.rect.height : PanelHeight;
            float halfWidth = rootRect.rect.width * 0.5f;
            float halfHeight = rootRect.rect.height * 0.5f;
            float minX = -halfWidth + width * panelRect.pivot.x + ScreenMargin;
            float maxX = halfWidth - width * (1f - panelRect.pivot.x) - ScreenMargin;
            float minY = -halfHeight + height * panelRect.pivot.y + ScreenMargin;
            float maxY = halfHeight - height * (1f - panelRect.pivot.y) - ScreenMargin;

            panelRect.anchoredPosition = new Vector2(Mathf.Clamp(localPoint.x, minX, maxX), Mathf.Clamp(localPoint.y, minY, maxY));
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

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment, bool wrap = false)
        {
            RectTransform rect = CreateRect(parent, name);
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
            LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = size;
            layout.preferredHeight = size;
            Image image = rect.gameObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0f);
            return image;
        }

        private Button CreateButton(Transform parent, string name, string label, float width)
        {
            RectTransform rect = CreateRect(parent, name);
            LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = width;
            layout.preferredHeight = 36f;
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.27f, 0.36f, 0.43f, 0.98f);
            image.raycastTarget = true;
            Outline outline = rect.gameObject.AddComponent<Outline>();
            ApplyOutline(outline, new Color(1f, 1f, 1f, 0.1f), new Vector2(1f, -1f));
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            CreateText(rect, "Label", label, 18f, Color.white, TextAlignmentOptions.Center);
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
            rect.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            Image background = rect.gameObject.AddComponent<Image>();
            background.color = new Color(0.18f, 0.22f, 0.3f, 0.94f);
            background.raycastTarget = true;
            Slider slider = rect.gameObject.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;

            RectTransform fillArea = CreateRect(rect, "FillArea");
            fillArea.anchorMin = Vector2.zero;
            fillArea.anchorMax = Vector2.one;
            fillArea.offsetMin = new Vector2(8f, 8f);
            fillArea.offsetMax = new Vector2(-8f, -8f);
            RectTransform fill = CreateRect(fillArea, "Fill");
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
            fill.gameObject.AddComponent<Image>().color = new Color(0.46f, 0.71f, 0.56f, 0.96f);

            RectTransform handle = CreateRect(rect, "Handle");
            handle.sizeDelta = new Vector2(16f, 24f);
            Image handleImage = handle.gameObject.AddComponent<Image>();
            handleImage.color = Color.white;
            handleImage.raycastTarget = true;

            slider.fillRect = fill;
            slider.handleRect = handle;
            slider.targetGraphic = handleImage;
            return slider;
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
