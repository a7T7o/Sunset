using System.Collections.Generic;
using FarmGame.Data;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    /// <summary>
    /// spring-day1 专用的最小工作台制作浮层。
    /// 运行时动态创建，跟随工作台显示，不依赖场景手工搭 UI。
    /// </summary>
    [DisallowMultipleComponent]
    public class SpringDay1WorkbenchCraftingOverlay : MonoBehaviour
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private const int AxeItemId = 0;
        private const int PickaxeItemId = 6;
        private const int HoeItemId = 12;
        private const int WoodItemId = 3200;
        private const int StoneItemId = 3201;
        private const float VerticalWorldOffset = 1.4f;
        private const float HorizontalMargin = 18f;
        private const float VerticalMargin = 18f;

        private static SpringDay1WorkbenchCraftingOverlay _instance;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private TextMeshProUGUI statusText;

        private readonly List<WorkbenchRecipeDefinition> _recipeDefinitions = new();
        private readonly List<RecipeRow> _recipeRows = new();
        private readonly List<RecipeData> _runtimeRecipes = new();

        private Transform _anchorTarget;
        private Canvas _rootCanvas;
        private RectTransform _rootCanvasRect;
        private TMP_FontAsset _resolvedFontAsset;
        private CraftingService _craftingService;
        private InventoryService _inventoryService;
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

            Transform parent = null;
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                parent = uiRoot.transform;
            }
            else
            {
                Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
                parent = canvas != null ? canvas.transform : null;
            }

            GameObject root = new GameObject(
                nameof(SpringDay1WorkbenchCraftingOverlay),
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));

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
            DetachInventoryListener();
        }

        private void OnDestroy()
        {
            for (int index = 0; index < _runtimeRecipes.Count; index++)
            {
                if (_runtimeRecipes[index] != null)
                {
                    Destroy(_runtimeRecipes[index]);
                }
            }
        }

        private void LateUpdate()
        {
            if (!_isVisible || panelRect == null || _rootCanvasRect == null)
            {
                return;
            }

            RepositionToAnchor();
        }

        public bool Toggle(Transform anchorTarget, CraftingService craftingService, CraftingStation station)
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

            Open(anchorTarget, craftingService);
            return true;
        }

        public void Open(Transform anchorTarget, CraftingService craftingService)
        {
            if (panelRect == null)
            {
                BuildUi();
            }

            if (craftingService == null)
            {
                Hide();
                return;
            }

            _anchorTarget = anchorTarget;
            _craftingService = craftingService;
            _craftingService.SetStation(CraftingStation.Workbench);

            BindInventoryListener(craftingService.Inventory);
            EnsureRecipeDefinitions();
            EnsureRuntimeRecipes();
            RefreshRecipeRows();

            _isVisible = true;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(true);
            RepositionToAnchor();
        }

        public void Hide()
        {
            _isVisible = false;
            _anchorTarget = null;
            _craftingService = null;
            DetachInventoryListener();
            HideImmediate();
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            Hide();
        }

        private void BuildUi()
        {
            panelRect = transform as RectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(440f, 0f);

            canvasGroup = GetComponent<CanvasGroup>();

            Image panelImage = GetComponent<Image>();
            panelImage.color = new Color(0.08f, 0.1f, 0.14f, 0.96f);
            panelImage.raycastTarget = true;

            VerticalLayoutGroup layout = GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 16, 16);
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            ContentSizeFitter fitter = GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _rootCanvas = GetComponentInParent<Canvas>();
            _rootCanvasRect = _rootCanvas != null ? _rootCanvas.transform as RectTransform : null;
            _resolvedFontAsset = ResolveFontAsset();

            titleText = CreateText("Title", "工作台制作", 28f, new Color(0.98f, 0.96f, 0.88f, 1f), TextAlignmentOptions.Center);
            hintText = CreateText("Hint", "点击配方直接制作 · 再按 E 关闭", 18f, new Color(0.83f, 0.87f, 0.94f, 0.92f), TextAlignmentOptions.Center);

            GameObject listRoot = new GameObject("RecipeList", typeof(RectTransform), typeof(VerticalLayoutGroup));
            listRoot.transform.SetParent(transform, false);

            VerticalLayoutGroup listLayout = listRoot.GetComponent<VerticalLayoutGroup>();
            listLayout.spacing = 8f;
            listLayout.childAlignment = TextAnchor.UpperCenter;
            listLayout.childControlHeight = true;
            listLayout.childControlWidth = true;
            listLayout.childForceExpandHeight = false;
            listLayout.childForceExpandWidth = true;

            RectTransform listRect = listRoot.GetComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0f, 1f);
            listRect.anchorMax = new Vector2(1f, 1f);
            listRect.pivot = new Vector2(0.5f, 1f);
            listRect.sizeDelta = new Vector2(0f, 0f);

            for (int index = 0; index < 3; index++)
            {
                _recipeRows.Add(CreateRecipeRow(index, listRoot.transform));
            }

            statusText = CreateText("Status", "制作配方已解锁。", 18f, new Color(0.76f, 0.9f, 0.82f, 1f), TextAlignmentOptions.Center);
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
            gameObject.SetActive(false);
        }

        private TextMeshProUGUI CreateText(string objectName, string text, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            textObject.transform.SetParent(transform, false);

            LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = fontSize + 10f;

            TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
            textComponent.font = _resolvedFontAsset;
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = color;
            textComponent.alignment = alignment;
            textComponent.textWrappingMode = TextWrappingModes.Normal;
            textComponent.raycastTarget = false;
            return textComponent;
        }

        private RecipeRow CreateRecipeRow(int rowIndex, Transform parent)
        {
            GameObject rowObject = new GameObject(
                $"RecipeRow_{rowIndex}",
                typeof(RectTransform),
                typeof(Image),
                typeof(HorizontalLayoutGroup),
                typeof(LayoutElement));
            rowObject.transform.SetParent(parent, false);

            Image rowBackground = rowObject.GetComponent<Image>();
            rowBackground.color = new Color(0.16f, 0.2f, 0.27f, 0.94f);
            rowBackground.raycastTarget = false;

            LayoutElement rowLayoutElement = rowObject.GetComponent<LayoutElement>();
            rowLayoutElement.preferredHeight = 72f;

            HorizontalLayoutGroup rowLayout = rowObject.GetComponent<HorizontalLayoutGroup>();
            rowLayout.padding = new RectOffset(10, 10, 8, 8);
            rowLayout.spacing = 10f;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlHeight = true;
            rowLayout.childControlWidth = true;
            rowLayout.childForceExpandHeight = false;
            rowLayout.childForceExpandWidth = false;

            GameObject iconObject = new GameObject("Icon", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            iconObject.transform.SetParent(rowObject.transform, false);
            LayoutElement iconLayout = iconObject.GetComponent<LayoutElement>();
            iconLayout.preferredWidth = 42f;
            iconLayout.preferredHeight = 42f;
            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            GameObject infoObject = new GameObject("Info", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            infoObject.transform.SetParent(rowObject.transform, false);
            LayoutElement infoLayout = infoObject.GetComponent<LayoutElement>();
            infoLayout.flexibleWidth = 1f;
            infoLayout.minWidth = 180f;

            VerticalLayoutGroup infoGroup = infoObject.GetComponent<VerticalLayoutGroup>();
            infoGroup.spacing = 2f;
            infoGroup.childAlignment = TextAnchor.MiddleLeft;
            infoGroup.childControlHeight = true;
            infoGroup.childControlWidth = true;
            infoGroup.childForceExpandHeight = false;
            infoGroup.childForceExpandWidth = true;

            TextMeshProUGUI nameText = CreateRowText("Name", infoObject.transform, 23f, new Color(0.98f, 0.95f, 0.9f, 1f), TextAlignmentOptions.Left);
            TextMeshProUGUI materialText = CreateRowText("Materials", infoObject.transform, 17f, new Color(0.84f, 0.9f, 0.94f, 0.95f), TextAlignmentOptions.Left);

            GameObject buttonObject = new GameObject(
                "CraftButton",
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(rowObject.transform, false);

            LayoutElement buttonLayout = buttonObject.GetComponent<LayoutElement>();
            buttonLayout.preferredWidth = 104f;
            buttonLayout.preferredHeight = 44f;

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.35f, 0.6f, 0.38f, 1f);

            Button craftButton = buttonObject.GetComponent<Button>();
            craftButton.targetGraphic = buttonImage;

            GameObject buttonTextObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            buttonTextObject.transform.SetParent(buttonObject.transform, false);
            TextMeshProUGUI buttonText = buttonTextObject.GetComponent<TextMeshProUGUI>();
            buttonText.font = _resolvedFontAsset;
            buttonText.fontSize = 19f;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.textWrappingMode = TextWrappingModes.NoWrap;
            buttonText.raycastTarget = false;

            RectTransform buttonTextRect = buttonText.rectTransform;
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;

            int capturedIndex = rowIndex;
            craftButton.onClick.AddListener(() => OnCraftButtonClicked(capturedIndex));

            return new RecipeRow
            {
                RootObject = rowObject,
                IconImage = iconImage,
                NameText = nameText,
                MaterialText = materialText,
                CraftButton = craftButton,
                CraftButtonBackground = buttonImage,
                CraftButtonLabel = buttonText
            };
        }

        private TextMeshProUGUI CreateRowText(string objectName, Transform parent, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            textObject.transform.SetParent(parent, false);

            LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = fontSize + 4f;

            TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
            textComponent.font = _resolvedFontAsset;
            textComponent.fontSize = fontSize;
            textComponent.color = color;
            textComponent.alignment = alignment;
            textComponent.textWrappingMode = TextWrappingModes.NoWrap;
            textComponent.overflowMode = TextOverflowModes.Ellipsis;
            textComponent.raycastTarget = false;
            return textComponent;
        }

        private TMP_FontAsset ResolveFontAsset()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return TMP_Settings.defaultFontAsset;
        }

        private void EnsureRecipeDefinitions()
        {
            if (_recipeDefinitions.Count > 0)
            {
                return;
            }

            _recipeDefinitions.Add(new WorkbenchRecipeDefinition
            {
                recipeId = 9100,
                resultItemId = AxeItemId,
                displayName = "Axe_0",
                ingredients = new List<IngredientDefinition>
                {
                    new IngredientDefinition(WoodItemId, 3)
                }
            });

            _recipeDefinitions.Add(new WorkbenchRecipeDefinition
            {
                recipeId = 9101,
                resultItemId = HoeItemId,
                displayName = "Hoe_0",
                ingredients = new List<IngredientDefinition>
                {
                    new IngredientDefinition(WoodItemId, 2)
                }
            });

            _recipeDefinitions.Add(new WorkbenchRecipeDefinition
            {
                recipeId = 9102,
                resultItemId = PickaxeItemId,
                displayName = "Pickaxe_0",
                ingredients = new List<IngredientDefinition>
                {
                    new IngredientDefinition(WoodItemId, 3),
                    new IngredientDefinition(StoneItemId, 2)
                }
            });
        }

        private void EnsureRuntimeRecipes()
        {
            if (_runtimeRecipes.Count == _recipeDefinitions.Count)
            {
                return;
            }

            for (int index = 0; index < _runtimeRecipes.Count; index++)
            {
                if (_runtimeRecipes[index] != null)
                {
                    Destroy(_runtimeRecipes[index]);
                }
            }

            _runtimeRecipes.Clear();

            for (int index = 0; index < _recipeDefinitions.Count; index++)
            {
                _runtimeRecipes.Add(CreateRuntimeRecipe(_recipeDefinitions[index]));
            }
        }

        private RecipeData CreateRuntimeRecipe(WorkbenchRecipeDefinition definition)
        {
            RecipeData recipe = ScriptableObject.CreateInstance<RecipeData>();
            recipe.hideFlags = HideFlags.HideAndDontSave;
            recipe.recipeID = definition.recipeId;
            recipe.recipeName = definition.displayName;
            recipe.description = "spring-day1 工作台临时配方";
            recipe.resultItemID = definition.resultItemId;
            recipe.resultAmount = 1;
            recipe.requiredStation = CraftingStation.Workbench;
            recipe.unlockedByDefault = true;
            recipe.requiredSkillLevel = 1;
            recipe.requiredLevel = 1;
            recipe.ingredients = new List<RecipeIngredient>();

            for (int index = 0; index < definition.ingredients.Count; index++)
            {
                IngredientDefinition ingredient = definition.ingredients[index];
                recipe.ingredients.Add(new RecipeIngredient
                {
                    itemID = ingredient.itemId,
                    amount = ingredient.amount
                });
            }

            return recipe;
        }

        private void RefreshRecipeRows()
        {
            for (int index = 0; index < _recipeRows.Count; index++)
            {
                bool hasRecipe = index < _recipeDefinitions.Count && index < _runtimeRecipes.Count;
                _recipeRows[index].SetVisible(hasRecipe);
                if (!hasRecipe)
                {
                    continue;
                }

                WorkbenchRecipeDefinition definition = _recipeDefinitions[index];
                RecipeData runtimeRecipe = _runtimeRecipes[index];
                ItemData resultItem = ResolveItemData(definition.resultItemId);

                _recipeRows[index].IconImage.sprite = resultItem != null ? resultItem.GetBagSprite() : null;
                _recipeRows[index].IconImage.color = resultItem != null ? Color.white : new Color(1f, 1f, 1f, 0f);
                _recipeRows[index].NameText.text = resultItem != null && !string.IsNullOrWhiteSpace(resultItem.itemName)
                    ? resultItem.itemName
                    : definition.displayName;
                _recipeRows[index].MaterialText.text = BuildMaterialLine(definition);

                bool canCraft = _craftingService != null && _craftingService.CanCraft(runtimeRecipe);
                _recipeRows[index].CraftButton.interactable = true;
                _recipeRows[index].CraftButtonLabel.text = canCraft ? "制作" : "缺材料";
                _recipeRows[index].CraftButtonBackground.color = canCraft
                    ? new Color(0.35f, 0.6f, 0.38f, 1f)
                    : new Color(0.4f, 0.34f, 0.24f, 1f);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        }

        private string BuildMaterialLine(WorkbenchRecipeDefinition definition)
        {
            List<string> parts = new List<string>(definition.ingredients.Count);
            for (int index = 0; index < definition.ingredients.Count; index++)
            {
                IngredientDefinition ingredient = definition.ingredients[index];
                ItemData itemData = ResolveItemData(ingredient.itemId);
                string itemName = itemData != null && !string.IsNullOrWhiteSpace(itemData.itemName)
                    ? itemData.itemName
                    : $"Item_{ingredient.itemId}";
                int ownedCount = _craftingService != null ? _craftingService.GetMaterialCount(ingredient.itemId) : 0;
                parts.Add($"{itemName} {ownedCount}/{ingredient.amount}");
            }

            return string.Join("  ·  ", parts);
        }

        private ItemData ResolveItemData(int itemId)
        {
            ItemDatabase database = _craftingService != null ? _craftingService.Database : null;
            if (database == null && _inventoryService != null)
            {
                database = _inventoryService.Database;
            }

            return database != null ? database.GetItemByID(itemId) : null;
        }

        private void OnCraftButtonClicked(int index)
        {
            if (_craftingService == null || index < 0 || index >= _runtimeRecipes.Count)
            {
                return;
            }

            CraftResult result = _craftingService.TryCraft(_runtimeRecipes[index]);
            statusText.color = result.success
                ? new Color(0.76f, 0.9f, 0.82f, 1f)
                : new Color(0.98f, 0.73f, 0.62f, 1f);
            statusText.text = result.message;
            RefreshRecipeRows();
        }

        private void BindInventoryListener(InventoryService inventoryService)
        {
            if (_inventoryService == inventoryService)
            {
                return;
            }

            DetachInventoryListener();
            _inventoryService = inventoryService;

            if (_inventoryService != null)
            {
                _inventoryService.OnInventoryChanged += HandleInventoryChanged;
            }
        }

        private void DetachInventoryListener()
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
                RefreshRecipeRows();
            }
        }

        private void RepositionToAnchor()
        {
            if (_anchorTarget == null || panelRect == null || _rootCanvasRect == null)
            {
                return;
            }

            Vector3 worldPosition = GetAnchorWorldPosition(_anchorTarget);
            Camera projectionCamera = null;
            if (_rootCanvas != null && _rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                projectionCamera = _rootCanvas.worldCamera != null ? _rootCanvas.worldCamera : Camera.main;
            }

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(projectionCamera, worldPosition);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootCanvasRect, screenPoint, projectionCamera, out Vector2 localPoint))
            {
                return;
            }

            float width = panelRect.rect.width > 0f ? panelRect.rect.width : 440f;
            float height = panelRect.rect.height > 0f ? panelRect.rect.height : 240f;

            float halfCanvasWidth = _rootCanvasRect.rect.width * 0.5f;
            float halfCanvasHeight = _rootCanvasRect.rect.height * 0.5f;

            float minX = -halfCanvasWidth + (width * 0.5f) + HorizontalMargin;
            float maxX = halfCanvasWidth - (width * 0.5f) - HorizontalMargin;
            float minY = -halfCanvasHeight + VerticalMargin;
            float maxY = halfCanvasHeight - height - VerticalMargin;

            panelRect.anchoredPosition = new Vector2(
                Mathf.Clamp(localPoint.x, minX, maxX),
                Mathf.Clamp(localPoint.y, minY, maxY));
        }

        private static Vector3 GetAnchorWorldPosition(Transform target)
        {
            Collider2D collider2D = target.GetComponent<Collider2D>();
            if (collider2D == null)
            {
                collider2D = target.GetComponentInChildren<Collider2D>();
            }

            if (collider2D != null)
            {
                return collider2D.bounds.center + new Vector3(0f, collider2D.bounds.extents.y + VerticalWorldOffset, 0f);
            }

            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = target.GetComponentInChildren<SpriteRenderer>();
            }

            if (spriteRenderer != null)
            {
                return spriteRenderer.bounds.center + new Vector3(0f, spriteRenderer.bounds.extents.y + VerticalWorldOffset, 0f);
            }

            return target.position + Vector3.up * VerticalWorldOffset;
        }

        private sealed class WorkbenchRecipeDefinition
        {
            public int recipeId;
            public int resultItemId;
            public string displayName;
            public List<IngredientDefinition> ingredients = new();
        }

        private readonly struct IngredientDefinition
        {
            public int itemId { get; }
            public int amount { get; }

            public IngredientDefinition(int itemId, int amount)
            {
                this.itemId = itemId;
                this.amount = amount;
            }
        }

        private sealed class RecipeRow
        {
            public GameObject RootObject;
            public Image IconImage;
            public TextMeshProUGUI NameText;
            public TextMeshProUGUI MaterialText;
            public Button CraftButton;
            public Image CraftButtonBackground;
            public TextMeshProUGUI CraftButtonLabel;

            public void SetVisible(bool visible)
            {
                if (RootObject != null)
                {
                    RootObject.SetActive(visible);
                }
            }
        }
    }
}
