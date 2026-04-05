using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tilemap 框选最小工作流面板。
/// 适合常驻停靠在 Tile Palette 旁边，做到“框选 -> 点生成”。
/// </summary>
public class TilemapSelectionToColliderWorkflow : EditorWindow
{
    private static readonly string[] GenerationModeLabels = { "逐格物体", "植被整体对象" };

    private TilemapToColliderObjects.GenerationMode generationMode = TilemapToColliderObjects.GenerationMode.PerTileObjects;
    private bool createSpriteRenderer = true;
    private bool copyTileColor = true;
    private bool copySortingFromTilemap = true;
    private TilemapToColliderObjects.ColliderMode colliderMode = TilemapToColliderObjects.ColliderMode.PolygonCollider2D;
    private bool colliderIsTrigger = false;
    private bool addRigidbody2D = false;
    private RigidbodyType2D rigidbodyType = RigidbodyType2D.Static;
    private bool clearSourceTilesAfterConversion = false;
    private bool disableTilemapRendererAfterConversion = false;
    private bool reuseExistingContainer = true;
    private string containerSuffix = "_ConvertedObjects";
    private int vegetationSortingOrderMultiplier = 100;
    private int vegetationSortingOrderOffset = 0;
    private int vegetationInternalOrderStep = 10;
    private Transform customParent;
    private Vector2 scrollPos;

    [MenuItem("Tools/Tilemap框选生成工作流")]
    public static void OpenWorkflow()
    {
        var window = GetWindow<TilemapSelectionToColliderWorkflow>("Tile 框选生成");
        window.minSize = new Vector2(380f, 480f);
        window.Focus();
    }

    [MenuItem("Tools/Tilemap/从当前框选打开生成器 %#g")]
    public static void OpenWorkflowShortcut()
    {
        OpenWorkflow();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Tile 框选生成工作流", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "推荐用法：把这个窗口停靠在 Tile Palette 旁边。\n" +
            "之后每次只需要：框选 Tilemap 区域 -> 选生成模式 -> 点“生成当前框选”。",
            MessageType.Info);

        DrawSelectionStatus();
        DrawSeparator();
        DrawQuickSettings();
        DrawSeparator();
        DrawActions();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSelectionStatus()
    {
        GUILayout.Label("当前框选", EditorStyles.boldLabel);

        if (!TilemapToColliderObjects.TryGetCurrentGridSelection(out Tilemap tilemap, out BoundsInt bounds))
        {
            EditorGUILayout.HelpBox("当前没有有效的 Tilemap 框选区域。", MessageType.Warning);
            return;
        }

        EditorGUILayout.ObjectField("目标 Tilemap", tilemap, typeof(Tilemap), true);
        EditorGUILayout.LabelField("范围", TilemapToColliderObjects.DescribeBounds(bounds));
        EditorGUILayout.LabelField("非空格子数", TilemapToColliderObjects.CountOccupiedCells(tilemap, bounds).ToString());
    }

    private void DrawQuickSettings()
    {
        GUILayout.Label("快速设置", EditorStyles.boldLabel);
        generationMode = (TilemapToColliderObjects.GenerationMode)EditorGUILayout.Popup("生成模式", (int)generationMode, GenerationModeLabels);

        customParent = (Transform)EditorGUILayout.ObjectField("自定义父物体（可选）", customParent, typeof(Transform), true);
        containerSuffix = EditorGUILayout.TextField("容器后缀", containerSuffix);
        reuseExistingContainer = EditorGUILayout.Toggle("复用同名容器", reuseExistingContainer);

        EditorGUILayout.Space();
        createSpriteRenderer = EditorGUILayout.Toggle("生成 SpriteRenderer", createSpriteRenderer);
        using (new EditorGUI.DisabledScope(!createSpriteRenderer))
        {
            copyTileColor = EditorGUILayout.Toggle("复制 Tile 颜色", copyTileColor);
            copySortingFromTilemap = EditorGUILayout.Toggle("复制 Tilemap 排序层", copySortingFromTilemap);
        }

        EditorGUILayout.Space();
        colliderMode = (TilemapToColliderObjects.ColliderMode)EditorGUILayout.EnumPopup("碰撞体类型", colliderMode);
        colliderIsTrigger = EditorGUILayout.Toggle("Collider Is Trigger", colliderIsTrigger);

        addRigidbody2D = EditorGUILayout.Toggle("附加 Rigidbody2D", addRigidbody2D);
        using (new EditorGUI.DisabledScope(!addRigidbody2D))
        {
            rigidbodyType = (RigidbodyType2D)EditorGUILayout.EnumPopup("Rigidbody 类型", rigidbodyType);
        }

        if (generationMode == TilemapToColliderObjects.GenerationMode.VegetationClusters)
        {
            EditorGUILayout.Space();
            vegetationSortingOrderMultiplier = EditorGUILayout.IntField("植被排序倍率", vegetationSortingOrderMultiplier);
            vegetationSortingOrderOffset = EditorGUILayout.IntField("植被排序偏移", vegetationSortingOrderOffset);
            vegetationInternalOrderStep = EditorGUILayout.IntField("组内行间距", vegetationInternalOrderStep);
            EditorGUILayout.HelpBox(
                "植被整体模式会把连成一片的 tile 切成整体对象，再按整丛植物做排序。\n" +
                "如果你只是想一格一个物体，请切回逐格模式。",
                MessageType.Info);
        }

        EditorGUILayout.Space();
        clearSourceTilesAfterConversion = EditorGUILayout.Toggle("清空源 Tile", clearSourceTilesAfterConversion);
        using (new EditorGUI.DisabledScope(!clearSourceTilesAfterConversion))
        {
            disableTilemapRendererAfterConversion = EditorGUILayout.Toggle("转换后关闭 TilemapRenderer", disableTilemapRendererAfterConversion);
        }

        if (clearSourceTilesAfterConversion)
        {
            EditorGUILayout.HelpBox("你当前选择的是替换式工作流：生成后会清空框选区域内的源 Tile。", MessageType.Warning);
        }
    }

    private void DrawActions()
    {
        bool hasSelection = TilemapToColliderObjects.TryGetCurrentGridSelection(out _, out _);

        GUI.enabled = hasSelection;
        if (GUILayout.Button("生成当前框选", GUILayout.Height(40f)))
        {
            TilemapToColliderObjects.RunGridSelectionConversion(
                generationMode,
                createSpriteRenderer,
                copyTileColor,
                copySortingFromTilemap,
                colliderMode,
                colliderIsTrigger,
                addRigidbody2D,
                rigidbodyType,
                clearSourceTilesAfterConversion,
                disableTilemapRendererAfterConversion,
                reuseExistingContainer,
                vegetationSortingOrderMultiplier,
                vegetationSortingOrderOffset,
                vegetationInternalOrderStep,
                customParent,
                containerSuffix);
        }

        GUI.enabled = true;

        EditorGUILayout.Space();

        if (GUILayout.Button("打开高级窗口", GUILayout.Height(28f)))
        {
            TilemapToColliderObjects.OpenForCurrentGridSelection();
        }
    }

    private void DrawSeparator()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
    }
}
