using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using FarmGame.Data;

/// <summary>
/// 批量生成物品 SO 工具
/// 完全贴合项目 ID 规范和物品类型设计
/// 
/// ID 规范：
/// 0XXX: 工具和武器 (00XX农具, 01XX采集, 02XX武器)
/// 1XXX: 种植类 (10XX种子, 11XX作物)
/// 2XXX: 动物产品
/// 3XXX: 矿物和材料 (30XX矿石, 31XX锭, 32XX自然, 33XX怪物掉落)
/// 4XXX: 消耗品 (40XX药水)
/// 5XXX: 食品 (50XX简单, 51XX高级)
/// 6XXX: 家具
/// 7XXX: 特殊物品
/// </summary>
public class Tool_BatchItemSOGenerator : EditorWindow
{
    #region 枚举定义

    /// <summary>
    /// 物品 SO 类型 - 对应项目中的实际数据类
    /// </summary>
    private enum ItemSOType
    {
        ItemData,       // 基础物品（通用）
        ToolData,       // 工具（锄头、斧头、镐子等）
        WeaponData,     // 武器（剑、弓、法杖）
        SeedData,       // 种子
        SaplingData,    // 树苗（可放置）
        CropData,       // 作物
        FoodData,       // 食物
        MaterialData,   // 材料（矿石、木材、怪物掉落）
        PotionData      // 药水
    }

    #endregion

    #region 字段

    private Vector2 scrollPos;
    private Vector2 spriteListScrollPos;
    private List<Sprite> selectedSprites = new List<Sprite>();

    // === 基础设置 ===
    private ItemSOType soType = ItemSOType.ItemData;
    private string outputFolder = "Assets/111_Data/Items";

    // === ID 设置 ===
    private bool useSequentialID = true;
    private int startID = 0;

    // === 通用属性（可选填写）===
    private bool setPrice = false;
    private int defaultBuyPrice = 0;
    private int defaultSellPrice = 0;
    private bool setMaxStack = false;
    private int defaultMaxStack = 99;

    // === 工具专属 ===
    // 注意：工具没有"等级"属性，品质通过后缀命名规范区分（如 Axe_0, Axe_1）
    private ToolType toolType = ToolType.Axe;
    private bool setToolEnergy = false;
    private int toolEnergyCost = 2;
    private bool setToolRadius = false;
    private int toolEffectRadius = 1;
    private bool setToolAnimFrames = false;
    private int toolAnimFrameCount = 8;

    // === 武器专属 ===
    // 注意：武器没有"等级"属性，品质通过后缀命名规范区分
    private WeaponType weaponType = WeaponType.Sword;
    private bool setWeaponAttack = false;
    private int weaponAttackPower = 10;
    private bool setWeaponSpeed = false;
    private float weaponAttackSpeed = 1.0f;
    private bool setWeaponCrit = false;
    private float weaponCritChance = 5f;

    // === 种子专属 ===
    private Season seedSeason = Season.Spring;
    private bool setSeedGrowth = false;
    private int seedGrowthDays = 4;
    private bool setSeedHarvest = false;
    private int seedHarvestCropID = 1100;

    // === 树苗专属 ===
    private GameObject saplingTreePrefab;
    private bool setSaplingExp = false;
    private int saplingPlantingExp = 5;

    // === 作物专属 ===
    private bool setCropSeedID = false;
    private int cropSeedID = 1000;
    private bool setCropExp = false;
    private int cropHarvestExp = 10;

    // === 食物专属 ===
    private bool setFoodEnergy = false;
    private int foodEnergyRestore = 30;
    private bool setFoodHealth = false;
    private int foodHealthRestore = 15;
    private BuffType foodBuffType = BuffType.None;

    // === 材料专属 ===
    private MaterialSubType materialSubType = MaterialSubType.Natural;
    private bool setMaterialSmelt = false;
    private bool materialCanSmelt = false;
    private int materialSmeltResultID = 0;

    // === 药水专属 ===
    private bool setPotionHealth = false;
    private int potionHealthRestore = 50;
    private bool setPotionEnergy = false;
    private int potionEnergyRestore = 0;
    private BuffType potionBuffType = BuffType.None;

    // === 显示尺寸配置 ===
    private bool setDisplaySize = false;
    private int displayPixelSize = 32;

    #endregion

    [MenuItem("Tools/📦 批量生成物品 SO")]
    public static void ShowWindow()
    {
        var window = GetWindow<Tool_BatchItemSOGenerator>("批量生成物品SO");
        window.minSize = new Vector2(520, 750);
        window.Show();
    }

    private void OnEnable()
    {
        LoadSettings();
        // 不再自动跟随选择，改为手动获取
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    /// <summary>
    /// 手动获取选中的 Sprite
    /// </summary>
    private void GetSelectedSprites()
    {
        selectedSprites.Clear();
        
        foreach (var obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                if (!selectedSprites.Contains(sprite))
                    selectedSprites.Add(sprite);
            }
            else if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();
                foreach (var s in sprites)
                {
                    if (!selectedSprites.Contains(s))
                        selectedSprites.Add(s);
                }
            }
            // 选中文件夹 - 递归获取所有 Sprite
            else if (obj is DefaultAsset)
            {
                string folderPath = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(folderPath))
                {
                    var spritesInFolder = GetAllSpritesInFolder(folderPath);
                    foreach (var s in spritesInFolder)
                    {
                        if (!selectedSprites.Contains(s))
                            selectedSprites.Add(s);
                    }
                }
            }
        }

        // 按名称排序（用于连续ID分配）
        selectedSprites = selectedSprites.OrderBy(s => s.name).ToList();
        Repaint();
    }

    /// <summary>
    /// 递归获取文件夹内所有 Sprite
    /// </summary>
    private List<Sprite> GetAllSpritesInFolder(string folderPath)
    {
        var result = new List<Sprite>();
        
        // 搜索所有 Texture2D 文件
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // 加载该纹理下的所有 Sprite（支持多 Sprite 模式）
            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>();
            result.AddRange(sprites);
        }
        
        return result;
    }

    private void OnGUI()
    {
        DrawHeader();
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        DrawSpriteSelection();
        DrawLine();
        DrawTypeSelection();
        DrawLine();
        DrawIDSettings();
        DrawLine();
        DrawCommonSettings();
        DrawLine();
        DrawTypeSpecificSettings();
        DrawLine();
        DrawOutputSettings();
        DrawLine();
        DrawGenerateButton();
        
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
        EditorGUILayout.LabelField("📦 批量生成物品 SO", style, GUILayout.Height(30));
    }

    private void DrawSpriteSelection()
    {
        EditorGUILayout.LabelField("🖼️ 选中的 Sprite", EditorStyles.boldLabel);
        
        // 获取选中项按钮
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("在 Project 窗口选择 Sprite、Texture 或文件夹", MessageType.None);
        if (GUILayout.Button("🔍 获取选中项", GUILayout.Width(100), GUILayout.Height(38)))
        {
            GetSelectedSprites();
        }
        EditorGUILayout.EndHorizontal();

        // 显示选中的 Sprite 列表
        if (selectedSprites.Count == 0)
        {
            EditorGUILayout.HelpBox("⚠️ 未选择任何 Sprite", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"✓ 已选择 {selectedSprites.Count} 个 Sprite（支持文件夹递归）", EditorStyles.boldLabel);
            
            spriteListScrollPos = EditorGUILayout.BeginScrollView(spriteListScrollPos, 
                GUILayout.Height(Mathf.Min(selectedSprites.Count * 26 + 5, 140)));
            
            int showCount = Mathf.Min(selectedSprites.Count, 10);
            for (int i = 0; i < showCount; i++)
            {
                var sprite = selectedSprites[i];
                EditorGUILayout.BeginHorizontal();
                
                // 预览图
                var rect = GUILayoutUtility.GetRect(22, 22, GUILayout.Width(22));
                if (sprite != null && sprite.texture != null)
                {
                    GUI.DrawTextureWithTexCoords(rect, sprite.texture, 
                        new Rect(
                            sprite.rect.x / sprite.texture.width,
                            sprite.rect.y / sprite.texture.height,
                            sprite.rect.width / sprite.texture.width,
                            sprite.rect.height / sprite.texture.height
                        ));
                }
                
                // 名称和预计 ID
                int predictedID = useSequentialID ? startID + i : startID;
                EditorGUILayout.LabelField($"{sprite.name}", GUILayout.Width(180));
                EditorGUILayout.LabelField($"→ ID: {predictedID}", EditorStyles.miniLabel, GUILayout.Width(80));
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (selectedSprites.Count > 10)
            {
                EditorGUILayout.LabelField($"... 还有 {selectedSprites.Count - 10} 项", EditorStyles.miniLabel);
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawTypeSelection()
    {
        EditorGUILayout.LabelField("📋 物品类型", EditorStyles.boldLabel);
        
        // 第一行：基础类型
        EditorGUILayout.BeginHorizontal();
        DrawTypeButton("基础", ItemSOType.ItemData, new Color(0.7f, 0.7f, 0.7f));
        DrawTypeButton("工具", ItemSOType.ToolData, new Color(1f, 0.8f, 0.3f));
        DrawTypeButton("武器", ItemSOType.WeaponData, new Color(1f, 0.5f, 0.5f));
        DrawTypeButton("种子", ItemSOType.SeedData, new Color(0.5f, 0.9f, 0.5f));
        DrawTypeButton("树苗", ItemSOType.SaplingData, new Color(0.4f, 0.8f, 0.4f));
        EditorGUILayout.EndHorizontal();
        
        // 第二行：其他类型
        EditorGUILayout.BeginHorizontal();
        DrawTypeButton("作物", ItemSOType.CropData, new Color(0.9f, 0.7f, 0.3f));
        DrawTypeButton("食物", ItemSOType.FoodData, new Color(1f, 0.6f, 0.8f));
        DrawTypeButton("材料", ItemSOType.MaterialData, new Color(0.6f, 0.6f, 0.8f));
        DrawTypeButton("药水", ItemSOType.PotionData, new Color(0.5f, 0.8f, 1f));
        EditorGUILayout.EndHorizontal();
        
        GUI.backgroundColor = Color.white;
        
        // 类型说明和 ID 范围提示
        EditorGUILayout.HelpBox(GetTypeDescription(), MessageType.Info);
    }

    private void DrawTypeButton(string label, ItemSOType type, Color color)
    {
        GUI.backgroundColor = soType == type ? color : Color.white;
        if (GUILayout.Button(label, GUILayout.Height(28)))
        {
            soType = type;
            AutoSetStartID();
        }
    }

    private string GetTypeDescription()
    {
        return soType switch
        {
            ItemSOType.ItemData => "基础物品 - 通用类型，无特殊属性\nID 范围：根据实际用途选择",
            ItemSOType.ToolData => "工具 - 锄头、斧头、镐子、水壶等\nID 范围：00XX(农具) / 01XX(采集工具)",
            ItemSOType.WeaponData => "武器 - 剑、弓、法杖等战斗装备\nID 范围：02XX",
            ItemSOType.SeedData => "种子 - 可种植的种子\nID 范围：10XX",
            ItemSOType.SaplingData => "树苗 - 可放置的树苗，种下后成为树木\nID 范围：12XX",
            ItemSOType.CropData => "作物 - 收获的农作物\nID 范围：11XX",
            ItemSOType.FoodData => "食物 - 可食用的料理\nID 范围：50XX(简单) / 51XX(高级)",
            ItemSOType.MaterialData => "材料 - 矿石、木材、怪物掉落等\nID 范围：30XX(矿石) / 31XX(锭) / 32XX(自然) / 33XX(怪物)",
            ItemSOType.PotionData => "药水 - HP药水、精力药水等\nID 范围：40XX",
            _ => ""
        };
    }

    private void AutoSetStartID()
    {
        // 根据类型自动设置推荐的起始 ID
        startID = soType switch
        {
            ItemSOType.ToolData => 0,
            ItemSOType.WeaponData => 200,
            ItemSOType.SeedData => 1000,
            ItemSOType.SaplingData => 1200,
            ItemSOType.CropData => 1100,
            ItemSOType.FoodData => 5000,
            ItemSOType.MaterialData => 3200,
            ItemSOType.PotionData => 4000,
            _ => 0
        };
    }

    private void DrawIDSettings()
    {
        EditorGUILayout.LabelField("🔢 ID 设置", EditorStyles.boldLabel);
        
        useSequentialID = EditorGUILayout.Toggle("连续 ID 模式", useSequentialID);
        
        string idHint = useSequentialID 
            ? $"按 Sprite 名称排序后依次递增：{startID} ~ {startID + Mathf.Max(0, selectedSprites.Count - 1)}"
            : "所有物品使用相同 ID（需手动修改）";
        EditorGUILayout.HelpBox(idHint, useSequentialID ? MessageType.Info : MessageType.Warning);
        
        startID = EditorGUILayout.IntField("起始 ID", startID);
    }

    private void DrawCommonSettings()
    {
        EditorGUILayout.LabelField("⚙️ 通用属性（可选，不勾选则留空）", EditorStyles.boldLabel);
        
        // 价格设置
        EditorGUILayout.BeginHorizontal();
        setPrice = EditorGUILayout.Toggle(setPrice, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!setPrice);
        EditorGUILayout.LabelField("价格", GUILayout.Width(40));
        defaultBuyPrice = EditorGUILayout.IntField("买", defaultBuyPrice, GUILayout.Width(80));
        defaultSellPrice = EditorGUILayout.IntField("卖", defaultSellPrice, GUILayout.Width(80));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        // 堆叠设置（工具和武器自动为1）
        bool canStack = soType != ItemSOType.ToolData && soType != ItemSOType.WeaponData;
        
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = canStack;
        setMaxStack = canStack && EditorGUILayout.Toggle(setMaxStack, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!setMaxStack || !canStack);
        defaultMaxStack = EditorGUILayout.IntField("最大堆叠数", defaultMaxStack);
        EditorGUI.EndDisabledGroup();
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        if (!canStack)
        {
            EditorGUILayout.HelpBox("工具和武器不可堆叠，固定为 1", MessageType.None);
        }
        
        // 显示尺寸设置
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        setDisplaySize = EditorGUILayout.Toggle(setDisplaySize, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!setDisplaySize);
        displayPixelSize = EditorGUILayout.IntSlider("世界显示尺寸 (像素)", displayPixelSize, 8, 128);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        if (setDisplaySize)
        {
            EditorGUILayout.HelpBox($"世界物品将等比例缩放至 {displayPixelSize}×{displayPixelSize} 像素方框内\n（不影响背包/工具栏显示）", MessageType.Info);
        }
    }

    private void DrawTypeSpecificSettings()
    {
        switch (soType)
        {
            case ItemSOType.ToolData:
                DrawToolSettings();
                break;
            case ItemSOType.WeaponData:
                DrawWeaponSettings();
                break;
            case ItemSOType.SeedData:
                DrawSeedSettings();
                break;
            case ItemSOType.SaplingData:
                DrawSaplingSettings();
                break;
            case ItemSOType.CropData:
                DrawCropSettings();
                break;
            case ItemSOType.FoodData:
                DrawFoodSettings();
                break;
            case ItemSOType.MaterialData:
                DrawMaterialSettings();
                break;
            case ItemSOType.PotionData:
                DrawPotionSettings();
                break;
        }
    }

    private void DrawToolSettings()
    {
        EditorGUILayout.LabelField("🔧 工具专属设置", EditorStyles.boldLabel);
        
        toolType = (ToolType)EditorGUILayout.EnumPopup("工具类型", toolType);
        
        // 动画动作类型（自动根据工具类型设置）
        AnimActionType autoAnimType = GetAnimActionType(toolType);
        GUI.enabled = false;
        EditorGUILayout.EnumPopup("动画动作（自动）", autoAnimType);
        GUI.enabled = true;
        
        EditorGUILayout.HelpBox("工具品质通过后缀命名区分（如 Axe_0, Axe_1），不使用等级属性", MessageType.Info);
        
        DrawOptionalInt(ref setToolEnergy, ref toolEnergyCost, "精力消耗", 1, 20);
        DrawOptionalInt(ref setToolRadius, ref toolEffectRadius, "作用范围", 1, 5);
        DrawOptionalInt(ref setToolAnimFrames, ref toolAnimFrameCount, "动画帧数", 1, 30);
    }

    private AnimActionType GetAnimActionType(ToolType type)
    {
        return type switch
        {
            ToolType.Axe => AnimActionType.Slice,
            ToolType.Sickle => AnimActionType.Slice,
            ToolType.Pickaxe => AnimActionType.Crush,
            ToolType.Hoe => AnimActionType.Crush,
            ToolType.FishingRod => AnimActionType.Fish,
            ToolType.WateringCan => AnimActionType.Watering,
            _ => AnimActionType.Slice
        };
    }

    private void DrawWeaponSettings()
    {
        EditorGUILayout.LabelField("⚔️ 武器专属设置", EditorStyles.boldLabel);
        
        weaponType = (WeaponType)EditorGUILayout.EnumPopup("武器类型", weaponType);
        
        EditorGUILayout.HelpBox("武器品质通过后缀命名区分，不使用等级属性", MessageType.Info);
        
        DrawOptionalInt(ref setWeaponAttack, ref weaponAttackPower, "攻击力", 1, 200);
        DrawOptionalFloat(ref setWeaponSpeed, ref weaponAttackSpeed, "攻击速度", 0.3f, 3.0f);
        DrawOptionalFloat(ref setWeaponCrit, ref weaponCritChance, "暴击率 (%)", 0f, 100f);
    }

    private void DrawSeedSettings()
    {
        EditorGUILayout.LabelField("🌱 种子专属设置", EditorStyles.boldLabel);
        
        seedSeason = (Season)EditorGUILayout.EnumPopup("适合季节", seedSeason);
        DrawOptionalInt(ref setSeedGrowth, ref seedGrowthDays, "生长天数", 1, 28);
        DrawOptionalInt(ref setSeedHarvest, ref seedHarvestCropID, "收获作物 ID", 1100, 1199);
    }

    private void DrawSaplingSettings()
    {
        EditorGUILayout.LabelField("🌳 树苗专属设置", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("树苗只需设置关联的树木预制体，季节样式由 TreeControllerV2 自动处理\n冬季无法种植树苗", MessageType.Info);
        
        saplingTreePrefab = (GameObject)EditorGUILayout.ObjectField("树木预制体", saplingTreePrefab, typeof(GameObject), false);
        
        if (saplingTreePrefab != null)
        {
            // 检查预制体是否包含 TreeControllerV2
            var treeController = saplingTreePrefab.GetComponentInChildren<TreeControllerV2>();
            if (treeController == null)
            {
                EditorGUILayout.HelpBox("⚠️ 预制体缺少 TreeControllerV2 组件！", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ 预制体包含 TreeControllerV2 组件", MessageType.None);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("请选择树木预制体（如 M1.prefab）", MessageType.Warning);
        }
        
        DrawOptionalInt(ref setSaplingExp, ref saplingPlantingExp, "种植经验", 1, 50);
    }

    private void DrawCropSettings()
    {
        EditorGUILayout.LabelField("🌾 作物专属设置", EditorStyles.boldLabel);
        
        DrawOptionalInt(ref setCropSeedID, ref cropSeedID, "对应种子 ID", 1000, 1099);
        DrawOptionalInt(ref setCropExp, ref cropHarvestExp, "收获经验", 1, 100);
    }

    private void DrawFoodSettings()
    {
        EditorGUILayout.LabelField("🍳 食物专属设置", EditorStyles.boldLabel);
        
        DrawOptionalInt(ref setFoodEnergy, ref foodEnergyRestore, "恢复精力", 0, 200);
        DrawOptionalInt(ref setFoodHealth, ref foodHealthRestore, "恢复 HP", 0, 200);
        foodBuffType = (BuffType)EditorGUILayout.EnumPopup("Buff 类型", foodBuffType);
    }

    private void DrawMaterialSettings()
    {
        EditorGUILayout.LabelField("🪨 材料专属设置", EditorStyles.boldLabel);
        
        materialSubType = (MaterialSubType)EditorGUILayout.EnumPopup("材料子类", materialSubType);
        
        // 根据子类自动调整推荐 ID
        string subTypeHint = materialSubType switch
        {
            MaterialSubType.Ore => "矿石 - 推荐 ID: 30XX",
            MaterialSubType.Ingot => "锭 - 推荐 ID: 31XX",
            MaterialSubType.Natural => "自然材料 - 推荐 ID: 32XX",
            MaterialSubType.Monster => "怪物掉落 - 推荐 ID: 33XX",
            _ => ""
        };
        EditorGUILayout.HelpBox(subTypeHint, MessageType.None);
        
        // 熔炼设置（仅矿石）
        if (materialSubType == MaterialSubType.Ore)
        {
            EditorGUILayout.BeginHorizontal();
            setMaterialSmelt = EditorGUILayout.Toggle(setMaterialSmelt, GUILayout.Width(20));
            EditorGUI.BeginDisabledGroup(!setMaterialSmelt);
            materialCanSmelt = EditorGUILayout.Toggle("可熔炼", materialCanSmelt);
            if (materialCanSmelt)
            {
                materialSmeltResultID = EditorGUILayout.IntField("产物 ID", materialSmeltResultID);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawPotionSettings()
    {
        EditorGUILayout.LabelField("🧪 药水专属设置", EditorStyles.boldLabel);
        
        DrawOptionalInt(ref setPotionHealth, ref potionHealthRestore, "恢复 HP", 0, 500);
        DrawOptionalInt(ref setPotionEnergy, ref potionEnergyRestore, "恢复精力", 0, 200);
        potionBuffType = (BuffType)EditorGUILayout.EnumPopup("Buff 类型", potionBuffType);
    }

    private void DrawOptionalInt(ref bool enabled, ref int value, string label, int min, int max)
    {
        EditorGUILayout.BeginHorizontal();
        enabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!enabled);
        value = EditorGUILayout.IntSlider(label, value, min, max);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawOptionalFloat(ref bool enabled, ref float value, string label, float min, float max)
    {
        EditorGUILayout.BeginHorizontal();
        enabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!enabled);
        value = EditorGUILayout.Slider(label, value, min, max);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawOutputSettings()
    {
        EditorGUILayout.LabelField("📁 输出设置", EditorStyles.boldLabel);
        
        // 自动设置输出文件夹
        string autoFolder = GetAutoOutputFolder();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出文件夹", GUILayout.Width(80));
        outputFolder = EditorGUILayout.TextField(outputFolder);
        if (GUILayout.Button("自动", GUILayout.Width(45)))
        {
            outputFolder = autoFolder;
        }
        if (GUILayout.Button("选择", GUILayout.Width(45)))
        {
            string path = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                outputFolder = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox($"推荐路径：{autoFolder}", MessageType.None);
    }

    private string GetAutoOutputFolder()
    {
        return soType switch
        {
            ItemSOType.ToolData => "Assets/111_Data/Items/Tools",
            ItemSOType.WeaponData => "Assets/111_Data/Items/Weapons",
            ItemSOType.SeedData => "Assets/111_Data/Items/Seeds",
            ItemSOType.SaplingData => "Assets/111_Data/Items/Saplings",
            ItemSOType.CropData => "Assets/111_Data/Items/Crops",
            ItemSOType.FoodData => "Assets/111_Data/Items/Foods",
            ItemSOType.MaterialData => "Assets/111_Data/Items/Materials",
            ItemSOType.PotionData => "Assets/111_Data/Items/Potions",
            _ => "Assets/111_Data/Items"
        };
    }

    private void DrawGenerateButton()
    {
        EditorGUILayout.Space(10);
        
        GUI.enabled = selectedSprites.Count > 0;
        GUI.backgroundColor = new Color(0.3f, 0.9f, 0.3f);
        
        if (GUILayout.Button($"🚀 生成 {selectedSprites.Count} 个 {GetTypeName()} SO", GUILayout.Height(45)))
        {
            GenerateItemSOs();
        }
        
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
        
        if (selectedSprites.Count == 0)
        {
            EditorGUILayout.HelpBox("请先在 Project 窗口选择 Sprite", MessageType.Warning);
        }
    }

    private string GetTypeName()
    {
        return soType switch
        {
            ItemSOType.ItemData => "基础物品",
            ItemSOType.ToolData => "工具",
            ItemSOType.WeaponData => "武器",
            ItemSOType.SeedData => "种子",
            ItemSOType.SaplingData => "树苗",
            ItemSOType.CropData => "作物",
            ItemSOType.FoodData => "食物",
            ItemSOType.MaterialData => "材料",
            ItemSOType.PotionData => "药水",
            _ => "物品"
        };
    }

    private void DrawLine()
    {
        EditorGUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        EditorGUILayout.Space(5);
    }

    #endregion

    #region 生成逻辑

    private void GenerateItemSOs()
    {
        // 确保输出文件夹存在
        EnsureFolderExists(outputFolder);

        int successCount = 0;
        List<string> createdFiles = new List<string>();

        for (int i = 0; i < selectedSprites.Count; i++)
        {
            var sprite = selectedSprites[i];
            int itemID = useSequentialID ? startID + i : startID;
            string itemName = sprite.name;

            ScriptableObject so = CreateItemSO(sprite, itemID, itemName);
            if (so != null)
            {
                string prefix = GetFilePrefix();
                string fileName = $"{prefix}_{itemID}_{itemName}.asset";
                string assetPath = $"{outputFolder}/{fileName}";

                // 检查是否已存在
                if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath) != null)
                {
                    if (!EditorUtility.DisplayDialog("文件已存在",
                        $"文件 {fileName} 已存在，是否覆盖？", "覆盖", "跳过"))
                    {
                        continue;
                    }
                    AssetDatabase.DeleteAsset(assetPath);
                }

                AssetDatabase.CreateAsset(so, assetPath);
                createdFiles.Add(assetPath);
                successCount++;
                
                Debug.Log($"<color=green>[批量生成] 创建: {assetPath}</color>");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 选中创建的文件
        if (createdFiles.Count > 0)
        {
            var assets = createdFiles.Select(p => AssetDatabase.LoadAssetAtPath<Object>(p)).ToArray();
            Selection.objects = assets;
        }

        // 自动同步数据库
        string syncMessage = "";
        if (successCount > 0)
        {
            if (DatabaseSyncHelper.DatabaseExists())
            {
                int syncCount = DatabaseSyncHelper.AutoCollectAllItems();
                if (syncCount >= 0)
                {
                    syncMessage = $"\n\n✅ 数据库已自动同步（共 {syncCount} 个物品）";
                }
                else
                {
                    syncMessage = "\n\n⚠️ 数据库同步失败，请手动执行";
                }
            }
            else
            {
                syncMessage = "\n\n⚠️ 数据库不存在，请先创建 MasterItemDatabase";
                if (DatabaseSyncHelper.ShowDatabaseNotFoundWarning())
                {
                    // 用户选择前往创建，可以打开创建菜单
                    EditorApplication.ExecuteMenuItem("Assets/Create/Farm/Database/Item Database");
                }
            }
        }

        EditorUtility.DisplayDialog("完成",
            $"成功创建 {successCount} 个 {GetTypeName()} SO\n保存位置：{outputFolder}{syncMessage}", "确定");

        Debug.Log($"<color=green>[批量生成] ✅ 完成！共创建 {successCount} 个物品</color>");
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

    private string GetFilePrefix()
    {
        return soType switch
        {
            ItemSOType.ToolData => "Tool",
            ItemSOType.WeaponData => "Weapon",
            ItemSOType.SeedData => "Seed",
            ItemSOType.SaplingData => "Sapling",
            ItemSOType.CropData => "Crop",
            ItemSOType.FoodData => "Food",
            ItemSOType.MaterialData => "Material",
            ItemSOType.PotionData => "Potion",
            _ => "Item"
        };
    }

    private ScriptableObject CreateItemSO(Sprite sprite, int itemID, string itemName)
    {
        return soType switch
        {
            ItemSOType.ToolData => CreateToolData(sprite, itemID, itemName),
            ItemSOType.WeaponData => CreateWeaponData(sprite, itemID, itemName),
            ItemSOType.SeedData => CreateSeedData(sprite, itemID, itemName),
            ItemSOType.SaplingData => CreateSaplingData(sprite, itemID, itemName),
            ItemSOType.CropData => CreateCropData(sprite, itemID, itemName),
            ItemSOType.FoodData => CreateFoodData(sprite, itemID, itemName),
            ItemSOType.MaterialData => CreateMaterialData(sprite, itemID, itemName),
            ItemSOType.PotionData => CreatePotionData(sprite, itemID, itemName),
            _ => CreateBaseItemData(sprite, itemID, itemName)
        };
    }

    private ItemData CreateBaseItemData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<ItemData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Special);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        return data;
    }

    private ToolData CreateToolData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<ToolData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Tool);
        data.maxStackSize = 1; // 工具不可堆叠
        
        // 工具专属（注意：工具没有等级属性，品质通过后缀命名区分）
        data.toolType = toolType;
        data.animActionType = GetAnimActionType(toolType);
        
        // 可选属性
        if (setToolEnergy) data.energyCost = toolEnergyCost;
        if (setToolRadius) data.effectRadius = toolEffectRadius;
        if (setToolAnimFrames) data.animationFrameCount = toolAnimFrameCount;
        
        return data;
    }

    private WeaponData CreateWeaponData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<WeaponData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Tool);
        data.maxStackSize = 1; // 武器不可堆叠
        
        // 武器专属（注意：武器没有等级属性，品质通过后缀命名区分）
        data.weaponType = weaponType;
        
        // 可选属性
        if (setWeaponAttack) data.attackPower = weaponAttackPower;
        if (setWeaponSpeed) data.attackSpeed = weaponAttackSpeed;
        if (setWeaponCrit) data.criticalChance = weaponCritChance;
        
        return data;
    }

    private SeedData CreateSeedData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<SeedData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Plant);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        else data.maxStackSize = 99; // 种子默认可堆叠99
        
        // 种子专属
        data.season = seedSeason;
        if (setSeedGrowth) data.growthDays = seedGrowthDays;
        if (setSeedHarvest) data.harvestCropID = seedHarvestCropID;
        
        return data;
    }

    private SaplingData CreateSaplingData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<SaplingData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Plant);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        else data.maxStackSize = 99; // 树苗默认可堆叠99
        
        // 树苗专属
        data.treePrefab = saplingTreePrefab;
        if (setSaplingExp) data.plantingExp = saplingPlantingExp;
        
        return data;
    }

    private CropData CreateCropData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<CropData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Plant);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        else data.maxStackSize = 99; // 作物默认可堆叠99
        
        // 作物专属
        if (setCropSeedID) data.seedID = cropSeedID;
        if (setCropExp) data.harvestExp = cropHarvestExp;
        
        return data;
    }

    private FoodData CreateFoodData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<FoodData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Food);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        else data.maxStackSize = 20; // 食物默认堆叠20
        
        // 食物专属
        if (setFoodEnergy) data.energyRestore = foodEnergyRestore;
        if (setFoodHealth) data.healthRestore = foodHealthRestore;
        data.buffType = foodBuffType;
        
        return data;
    }

    private MaterialData CreateMaterialData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<MaterialData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Material);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        else data.maxStackSize = 99; // 材料默认可堆叠99
        
        // 材料专属
        data.materialSubType = materialSubType;
        
        // 熔炼设置
        if (setMaterialSmelt && materialSubType == MaterialSubType.Ore)
        {
            data.canBeSmelt = materialCanSmelt;
            if (materialCanSmelt) data.smeltResultID = materialSmeltResultID;
        }
        
        return data;
    }

    private PotionData CreatePotionData(Sprite sprite, int itemID, string itemName)
    {
        var data = ScriptableObject.CreateInstance<PotionData>();
        SetCommonProperties(data, sprite, itemID, itemName, ItemCategory.Consumable);
        if (setMaxStack) data.maxStackSize = defaultMaxStack;
        else data.maxStackSize = 20; // 药水默认堆叠20
        
        // 药水专属
        if (setPotionHealth) data.healthRestore = potionHealthRestore;
        if (setPotionEnergy) data.energyRestore = potionEnergyRestore;
        data.buffType = potionBuffType;
        
        return data;
    }

    private void SetCommonProperties(ItemData data, Sprite sprite, int itemID, string itemName, ItemCategory category)
    {
        data.itemID = itemID;
        data.itemName = itemName;
        data.description = ""; // 留空，后续手动填写
        data.category = category;
        data.icon = sprite;
        data.bagSprite = null; // 使用 GetBagSprite() 回退到 icon
        data.worldPrefab = null; // 后续使用批量生成工具创建
        
        // 可选价格
        if (setPrice)
        {
            data.buyPrice = defaultBuyPrice;
            data.sellPrice = defaultSellPrice;
        }
        
        // 可选显示尺寸
        if (setDisplaySize)
        {
            data.useCustomDisplaySize = true;
            data.displayPixelSize = displayPixelSize;
        }
    }

    #endregion

    #region 设置保存/加载

    private void LoadSettings()
    {
        soType = (ItemSOType)EditorPrefs.GetInt("BatchItemSO_Type", 0);
        useSequentialID = EditorPrefs.GetBool("BatchItemSO_SeqID", true);
        startID = EditorPrefs.GetInt("BatchItemSO_StartID", 0);
        outputFolder = EditorPrefs.GetString("BatchItemSO_Output", "Assets/111_Data/Items");
        
        // 通用
        setPrice = EditorPrefs.GetBool("BatchItemSO_SetPrice", false);
        defaultBuyPrice = EditorPrefs.GetInt("BatchItemSO_BuyPrice", 0);
        defaultSellPrice = EditorPrefs.GetInt("BatchItemSO_SellPrice", 0);
        setMaxStack = EditorPrefs.GetBool("BatchItemSO_SetStack", false);
        defaultMaxStack = EditorPrefs.GetInt("BatchItemSO_MaxStack", 99);
        
        // 显示尺寸
        setDisplaySize = EditorPrefs.GetBool("BatchItemSO_SetDisplaySize", false);
        displayPixelSize = EditorPrefs.GetInt("BatchItemSO_DisplaySize", 32);
        
        // 工具（注意：工具没有等级属性）
        toolType = (ToolType)EditorPrefs.GetInt("BatchItemSO_ToolType", 0);
        setToolEnergy = EditorPrefs.GetBool("BatchItemSO_SetToolEnergy", false);
        toolEnergyCost = EditorPrefs.GetInt("BatchItemSO_ToolEnergy", 2);
        
        // 武器（注意：武器没有等级属性）
        weaponType = (WeaponType)EditorPrefs.GetInt("BatchItemSO_WeaponType", 0);
        setWeaponAttack = EditorPrefs.GetBool("BatchItemSO_SetWeaponAtk", false);
        weaponAttackPower = EditorPrefs.GetInt("BatchItemSO_WeaponAtk", 10);
        
        // 种子
        seedSeason = (Season)EditorPrefs.GetInt("BatchItemSO_SeedSeason", 0);
        setSeedGrowth = EditorPrefs.GetBool("BatchItemSO_SetSeedGrowth", false);
        seedGrowthDays = EditorPrefs.GetInt("BatchItemSO_SeedGrowth", 4);
        
        // 材料
        materialSubType = (MaterialSubType)EditorPrefs.GetInt("BatchItemSO_MatSubType", 2);
    }

    private void SaveSettings()
    {
        EditorPrefs.SetInt("BatchItemSO_Type", (int)soType);
        EditorPrefs.SetBool("BatchItemSO_SeqID", useSequentialID);
        EditorPrefs.SetInt("BatchItemSO_StartID", startID);
        EditorPrefs.SetString("BatchItemSO_Output", outputFolder);
        
        // 通用
        EditorPrefs.SetBool("BatchItemSO_SetPrice", setPrice);
        EditorPrefs.SetInt("BatchItemSO_BuyPrice", defaultBuyPrice);
        EditorPrefs.SetInt("BatchItemSO_SellPrice", defaultSellPrice);
        EditorPrefs.SetBool("BatchItemSO_SetStack", setMaxStack);
        EditorPrefs.SetInt("BatchItemSO_MaxStack", defaultMaxStack);
        
        // 显示尺寸
        EditorPrefs.SetBool("BatchItemSO_SetDisplaySize", setDisplaySize);
        EditorPrefs.SetInt("BatchItemSO_DisplaySize", displayPixelSize);
        
        // 工具（注意：工具没有等级属性）
        EditorPrefs.SetInt("BatchItemSO_ToolType", (int)toolType);
        EditorPrefs.SetBool("BatchItemSO_SetToolEnergy", setToolEnergy);
        EditorPrefs.SetInt("BatchItemSO_ToolEnergy", toolEnergyCost);
        
        // 武器（注意：武器没有等级属性）
        EditorPrefs.SetInt("BatchItemSO_WeaponType", (int)weaponType);
        EditorPrefs.SetBool("BatchItemSO_SetWeaponAtk", setWeaponAttack);
        EditorPrefs.SetInt("BatchItemSO_WeaponAtk", weaponAttackPower);
        
        // 种子
        EditorPrefs.SetInt("BatchItemSO_SeedSeason", (int)seedSeason);
        EditorPrefs.SetBool("BatchItemSO_SetSeedGrowth", setSeedGrowth);
        EditorPrefs.SetInt("BatchItemSO_SeedGrowth", seedGrowthDays);
        
        // 材料
        EditorPrefs.SetInt("BatchItemSO_MatSubType", (int)materialSubType);
    }

    #endregion
}
