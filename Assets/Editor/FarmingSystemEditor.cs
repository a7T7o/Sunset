using UnityEngine;
using UnityEditor;
using FarmGame.Farm;

/// <summary>
/// 农田系统编辑器工具
/// 用于快速测试和调试
/// </summary>
public class FarmingSystemEditor : EditorWindow
{
    private FarmingManager farmingManager;
    private Vector2 scrollPos;

    [MenuItem("Farm/农田系统调试工具")]
    public static void ShowWindow()
    {
        GetWindow<FarmingSystemEditor>("农田系统调试");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("农田系统调试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 获取FarmingManager
        if (farmingManager == null)
        {
            farmingManager = FindFirstObjectByType<FarmingManager>();
        }

        if (farmingManager == null)
        {
            EditorGUILayout.HelpBox("场景中未找到FarmingManager！\n请确保场景中有FarmingManager组件。", MessageType.Warning);
            
            if (GUILayout.Button("创建FarmingManager", GUILayout.Height(30)))
            {
                CreateFarmingManager();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("已找到FarmingManager", MessageType.Info);
            
            EditorGUILayout.Space();
            DrawSeparator();
            EditorGUILayout.Space();

            // 统计信息
            var allTiles = farmingManager.GetAllFarmTiles();
            GUILayout.Label("统计信息", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("总耕地数量", allTiles.Count.ToString());
            
            int cropCount = 0;
            int wateredCount = 0;
            int matureCount = 0;
            int witheredCount = 0;

            foreach (var kvp in allTiles)
            {
                if (kvp.Value.HasCrop())
                {
                    cropCount++;
                    if (kvp.Value.crop.IsMature()) matureCount++;
                    if (kvp.Value.crop.isWithered) witheredCount++;
                }
                // 统计今天已浇水的耕地
                if (kvp.Value.wateredToday) wateredCount++;
            }

            EditorGUILayout.LabelField("作物数量", cropCount.ToString());
            EditorGUILayout.LabelField("已浇水", wateredCount.ToString());
            EditorGUILayout.LabelField("成熟作物", matureCount.ToString());
            EditorGUILayout.LabelField("枯萎作物", witheredCount.ToString());

            EditorGUILayout.Space();
            DrawSeparator();
            EditorGUILayout.Space();

            // 快捷操作
            GUILayout.Label("快捷操作", EditorStyles.boldLabel);
            
            if (GUILayout.Button("清除所有耕地", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("确认", "是否清除所有耕地和作物？", "确定", "取消"))
                {
                    ClearAllFarmlands();
                }
            }

            if (GUILayout.Button("浇灌所有耕地", GUILayout.Height(30)))
            {
                WaterAllFarmlands();
            }

            if (GUILayout.Button("收获所有成熟作物", GUILayout.Height(30)))
            {
                HarvestAllMatureCrops();
            }

            EditorGUILayout.Space();
            DrawSeparator();
            EditorGUILayout.Space();

            // 测试工具
            GUILayout.Label("测试工具", EditorStyles.boldLabel);
            
            if (GUILayout.Button("手动触发每日生长", GUILayout.Height(30)))
            {
                ManualGrowthUpdate();
            }

            if (GUILayout.Button("手动触发水分蒸发", GUILayout.Height(30)))
            {
                ManualEvaporationUpdate();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateFarmingManager()
    {
        GameObject go = new GameObject("FarmingSystem");
        go.AddComponent<FarmingManager>();
        go.AddComponent<CropGrowthSystem>();
        // 注意：WaterEvaporationSystem已移除，水分蒸发现在由FarmingManager自动处理
        
        farmingManager = go.GetComponent<FarmingManager>();
        
        Debug.Log("<color=green>[农田系统] 已创建FarmingManager！</color>");
        EditorUtility.DisplayDialog("成功", "FarmingSystem已创建！\n\n请在Inspector中设置Tilemap引用。", "确定");
    }

    private void ClearAllFarmlands()
    {
        var allTiles = farmingManager.GetAllFarmTiles();
        int count = allTiles.Count;

        foreach (var kvp in allTiles.Values)
        {
            kvp.ClearCrop();
        }

        allTiles.Clear();

        Debug.Log($"<color=green>[农田系统] 已清除{count}个耕地</color>");
        EditorUtility.DisplayDialog("完成", $"已清除{count}个耕地", "确定");
    }

    private void WaterAllFarmlands()
    {
        var allTiles = farmingManager.GetAllFarmTiles();
        int count = 0;

        foreach (var kvp in allTiles)
        {
            if (farmingManager.WaterTileAtCell(kvp.Key))
            {
                count++;
            }
        }

        Debug.Log($"<color=green>[农田系统] 已浇水{count}个耕地</color>");
        EditorUtility.DisplayDialog("完成", $"已浇水{count}个耕地", "确定");
    }

    private void HarvestAllMatureCrops()
    {
        var allTiles = farmingManager.GetAllFarmTiles();
        int count = 0;

        foreach (var kvp in allTiles)
        {
            if (kvp.Value.HasCrop() && kvp.Value.crop.IsMature())
            {
                if (farmingManager.HarvestCropAtCell(kvp.Key, out var crop, out int amount))
                {
                    count++;
                    Debug.Log($"收获: {crop.itemName} x{amount}");
                }
            }
        }

        Debug.Log($"<color=green>[农田系统] 已收获{count}个作物</color>");
        EditorUtility.DisplayDialog("完成", $"已收获{count}个作物", "确定");
    }

    private void ManualGrowthUpdate()
    {
        CropGrowthSystem growthSystem = FindFirstObjectByType<CropGrowthSystem>();
        if (growthSystem != null)
        {
            growthSystem.ManualGrowthUpdate();
            Debug.Log("<color=green>[农田系统] 已手动触发生长检查</color>");
        }
        else
        {
            Debug.LogWarning("[农田系统] 未找到CropGrowthSystem！");
        }
    }

    private void ManualEvaporationUpdate()
    {
        var allTiles = farmingManager.GetAllFarmTiles();
        int count = 0;

        // 新版本：模拟进入第二天，所有土地变干
        foreach (var kvp in allTiles)
        {
            if (kvp.Value.wateredToday || kvp.Value.moistureState != SoilMoistureState.Dry)
            {
                kvp.Value.wateredYesterday = kvp.Value.wateredToday;
                kvp.Value.wateredToday = false;
                kvp.Value.waterTime = -1f;
                kvp.Value.moistureState = SoilMoistureState.Dry;
                count++;
            }
        }

        Debug.Log($"<color=green>[农田系统] 已模拟蒸发（第二天）{count}个湿润耕地</color>");
        EditorUtility.DisplayDialog("完成", $"已模拟蒸发{count}个湿润耕地\n\n提示：新版本中水分蒸发在每天开始时自动进行。", "确定");
    }

    private void DrawSeparator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
