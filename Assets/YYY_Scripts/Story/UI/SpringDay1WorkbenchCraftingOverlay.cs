using System.Collections.Generic;
using System.Linq;
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
        private const float PanelWidth = 660f;
        private const float PanelHeight = 360f;
        private const float ScreenMargin = 20f;
        private const float AboveOffset = 1.0f;
        private const float BelowOffset = 0.9f;

        private static SpringDay1WorkbenchCraftingOverlay _instance;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI hintText;
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
        [SerializeField] private TextMeshProUGUI craftButtonLabel;
        [SerializeField] private TextMeshProUGUI statusText;

        private readonly List<RecipeData> _recipes = new();
        private readonly List<RowRefs> _rows = new();

        private TMP_FontAsset _fontAsset;
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private Transform _anchorTarget;
        private Transform _playerTransform;
        private CraftingService _craftingService;
        private InventoryService _inventoryService;
        private int _selectedIndex = -1;
        private int _selectedQuantity = 1;
        private float _autoHideDistance = 1.5f;
        private bool _displayBelow;
        private bool _isVisible;

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
            GameObject root = new GameObject(nameof(SpringDay1WorkbenchCraftingOverlay), typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
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
            UnbindInventory();
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
            _displayBelow = _playerTransform != null && _playerTransform.position.y > GetAnchorBounds().center.y;
            panelRect.pivot = _displayBelow ? new Vector2(0.5f, 1f) : new Vector2(0.5f, 0f);
            BindInventory(craftingService.Inventory);

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
            transform.SetAsLastSibling();
            Reposition();
        }

        public void Hide()
        {
            _isVisible = false;
            _anchorTarget = null;
            _playerTransform = null;
            _craftingService = null;
            UnbindInventory();
            HideImmediate();
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            Hide();
        }

        private void BuildUi()
        {
            panelRect = transform as RectTransform;
            canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas != null ? _canvas.transform as RectTransform : null;
            _fontAsset = ResolveFont();

            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);
            GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.12f, 0.9f);

            var rootLayout = gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(18, 18, 14, 14);
            rootLayout.spacing = 10f;
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;

            titleText = CreateText(transform, "Title", "工作台制作", 28f, new Color(0.98f, 0.96f, 0.9f, 1f), TextAlignmentOptions.Center);
            hintText = CreateText(transform, "Hint", "靠近后按 E 打开，离开 1.5 米自动关闭", 15f, new Color(0.8f, 0.86f, 0.94f, 0.95f), TextAlignmentOptions.Center);

            RectTransform body = CreateRect(transform, "Body");
            body.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            var bodyLayout = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            bodyLayout.spacing = 12f;
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = false;
            bodyLayout.childForceExpandHeight = true;

            RectTransform left = CreatePanel(body, "RecipeColumn", 228f);
            CreateText(left, "RecipeHeader", "配方选择", 18f, new Color(0.96f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Left);
            RectTransform scrollRoot = CreatePanel(left, "RecipeScrollRoot");
            scrollRoot.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;
            ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            RectTransform viewport = CreateRect(scrollRoot, "Viewport");
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(4f, 4f);
            viewport.offsetMax = new Vector2(-4f, -4f);
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
            right.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            RectTransform detailHeader = CreateRect(right, "DetailHeader");
            detailHeader.gameObject.AddComponent<HorizontalLayoutGroup>().spacing = 10f;
            selectedIcon = CreateIcon(detailHeader, "SelectedIcon", 48f);
            RectTransform titleBlock = CreateRect(detailHeader, "TitleBlock");
            titleBlock.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            titleBlock.gameObject.AddComponent<VerticalLayoutGroup>().spacing = 4f;
            selectedNameText = CreateText(titleBlock, "SelectedName", "请选择配方", 24f, new Color(0.98f, 0.96f, 0.9f, 1f), TextAlignmentOptions.Left);
            selectedDescriptionText = CreateText(titleBlock, "SelectedDescription", "从左侧选择一个配方后，右侧会显示材料和制作说明。", 14f, new Color(0.82f, 0.88f, 0.94f, 0.95f), TextAlignmentOptions.TopLeft, true);

            selectedMaterialsText = CreateText(right, "SelectedMaterials", "材料需求会实时读取配方 SO 与当前背包。", 15f, new Color(0.77f, 0.84f, 0.9f, 1f), TextAlignmentOptions.TopLeft, true);
            selectedMaterialsText.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1f;

            RectTransform quantityArea = CreateRect(right, "QuantityArea");
            quantityArea.gameObject.AddComponent<VerticalLayoutGroup>().spacing = 6f;
            CreateText(quantityArea, "QuantityTitle", "制作数量", 18f, new Color(0.96f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Left);
            RectTransform controls = CreateRect(quantityArea, "QuantityControls");
            controls.gameObject.AddComponent<HorizontalLayoutGroup>().spacing = 8f;
            decreaseButton = CreateButton(controls, "Decrease", "-", 34f);
            quantitySlider = CreateSlider(controls);
            quantityValueText = CreateText(controls, "QuantityValue", "x1", 16f, new Color(0.77f, 0.9f, 0.84f, 1f), TextAlignmentOptions.Center);
            quantityValueText.gameObject.AddComponent<LayoutElement>().preferredWidth = 58f;
            increaseButton = CreateButton(controls, "Increase", "+", 34f);

            RectTransform footer = CreateRect(right, "Footer");
            footer.gameObject.AddComponent<HorizontalLayoutGroup>().spacing = 10f;
            statusText = CreateText(footer, "Status", "选择一个配方开始制作。", 15f, new Color(0.78f, 0.9f, 0.82f, 1f), TextAlignmentOptions.Left, true);
            statusText.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            craftButton = CreateButton(footer, "Craft", "制作 x1", 150f);
            craftButtonLabel = craftButton.GetComponentInChildren<TextMeshProUGUI>();

            decreaseButton.onClick.AddListener(() => ChangeQuantity(-1));
            increaseButton.onClick.AddListener(() => ChangeQuantity(1));
            quantitySlider.onValueChanged.AddListener(v => SetQuantity(Mathf.RoundToInt(v), false));
            craftButton.onClick.AddListener(OnCraftButtonClicked);
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
                row.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;
                var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 8f;
                layout.padding = new RectOffset(8, 8, 8, 8);
                layout.childControlWidth = false;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;

                Image icon = CreateIcon(row, "Icon", 30f);
                RectTransform info = CreateRect(row, "Info");
                info.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
                info.gameObject.AddComponent<VerticalLayoutGroup>().spacing = 2f;
                TextMeshProUGUI name = CreateText(info, "Name", "配方", 17f, new Color(0.96f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Left);
                TextMeshProUGUI summary = CreateText(info, "Summary", "材料摘要", 12f, new Color(0.78f, 0.84f, 0.9f, 0.95f), TextAlignmentOptions.Left, true);

                Button button = row.gameObject.AddComponent<Button>();
                button.targetGraphic = row.GetComponent<Image>();
                button.onClick.AddListener(() => SelectRecipe(capturedIndex));

                _rows.Add(new RowRefs { root = row.gameObject, background = row.GetComponent<Image>(), icon = icon, name = name, summary = summary });
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
                    ? new Color(0.24f, 0.32f, 0.25f, 0.96f)
                    : canCraft ? new Color(0.16f, 0.2f, 0.27f, 0.92f) : new Color(0.25f, 0.18f, 0.15f, 0.9f);
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
            selectedDescriptionText.text = string.IsNullOrWhiteSpace(recipe.description) ? "暂无配方说明。" : recipe.description;
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
            CraftResult lastResult = default;
            for (int index = 0; index < _selectedQuantity; index++)
            {
                lastResult = _craftingService.TryCraft(recipe);
                if (!lastResult.success)
                {
                    break;
                }

                successCount++;
            }

            if (successCount > 0)
            {
                statusText.color = new Color(0.78f, 0.9f, 0.82f, 1f);
                statusText.text = $"已制作 {recipe.recipeName} x{successCount}";
            }
            else
            {
                statusText.color = new Color(0.98f, 0.75f, 0.62f, 1f);
                statusText.text = string.IsNullOrWhiteSpace(lastResult.message) ? "当前无法制作该配方。" : lastResult.message;
            }

            _selectedQuantity = 1;
            RefreshAll();
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
                lines.Add($"{item?.itemName ?? $"材料 {ingredient.itemID}"}：{owned}/{required}");
            }

            int maxCraftable = GetMaxCraftableCount(recipe);
            lines.Add(string.Empty);
            lines.Add(maxCraftable > 0 ? $"当前最多可制作 {maxCraftable} 次" : "当前材料不足，暂时无法制作");
            lines.Add("工作台 UI 只响应鼠标左键，右键停在面板上不会透到底板。");
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
            if (_canvasRect == null)
            {
                return;
            }

            Camera projectionCamera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? (_canvas.worldCamera != null ? _canvas.worldCamera : Camera.main)
                : null;
            Vector3 worldPosition = GetAnchorBounds().center + Vector3.up * (_displayBelow ? -BelowOffset : AboveOffset);
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(projectionCamera, worldPosition);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPoint, projectionCamera, out Vector2 localPoint))
            {
                return;
            }

            float width = panelRect.rect.width > 0f ? panelRect.rect.width : PanelWidth;
            float height = panelRect.rect.height > 0f ? panelRect.rect.height : PanelHeight;
            float halfWidth = _canvasRect.rect.width * 0.5f;
            float halfHeight = _canvasRect.rect.height * 0.5f;
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
            image.color = new Color(0.12f, 0.15f, 0.21f, 0.82f);
            image.raycastTarget = true;
            var layout = rect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 12, 12);
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
            image.color = new Color(0.12f, 0.15f, 0.21f, 0.82f);
            image.raycastTarget = true;
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
            image.color = new Color(0.3f, 0.38f, 0.48f, 0.96f);
            image.raycastTarget = true;
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            CreateText(rect, "Label", label, 18f, Color.white, TextAlignmentOptions.Center);
            return button;
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
            public Image icon;
            public TextMeshProUGUI name;
            public TextMeshProUGUI summary;
        }
    }
}
