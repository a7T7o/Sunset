using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

/// <summary>
/// 农田Tile快速创建工具
/// 帮助用户快速创建干燥和湿润的农田Tile
/// </summary>
public class FarmlandTileCreator : EditorWindow
{
    private Sprite drySprite;
    private Sprite wetSprite;
    private string savePath = "Assets/Tiles/Farm";
    private Vector2 scrollPos;

    [MenuItem("Farm/创建农田Tile工具")]
    public static void ShowWindow()
    {
        GetWindow<FarmlandTileCreator>("创建农田Tile");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("农田Tile创建工具", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox(
            "使用步骤：\n" +
            "1. 先使用【Tilemap转Sprite工具】创建农田Sprite\n" +
            "   - 在Scene中用Tilemap画出耕地样式\n" +
            "   - 使用工具导出为Sprite\n" +
            "2. 将导出的Sprite拖入下方对应字段\n" +
            "3. 点击【创建Tile资产】按钮\n\n" +
            "建议Sprite命名：\n" +
            "- Farmland_Dry.png (干燥)\n" +
            "- Farmland_Wet.png (湿润)",
            MessageType.Info
        );

        EditorGUILayout.Space();
        DrawSeparator();
        EditorGUILayout.Space();

        GUILayout.Label("Sprite设置", EditorStyles.boldLabel);

        drySprite = (Sprite)EditorGUILayout.ObjectField(
            "干燥农田 Sprite",
            drySprite,
            typeof(Sprite),
            false
        );

        wetSprite = (Sprite)EditorGUILayout.ObjectField(
            "湿润农田 Sprite",
            wetSprite,
            typeof(Sprite),
            false
        );

        EditorGUILayout.Space();

        savePath = EditorGUILayout.TextField("保存路径", savePath);
        
        if (GUILayout.Button("选择保存路径"))
        {
            string path = EditorUtility.SaveFolderPanel("选择Tile保存路径", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对路径
                if (path.StartsWith(Application.dataPath))
                {
                    savePath = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
        }

        EditorGUILayout.Space();
        DrawSeparator();
        EditorGUILayout.Space();

        // 预览信息
        if (drySprite != null)
        {
            EditorGUILayout.LabelField("干燥Sprite", drySprite.name);
            EditorGUILayout.LabelField("尺寸", $"{drySprite.texture.width} x {drySprite.texture.height}");
        }
        
        if (wetSprite != null)
        {
            EditorGUILayout.LabelField("湿润Sprite", wetSprite.name);
            EditorGUILayout.LabelField("尺寸", $"{wetSprite.texture.width} x {wetSprite.texture.height}");
        }

        EditorGUILayout.Space();

        GUI.enabled = drySprite != null && wetSprite != null;
        if (GUILayout.Button("创建Tile资产", GUILayout.Height(40)))
        {
            CreateTiles();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        DrawSeparator();
        EditorGUILayout.Space();

        // 快速测试区域
        GUILayout.Label("快速测试", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "没有Sprite？可以使用纯色创建测试Tile：\n" +
            "- 干燥：棕色\n" +
            "- 湿润：深棕色",
            MessageType.Info
        );

        if (GUILayout.Button("创建纯色测试Tile", GUILayout.Height(35)))
        {
            CreateTestTiles();
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateTiles()
    {
        // 确保保存路径存在
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // 创建干燥Tile
        Tile dryTile = ScriptableObject.CreateInstance<Tile>();
        dryTile.sprite = drySprite;
        dryTile.color = Color.white;
        string dryPath = $"{savePath}/Farmland_Dry.asset";
        AssetDatabase.CreateAsset(dryTile, dryPath);

        // 创建湿润Tile
        Tile wetTile = ScriptableObject.CreateInstance<Tile>();
        wetTile.sprite = wetSprite;
        wetTile.color = Color.white;
        string wetPath = $"{savePath}/Farmland_Wet.asset";
        AssetDatabase.CreateAsset(wetTile, wetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>Tile创建成功！</color>\n干燥Tile: {dryPath}\n湿润Tile: {wetPath}");
        EditorUtility.DisplayDialog(
            "成功",
            $"Tile已创建！\n\n保存路径：{savePath}\n\n现在可以将这些Tile分配给FarmingManager了",
            "确定"
        );

        // 在Project窗口中高亮显示
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(dryPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    private void CreateTestTiles()
    {
        // 创建测试用的纯色Sprite
        Texture2D dryTexture = CreateSolidColorTexture(16, 16, new Color(0.6f, 0.4f, 0.2f, 1f)); // 棕色
        Texture2D wetTexture = CreateSolidColorTexture(16, 16, new Color(0.4f, 0.25f, 0.15f, 1f)); // 深棕色

        // 保存纹理
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string dryTexPath = $"{savePath}/Test_Farmland_Dry.png";
        string wetTexPath = $"{savePath}/Test_Farmland_Wet.png";

        File.WriteAllBytes(dryTexPath, dryTexture.EncodeToPNG());
        File.WriteAllBytes(wetTexPath, wetTexture.EncodeToPNG());

        AssetDatabase.Refresh();

        // 设置导入设置
        SetupTextureImporter(dryTexPath);
        SetupTextureImporter(wetTexPath);

        // 加载Sprite
        Sprite dryTestSprite = AssetDatabase.LoadAssetAtPath<Sprite>(dryTexPath);
        Sprite wetTestSprite = AssetDatabase.LoadAssetAtPath<Sprite>(wetTexPath);

        // 创建Tile
        Tile dryTile = ScriptableObject.CreateInstance<Tile>();
        dryTile.sprite = dryTestSprite;
        dryTile.color = Color.white;
        string dryTilePath = $"{savePath}/Test_Farmland_Dry.asset";
        AssetDatabase.CreateAsset(dryTile, dryTilePath);

        Tile wetTile = ScriptableObject.CreateInstance<Tile>();
        wetTile.sprite = wetTestSprite;
        wetTile.color = Color.white;
        string wetTilePath = $"{savePath}/Test_Farmland_Wet.asset";
        AssetDatabase.CreateAsset(wetTile, wetTilePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>测试Tile创建成功！</color>\n{dryTilePath}\n{wetTilePath}");
        EditorUtility.DisplayDialog(
            "成功",
            "测试Tile已创建！\n\n这些是纯色Tile，仅用于测试。\n建议后续替换为美术资源。",
            "确定"
        );

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(dryTilePath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    private Texture2D CreateSolidColorTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void SetupTextureImporter(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private void DrawSeparator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
