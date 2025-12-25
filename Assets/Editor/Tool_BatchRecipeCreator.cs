using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using FarmGame.Data;

/// <summary>
/// 配方批量创建工具
/// 批量创建 RecipeData SO 资产
/// 
/// 功能：
/// - 连续 ID 模式（首个 ID 后自动递增）
/// - 按行输入配方名称、产物 ID、产物数量
/// - 共享材料列表（所有配方使用相同材料）
/// - 制作设施选择
/// - 创建后自动同步数据库
/// 
/// **Feature: so-design-system**
/// **Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7**
/// </summary>
public class Tool_BatchRecipeCreator : EditorWindow
{
    #region 字段

    private Vector2 scrollPos;

    // === ID 设置 ===
    private bool useSequentialID = true;
    private int startID = 8000;
    
    // === 配方信息输入 ===
    private string inputRecipeNames = "";
    private string inputResultIds = "";
    private string inputResultAmounts = "";
    
    // === 共享材料列表 ===
    private List<RecipeIngredient> sharedIngredients = new List<RecipeIngredient>();
    private Vector2 ingredientScrollPos;
    
    // === 制作设施 ===
    private CraftingStation craftingStation = CraftingStation.Workbench;
    
    // === 其他配方属性 ===
    private int requiredLevel = 1;
    private float craftingTime = 0f;
    private bool unlockedByDefault = true;
    private int craftingExp = 10;
    
    // === 技能解锁条件 ===
    private SkillType requiredSkillType = SkillType.Crafting;
    private int requiredSkillLevel = 1;
    private bool isHiddenRecipe = false;
    
    // === 输出设置 ===
    private string outputFolder = "Assets/Data/Recipes";

    #endregion

    [MenuItem("Tools/📜 批量创建配方 SO")]
    public static void ShowWindow()
    {
        var window = GetWindow<Tool_BatchRecipeCreator>("批量创建配方SO");
        window.minSize = new Vector2(520, 700);
        window.Show();
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnGUI()
    {
        DrawHeader();
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        DrawIDSettings();
        DrawLine();
        DrawRecipeInfoInput();
        DrawLine();
        DrawIngredientsList();
        DrawLine();
        DrawCraftingSettings();
        DrawLine();
        DrawOutputSettings();
        DrawLine();
        DrawCreateButton();
        
        EditorGUILayout.EndScrollView();
    }

    #region UI 绘制

    private void DrawHeader()
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        EditorGUILayout.LabelField("📜 批量创建配方 SO", style, GUILayout.Height(30));
    }

    private void DrawIDSettings()
    {
        EditorGUILayout.LabelField("🔢 ID 设置", EditorStyles.boldLabel);
        
        useSequentialID = EditorGUILayout.Toggle("连续 ID 模式", useSequentialID);
        
        string idHint = useSequentialID 
            ? $"按行顺序依次递增：{startID}, {startID + 1}, {startID + 2}..."
            : "所有配方使用相同 ID（需手动修改）";
        EditorGUILayout.HelpBox(idHint, useSequentialID ? MessageType.Info : MessageType.Warning);
        
        startID = EditorGUILayout.IntField("起始 ID", startID);
    }

    private void DrawRecipeInfoInput()
    {
        EditorGUILayout.LabelField("📝 配方信息（按行输入）", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("每行一个配方，行数需要一致。产物数量留空默认为 1。", MessageType.Info);
        
        // 配方名称
        EditorGUILayout.LabelField("配方名称：");
        inputRecipeNames = EditorGUILayout.TextArea(inputRecipeNames, GUILayout.Height(80));
        
        // 产物 ID
        EditorGUILayout.LabelField("产物 ID：");
        inputResultIds = EditorGUILayout.TextArea(inputResultIds, GUILayout.Height(80));
        
        // 产物数量
        EditorGUILayout.LabelField("产物数量（可选，默认 1）：");
        inputResultAmounts = EditorGUILayout.TextArea(inputResultAmounts, GUILayout.Height(60));
        
        // 统计行数
        int nameCount = CountLines(inputRecipeNames);
        int idCount = CountLines(inputResultIds);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"名称: {nameCount} 行 | 产物ID: {idCount} 行", EditorStyles.miniLabel);
        if (nameCount != idCount && nameCount > 0 && idCount > 0)
        {
            EditorGUILayout.LabelField("⚠️ 行数不一致！", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawIngredientsList()
    {
        EditorGUILayout.LabelField("🧪 共享材料（所有配方使用相同材料）", EditorStyles.boldLabel);
        
        ingredientScrollPos = EditorGUILayout.BeginScrollView(ingredientScrollPos, 
            EditorStyles.helpBox, GUILayout.Height(Mathf.Min(sharedIngredients.Count * 26 + 40, 150)));
        
        int removeIndex = -1;
        for (int i = 0; i < sharedIngredients.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"材料 {i + 1}:", GUILayout.Width(60));
            sharedIngredients[i].itemID = EditorGUILayout.IntField("ID", sharedIngredients[i].itemID, GUILayout.Width(100));
            sharedIngredients[i].amount = EditorGUILayout.IntField("数量", sharedIngredients[i].amount, GUILayout.Width(100));
            if (GUILayout.Button("✖", GUILayout.Width(25)))
            {
                removeIndex = i;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        if (removeIndex >= 0)
        {
            sharedIngredients.RemoveAt(removeIndex);
        }
        
        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button("+ 添加材料"))
        {
            sharedIngredients.Add(new RecipeIngredient { itemID = 0, amount = 1 });
        }
    }

    private void DrawCraftingSettings()
    {
        EditorGUILayout.LabelField("🏭 制作设置", EditorStyles.boldLabel);
        
        craftingStation = (CraftingStation)EditorGUILayout.EnumPopup("制作设施", craftingStation);
        requiredLevel = EditorGUILayout.IntSlider("需要等级（旧）", requiredLevel, 1, 50);
        craftingTime = EditorGUILayout.Slider("制作时间（秒）", craftingTime, 0f, 60f);
        unlockedByDefault = EditorGUILayout.Toggle("默认解锁", unlockedByDefault);
        craftingExp = EditorGUILayout.IntSlider("制作经验", craftingExp, 0, 100);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("🎯 技能解锁条件", EditorStyles.boldLabel);
        requiredSkillType = (SkillType)EditorGUILayout.EnumPopup("所需技能类型", requiredSkillType);
        requiredSkillLevel = EditorGUILayout.IntSlider("所需技能等级", requiredSkillLevel, 1, 10);
        isHiddenRecipe = EditorGUILayout.Toggle("隐藏配方", isHiddenRecipe);
        
        if (isHiddenRecipe)
        {
            EditorGUILayout.HelpBox("隐藏配方不会显示在配方列表中，需要通过特殊方式解锁", MessageType.Info);
        }
    }

    private void DrawOutputSettings()
    {
        EditorGUILayout.LabelField("📁 输出设置", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出文件夹", GUILayout.Width(80));
        outputFolder = EditorGUILayout.TextField(outputFolder);
        if (GUILayout.Button("选择", GUILayout.Width(50)))
        {
            string path = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                outputFolder = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawCreateButton()
    {
        EditorGUILayout.Space(10);
        
        int recipeCount = CountLines(inputRecipeNames);
        bool canCreate = recipeCount > 0 && CountLines(inputResultIds) == recipeCount;
        
        GUI.enabled = canCreate;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        
        if (GUILayout.Button($"🚀 创建 {recipeCount} 个配方 SO", GUILayout.Height(45)))
        {
            CreateRecipes();
        }
        
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
        
        if (!canCreate)
        {
            if (recipeCount == 0)
            {
                EditorGUILayout.HelpBox("请输入配方名称", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("配方名称和产物 ID 行数不一致", MessageType.Warning);
            }
        }
    }

    private void DrawLine()
    {
        EditorGUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        EditorGUILayout.Space(5);
    }

    #endregion

    #region 创建逻辑

    /// <summary>
    /// 批量创建配方
    /// **Property 5: ID 序列生成正确性**
    /// **Property 6: 配方输入解析正确性**
    /// </summary>
    private void CreateRecipes()
    {
        // 解析输入
        string[] names = ParseLines(inputRecipeNames);
        int[] resultIds = ParseIds(inputResultIds, names.Length);
        int[] resultAmounts = ParseAmounts(inputResultAmounts, names.Length);
        
        if (names.Length == 0)
        {
            EditorUtility.DisplayDialog("错误", "请输入配方名称", "确定");
            return;
        }
        
        if (resultIds.Length != names.Length)
        {
            EditorUtility.DisplayDialog("错误", "产物 ID 行数与配方名称不一致", "确定");
            return;
        }
        
        // 确保输出文件夹存在
        EnsureFolderExists(outputFolder);
        
        int successCount = 0;
        int skipCount = 0;
        List<string> createdFiles = new List<string>();
        
        for (int i = 0; i < names.Length; i++)
        {
            int recipeID = useSequentialID ? startID + i : startID;
            string recipeName = names[i].Trim();
            int resultItemID = resultIds[i];
            int resultAmount = resultAmounts[i];
            
            if (string.IsNullOrEmpty(recipeName)) continue;
            
            // 创建配方
            var recipe = ScriptableObject.CreateInstance<RecipeData>();
            recipe.recipeID = recipeID;
            recipe.recipeName = recipeName;
            recipe.description = "";
            recipe.resultItemID = resultItemID;
            recipe.resultAmount = resultAmount;
            recipe.requiredStation = craftingStation;
            recipe.requiredLevel = requiredLevel;
            recipe.craftingTime = craftingTime;
            recipe.unlockedByDefault = unlockedByDefault;
            recipe.craftingExp = craftingExp;
            
            // 技能解锁条件
            recipe.requiredSkillType = requiredSkillType;
            recipe.requiredSkillLevel = requiredSkillLevel;
            recipe.isHiddenRecipe = isHiddenRecipe;
            recipe.isUnlocked = false;  // 运行时状态，默认未解锁
            
            // 复制材料列表
            recipe.ingredients = new List<RecipeIngredient>();
            foreach (var ing in sharedIngredients)
            {
                recipe.ingredients.Add(new RecipeIngredient
                {
                    itemID = ing.itemID,
                    amount = ing.amount
                });
            }
            
            // 保存资产
            string safeName = SanitizeFileName(recipeName);
            string assetPath = $"{outputFolder}/Recipe_{recipeID}_{safeName}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<RecipeData>(assetPath) != null)
            {
                if (!EditorUtility.DisplayDialog("文件已存在",
                    $"文件 Recipe_{recipeID}_{safeName}.asset 已存在，是否覆盖？", "覆盖", "跳过"))
                {
                    skipCount++;
                    continue;
                }
                AssetDatabase.DeleteAsset(assetPath);
            }
            
            AssetDatabase.CreateAsset(recipe, assetPath);
            createdFiles.Add(assetPath);
            successCount++;
            
            Debug.Log($"<color=green>[配方创建] 创建: {assetPath}</color>");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // 选中创建的文件
        if (createdFiles.Count > 0)
        {
            var assets = new List<Object>();
            foreach (var path in createdFiles)
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null) assets.Add(asset);
            }
            Selection.objects = assets.ToArray();
        }
        
        // 自动同步数据库
        string syncMessage = "";
        if (successCount > 0)
        {
            if (DatabaseSyncHelper.DatabaseExists())
            {
                int syncCount = DatabaseSyncHelper.AutoCollectAllRecipes();
                if (syncCount >= 0)
                {
                    syncMessage = $"\n\n✅ 数据库已自动同步（共 {syncCount} 个配方）";
                }
                else
                {
                    syncMessage = "\n\n⚠️ 数据库同步失败，请手动执行";
                }
            }
            else
            {
                syncMessage = "\n\n⚠️ 数据库不存在，请先创建 MasterItemDatabase";
            }
        }
        
        EditorUtility.DisplayDialog("完成",
            $"成功创建 {successCount} 个配方 SO\n跳过 {skipCount} 个\n保存位置：{outputFolder}{syncMessage}", "确定");
        
        Debug.Log($"<color=green>[配方创建] ✅ 完成！共创建 {successCount} 个配方</color>");
    }

    #endregion

    #region 辅助方法

    private int CountLines(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        string[] lines = text.Replace("\r", "").Split('\n');
        int count = 0;
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line)) count++;
        }
        return count;
    }

    private string[] ParseLines(string text)
    {
        if (string.IsNullOrEmpty(text)) return new string[0];
        string[] lines = text.Replace("\r", "").Split('\n');
        var result = new List<string>();
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
                result.Add(line.Trim());
        }
        return result.ToArray();
    }

    /// <summary>
    /// 解析 ID 输入
    /// **Property 5: ID 序列生成正确性**
    /// </summary>
    private int[] ParseIds(string text, int expectedCount)
    {
        string[] lines = ParseLines(text);
        int[] ids = new int[expectedCount];
        
        for (int i = 0; i < expectedCount; i++)
        {
            if (i < lines.Length && int.TryParse(lines[i], out int parsed))
            {
                ids[i] = parsed;
            }
            else if (i > 0)
            {
                ids[i] = ids[i - 1] + 1; // 自动递增
            }
            else
            {
                ids[i] = 0;
            }
        }
        
        return ids;
    }

    private int[] ParseAmounts(string text, int expectedCount)
    {
        string[] lines = ParseLines(text);
        int[] amounts = new int[expectedCount];
        
        for (int i = 0; i < expectedCount; i++)
        {
            if (i < lines.Length && int.TryParse(lines[i], out int parsed) && parsed > 0)
            {
                amounts[i] = parsed;
            }
            else
            {
                amounts[i] = 1; // 默认数量为 1
            }
        }
        
        return amounts;
    }

    private string SanitizeFileName(string name)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        foreach (char c in invalid)
        {
            name = name.Replace(c, '_');
        }
        return name;
    }

    private void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;
        
        string[] folders = folderPath.Split('/');
        string currentPath = folders[0];
        
        for (int i = 1; i < folders.Length; i++)
        {
            string newPath = currentPath + "/" + folders[i];
            if (!AssetDatabase.IsValidFolder(newPath))
            {
                AssetDatabase.CreateFolder(currentPath, folders[i]);
            }
            currentPath = newPath;
        }
    }

    #endregion

    #region 设置保存/加载

    private void LoadSettings()
    {
        useSequentialID = EditorPrefs.GetBool("BatchRecipe_SeqID", true);
        startID = EditorPrefs.GetInt("BatchRecipe_StartID", 8000);
        outputFolder = EditorPrefs.GetString("BatchRecipe_Output", "Assets/Data/Recipes");
        craftingStation = (CraftingStation)EditorPrefs.GetInt("BatchRecipe_Station", 0);
        requiredLevel = EditorPrefs.GetInt("BatchRecipe_Level", 1);
        craftingTime = EditorPrefs.GetFloat("BatchRecipe_Time", 0f);
        unlockedByDefault = EditorPrefs.GetBool("BatchRecipe_Unlocked", true);
        craftingExp = EditorPrefs.GetInt("BatchRecipe_Exp", 10);
        
        // 技能解锁条件
        requiredSkillType = (SkillType)EditorPrefs.GetInt("BatchRecipe_SkillType", 0);
        requiredSkillLevel = EditorPrefs.GetInt("BatchRecipe_SkillLevel", 1);
        isHiddenRecipe = EditorPrefs.GetBool("BatchRecipe_Hidden", false);
    }

    private void SaveSettings()
    {
        EditorPrefs.SetBool("BatchRecipe_SeqID", useSequentialID);
        EditorPrefs.SetInt("BatchRecipe_StartID", startID);
        EditorPrefs.SetString("BatchRecipe_Output", outputFolder);
        EditorPrefs.SetInt("BatchRecipe_Station", (int)craftingStation);
        EditorPrefs.SetInt("BatchRecipe_Level", requiredLevel);
        EditorPrefs.SetFloat("BatchRecipe_Time", craftingTime);
        EditorPrefs.SetBool("BatchRecipe_Unlocked", unlockedByDefault);
        EditorPrefs.SetInt("BatchRecipe_Exp", craftingExp);
        
        // 技能解锁条件
        EditorPrefs.SetInt("BatchRecipe_SkillType", (int)requiredSkillType);
        EditorPrefs.SetInt("BatchRecipe_SkillLevel", requiredSkillLevel);
        EditorPrefs.SetBool("BatchRecipe_Hidden", isHiddenRecipe);
    }

    #endregion
}
