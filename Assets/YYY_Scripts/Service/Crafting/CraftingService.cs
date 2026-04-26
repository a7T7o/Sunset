using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using FarmGame.Data;

/// <summary>
/// 制作服务 - 处理制作逻辑
///
/// 功能：
/// - 根据设施类型过滤配方
/// - 检查材料是否足够
/// - 执行制作（扣除材料、添加产物）
/// - 检查配方解锁状态
///
/// **Feature: ui-system**
/// **Validates: Requirements 1.1, 2.1, 3.1, 4.1, 5.1**
/// </summary>
public class CraftingService : MonoBehaviour
{
    #region 字段

    [Header("依赖")]
    [SerializeField] private ItemDatabase database;
    [SerializeField] private InventoryService inventory;

    // 当前设施类型
    private CraftingStation currentStation = CraftingStation.None;

    // 玩家等级（临时，后续从玩家系统获取）
    private int playerLevel = 1;

    #endregion

    #region 事件

    /// <summary>制作成功事件</summary>
    public event Action<RecipeData, CraftResult> OnCraftSuccess;

    /// <summary>制作失败事件</summary>
    public event Action<RecipeData, CraftResult> OnCraftFailed;

    /// <summary>配方列表变化事件（设施切换时触发）</summary>
    public event Action OnRecipeListChanged;

    /// <summary>配方解锁事件（等级提升解锁新配方时触发）</summary>
    public event Action<RecipeData> OnRecipeUnlocked;

    #endregion

    #region 属性

    public CraftingStation CurrentStation => currentStation;
    public ItemDatabase Database => database;
    public InventoryService Inventory => inventory;

    #endregion

    #region 运行时上下文

    public void ConfigureRuntimeContext(InventoryService runtimeInventory, ItemDatabase runtimeDatabase = null)
    {
        if (runtimeInventory != null)
        {
            inventory = runtimeInventory;
        }

        if (runtimeDatabase != null)
        {
            database = runtimeDatabase;
        }
        else if (inventory != null)
        {
            database = inventory.Database;
        }
    }

    public void RefreshRuntimeContextFromScene()
    {
        PackagePanelTabsUI packageTabs = PersistentPlayerSceneBridge.GetPreferredRuntimePackageTabs()
            ?? FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
        if (packageTabs != null && packageTabs.RuntimeInventoryService != null)
        {
            ConfigureRuntimeContext(packageTabs.RuntimeInventoryService, packageTabs.RuntimeDatabase);
            return;
        }

        InventoryService preferredInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService();
        if (preferredInventory != null)
        {
            ConfigureRuntimeContext(preferredInventory);
            return;
        }

        InventoryService activeSceneInventory = FindInventoryInActiveScene();
        if (activeSceneInventory != null)
        {
            ConfigureRuntimeContext(activeSceneInventory);
            return;
        }

        if (inventory == null)
        {
            ConfigureRuntimeContext(
                PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include));
        }
        else if (database == null)
        {
            database = inventory.Database;
        }
    }

    private static InventoryService FindInventoryInActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        InventoryService[] inventories = FindObjectsByType<InventoryService>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < inventories.Length; index++)
        {
            InventoryService candidate = inventories[index];
            if (candidate != null && candidate.gameObject.scene == activeScene)
            {
                return candidate;
            }
        }

        return null;
    }

    #endregion

    #region 初始化

    private void Awake()
    {
        // 尝试自动获取依赖
        if (inventory == null)
        {
            RefreshRuntimeContextFromScene();
        }
        else if (database == null)
        {
            database = inventory.Database;
        }
    }

    private void OnEnable()
    {
        // 订阅技能升级事件
        if (SkillLevelService.Instance != null)
        {
            SkillLevelService.Instance.OnLevelUp += OnSkillLevelUp;
        }
    }

    private void OnDisable()
    {
        // 取消订阅
        if (SkillLevelService.Instance != null)
        {
            SkillLevelService.Instance.OnLevelUp -= OnSkillLevelUp;
        }
    }

    /// <summary>
    /// 技能升级回调 - 检查并触发配方解锁
    /// </summary>
    private void OnSkillLevelUp(SkillType skillType, int newLevel)
    {
        if (database == null || database.allRecipes == null) return;

        // 检查所有配方，找出因本次升级而解锁的配方
        foreach (var recipe in database.allRecipes)
        {
            if (recipe == null) continue;

            // 跳过已解锁的配方
            if (recipe.isUnlocked) continue;

            // 跳过隐藏配方
            if (recipe.isHiddenRecipe) continue;

            // 检查是否是本技能类型的配方
            if (recipe.requiredSkillType != skillType) continue;

            // 检查是否刚好达到解锁等级
            if (recipe.requiredSkillLevel == newLevel && recipe.unlockedByDefault)
            {
                // 标记为已解锁
                recipe.isUnlocked = true;

                // 触发解锁事件
                OnRecipeUnlocked?.Invoke(recipe);

                Debug.Log($"<color=lime>[CraftingService] 🔓 配方解锁: {recipe.recipeName} (需要{GetSkillName(skillType)}等级 {newLevel})</color>");
            }
        }

        // 刷新配方列表
        OnRecipeListChanged?.Invoke();
    }

    /// <summary>
    /// 获取技能名称
    /// </summary>
    private string GetSkillName(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Combat => "战斗",
            SkillType.Gathering => "采集",
            SkillType.Crafting => "制作",
            SkillType.Cooking => "烹饪",
            SkillType.Fishing => "钓鱼",
            _ => "技能"
        };
    }

    /// <summary>
    /// 设置当前制作设施
    /// </summary>
    public void SetStation(CraftingStation station)
    {
        if (currentStation != station)
        {
            currentStation = station;
            OnRecipeListChanged?.Invoke();
            Debug.Log($"<color=cyan>[CraftingService] 切换设施: {station}</color>");
        }
    }

    /// <summary>
    /// 设置玩家等级（临时方法，后续从玩家系统获取）
    /// </summary>
    public void SetPlayerLevel(int level)
    {
        playerLevel = Mathf.Max(1, level);
    }

    #endregion

    #region 配方获取

    /// <summary>
    /// 获取当前设施可用的配方列表（已过滤、已排序）
    /// **Property 1: 设施配方过滤正确性**
    /// **Property 2: 隐藏配方不显示**
    /// **Property 3: 配方排序正确性**
    /// </summary>
    public List<RecipeData> GetAvailableRecipes()
    {
        var result = new List<RecipeData>();

        if (database == null || database.allRecipes == null)
        {
            Debug.LogWarning("[CraftingService] 数据库或配方列表为空");
            return result;
        }

        foreach (var recipe in database.allRecipes)
        {
            if (recipe == null) continue;

            // 过滤设施类型
            if (currentStation != CraftingStation.None &&
                recipe.requiredStation != currentStation)
            {
                continue;
            }

            // 过滤隐藏配方（隐藏配方且未解锁 → 不显示）
            if (recipe.isHiddenRecipe && !recipe.isUnlocked)
            {
                continue;
            }

            // 只显示已解锁或可通过升级解锁的配方
            if (!IsRecipeVisible(recipe))
            {
                continue;
            }

            result.Add(recipe);
        }

        // 按配方 ID 升序排列
        result = result.OrderBy(r => r.recipeID).ToList();

        return result;
    }

    /// <summary>
    /// 检查配方是否应该显示
    /// </summary>
    private bool IsRecipeVisible(RecipeData recipe)
    {
        // 已解锁 → 显示
        if (IsRecipeUnlocked(recipe)) return true;

        // 隐藏配方且未解锁 → 不显示
        if (recipe.isHiddenRecipe) return false;

        // 可通过升级解锁 → 显示（降低透明度）
        return true;
    }

    /// <summary>
    /// 获取已解锁的配方列表
    /// </summary>
    public List<RecipeData> GetUnlockedRecipes()
    {
        var all = GetAvailableRecipes();
        var result = new List<RecipeData>();

        foreach (var recipe in all)
        {
            if (IsRecipeUnlocked(recipe))
            {
                result.Add(recipe);
            }
        }

        return result;
    }

    #endregion

    #region 材料检查

    /// <summary>
    /// 获取玩家持有的某物品总数量
    /// **Property 2: 材料检查一致性**
    /// </summary>
    public int GetMaterialCount(int itemId)
    {
        if (inventory == null) return 0;

        int total = 0;
        for (int i = 0; i < inventory.Size; i++)
        {
            var slot = inventory.GetSlot(i);
            if (!slot.IsEmpty && slot.itemId == itemId)
            {
                total += slot.amount;
            }
        }

        return total;
    }

    /// <summary>
    /// 检查是否可以制作某配方
    /// **Property 2: 材料检查一致性**
    /// </summary>
    public bool CanCraft(RecipeData recipe)
    {
        if (recipe == null) return false;
        if (!IsRecipeUnlocked(recipe)) return false;

        // 检查所有材料
        foreach (var ingredient in recipe.ingredients)
        {
            int owned = GetMaterialCount(ingredient.itemID);
            if (owned < ingredient.amount)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 检查是否有足够材料可预留指定次数的制作。
    /// 只检查材料与解锁，不触发任何事件。
    /// </summary>
    public bool CanReserveMaterials(RecipeData recipe, int craftCount)
    {
        if (recipe == null || craftCount <= 0) return false;
        if (!IsRecipeUnlocked(recipe)) return false;

        foreach (var ingredient in recipe.ingredients)
        {
            int requiredAmount = Mathf.Max(0, ingredient.amount) * craftCount;
            if (requiredAmount <= 0)
            {
                continue;
            }

            int owned = GetMaterialCount(ingredient.itemID);
            if (owned < requiredAmount)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 获取材料状态列表
    /// </summary>
    public List<MaterialStatus> GetMaterialStatus(RecipeData recipe)
    {
        var result = new List<MaterialStatus>();

        if (recipe == null) return result;

        foreach (var ingredient in recipe.ingredients)
        {
            int owned = GetMaterialCount(ingredient.itemID);
            var itemData = database?.GetItemByID(ingredient.itemID);

            result.Add(new MaterialStatus
            {
                itemId = ingredient.itemID,
                itemName = itemData?.itemName ?? $"物品#{ingredient.itemID}",
                icon = itemData?.icon,
                required = ingredient.amount,
                owned = owned,
                sufficient = owned >= ingredient.amount
            });
        }

        return result;
    }

    #endregion

    #region 解锁检查

    /// <summary>
    /// 检查配方是否已解锁
    /// **Property 5: 解锁状态过滤正确性**
    /// **Property 6: 等级检查正确性**
    /// </summary>
    public bool IsRecipeUnlocked(RecipeData recipe)
    {
        if (recipe == null) return false;

        // 已标记为解锁（运行时状态）
        if (recipe.isUnlocked) return true;

        // 默认解锁的配方
        if (recipe.unlockedByDefault)
        {
            // 检查技能等级
            return CheckSkillLevel(recipe);
        }

        // 非默认解锁的配方需要满足解锁条件
        return false;
    }

    /// <summary>
    /// 检查技能等级是否满足配方要求
    /// </summary>
    private bool CheckSkillLevel(RecipeData recipe)
    {
        // 使用 SkillLevelService 检查等级
        if (SkillLevelService.Instance != null)
        {
            int playerSkillLevel = SkillLevelService.Instance.GetLevel(recipe.requiredSkillType);
            return playerSkillLevel >= recipe.requiredSkillLevel;
        }

        // 如果没有技能服务，使用旧的 playerLevel 字段
        return playerLevel >= recipe.requiredLevel;
    }

    /// <summary>
    /// 获取配方所需的技能等级
    /// </summary>
    public int GetRequiredSkillLevel(RecipeData recipe)
    {
        return recipe?.requiredSkillLevel ?? 1;
    }

    /// <summary>
    /// 获取配方所需的技能类型
    /// </summary>
    public SkillType GetRequiredSkillType(RecipeData recipe)
    {
        return recipe?.requiredSkillType ?? SkillType.Crafting;
    }

    #endregion

    #region 制作执行

    /// <summary>
    /// 尝试制作配方
    /// **Property 3: 制作材料扣除正确性**
    /// **Property 4: 制作产物添加正确性**
    /// </summary>
    public CraftResult TryCraft(RecipeData recipe)
    {
        return TryCraft(recipe, deliverToInventory: true);
    }

    /// <summary>
    /// 尝试制作配方，并允许调用方决定产物是否立即进入背包。
    /// deliverToInventory=false 时，只扣材料并返回产物信息，由调用方负责后续领取/掉落。
    /// </summary>
    public CraftResult TryCraft(RecipeData recipe, bool deliverToInventory)
    {
        var result = new CraftResult();

        // 检查配方有效性
        if (recipe == null)
        {
            result.success = false;
            result.failReason = FailReason.InvalidRecipe;
            result.message = "无效的配方";
            OnCraftFailed?.Invoke(recipe, result);
            return result;
        }

        // 检查解锁状态
        if (!IsRecipeUnlocked(recipe))
        {
            result.success = false;
            result.failReason = playerLevel < recipe.requiredLevel
                ? FailReason.LevelTooLow
                : FailReason.RecipeLocked;
            result.message = playerLevel < recipe.requiredLevel
                ? $"需要等级 {recipe.requiredLevel}"
                : "配方未解锁";
            OnCraftFailed?.Invoke(recipe, result);
            return result;
        }

        // 检查材料
        if (!CanCraft(recipe))
        {
            result.success = false;
            result.failReason = FailReason.InsufficientMaterials;
            result.message = "材料不足";
            OnCraftFailed?.Invoke(recipe, result);
            return result;
        }

        // 仅在需要直接入包时检查背包空间
        if (deliverToInventory && !HasSpaceForResult(recipe))
        {
            result.success = false;
            result.failReason = FailReason.InventoryFull;
            result.message = "背包已满";
            OnCraftFailed?.Invoke(recipe, result);
            return result;
        }

        // 扣除材料
        foreach (var ingredient in recipe.ingredients)
        {
            RemoveMaterial(ingredient.itemID, ingredient.amount);
        }

        // 设置结果
        result.success = true;
        result.resultItemId = recipe.resultItemID;
        result.resultAmount = recipe.resultAmount;
        if (deliverToInventory)
        {
            int remaining = inventory.AddItem(recipe.resultItemID, 0, recipe.resultAmount);
            result.resultAmount = recipe.resultAmount - remaining;
        }
        result.message = $"成功制作 {recipe.recipeName}";

        OnCraftSuccess?.Invoke(recipe, result);
        Debug.Log($"<color=green>[CraftingService] 制作成功: {recipe.recipeName} x{result.resultAmount}</color>");

        return result;
    }

    /// <summary>
    /// 只预扣材料，不触发制作成功事件，也不发放产物。
    /// 用于工作台“点击即交料”的队列预留语义。
    /// </summary>
    public CraftResult TryReserveMaterials(RecipeData recipe, int craftCount)
    {
        var result = new CraftResult();

        if (recipe == null)
        {
            result.success = false;
            result.failReason = FailReason.InvalidRecipe;
            result.message = "无效的配方";
            return result;
        }

        if (craftCount <= 0)
        {
            result.success = false;
            result.failReason = FailReason.InvalidRecipe;
            result.message = "无效的制作数量";
            return result;
        }

        if (!IsRecipeUnlocked(recipe))
        {
            result.success = false;
            result.failReason = playerLevel < recipe.requiredLevel
                ? FailReason.LevelTooLow
                : FailReason.RecipeLocked;
            result.message = playerLevel < recipe.requiredLevel
                ? $"需要等级 {recipe.requiredLevel}"
                : "配方未解锁";
            return result;
        }

        if (!CanReserveMaterials(recipe, craftCount))
        {
            result.success = false;
            result.failReason = FailReason.InsufficientMaterials;
            result.message = "材料不足";
            return result;
        }

        foreach (var ingredient in recipe.ingredients)
        {
            int reserveAmount = Mathf.Max(0, ingredient.amount) * craftCount;
            if (reserveAmount <= 0)
            {
                continue;
            }

            RemoveMaterial(ingredient.itemID, reserveAmount);
        }

        result.success = true;
        result.resultItemId = recipe.resultItemID;
        result.resultAmount = Mathf.Max(1, recipe.resultAmount) * craftCount;
        result.message = $"已预留 {recipe.recipeName} x{craftCount} 的制作材料";
        return result;
    }

    /// <summary>
    /// 显式通知“一件制作完成”。
    /// 用于工作台预扣模式下，把完成时刻与点击时刻解耦。
    /// </summary>
    public void NotifyCraftCompleted(RecipeData recipe)
    {
        if (recipe == null)
        {
            return;
        }

        CraftResult result = new CraftResult
        {
            success = true,
            message = $"成功制作 {recipe.recipeName}",
            resultItemId = recipe.resultItemID,
            resultAmount = Mathf.Max(1, recipe.resultAmount),
            failReason = FailReason.None
        };

        OnCraftSuccess?.Invoke(recipe, result);
        Debug.Log($"<color=green>[CraftingService] 制作完成: {recipe.recipeName} x{result.resultAmount}</color>");
    }

    /// <summary>
    /// 检查背包是否有空间放置产物
    /// </summary>
    private bool HasSpaceForResult(RecipeData recipe)
    {
        if (inventory == null) return false;

        // 检查是否有空位
        for (int i = 0; i < inventory.Size; i++)
        {
            var slot = inventory.GetSlot(i);
            if (slot.IsEmpty) return true;

            // 检查是否可以堆叠
            if (slot.itemId == recipe.resultItemID)
            {
                int maxStack = inventory.GetMaxStack(recipe.resultItemID);
                if (slot.amount + recipe.resultAmount <= maxStack)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 从背包移除指定数量的材料
    /// </summary>
    private void RemoveMaterial(int itemId, int amount)
    {
        if (inventory == null) return;

        int remaining = amount;

        for (int i = 0; i < inventory.Size && remaining > 0; i++)
        {
            var slot = inventory.GetSlot(i);
            if (!slot.IsEmpty && slot.itemId == itemId)
            {
                int remove = Mathf.Min(remaining, slot.amount);
                inventory.RemoveFromSlot(i, remove);
                remaining -= remove;
            }
        }
    }

    #endregion
}
